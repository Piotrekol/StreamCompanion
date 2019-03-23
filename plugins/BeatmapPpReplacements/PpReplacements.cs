using CollectionManager.DataTypes;
using CollectionManager.Enums;
using PpCalculator;
using StreamCompanionTypes;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BeatmapPpReplacements
{
    public class PpReplacements : IPlugin, ITokensProvider, ISettings, IModParserGetter
    {
        private readonly SettingNames _names = SettingNames.Instance;

        private const string PpFormat = "{0:0.00}";
        private Tokens.TokenSetter _tokenGetter;

        private PpCalculator.PpCalculator _ppCalculator = null;

        private ISettingsHandler _settings;
        private string _lastShortMods = "";
        private string _lastModsStr = "None";
        private List<IModParser> _modParsers;
        private IModParser ModParser => _modParsers[0];
        public bool Started { get; set; }

        public string Description { get; } = "";
        public string Name { get; } = nameof(PpReplacements);
        public string Author { get; } = "Piotrekol";
        public string Url { get; } = "";
        public string UpdateUrl { get; } = "";

        public void Start(ILogger logger)
        {
            _tokenGetter = Tokens.CreateTokenSetter(Name);
            Started = true;
        }

        public void CreateTokens(MapSearchResult map)
        {
            foreach (var tokenkv in Tokens.AllTokens.Where(t => t.Value.PluginName == Name))
            {
                tokenkv.Value.Reset();
            }

            if (!map.FoundBeatmaps ||
                !map.BeatmapsFound[0].IsValidBeatmap(_settings, out var mapLocation)
                )
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

            _tokenGetter("GameMode", playMode.ToString());

            string mods = "";

            if (playMode == PlayMode.OsuMania)
            {
                _tokenGetter("1 000 000PP", GetPp(_ppCalculator, 0, mods, 1_000_000), format: PpFormat);
                _tokenGetter("990 000PP", GetPp(_ppCalculator, 0, mods, 990_000), format: PpFormat);
                _tokenGetter("950 000PP", GetPp(_ppCalculator, 0, mods, 950_000), format: PpFormat);
                _tokenGetter("900 000PP", GetPp(_ppCalculator, 0, mods, 900_000), format: PpFormat);
                _tokenGetter("800 000PP", GetPp(_ppCalculator, 0, mods, 800_000), format: PpFormat);
                _tokenGetter("700 000PP", GetPp(_ppCalculator, 0, mods, 700_000), format: PpFormat);
                _tokenGetter("600 000PP", GetPp(_ppCalculator, 0, mods, 600_000), format: PpFormat);
            }
            else
            {
                _tokenGetter("SSPP", GetPp(_ppCalculator, 100d), format: PpFormat);
                _tokenGetter("99.9PP", GetPp(_ppCalculator, 99.9d), format: PpFormat);
                _tokenGetter("99PP", GetPp(_ppCalculator, 99d), format: PpFormat);
                _tokenGetter("98PP", GetPp(_ppCalculator, 98d), format: PpFormat);
                _tokenGetter("95PP", GetPp(_ppCalculator, 95d), format: PpFormat);
                _tokenGetter("90PP", GetPp(_ppCalculator, 90d), format: PpFormat);
            }
            _tokenGetter("MaxCombo", _ppCalculator.GetMaxCombo());


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

            _tokenGetter("mMod", modsStr);

            if (playMode == PlayMode.OsuMania)
            {
                _tokenGetter("m1 000 000PP", GetPp(_ppCalculator, 0, mods, 1_000_000), format: PpFormat);
                _tokenGetter("m990 000PP", GetPp(_ppCalculator, 0, mods, 990_000), format: PpFormat);
                _tokenGetter("m950 000PP", GetPp(_ppCalculator, 0, mods, 950_000), format: PpFormat);
                _tokenGetter("m900 000PP", GetPp(_ppCalculator, 0, mods, 900_000), format: PpFormat);
                _tokenGetter("m800 000PP", GetPp(_ppCalculator, 0, mods, 800_000), format: PpFormat);
                _tokenGetter("m700 000PP", GetPp(_ppCalculator, 0, mods, 700_000), format: PpFormat);
                _tokenGetter("m600 000PP", GetPp(_ppCalculator, 0, mods, 600_000), format: PpFormat);
            }
            else
            {
                _tokenGetter("mSSPP", GetPp(_ppCalculator, 100d, mods), format: PpFormat);
                _tokenGetter("m99.9PP", GetPp(_ppCalculator, 99.9d, mods), format: PpFormat);
                _tokenGetter("m99PP", GetPp(_ppCalculator, 99d, mods), format: PpFormat);
                _tokenGetter("m98PP", GetPp(_ppCalculator, 98d, mods), format: PpFormat);
                _tokenGetter("m95PP", GetPp(_ppCalculator, 95d, mods), format: PpFormat);
                _tokenGetter("m90PP", GetPp(_ppCalculator, 90d, mods), format: PpFormat);
            }
        }
        private double GetPp(PpCalculator.PpCalculator ppCalculator, double acc, string mods = "", int score = 0)
        {
            ppCalculator.Accuracy = acc;
            ppCalculator.Score = score;

            _ppCalculator.Mods = mods.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            try
            {
                return ppCalculator.Calculate();
            }
            catch (ArgumentException e) when (e.Message.Contains("Invalid mod provided"))
            {
            }

            return -1;
        }


        private string GetOsuDir()
        {
            return _settings.Get<string>(_names.MainOsuDirectory);
        }

        public void SetSettingsHandle(ISettingsHandler settings)
        {
            this._settings = settings;
        }

        public void SetModParserHandle(List<IModParser> modParser)
        {
            _modParsers = modParser;
        }
    }
}