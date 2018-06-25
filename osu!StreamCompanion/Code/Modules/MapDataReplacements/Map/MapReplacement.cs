using System.Collections.Generic;
using System.IO;
using osu_StreamCompanion.Code.Core;
using osu_StreamCompanion.Code.Helpers;
using osu_StreamCompanion.Code.Misc;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Interfaces;
using Beatmap = StreamCompanionTypes.DataTypes.Beatmap;

namespace osu_StreamCompanion.Code.Modules.MapDataReplacements.Map
{
    class MapReplacement : IModule, IMapDataReplacements, ISettings
    {
        private readonly SettingNames _names = SettingNames.Instance;
        private ISettingsHandler _settings;
        public bool Started { get; set; }
        public void Start(ILogger logger) { Started = true; }

        public Dictionary<string, string> GetMapReplacements(MapSearchResult map)
        {
            Dictionary<string, string> dict;
            if (map.FoundBeatmaps)
            {
                dict = map.BeatmapsFound[0].GetDict();

                var osuLocation = _settings.Get<string>(_names.MainOsuDirectory);
                var customSongsLocation = _settings.Get<string>(_names.SongsFolderLocation);
                if (string.IsNullOrWhiteSpace(osuLocation))
                    dict.Add("!OsuFileLocation!", "");
                else
                {
                    string baseDirectory = customSongsLocation == _names.SongsFolderLocation.Default<string>() 
                        ? Path.Combine(osuLocation, "Songs") 
                        : customSongsLocation;

                        dict.Add("!OsuFileLocation!",
                            Path.Combine(baseDirectory, map.BeatmapsFound[0].Dir,
                                map.BeatmapsFound[0].OsuFileName));
                }

            }
            else
            {
                dict = ((Beatmap)null).GetDict(true);
                dict.Add("!OsuFileLocation!", "");
            }

            return dict;
        }

        public void SetSettingsHandle(ISettingsHandler settings)
        {
            _settings = settings;
        }
    }
}
