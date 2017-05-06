using System.Collections.Generic;
using osu_StreamCompanion.Code.Core;
using osu_StreamCompanion.Code.Core.DataTypes;
using osu_StreamCompanion.Code.Helpers;
using osu_StreamCompanion.Code.Interfeaces;
using osu_StreamCompanion.Code.Misc;

namespace osu_StreamCompanion.Code.Modules.MapDataReplacements.Map
{
    class MapReplacement :IModule,IMapDataReplacements,ISettings
    {
        private readonly SettingNames _names = SettingNames.Instance;
        private Settings _settings;
        public bool Started { get; set; }
        public void Start(ILogger logger){ Started = true; }

        public Dictionary<string, string> GetMapReplacements(MapSearchResult map)
        {
            if (map.FoundBeatmaps)
            {
                var dict = map.BeatmapsFound[0].GetDict(map.Mods?.Item2 ?? "");

                var osuLocation = _settings.Get<string>(_names.MainOsuDirectory);
                if (string.IsNullOrWhiteSpace(osuLocation))
                    dict.Add("!OsuFileLocation!","");
                else
                    dict.Add("!OsuFileLocation!",System.IO.Path.Combine(osuLocation, "Songs", map.BeatmapsFound[0].Dir, map.BeatmapsFound[0].OsuFileName));

                return dict;
            }

            return new Dictionary<string, string>();
        }

        public void SetSettingsHandle(Settings settings)
        {
            _settings = settings;
        }
    }
}
