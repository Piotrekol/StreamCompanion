using System.Collections.Generic;
using CollectionManager.DataTypes;
using osu_StreamCompanion.Code.Core;
using osu_StreamCompanion.Code.Core.DataTypes;
using osu_StreamCompanion.Code.Helpers;
using osu_StreamCompanion.Code.Interfaces;
using osu_StreamCompanion.Code.Misc;
using Beatmap = osu_StreamCompanion.Code.Core.DataTypes.Beatmap;

namespace osu_StreamCompanion.Code.Modules.MapDataReplacements.Map
{
    class MapReplacement : IModule, IMapDataReplacements, ISettings
    {
        private readonly SettingNames _names = SettingNames.Instance;
        private Settings _settings;
        public bool Started { get; set; }
        public void Start(ILogger logger) { Started = true; }

        public Dictionary<string, string> GetMapReplacements(MapSearchResult map)
        {
            Dictionary<string, string> dict;
            if (map.FoundBeatmaps)
            {
                dict = map.BeatmapsFound[0].GetDict(map.Mods);

                var osuLocation = _settings.Get<string>(_names.MainOsuDirectory);
                if (string.IsNullOrWhiteSpace(osuLocation))
                    dict.Add("!OsuFileLocation!", "");
                else
                    dict.Add("!OsuFileLocation!",
                        System.IO.Path.Combine(osuLocation, "Songs", map.BeatmapsFound[0].Dir,
                            map.BeatmapsFound[0].OsuFileName));

            }
            else
            {
                dict = ((Beatmap)null).GetDict(map.Mods, true);
                dict.Add("!OsuFileLocation!", "");
            }

            return dict;
        }

        public void SetSettingsHandle(Settings settings)
        {
            _settings = settings;
        }
    }
}
