using System.Collections.Generic;
using osu.Game.Rulesets;
using osu.Game.Rulesets.Mania;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Scoring;

namespace PpCalculator
{
    public class ManiaCalculator : PpCalculator
    {
        protected override Ruleset Ruleset { get; } = new ManiaRuleset();

        protected override int GetMaxCombo(IReadOnlyList<HitObject> hitObjects) => 0;

        protected override Dictionary<HitResult, int> GenerateHitResults(double accuracy, IReadOnlyList<HitObject> hitObjects, int countMiss, int? countMeh, int? countGood, int? countKatu = null, int? hit300 = null)
        {
            if (!(countMeh.HasValue && countGood.HasValue && countKatu.HasValue && hit300.HasValue))
                return new Dictionary<HitResult, int>
                {
                    { HitResult.Perfect, hitObjects.Count },
                    { HitResult.Great, 0 },
                    { HitResult.Ok, 0 },
                    { HitResult.Good, 0 },
                    { HitResult.Meh, 0 },
                    { HitResult.Miss, 0 }
                };

            var otherCounts = countMiss + countMeh.Value + countGood.Value + countKatu.Value + hit300.Value;
            return new Dictionary<HitResult, int>
            {
                { HitResult.Perfect, hitObjects.Count - otherCounts },
                { HitResult.Great, hit300.Value },
                { HitResult.Ok, countGood.Value },
                { HitResult.Good, countKatu.Value },
                { HitResult.Meh, countMeh.Value },
                { HitResult.Miss, countMiss }
            };
        }

        protected override double GetAccuracy(Dictionary<HitResult, int> statistics) => 0;
    }
}