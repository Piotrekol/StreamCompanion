using System;
using System.Collections.Generic;
using System.Windows.Forms;
using CollectionManager.DataTypes;
using CollectionManager.Enums;
using osu_StreamCompanion.Code.Core;
using osu_StreamCompanion.Code.Core.DataTypes;
using osu_StreamCompanion.Code.Interfaces;
using Beatmap = osu_StreamCompanion.Code.Core.DataTypes.Beatmap;

namespace osu_StreamCompanion.Code.Modules.ModsHandler
{
    public class ModsHandler : IModule, IModParser, ISettingsProvider, IDifficultyCalculator, IMapDataReplacements
    {
        private readonly ModParser _modParser = new ModParser();
        private readonly DifficultyCalculator _difficultyCalculator = new DifficultyCalculator();
        public bool Started { get; set; }
        public void Start(ILogger logger)
        {
            Started = true;
        }

        public Mods GetMods(int modsEnum)
        {
            _modParser.Start(null);
            return (Mods)modsEnum;
        }
        public string GetModsFromEnum(int modsEnum)
        {
            return _modParser.GetModsFromEnum(modsEnum);
        }

        public void SetSettingsHandle(Settings settings)
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

        public Dictionary<string, string> GetMapReplacements(MapSearchResult map)
        {
            if (map.FoundBeatmaps)
            {
                var mods = map.Mods?.Item1 ?? Mods.Omod;
                var c = _difficultyCalculator.ApplyMods(map.BeatmapsFound[0], mods);
                var dict = new Dictionary<string, string>()
                {
                    { "!mAR!", Math.Round(c["AR"], 1).ToString(System.Globalization.CultureInfo.InvariantCulture)},
                    { "!mCS!", Math.Round(c["CS"], 1).ToString(System.Globalization.CultureInfo.InvariantCulture)},
                    { "!mOD!", Math.Round(c["OD"], 1).ToString(System.Globalization.CultureInfo.InvariantCulture)},
                    { "!mStars!", Math.Round(map.BeatmapsFound[0].Stars(PlayMode.Osu,mods) ,2)
                    .ToString(System.Globalization.CultureInfo.InvariantCulture)}
                };
                return dict;
            }

            return new Dictionary<string, string>();
        }
    }
}