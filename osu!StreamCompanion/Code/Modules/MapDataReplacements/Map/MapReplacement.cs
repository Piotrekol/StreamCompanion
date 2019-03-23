using osu_StreamCompanion.Code.Core;
using osu_StreamCompanion.Code.Helpers;
using osu_StreamCompanion.Code.Misc;
using StreamCompanionTypes;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Interfaces;
using System.Collections.Generic;
using System.IO;
using Beatmap = StreamCompanionTypes.DataTypes.Beatmap;

namespace osu_StreamCompanion.Code.Modules.MapDataReplacements.Map
{
    class MapReplacement : IModule, ITokensProvider, ISettings
    {
        private readonly SettingNames _names = SettingNames.Instance;
        private ISettingsHandler _settings;
        private Tokens.TokenGetter _tokenGetter;
        public bool Started { get; set; }

        public void Start(ILogger logger)
        {
            _tokenGetter = Tokens.CreateTokenGetter("MapReplacements");
            Started = true;
        }

        public void CreateTokens(MapSearchResult map)
        {
            Dictionary<string, object> dict;
            var OsuFileLocationToken = _tokenGetter("OsuFileLocation", null);
            if (map.FoundBeatmaps)
            {
                dict = map.BeatmapsFound[0].GetTokens();

                var osuLocation = _settings.Get<string>(_names.MainOsuDirectory);
                var customSongsLocation = _settings.Get<string>(_names.SongsFolderLocation);
                if (string.IsNullOrWhiteSpace(osuLocation))
                    OsuFileLocationToken.Value = null;
                else
                {
                    string baseDirectory = customSongsLocation == _names.SongsFolderLocation.Default<string>()
                        ? Path.Combine(osuLocation, "Songs")
                        : customSongsLocation;
                    OsuFileLocationToken.Value = Path.Combine(baseDirectory, map.BeatmapsFound[0].Dir,
                        map.BeatmapsFound[0].OsuFileName);
                }
            }
            else
            {
                dict = ((Beatmap)null).GetTokens(true);
                OsuFileLocationToken.Value = null;
            }

            foreach (var token in dict)
            {
                _tokenGetter(token.Key, token.Value);
            }

        }
        public void SetSettingsHandle(ISettingsHandler settings)
        {
            _settings = settings;
        }
    }
}
