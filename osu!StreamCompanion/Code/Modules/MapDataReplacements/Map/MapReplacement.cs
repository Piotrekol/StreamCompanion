using osu_StreamCompanion.Code.Helpers;
using StreamCompanionTypes;
using StreamCompanionTypes.DataTypes;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using osu_StreamCompanion.Code.Misc;
using StreamCompanionTypes.Interfaces.Services;
using StreamCompanionTypes.Interfaces.Sources;
using System;
using System.Globalization;

namespace osu_StreamCompanion.Code.Modules.MapDataReplacements.Map
{
    class MapReplacement : IModule, ITokensSource
    {
        private readonly SettingNames _names = SettingNames.Instance;
        private ISettings _settings;
        private Tokens.TokenSetter _tokenSetter;
        public bool Started { get; set; }

        public MapReplacement(ILogger logger, ISettings settings)
        {
            _settings = settings;
            _tokenSetter = Tokens.CreateTokenSetter("MapReplacements");

        }
        public void Start(ILogger logger)
        {
            Started = true;
        }

        public Task CreateTokensAsync(IMapSearchResult map, CancellationToken cancellationToken)
        {
            Dictionary<string, object> dict;
            var OsuFileLocationToken = _tokenSetter("osuFileLocation", string.Empty);
            if (map.BeatmapsFound.Any())
            {
                dict = GetTokens(map.BeatmapsFound[0]);

                var osuLocation = _settings.Get<string>(_names.MainOsuDirectory);
                var customSongsLocation = _settings.Get<string>(_names.SongsFolderLocation);
                if (string.IsNullOrWhiteSpace(osuLocation))
                    OsuFileLocationToken.Value = string.Empty;
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
                dict = GetTokens(null, true);
                OsuFileLocationToken.Value = string.Empty;
            }

            foreach (var token in dict)
            {
                _tokenSetter(token.Key, token.Value);
            }

            return Task.CompletedTask;
        }

        private Dictionary<string, object> GetTokens(IBeatmap bm, bool empty = false)
        {
            Dictionary<string, object> dict;
            if (bm == null || empty)
            {
                dict = new Dictionary<string, object>
                {
                    {"titleRoman", null},
                    {"artistRoman", null},
                    {"titleUnicode", null},
                    {"artistUnicode", null},
                    {"mapArtistTitle", null},
                    {"mapArtistTitleUnicode", null},
                    {"mapDiff", null},
                    {"creator", null},
                    {"diffName", null},
                    {"mp3Name", null},
                    {"md5", null},
                    {"osuFileName", null},
                    {"maxBpm", null},
                    {"minBpm", null},
                    {"bpm", null},
                    {"mainBpm", null },
                    {"tags", null},
                    {"circles", null},
                    {"sliders", null},
                    {"spinners", null},
                    {"ar", null},
                    {"cs", null},
                    {"hp", null},
                    {"od", null},
                    {"sv", null},
                    {"starsNomod", null},
                    {"drainingtime", null},
                    {"totaltime", null},
                    {"previewtime", null},
                    {"mapid", null},
                    {"dl", null},
                    {"mapsetid", null},
                    {"threadid", null},
                    {"sl", null},
                    {"mode", null},
                    {"source", null},
                    {"dir", null},
                };
            }
            else
            {
                dict = new Dictionary<string, object>
                {
                    {"titleRoman", bm.TitleRoman},
                    {"artistRoman", bm.ArtistRoman},
                    {"titleUnicode", bm.TitleUnicode},
                    {"artistUnicode", bm.ArtistUnicode},
                    {"mapArtistTitle", string.Format("{0} - {1}", bm.ArtistRoman, bm.TitleRoman) },
                    {"mapArtistTitleUnicode", string.Format("{0} - {1}", bm.ArtistUnicode, bm.TitleUnicode) },
                    {"mapDiff", string.IsNullOrWhiteSpace(bm.DiffName)? "" : "[" + bm.DiffName + "]" },
                    {"creator", bm.Creator},
                    {"diffName", bm.DiffName},
                    {"mp3Name", bm.Mp3Name},
                    {"md5", bm.Md5},
                    {"osuFileName", bm.OsuFileName},
                    {"maxBpm", Math.Round(bm.MaxBpm, 2)},
                    {"minBpm", Math.Round(bm.MinBpm, 2)},
                    {"bpm", Math.Abs(bm.MinBpm - bm.MaxBpm) < double.Epsilon
                        ? Math.Round(bm.MinBpm, 2).ToString(CultureInfo.InvariantCulture)
                        : string.Format(CultureInfo.InvariantCulture, "{0:0.##} - {1:0.##} ({2:0.##})", bm.MinBpm, bm.MaxBpm, bm.MainBpm)
                    },
                    {"mainBpm", Math.Round(bm.MainBpm, 2) },
                    {"tags", bm.Tags},
                    {"circles", bm.Circles},
                    {"sliders", bm.Sliders},
                    {"spinners", bm.Spinners},
                    {"ar", Math.Round(bm.ApproachRate, 2)},
                    {"cs", Math.Round(bm.CircleSize, 2)},
                    {"hp", Math.Round(bm.HpDrainRate, 2)},
                    {"od", Math.Round(bm.OverallDifficulty, 2)},
                    {"sv", Math.Round(bm.SliderVelocity ?? 0, 2)},
                    {"starsNomod", Math.Round(bm.StarsNomod, 2)},
                    {"drainingtime", bm.DrainingTime},
                    {"totaltime", bm.TotalTime},
                    {"previewtime", bm.PreviewTime},
                    {"dl", bm.MapLink},
                    {"threadid", bm.ThreadId},
                    {"sl", Math.Round(bm.StackLeniency ?? 0, 3)},
                    {"mode", bm.PlayMode.GetHashCode().ToString()},
                    {"source", bm.Source},
                    {"dir", bm.Dir},
                };
            }

            dict["lb"] = Environment.NewLine;

            return dict;
        }
    }
}
