using osu.Game.Rulesets;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Osu.Objects;
using osu.Game.Rulesets.Scoring;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PpCalculator;

public class OsuCalculator : PpCalculator
{
    protected override Ruleset Ruleset { get; } = new osu.Game.Rulesets.Osu.OsuRuleset();

    protected override int GetMaxCombo(IReadOnlyList<HitObject> hitObjects) =>
        hitObjects.Count + hitObjects.OfType<Slider>().Sum(s => s.NestedHitObjects.Count - 1);

    protected override Dictionary<HitResult, int> GenerateHitResults(double accuracy, IReadOnlyList<HitObject> hitObjects, int countMiss, int? countMeh, int? countGood, int? countKatu = null, int? hit300 = null)
    {
        int countGreat;

        int totalResultCount = hitObjects.Count;

        if (countMeh != null || countGood != null)
        {
            countGreat = totalResultCount - (countGood ?? 0) - (countMeh ?? 0) - countMiss;
        }
        else
        {
            // Total result count excluding countMiss
            int relevantResultCount = totalResultCount - countMiss;

            // Accuracy excluding countMiss. We need that because we're trying to achieve target accuracy without touching countMiss
            // So it's better to pretened that there were 0 misses in the 1st place
            double relevantAccuracy = accuracy * totalResultCount / relevantResultCount;

            // Clamp accuracy to account for user trying to break the algorithm by inputting impossible values
            relevantAccuracy = Math.Clamp(relevantAccuracy, 0, 1);

            // Main curve for accuracy > 25%, the closer accuracy is to 25% - the more 50s it adds
            if (relevantAccuracy >= 0.25)
            {
                // Main curve. Zero 50s if accuracy is 100%, one 50 per 9 100s if accuracy is 75% (excluding misses), 4 50s per 9 100s if accuracy is 50%
                double ratio50To100 = Math.Pow(1 - ((relevantAccuracy - 0.25) / 0.75), 2);

                // Derived from the formula: Accuracy = (6 * c300 + 2 * c100 + c50) / (6 * totalHits), assuming that c50 = c100 * ratio50to100
                double count100Estimate = 6 * relevantResultCount * (1 - relevantAccuracy) / ((5 * ratio50To100) + 4);

                // Get count50 according to c50 = c100 * ratio50to100
                double count50Estimate = count100Estimate * ratio50To100;

                // Round it to get int number of 100s
                countGood = (int?)Math.Round(count100Estimate);

                // Get number of 50s as difference between total mistimed hits and count100
                countMeh = (int?)(Math.Round(count100Estimate + count50Estimate) - countGood);
            }
            // If accuracy is between 16.67% and 25% - we assume that we have no 300s
            else if (relevantAccuracy >= 1.0 / 6)
            {
                // Derived from the formula: Accuracy = (6 * c300 + 2 * c100 + c50) / (6 * totalHits), assuming that c300 = 0
                double count100Estimate = (6 * relevantResultCount * relevantAccuracy) - relevantResultCount;

                // We only had 100s and 50s in that scenario so rest of the hits are 50s
                double count50Estimate = relevantResultCount - count100Estimate;

                // Round it to get int number of 100s
                countGood = (int?)Math.Round(count100Estimate);

                // Get number of 50s as difference between total mistimed hits and count100
                countMeh = (int?)(Math.Round(count100Estimate + count50Estimate) - countGood);
            }
            // If accuracy is less than 16.67% - it means that we have only 50s or misses
            // Assuming that we removed misses in the 1st place - that means that we need to add additional misses to achieve target accuracy
            else
            {
                // Derived from the formula: Accuracy = (6 * c300 + 2 * c100 + c50) / (6 * totalHits), assuming that c300 = c100 = 0
                double count50Estimate = 6 * relevantResultCount * relevantAccuracy;

                // We have 0 100s, because we can't start adding 100s again after reaching "only 50s" point
                countGood = 0;

                // Round it to get int number of 50s
                countMeh = (int?)Math.Round(count50Estimate);

                // Fill the rest results with misses overwriting initial countMiss
                countMiss = (int)(totalResultCount - countMeh);
            }

            // Rest of the hits are 300s
            countGreat = (int)(totalResultCount - countGood - countMeh - countMiss);
        }

        return new Dictionary<HitResult, int>
        {
            { HitResult.Great, countGreat },
            { HitResult.Ok, countGood ?? 0 },
            { HitResult.Meh, countMeh ?? 0 },
            { HitResult.LargeTickMiss, 0 },
            { HitResult.SliderTailHit, hitObjects.Count(x => x is Slider) },
            { HitResult.Miss, countMiss }
        };
    }
    protected override double GetAccuracy(Dictionary<HitResult, int> statistics)
    {
        int countGreat = statistics[HitResult.Great];
        int countGood = statistics[HitResult.Ok];
        int countMeh = statistics[HitResult.Meh];
        int countMiss = statistics[HitResult.Miss];
        int total = countGreat + countGood + countMeh + countMiss;

        return (double)((6 * countGreat) + (2 * countGood) + countMeh) / (6 * total);
    }
}
