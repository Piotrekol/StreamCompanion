using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using CollectionManager.DataTypes;
using OsuMemoryDataProvider;
using StreamCompanionTypes;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Interfaces;
using Timer = System.Threading.Timer;

namespace OsuMemoryEventSource
{
    //TODO: OsuMemoryEventSource should get refactored into several interface-scoped classes
    public class OsuMemoryEventSource : OsuMemoryEventSourceBase,
        IMapDataFinder, ISettingsProvider
    {
        public string SettingGroup { get; } = "Map matching";

        protected override void OnSettingsSettingUpdated(object sender, SettingUpdated e)
        {
            if (e.Name == _names.EnableMemoryPooling.Name)
            {
                if (_settings.Get<bool>(_names.EnableMemoryPooling))
                {
                    EnableTimer();
                }
                else
                    DisableTimer();
            }
            base.OnSettingsSettingUpdated(sender, e);
        }

        public void Free()
        {
            userSettings?.Dispose();
        }

        private MemoryDataFinderSettings userSettings = null;
        public UserControl GetUiSettings()
        {
            if (userSettings == null || userSettings.IsDisposed)
            {
                userSettings = new MemoryDataFinderSettings(_settings);
            }

            return userSettings;
        }


        public MapSearchResult FindBeatmap(MapSearchArgs searchArgs)
        {
            var result = new MapSearchResult();
            result.MapSearchString = searchArgs.Raw;
            if (!_settings.Get<bool>(_names.EnableMemoryScanner))
                return result;

            int mapId = _memoryReader.GetMapId();
            int mods = 0;

            if (searchArgs.Status == OsuStatus.Playing)
            {
                Thread.Sleep(250);
                mods = _memoryReader.GetMods();
                result.Mods = new Tuple<Mods, string>((Mods)mods, _modParser[0].GetModsFromEnum(mods));
            }

            _logger.Log(">Got {0} & {1} from memory", LogLevel.Advanced, mapId.ToString(), mods.ToString());

            Mods eMods = result?.Mods?.Item1 ?? Mods.Omod;
            if (mapId > 2000000 || mapId < 0 || Helpers.IsInvalidCombination(eMods))
            {
                _logger.Log("Sanity check tiggered - invalidating last result", LogLevel.Advanced);
                result.Mods = null;
                return result;
            }

            var b = _sqLiteController.GetBeatmap(mapId);
            if (b != null)
            {
                result.BeatmapsFound.Add(b);
            }

            return result;
        }
    }
}
