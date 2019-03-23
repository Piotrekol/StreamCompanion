using CollectionManager.DataTypes;
using CollectionManager.Enums;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Enums;
using StreamCompanionTypes.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Beatmap = StreamCompanionTypes.DataTypes.Beatmap;

namespace ModsHandler
{
    public class ModsHandler : IPlugin, IModParser, ISettingsProvider, IDifficultyCalculator, ITokensProvider
    {
        private readonly ModParser _modParser = new ModParser();
        private readonly DifficultyCalculator _difficultyCalculator = new DifficultyCalculator();
        private Tokens.TokenSetter _tokenGetter;

        public bool Started { get; set; }


        public string Description { get; } = "";
        public string Name { get; } = nameof(ModsHandler);
        public string Author { get; } = "Piotrekol";
        public string Url { get; } = "";
        public string UpdateUrl { get; } = "";

        public void Start(ILogger logger)
        {
            _tokenGetter = Tokens.CreateTokenSetter(Name);
            Started = true;
        }

        public Mods GetMods(int modsEnum)
        {
            _modParser.Start(null);
            return (Mods)modsEnum;
        }
        public ModsEx GetModsFromEnum(int modsEnum)
        {
            return _modParser.GetModsFromEnum(modsEnum);
        }

        public string GetModsFromEnum(int modsEnum, bool shortMod = false)
        {
            return _modParser.GetModsFromEnum(modsEnum, shortMod);
        }

        public void SetSettingsHandle(ISettingsHandler settings)
        {
            _modParser.SetSettingsHandle(settings);
        }

        public string SettingGroup => _modParser.SettingGroup;

        public void Free()
        {
            _modParser.Free();
        }

        public UserControl GetUiSettings()
        {
            return _modParser.GetUiSettings();
        }

        public Beatmap ApplyMods(Beatmap map, Mods mods)
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

                _tokenGetter("mods", map.Mods?.ShownMods);
                _tokenGetter("mAR", Math.Round(c["AR"], 1));
                _tokenGetter("mCS", Math.Round(c["CS"], 1));
                _tokenGetter("mOD", Math.Round(c["OD"], 1));
                _tokenGetter("mHP", c["HP"], format: "{0:0.##}");
                _tokenGetter("mStars", Math.Round(foundMap.Stars(PlayMode.Osu, mods), 2));
                _tokenGetter("mBpm", bpm);
                _tokenGetter("mMaxBpm", c["MaxBpm"], format: "{0:0}");
                _tokenGetter("mMinBpm", c["MinBpm"], format: "{0:0}");
            }
            else
            {
                foreach (var tokenkv in Tokens.AllTokens.Where(t => t.Value.PluginName == Name))
                {
                    tokenkv.Value.Reset();
                }
            }
        }
    }
}