using System;
using System.Collections.Generic;
using System.Linq;
using osu.Game.Rulesets;
using osu.Game.Rulesets.Mania;
using osu.Game.Rulesets.Mania.Objects;
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
            // One judgement per normal note. Two judgements per hold note (head + tail).
            var totalHits = hitObjects.Count + hitObjects.Count(ho => ho is HoldNote);

            if (countMeh != null || countKatu != null || countGood != null || hit300 != null)
            {
                int countPerfect = Gekis ?? totalHits - (countMiss + (countMeh ?? 0) + (countKatu ?? 0) + (countGood ?? 0) + (hit300 ?? 0));

                return new Dictionary<HitResult, int>
                {
                    [HitResult.Perfect] = countPerfect,
                    [HitResult.Great] = hit300 ?? 0,
                    [HitResult.Good] = countKatu ?? 0,
                    [HitResult.Ok] = countGood ?? 0,
                    [HitResult.Meh] = countMeh ?? 0,
                    [HitResult.Miss] = countMiss
                };
            }

            // Let Great=Perfect=6, Good=4, Ok=2, Meh=1, Miss=0. The total should be this.
            var targetTotal = (int)Math.Round(accuracy * totalHits * 6);

            // Start by assuming every non miss is a meh
            // This is how much increase is needed by the rest
            int remainingHits = totalHits - countMiss;
            int delta = targetTotal - remainingHits;

            // Each great and perfect increases total by 5 (great-meh=5)
            // There IS A difference in accuracy between them. Assume all perfect.
            int greatsAndPerfects = Math.Min(delta / 5, remainingHits);
            int countGreat = 0;
            int perfects = greatsAndPerfects;
            delta -= (countGreat + perfects) * 5;
            remainingHits -= countGreat + perfects;

            // Each good increases total by 3 (good-meh=3).
            countGood = Math.Min(delta / 3, remainingHits);
            delta -= countGood.Value * 3;
            remainingHits -= countGood.Value;

            // Each ok increases total by 1 (ok-meh=1).
            int countOk = delta;
            remainingHits -= countOk;

            // Everything else is a meh, as initially assumed.
            countMeh = remainingHits;

            return new Dictionary<HitResult, int>
            {
                { HitResult.Perfect, perfects },
                { HitResult.Great, countGreat },
                { HitResult.Ok, countOk },
                { HitResult.Good, countGood.Value },
                { HitResult.Meh, countMeh.Value },
                { HitResult.Miss, countMiss }
            };
        }

        protected override double GetAccuracy(Dictionary<HitResult, int> statistics) => 0;
    }
}