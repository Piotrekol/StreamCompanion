using CollectionManager.Enums;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PpCalculatorTypes;
using StreamCompanion.Common;
using StreamCompanionTypes.Attributes;
using StreamCompanionTypes.Enums;
using StreamCompanionTypes.Interfaces.Services;
using StreamCompanionTypes.Interfaces.Sources;

namespace BeatmapPpReplacements
{
    [SCPlugin(Name, "Calculates performance values for current map with specific accuracies", Consts.SCPLUGIN_AUTHOR, Consts.SCPLUGIN_BASEURL)]
    public class PpReplacements : IPlugin, ITokensSource
    {
        public const string Name = "PPerformance tokens";
        private const string PpFormat = "{0:0.00}";
        private Tokens.TokenSetter _tokenSetter;
        private ISettings _settings;
        private IPpCalculator _ppCalculator = null;
        private readonly ILogger _logger;
        private readonly IToken _strainsToken;
        public static ConfigEntry StrainsAmount = new ConfigEntry("StrainsAmount", (int?)100);
        private readonly Dictionary<TokenMode, Dictionary<string, PpValue>> ppTokenDefinitions;

        private delegate double PpValue(CancellationToken cancellationToken, IPpCalculator ppCalculator, string mods = "");
        enum TokenMode
        {
            Osu,
            Mania
        }

        public PpReplacements(ISettings settings, ILogger logger)
        {
            _logger = logger;
            _settings = settings;
            _tokenSetter = Tokens.CreateTokenSetter(Name);
            _strainsToken = _tokenSetter("mapStrains", new Lazy<object>(() => new Dictionary<int, double>()), TokenType.Normal, ",", new Lazy<object>(() => new Dictionary<int, double>()));
            ppTokenDefinitions = new Dictionary<TokenMode, Dictionary<string, PpValue>>
            {
                {TokenMode.Osu,new Dictionary<string, PpValue>
                {
                    {"osu_SSPP",(c, ppCalculator, mods)=>GetPp(c,ppCalculator, 100d)},
                    {"osu_99_9PP",(c, ppCalculator, mods)=>GetPp(c,ppCalculator, 99.9d)},
                    {"osu_99PP",(c, ppCalculator, mods)=>GetPp(c,ppCalculator, 99d)},
                    {"osu_98PP",(c, ppCalculator, mods)=>GetPp(c,ppCalculator, 98d)},
                    {"osu_97PP",(c, ppCalculator, mods)=>GetPp(c,ppCalculator, 97d)},
                    {"osu_96PP",(c, ppCalculator, mods)=>GetPp(c,ppCalculator, 96d)},
                    {"osu_95PP",(c, ppCalculator, mods)=>GetPp(c,ppCalculator, 95d)},
                    {"osu_90PP",(c, ppCalculator, mods)=>GetPp(c,ppCalculator, 90d)},
                    {"osu_mSSPP",(c, ppCalculator, mods)=>GetPp(c,ppCalculator, 100d, mods)},
                    {"osu_m99_9PP",(c, ppCalculator, mods)=>GetPp(c,ppCalculator, 99.9d, mods)},
                    {"osu_m99PP",(c, ppCalculator, mods)=>GetPp(c,ppCalculator, 99d, mods)},
                    {"osu_m98PP",(c, ppCalculator, mods)=>GetPp(c,ppCalculator, 98d, mods)},
                    {"osu_m97PP",(c, ppCalculator, mods)=>GetPp(c,ppCalculator, 97d, mods)},
                    {"osu_m96PP",(c, ppCalculator, mods)=>GetPp(c,ppCalculator, 96d, mods)},
                    {"osu_m95PP",(c, ppCalculator, mods)=>GetPp(c,ppCalculator, 95d, mods)},
                    {"osu_m90PP",(c, ppCalculator, mods)=>GetPp(c,ppCalculator, 90d, mods)},
                }
                },
                {TokenMode.Mania,new Dictionary<string,PpValue>
                {
                    {"mania_1_000_000PP",(c,ppCalculator,mods)=>GetPp(c,ppCalculator, 100, "")},
                    {"mania_m1_000_000PP",(c,ppCalculator,mods)=>GetPp(c,ppCalculator, 100, mods)},
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
                _tokenSetter(tokenDefinition.Key, new Lazy<object>(() => 0d), format: PpFormat, defaultValue: 0d);
            }
        }

        public async Task CreateTokensAsync(IMapSearchResult map, CancellationToken cancellationToken)
        {
            if (map.SearchArgs.EventType != OsuEventType.MapChange)
                return;

            _ppCalculator = await map.GetPpCalculator(cancellationToken);

            if (_ppCalculator is null)
            {
                ResetTokens(TokenMode.Osu);
                ResetTokens(TokenMode.Mania);
                _tokenSetter("gameMode", null);
                _tokenSetter("maxCombo", -1);
                return;
            }

            _ = Task.Run(() =>
            {
                var ppCalculator = (IPpCalculator)_ppCalculator?.Clone();
                try
                {
                    var strains = SmoothStrains(ppCalculator.GetStrainValues(_settings.Get<int?>(StrainsAmount) ?? 100), 1);
                    _strainsToken.Value = new Lazy<object>(() => strains);
                }
                catch (Exception exception)
                {
                    _logger.Log("mapStrains calculation failed", LogLevel.Warning);
                    _logger.Log(exception, LogLevel.Warning);
                    _strainsToken.Value = new Lazy<object>(() => null);
                }
            });
            var playMode = (PlayMode)_ppCalculator.RulesetId;
            _tokenSetter("gameMode", playMode.ToString());

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

                _tokenSetter(tokenDefinition.Key, new Lazy<object>(
                        () => tokenDefinition.Value(cancellationToken, (IPpCalculator)_ppCalculator?.Clone(), mods)
                    ), format: PpFormat
                );
            }

            _tokenSetter("maxCombo", _ppCalculator.GetMaxCombo());
            ResetTokens(tokenMode == TokenMode.Osu ? TokenMode.Mania : TokenMode.Osu);
        }

        private static Dictionary<int, double> SmoothStrains(Dictionary<int, double> strains, int windowRadius)
        {
            KeyValuePair<int, double>[] orderedStrains = strains.OrderBy(strain => strain.Key).ToArray();
            Dictionary<int, double> smoothedStrains = new(orderedStrains.Length);
            for (int strainIndex = 0; strainIndex < orderedStrains.Length; strainIndex++)
            {
                double sum = 0;
                int count = 0;
                for (int offset = -windowRadius; offset <= windowRadius; offset++)
                {
                    int neighborIndex = strainIndex + offset;
                    if (neighborIndex < 0 || neighborIndex >= orderedStrains.Length)
                        continue;

                    sum += orderedStrains[neighborIndex].Value;
                    count++;
                }

                smoothedStrains[orderedStrains[strainIndex].Key] = Math.Max(orderedStrains[strainIndex].Value, sum / count);
            }

            return smoothedStrains;
        }

        private double GetPp(CancellationToken cancellationToken, IPpCalculator ppCalculator, double acc, string mods = "")
        {
            ppCalculator.Mods = mods.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            ppCalculator.Accuracy = acc;
            return Math.Round(ppCalculator.Calculate(cancellationToken).Total, 3);
        }
    }
}