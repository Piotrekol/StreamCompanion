﻿using System.Collections.Generic;
using osu.Game.Beatmaps;
using osu.Game.Rulesets;
using osu.Game.Rulesets.Mania;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Scoring;

namespace PpCalculator
{
    public class ManiaCalculator : PpCalculator
    {
        public override Ruleset Ruleset { get; } = new ManiaRuleset();

        protected override int GetMaxCombo(IReadOnlyList<HitObject> hitObjects) => 0;

        protected override Dictionary<HitResult, int> GenerateHitResults(double accuracy, IBeatmap beatmap, int countMiss, int? countMeh, int? countGood)
        {
            var totalHits = beatmap.HitObjects.Count;

            // Only total number of hits is considered currently, so specifics don't matter
            return new Dictionary<HitResult, int>
            {
                { HitResult.Perfect, totalHits },
                { HitResult.Great, 0 },
                { HitResult.Ok, 0 },
                { HitResult.Good, 0 },
                { HitResult.Meh, 0 },
                { HitResult.Miss, 0 }
            };
        }

        protected override double GetAccuracy(Dictionary<HitResult, int> statistics) => 0;
    }
}