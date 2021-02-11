using CollectionManager.Enums;
using PpCalculator;
using StreamCompanionTypes;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

        private delegate double PpValue(CancellationToken cancellationToken, string mods = "");
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
                    {"osu_SSPP",(c,mods)=>GetPp(c,_ppCalculator, 100d)},
                    {"osu_99_9PP",(c,mods)=>GetPp(c,_ppCalculator, 99.9d)},
                    {"osu_99PP",(c,mods)=>GetPp(c,_ppCalculator, 99d)},
                    {"osu_98PP",(c,mods)=>GetPp(c,_ppCalculator, 98d)},
                    {"osu_95PP",(c,mods)=>GetPp(c,_ppCalculator, 95d)},
                    {"osu_90PP",(c,mods)=>GetPp(c,_ppCalculator, 90d)},
                    {"osu_mSSPP",(c,mods)=>GetPp(c,_ppCalculator, 100d, mods)},
                    {"osu_m99_9PP",(c,mods)=>GetPp(c,_ppCalculator, 99.9d, mods)},
                    {"osu_m99PP",(c,mods)=>GetPp(c,_ppCalculator, 99d, mods)},
                    {"osu_m98PP",(c,mods)=>GetPp(c,_ppCalculator, 98d, mods)},
                    {"osu_m95PP",(c,mods)=>GetPp(c,_ppCalculator, 95d, mods)},
                    {"osu_m90PP",(c,mods)=>GetPp(c,_ppCalculator, 90d, mods)},
                }
                },
                {TokenMode.Mania,new Dictionary<string,PpValue>
                {
                    {"mania_1_000_000PP",(c,mods)=>GetPp(c,_ppCalculator, 0, "", 1_000_000)},
                    {"mania_990_000PP",(c,mods)=>GetPp(c,_ppCalculator, 0, "", 990_000)},
                    {"mania_950_000PP",(c,mods)=>GetPp(c,_ppCalculator, 0, "", 950_000)},
                    {"mania_900_000PP",(c,mods)=>GetPp(c,_ppCalculator, 0, "", 900_000)},
                    {"mania_800_000PP",(c,mods)=>GetPp(c,_ppCalculator, 0, "", 800_000)},
                    {"mania_700_000PP",(c,mods)=>GetPp(c,_ppCalculator, 0, "", 700_000)},
                    {"mania_600_000PP",(c,mods)=>GetPp(c,_ppCalculator, 0, "", 600_000)},
                    {"mania_m1_000_000PP",(c,mods)=>GetPp(c,_ppCalculator, 0, mods, 1_000_000)},
                    {"mania_m990_000PP",(c,mods)=>GetPp(c,_ppCalculator, 0, mods, 990_000)},
                    {"mania_m950_000PP",(c,mods)=>GetPp(c,_ppCalculator, 0, mods, 950_000)},
                    {"mania_m900_000PP",(c,mods)=>GetPp(c,_ppCalculator, 0, mods, 900_000)},
                    {"mania_m800_000PP",(c,mods)=>GetPp(c,_ppCalculator, 0, mods, 800_000)},
                    {"mania_m700_000PP",(c,mods)=>GetPp(c,_ppCalculator, 0, mods, 700_000)},
                    {"mania_m600_000PP",(c,mods)=>GetPp(c,_ppCalculator, 0, mods, 600_000)},
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

        public Task CreateTokensAsync(IMapSearchResult map, CancellationToken cancellationToken)
        {
            if (map.SearchArgs.EventType != OsuEventType.MapChange)
                return Task.CompletedTask;

            if (!map.BeatmapsFound.Any() ||
                !map.BeatmapsFound[0].IsValidBeatmap(_settings, out var mapLocation))
            {
                ResetTokens(TokenMode.Osu);
                ResetTokens(TokenMode.Mania);
                _tokenSetter("gameMode", null);
                _tokenSetter("maxCombo", -1);
                return Task.CompletedTask;
            }

            var playMode = (PlayMode)PpCalculatorHelpers.GetRulesetId((int)map.BeatmapsFound[0].PlayMode, map.PlayMode.HasValue ? (int?)map.PlayMode : null);
            _ppCalculator = PpCalculatorHelpers.GetPpCalculator((int)playMode, mapLocation, _ppCalculator);
            _tokenSetter("gameMode", playMode.ToString());

            if (_ppCalculator == null)
            {//Ctb not supported :(
                ResetTokens(TokenMode.Osu);
                ResetTokens(TokenMode.Mania);
                _tokenSetter("maxCombo", -1);
                return Task.CompletedTask;
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
                cancellationToken.ThrowIfCancellationRequested();
                _tokenSetter(tokenDefinition.Key, tokenDefinition.Value(cancellationToken, mods), format: PpFormat);
            }

            _tokenSetter("maxCombo", _ppCalculator.GetMaxCombo());

            ResetTokens(tokenMode == TokenMode.Osu ? TokenMode.Mania : TokenMode.Osu);
            return Task.CompletedTask;
        }

        private double GetPp(CancellationToken cancellationToken, PpCalculator.PpCalculator ppCalculator, double acc, string mods = "", int score = 0)
        {
            ppCalculator.Accuracy = acc;
            ppCalculator.Score = score;

            _ppCalculator.Mods = mods.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            return ppCalculator.Calculate(cancellationToken);
        }
    }
}