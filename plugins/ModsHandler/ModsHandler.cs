using CollectionManager.DataTypes;
using CollectionManager.Enums;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Enums;
using StreamCompanionTypes.Interfaces;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Beatmap = StreamCompanionTypes.DataTypes.Beatmap;

namespace ModsHandler
{
    public class ModsHandler : IPlugin, IModParser, ISettingsProvider, IDifficultyCalculator, IMapDataReplacements
    {
        private readonly ModParser _modParser = new ModParser();
        private readonly DifficultyCalculator _difficultyCalculator = new DifficultyCalculator();
        public bool Started { get; set; }


        public string Description { get; } = "";
        public string Name { get; } = nameof(ModsHandler);
        public string Author { get; } = "Piotrekol";
        public string Url { get; } = "";
        public string UpdateUrl { get; } = "";

        public void Start(ILogger logger)
        {
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
        
        public Tokens GetMapReplacements(MapSearchResult map)
        {
            Tokens dict;
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
                return new Tokens
                {
                    {"mods", new TokenWithFormat( map.Mods?.ShownMods )},
                    {"mAR", new TokenWithFormat( Math.Round(c["AR"], 1))},
                    {"mCS", new TokenWithFormat( Math.Round(c["CS"], 1))},
                    {"mOD", new TokenWithFormat( Math.Round(c["OD"], 1))},
                    {"mHP", new TokenWithFormat( c["HP"], format:"{0:0.##}" )},
                    {"mStars", new TokenWithFormat( Math.Round(foundMap.Stars(PlayMode.Osu, mods), 2))},
                    {"mBpm", new TokenWithFormat(bpm )},
                    {"mMaxBpm", new TokenWithFormat(c["MaxBpm"], format:"{0:0}")},
                    {"mMinBpm", new TokenWithFormat(c["MinBpm"], format:"{0:0}")},
                };
            }
            else
            {
                return new Tokens
                {
                    {"mods", new Token(null)},
                    {"mAR", new Token(null)},
                    {"mCS", new Token(null)},
                    {"mOD", new Token(null)},
                    {"mHP", new Token(null)},
                    {"mStars", new Token(null)},
                    {"mBpm", new Token(null)},
                    {"mMaxBpm", new Token(null)},
                    {"mMinBpm", new Token(null)},
                };
            }
        }
    }
}