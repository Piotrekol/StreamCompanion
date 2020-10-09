using CollectionManager.Enums;
using PpCalculator;
using StreamCompanionTypes;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Interfaces;
using System;
using System.Collections.Generic;
using StreamCompanionTypes.Enums;
using StreamCompanionTypes.Interfaces.Services;
using StreamCompanionTypes.Interfaces.Sources;

namespace BeatmapPpReplacements
{
    public class PpReplacements : IPlugin, ITokensSource
    {
        private const string PpFormat = "{0:0.00}";
        private Tokens.TokenSetter _tokenSetter;

        private PpCalculator.PpCalculator _ppCalculator = null;

        private ISettings _settings;

        public string Description { get; } = "";
        public string Name { get; } = nameof(PpReplacements);
        public string Author { get; } = "Piotrekol";
        public string Url { get; } = "";
        public string UpdateUrl { get; } = "";
        private readonly Dictionary<TokenMode, Dictionary<string, PpValue>> ppTokenDefinitions;

        private delegate double PpValue(string mods = "");
        enum TokenMode
        {
            Osu,
            Mania
        }

        public PpReplacements(ISettings settings)
        {
            _settings = settings;
            _tokenSetter = Tokens.CreateTokenSetter(Name);

            ppTokenDefinitions = new Dictionary<TokenMode, Dictionary<string, PpValue>>
            {
                {TokenMode.Osu,new Dictionary<string, PpValue>
                {
                    {"osu_SSPP",(mods)=>GetPp(_ppCalculator, 100d)},
                    {"osu_99.9PP",(mods)=>GetPp(_ppCalculator, 99.9d)},
                    {"osu_99PP",(mods)=>GetPp(_ppCalculator, 99d)},
                    {"osu_98PP",(mods)=>GetPp(_ppCalculator, 98d)},
                    {"osu_95PP",(mods)=>GetPp(_ppCalculator, 95d)},
                    {"osu_90PP",(mods)=>GetPp(_ppCalculator, 90d)},
                    {"osu_mSSPP",(mods)=>GetPp(_ppCalculator, 100d, mods)},
                    {"osu_m99.9PP",(mods)=>GetPp(_ppCalculator, 99.9d, mods)},
                    {"osu_m99PP",(mods)=>GetPp(_ppCalculator, 99d, mods)},
                    {"osu_m98PP",(mods)=>GetPp(_ppCalculator, 98d, mods)},
                    {"osu_m95PP",(mods)=>GetPp(_ppCalculator, 95d, mods)},
                    {"osu_m90PP",(mods)=>GetPp(_ppCalculator, 90d, mods)},
                }
                },
                {TokenMode.Mania,new Dictionary<string,PpValue>
                {
                    {"mania_1_000_000PP",(mods)=>GetPp(_ppCalculator, 0, "", 1_000_000)},
                    {"mania_990_000PP",(mods)=>GetPp(_ppCalculator, 0, "", 990_000)},
                    {"mania_950_000PP",(mods)=>GetPp(_ppCalculator, 0, "", 950_000)},
                    {"mania_900_000PP",(mods)=>GetPp(_ppCalculator, 0, "", 900_000)},
                    {"mania_800_000PP",(mods)=>GetPp(_ppCalculator, 0, "", 800_000)},
                    {"mania_700_000PP",(mods)=>GetPp(_ppCalculator, 0, "", 700_000)},
                    {"mania_600_000PP",(mods)=>GetPp(_ppCalculator, 0, "", 600_000)},
                    {"mania_m1_000_000PP",(mods)=>GetPp(_ppCalculator, 0, mods, 1_000_000)},
                    {"mania_m990_000PP",(mods)=>GetPp(_ppCalculator, 0, mods, 990_000)},
                    {"mania_m950_000PP",(mods)=>GetPp(_ppCalculator, 0, mods, 950_000)},
                    {"mania_m900_000PP",(mods)=>GetPp(_ppCalculator, 0, mods, 900_000)},
                    {"mania_m800_000PP",(mods)=>GetPp(_ppCalculator, 0, mods, 800_000)},
                    {"mania_m700_000PP",(mods)=>GetPp(_ppCalculator, 0, mods, 700_000)},
                    {"mania_m600_000PP",(mods)=>GetPp(_ppCalculator, 0, mods, 600_000)},
                }
                }
            };

            ResetTokens(TokenMode.Osu);
            ResetTokens(TokenMode.Mania);
        }

        private void ResetTokens(TokenMode tokenMode)
        {
            foreach (var tokenDefinition in ppTokenDefinitions[tokenMode])
            {
                _tokenSetter(tokenDefinition.Key, null, format: PpFormat, defaultValue: 0);
            }
        }

        public void CreateTokens(MapSearchResult map)
        {
            if (map.SearchArgs.EventType != OsuEventType.MapChange)
                return;

            if (!map.FoundBeatmaps ||
                !map.BeatmapsFound[0].IsValidBeatmap(_settings, out var mapLocation))
            {
                ResetTokens(TokenMode.Osu);
                ResetTokens(TokenMode.Mania);
                _tokenSetter("gameMode", null);
                _tokenSetter("maxCombo", -1);
                return;
            }

            var workingBeatmap = new ProcessorWorkingBeatmap(mapLocation);
            var playMode = (PlayMode)PpCalculatorHelpers.GetRulesetId(workingBeatmap.RulesetID, map.PlayMode.HasValue ? (int?)map.PlayMode : null);
            _ppCalculator = PpCalculatorHelpers.GetPpCalculator((int)playMode, mapLocation, _ppCalculator);
            _tokenSetter("gameMode", playMode.ToString());

            if (_ppCalculator == null)
            {//Ctb not supported :(
                ResetTokens(TokenMode.Osu);
                ResetTokens(TokenMode.Mania);
                _tokenSetter("maxCombo", -1);
                return;
            }

            _ppCalculator.Score = playMode == PlayMode.OsuMania
                ? 1_000_000
                : 0;
            _ppCalculator.Mods = null;
            var mods = map.Mods?.WorkingMods ?? "";
            var tokenMode = playMode == PlayMode.OsuMania
                ? TokenMode.Mania
                : TokenMode.Osu;

            foreach (var tokenDefinition in ppTokenDefinitions[tokenMode])
            {
                _tokenSetter(tokenDefinition.Key, tokenDefinition.Value(mods), format: PpFormat);
            }

            _tokenSetter("maxCombo", _ppCalculator.GetMaxCombo());

            ResetTokens(tokenMode == TokenMode.Osu ? TokenMode.Mania : TokenMode.Osu);
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