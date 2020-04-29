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

        public MapSearchResult FindBeatmap(MapSearchArgs searchArgs)
        {
            if (!Started)
                return null;

            if (searchArgs == null)
                throw new ArgumentException(nameof(searchArgs));
            
            var result = new MapSearchResult(searchArgs);

            if (!Started || !_settings.Get<bool>(_names.EnableMemoryScanner))
                return result;

            int mapId = _memoryReader.GetMapId();
            int mods = 0;

            if (searchArgs.Status == OsuStatus.Playing)
            {
                Thread.Sleep(250);
                mods = _memoryReader.GetPlayingMods();
                result.Mods = GetMods(mods);
            }
            else
            {
                result.Mods = GetMods(_memoryReader.GetMods());
            }

            Logger?.Log(">Got {0} & {1} from memory", LogLevel.Advanced, mapId.ToString(), mods.ToString());

            Mods eMods = result.Mods?.Mods ?? Mods.Omod;
            if (mapId > 200_000_000 || mapId < 0 || Helpers.IsInvalidCombination(eMods))
            {
                Logger?.Log("Sanity check tiggered - invalidating last result", LogLevel.Advanced);
                result.Mods = null;
                return result;
            }

            if (mapId == 0)
            {
                Logger?.Log("Map has no ID", LogLevel.Advanced);
                return result;
            }

            var b = _databaseController?.GetBeatmap(mapId);
            if (b != null)
            {
                result.BeatmapsFound.Add(b);
            }

            return result;
        }

        private IModsEx GetMods(int modsValue)
        {
            return _modParser?.GetModsFromEnum(modsValue);
        }
        private FirstRunMemoryCalibration _firstRunMemoryCalibration = null;
        public List<IFirstRunControl> GetFirstRunUserControls()
        {
            if (_firstRunMemoryCalibration == null || _firstRunMemoryCalibration.IsDisposed)
            {
                _firstRunMemoryCalibration = new FirstRunMemoryCalibration(_memoryReader, _settings);
            }

            NewOsuEvent += (s, args) =>
            {
                _firstRunMemoryCalibration?.GotMemory(args.MapId, args.Status, args.Raw);
            };
            return new List<IFirstRunControl> { _firstRunMemoryCalibration };
        }
    }
}
