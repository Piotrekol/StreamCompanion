using System.Collections.Generic;
using System.Windows.Forms;
using osu_StreamCompanion.Code.Core;
using osu_StreamCompanion.Code.Core.DataTypes;
using osu_StreamCompanion.Code.Interfeaces;

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

        public EMods GetMods(int modsEnum)
        {
            _modParser.Start(null);
            return (EMods)modsEnum;
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

        public Beatmap ApplyMods(Beatmap map, EMods mods)
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
                var c = _difficultyCalculator.ApplyMods(map.BeatmapsFound[0], map.Mods.Item1);
                var dict = new Dictionary<string, string>()
                {
                    { "!mAR!", c["AR"].ToString(System.Globalization.CultureInfo.InvariantCulture)},
                    { "!mCS!", c["CS"].ToString(System.Globalization.CultureInfo.InvariantCulture)},
                    { "!mOD!", c["OD"].ToString(System.Globalization.CultureInfo.InvariantCulture)}
                };
                return dict;
            }

            return new Dictionary<string, string>();
        }
    }
}