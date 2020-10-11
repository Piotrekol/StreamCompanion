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
using Beatmap = StreamCompanionTypes.DataTypes.Beatmap;

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
                dict = map.BeatmapsFound[0].GetTokens();

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
                dict = ((Beatmap)null).GetTokens(true);
                OsuFileLocationToken.Value = string.Empty;
            }

            foreach (var token in dict)
            {
                _tokenSetter(token.Key, token.Value);
            }

            return Task.CompletedTask;
        }
    }
}
