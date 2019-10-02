using CollectionManager.Enums;
using PpCalculator;
using StreamCompanionTypes;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Interfaces;
using System;
using System.Linq;

namespace BeatmapPpReplacements
{
    public class PpReplacements : IPlugin, ITokensProvider
    {
        private const string PpFormat = "{0:0.00}";
        private Tokens.TokenSetter _tokenSetter;

        private PpCalculator.PpCalculator _ppCalculator = null;

        private ISettingsHandler _settings;
        private string _lastShortMods = "";
        private string _lastModsStr = "None";



        public string Description { get; } = "";
        public string Name { get; } = nameof(PpReplacements);
        public string Author { get; } = "Piotrekol";
        public string Url { get; } = "";
        public string UpdateUrl { get; } = "";

        public PpReplacements(ISettingsHandler settings)
        {
            _settings = settings;
            _tokenSetter = Tokens.CreateTokenSetter(Name);
        }

        public void CreateTokens(MapSearchResult map)
        {
            foreach (var tokenkv in Tokens.AllTokens.Where(t => t.Value.PluginName == Name))
            {
                tokenkv.Value.Reset();
            }

            if (!map.FoundBeatmaps ||
                !map.BeatmapsFound[0].IsValidBeatmap(_settings, out var mapLocation))
                return;


            var workingBeatmap = new ProcessorWorkingBeatmap(mapLocation);

            var playMode = (PlayMode)PpCalculatorHelpers.GetRulesetId(workingBeatmap.RulesetID, map.PlayMode.HasValue ? (int?)map.PlayMode : null);

            _ppCalculator = PpCalculatorHelpers.GetPpCalculator((int)playMode, mapLocation, _ppCalculator);

            if (_ppCalculator == null)
                return;//Ctb not supported :(

            if (playMode == PlayMode.OsuMania)
                _ppCalculator.Score = 1_000_000;
            else
                _ppCalculator.Score = 0;

            _ppCalculator.Mods = null;

            _tokenSetter("GameMode", playMode.ToString());

            string mods = "";

            if (playMode == PlayMode.OsuMania)
            {
                _tokenSetter("1 000 000PP", GetPp(_ppCalculator, 0, mods, 1_000_000), format: PpFormat);
                _tokenSetter("990 000PP", GetPp(_ppCalculator, 0, mods, 990_000), format: PpFormat);
                _tokenSetter("950 000PP", GetPp(_ppCalculator, 0, mods, 950_000), format: PpFormat);
                _tokenSetter("900 000PP", GetPp(_ppCalculator, 0, mods, 900_000), format: PpFormat);
                _tokenSetter("800 000PP", GetPp(_ppCalculator, 0, mods, 800_000), format: PpFormat);
                _tokenSetter("700 000PP", GetPp(_ppCalculator, 0, mods, 700_000), format: PpFormat);
                _tokenSetter("600 000PP", GetPp(_ppCalculator, 0, mods, 600_000), format: PpFormat);
            }
            else
            {
                _tokenSetter("SSPP", GetPp(_ppCalculator, 100d), format: PpFormat);
                _tokenSetter("99.9PP", GetPp(_ppCalculator, 99.9d), format: PpFormat);
                _tokenSetter("99PP", GetPp(_ppCalculator, 99d), format: PpFormat);
                _tokenSetter("98PP", GetPp(_ppCalculator, 98d), format: PpFormat);
                _tokenSetter("95PP", GetPp(_ppCalculator, 95d), format: PpFormat);
                _tokenSetter("90PP", GetPp(_ppCalculator, 90d), format: PpFormat);
            }
            _tokenSetter("MaxCombo", _ppCalculator.GetMaxCombo());


            string modsStr;
            if (map.Action == OsuStatus.Playing || map.Action == OsuStatus.Watching)
            {
                mods = map.Mods?.WorkingMods ?? "";
                modsStr = map.Mods?.ShownMods ?? "";
                _lastShortMods = mods;
                _lastModsStr = modsStr;
            }
            else
            {
                mods = _lastShortMods;
                modsStr = _lastModsStr;
            }

            _tokenSetter("mMod", modsStr);

            if (playMode == PlayMode.OsuMania)
            {
                _tokenSetter("m1 000 000PP", GetPp(_ppCalculator, 0, mods, 1_000_000), format: PpFormat);
                _tokenSetter("m990 000PP", GetPp(_ppCalculator, 0, mods, 990_000), format: PpFormat);
                _tokenSetter("m950 000PP", GetPp(_ppCalculator, 0, mods, 950_000), format: PpFormat);
                _tokenSetter("m900 000PP", GetPp(_ppCalculator, 0, mods, 900_000), format: PpFormat);
                _tokenSetter("m800 000PP", GetPp(_ppCalculator, 0, mods, 800_000), format: PpFormat);
                _tokenSetter("m700 000PP", GetPp(_ppCalculator, 0, mods, 700_000), format: PpFormat);
                _tokenSetter("m600 000PP", GetPp(_ppCalculator, 0, mods, 600_000), format: PpFormat);
            }
            else
            {
                _tokenSetter("mSSPP", GetPp(_ppCalculator, 100d, mods), format: PpFormat);
                _tokenSetter("m99.9PP", GetPp(_ppCalculator, 99.9d, mods), format: PpFormat);
                _tokenSetter("m99PP", GetPp(_ppCalculator, 99d, mods), format: PpFormat);
                _tokenSetter("m98PP", GetPp(_ppCalculator, 98d, mods), format: PpFormat);
                _tokenSetter("m95PP", GetPp(_ppCalculator, 95d, mods), format: PpFormat);
                _tokenSetter("m90PP", GetPp(_ppCalculator, 90d, mods), format: PpFormat);
            }
        }
        private double GetPp(PpCalculator.PpCalculator ppCalculator, double acc, string mods = "", int score = 0)
        {
            ppCalculator.Accuracy = acc;
            ppCalculator.Score = score;

            _ppCalculator.Mods = mods.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            return ppCalculator.Calculate();
        }
    }
}