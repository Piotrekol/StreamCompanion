using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using StreamCompanionTypes.Enums;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Interfaces;
using StreamCompanionTypes.Interfaces.Consumers;
using StreamCompanionTypes.Interfaces.Services;
using StreamCompanionTypes.Interfaces.Sources;
using Mods = CollectionManager.DataTypes.Mods;

namespace OsuMemoryEventSource
{
    //TODO: OsuMemoryEventSource should get refactored into several interface-scoped classes
    public class OsuMemoryEventSource : OsuMemoryEventSourceBase,
        IMapDataFinder, ISettingsSource, IFirstRunControlProvider
    {
        public string SettingGroup { get; } = "General__Miscellaneous";
        public int Priority { get; set; } = 100;
        public OsuMemoryEventSource(IContextAwareLogger logger, ISettings settings,
            IModParser modParser, List<Lazy<IHighFrequencyDataConsumer>> highFrequencyDataConsumers,
            ISaver saver, Delegates.Exit exiter) : base(logger, settings, modParser, highFrequencyDataConsumers, saver, exiter)
        {
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

        public Task<IMapSearchResult> FindBeatmap(IMapSearchArgs searchArgs, CancellationToken cancellationToken)
        {
            if (searchArgs == null)
                throw new ArgumentException(nameof(searchArgs));

            var result = new MapSearchResult(searchArgs);
            var mods = (int)searchArgs.Mods;
            result.Mods = GetModsEx(mods);
            Logger?.Log($">Got mods from memory: {result.Mods.ShownMods}({mods})", LogLevel.Debug);

            Mods eMods = result.Mods?.Mods ?? Mods.Omod;
            if (Helpers.IsInvalidCombination(eMods))
            {
                Logger?.Log("Sanity check tiggered - invalidating last result", LogLevel.Debug);
                result.Mods = null;
            }

            return Task.FromResult<IMapSearchResult>(result);
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
                _firstRunMemoryCalibration = new FirstRunMemoryCalibration(MemoryReader, _settings, Logger);
            }

            NewOsuEvent += (s, args) =>
            {
                _firstRunMemoryCalibration?.GotMemory(args.MapId, args.Status, args.Raw);
            };
            return new List<IFirstRunControl> { _firstRunMemoryCalibration };
        }
    }
}
