using CollectionManager.DataTypes;
using CollectionManager.Enums;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Interfaces;
using System;
using System.Linq;
using System.Windows.Forms;
using Beatmap = StreamCompanionTypes.DataTypes.Beatmap;

namespace ModsHandler
{
    public class ModsHandler : IPlugin, IModParser, IDifficultyCalculator, ITokensProvider, ISettingsProvider
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

        public ModsHandler(ISettingsHandler settings)
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

        public void CreateTokens(MapSearchResult map)
        {

            if (map.FoundBeatmaps)
            {
                var foundMap = map.BeatmapsFound[0];
                var mods = map.Mods?.Mods ?? Mods.Omod;
                var c = _difficultyCalculator.ApplyMods(foundMap, mods);

                if ((mods & Mods.Nc) != 0)
                {
                    mods -= Mods.Nc;
                    mods |= Mods.Dt;
                }
                var bpm = Math.Abs(c["MinBpm"] - c["MaxBpm"]) < 0.95 ? c["MinBpm"].ToString("0") : string.Format("{0:0}-{1:0}", c["MinBpm"], c["MaxBpm"]);

                _tokenSetter("mods", map.Mods?.ShownMods);
                _tokenSetter("mAR", Math.Round(c["AR"], 2));
                _tokenSetter("mCS", Math.Round(c["CS"], 2));
                _tokenSetter("mOD", Math.Round(c["OD"], 2));
                _tokenSetter("mOD-Real", Math.Round(c["OD-Real"], 2));
                _tokenSetter("mHP", c["HP"], format: "{0:0.##}");
                _tokenSetter("mStars", Math.Round(foundMap.Stars(PlayMode.Osu, mods), 2));
                _tokenSetter("mBpm", bpm);
                _tokenSetter("mMaxBpm", c["MaxBpm"], format: "{0:0}");
                _tokenSetter("mMinBpm", c["MinBpm"], format: "{0:0}");
            }
            else
            {
                foreach (var tokenkv in Tokens.AllTokens.Where(t => t.Value.PluginName == Name))
                {
                    tokenkv.Value.Reset();
                }
            }
        }

        public void Free()
        {
            _modParser.Free();
        }

        public object GetUiSettings()
            => _modParser.GetUiSettings();
    }
}
