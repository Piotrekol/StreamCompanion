using CollectionManager.DataTypes;
using CollectionManager.Enums;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Interfaces;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using StreamCompanionTypes.Enums;
using StreamCompanionTypes.Interfaces.Services;
using StreamCompanionTypes.Interfaces.Sources;
using Beatmap = StreamCompanionTypes.DataTypes.Beatmap;

namespace ModsHandler
{
    public class ModsHandler : IPlugin, IModParser, IDifficultyCalculator, ITokensSource, ISettingsSource
    {
        private readonly ModParser _modParser;
        private readonly DifficultyCalculator _difficultyCalculator = new DifficultyCalculator();
        private Tokens.TokenSetter _tokenSetter;

        public string Description { get; } = "";
        public string Name { get; } = nameof(ModsHandler);
        public string Author { get; } = "Piotrekol";
        public string Url { get; } = "";
        public string UpdateUrl { get; } = "";
        public string SettingGroup => _modParser.SettingGroup;

        public ModsHandler(ISettings settings)
        {
            _modParser = new ModParser(settings);
            _tokenSetter = Tokens.CreateTokenSetter(Name);
        }

        public IModsEx GetModsFromEnum(int modsEnum)
        {
            return _modParser.GetModsFromEnum(modsEnum);
        }

        public string GetModsFromEnum(int modsEnum, bool shortMod = false)
        {
            return _modParser.GetModsFromEnum(modsEnum, shortMod);
        }

        public IBeatmap ApplyMods(IBeatmap map, Mods mods)
        {
            var c = _difficultyCalculator.ApplyMods(map, mods);
            var retMap = (Beatmap)map.Clone();
            retMap.ApproachRate = c["AR"];
            retMap.CircleSize = c["CS"];
            retMap.OverallDifficulty = c["OD"];
            return retMap;
        }

        public Task CreateTokensAsync(IMapSearchResult map, CancellationToken cancellationToken)
        {
            if (map.BeatmapsFound.Any())
            {
                var foundMap = map.BeatmapsFound[0];
                var mods = map.Mods?.Mods ?? Mods.Omod;
                var c = _difficultyCalculator.ApplyMods(foundMap, mods);

                if ((mods & Mods.Nc) != 0)
                {
                    mods -= Mods.Nc;
                    mods |= Mods.Dt;
                }
                var bpm = Math.Abs(c["MinBpm"] - c["MaxBpm"]) < float.Epsilon ? c["MinBpm"].ToString("0") : $"{c["MinBpm"]:0}-{c["MaxBpm"]:0} ({c["MainBpm"]:0})";

                _tokenSetter("mods", map.Mods?.ShownMods);
                _tokenSetter("modsEnum", map.Mods?.Mods);
                _tokenSetter("mAR", Math.Round(c["AR"], 2));
                _tokenSetter("mCS", Math.Round(c["CS"], 2));
                _tokenSetter("mOD", Math.Round(c["OD"], 2));
                _tokenSetter("mHP", c["HP"], format: "{0:0.##}");
                _tokenSetter("mStars", Math.Round(foundMap.Stars(map.PlayMode ?? PlayMode.Osu, mods), 2));
                _tokenSetter("mBpm", bpm);
                _tokenSetter("mMaxBpm", c["MaxBpm"], format: "{0:0}");
                _tokenSetter("mMinBpm", c["MinBpm"], format: "{0:0}");
                _tokenSetter("mMainBpm", c["MainBpm"], format: "{0:0}");
            }
            else
            {
                foreach (var tokenkv in Tokens.AllTokens.Where(t => t.Value.PluginName == Name))
                {
                    tokenkv.Value.Reset();
                }
            }

            return Task.CompletedTask;
        }

        public void Free()
        {
            _modParser.Free();
        }

        public object GetUiSettings()
            => _modParser.GetUiSettings();
    }
}