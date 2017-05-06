using System.Windows.Forms;
using osu_StreamCompanion.Code.Core;
using osu_StreamCompanion.Code.Core.DataTypes;
using osu_StreamCompanion.Code.Interfeaces;

namespace osu_StreamCompanion.Code.Modules.ModsHandler
{
    public class ModsHandler : IModule, IModParser, ISettingsProvider, IDifficultyCalculator
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

        public void ApplyMods(Beatmap map, EMods mods)
        {
            var c = _difficultyCalculator.ApplyMods(map, mods);
            map.ApproachRate = c["AR"];
            map.CircleSize = c["CS"];
            map.OverallDifficulty = c["OD"];
        }
    }
}