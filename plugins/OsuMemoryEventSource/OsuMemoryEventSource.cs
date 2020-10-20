using System;
using System.Collections.Generic;
using System.Threading;
using StreamCompanionTypes.Enums;
using CollectionManager.DataTypes;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Interfaces;
using StreamCompanionTypes.Interfaces.Consumers;
using StreamCompanionTypes.Interfaces.Services;
using StreamCompanionTypes.Interfaces.Sources;

namespace OsuMemoryEventSource
{
    //TODO: OsuMemoryEventSource should get refactored into several interface-scoped classes
    public class OsuMemoryEventSource : OsuMemoryEventSourceBase,
        IMapDataFinder, ISettingsSource, IFirstRunControlProvider
    {
        public string SettingGroup { get; } = "Map matching";
        public int Priority { get; set; } = 100;
        public OsuMemoryEventSource(IContextAwareLogger logger, ISettings settings, IDatabaseController databaseController,
            IModParser modParser, List<IHighFrequencyDataConsumer> highFrequencyDataConsumers,
            ISaver saver) : base(logger, settings, databaseController, modParser, highFrequencyDataConsumers, saver)
        {
        }

        protected override void OnSettingsSettingUpdated(object sender, SettingUpdated e)
        {
            if (e.Name == _names.EnableMemoryPooling.Name)
            {
                base.MemoryPoolingIsEnabled = _settings.Get<bool>(_names.EnableMemoryPooling);
            }
            base.OnSettingsSettingUpdated(sender, e);
        }

        public void Free()
        {
            userSettings?.Dispose();
        }

        private MemoryDataFinderSettings userSettings = null;
        public object GetUiSettings()
        {
            if (userSettings == null || userSettings.IsDisposed)
            {
                userSettings = new MemoryDataFinderSettings(_settings);
            }

            return userSettings;
        }

        public IMapSearchResult FindBeatmap(IMapSearchArgs searchArgs)
        {
            if (!Started)
                return null;

            if (searchArgs == null)
                throw new ArgumentException(nameof(searchArgs));

            var result = new MapSearchResult(searchArgs);

            if (!Started || !_settings.Get<bool>(_names.EnableMemoryScanner))
                return result;

            var mods = ReadMods(searchArgs.Status);
            result.Mods = GetModsEx(mods);
            
            Logger?.Log($">Got mods from memory: {result.Mods.ShownMods}({mods})", LogLevel.Debug);

            Mods eMods = result.Mods?.Mods ?? Mods.Omod;
            if (Helpers.IsInvalidCombination(eMods))
            {
                Logger?.Log("Sanity check tiggered - invalidating last result", LogLevel.Debug);
                result.Mods = null;
            }

            return result;
        }

        private int ReadMods(OsuStatus osuStatus, int retryCount = 0)
        {
            int mods;
            if (osuStatus == OsuStatus.Playing || osuStatus == OsuStatus.Watching)
            {
                Thread.Sleep(250);
                mods = _memoryReader.GetPlayingMods();
            }
            else
            {
                mods = _memoryReader.GetMods();
            }

            if ((mods < 0 || Helpers.IsInvalidCombination((Mods)mods)))
            {
                if (retryCount < 5)
                {
                    Logger.Log($"Mods read attempt failed - retrying (attempt {retryCount}); Status: {osuStatus}; read: {(Mods)mods}({mods})", LogLevel.Debug);
                    return ReadMods(osuStatus, ++retryCount);
                }

                Logger.Log($"Mods read attempt failed after {retryCount} retries; Status: {osuStatus}", LogLevel.Debug);
                mods = 0;
            }

            return mods;
        }
        private IModsEx GetModsEx(int modsValue)
        {
            return _modParser?.GetModsFromEnum(modsValue);
        }
        private FirstRunMemoryCalibration _firstRunMemoryCalibration = null;
        public List<IFirstRunControl> GetFirstRunUserControls()
        {
            if (_firstRunMemoryCalibration == null || _firstRunMemoryCalibration.IsDisposed)
            {
                _firstRunMemoryCalibration = new FirstRunMemoryCalibration(_memoryReader, _settings, Logger);
            }

            NewOsuEvent += (s, args) =>
            {
                _firstRunMemoryCalibration?.GotMemory(args.MapId, args.Status, args.Raw);
            };
            return new List<IFirstRunControl> { _firstRunMemoryCalibration };
        }
    }
}
