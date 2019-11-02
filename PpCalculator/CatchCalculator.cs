using System.Collections.Generic;
using System.Linq;
using osu.Game.Beatmaps;
using osu.Game.Rulesets;
using osu.Game.Rulesets.Catch;
using osu.Game.Rulesets.Catch.Objects;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Scoring;

namespace PpCalculator
{
    public class CatchCalculator : PpCalculator
    {
        public override Ruleset Ruleset { get; } = new CatchRuleset();

        protected override int GetMaxCombo(IReadOnlyList<HitObject> hitObjects) => 0;

        protected override Dictionary<HitResult, int> GenerateHitResults(double accuracy, IReadOnlyList<HitObject> hitObjects, int countMiss, int? countMeh, int? countGood) =>
            new Dictionary<HitResult, int>
            {
                [HitResult.Perfect] = hitObjects.OfType<Fruit>().Count() + hitObjects.OfType<JuiceStream>().Sum(s => s.RepeatCount) + (hitObjects.OfType<JuiceStream>().Count() * 2),
                [HitResult.Good] = hitObjects.OfType<JuiceStream>().Sum(s => s.NestedHitObjects.OfType<Droplet>().Sum(d => (d is TinyDroplet) ? 0 : 1)),
                [HitResult.Meh] = hitObjects.OfType<JuiceStream>().Sum(s => s.NestedHitObjects.OfType<TinyDroplet>().Count()),
                [HitResult.Miss] = 0,
                [HitResult.Ok] = 0
            };

        protected override double GetAccuracy(Dictionary<HitResult, int> statistics)
        {
            double fruits = statistics[HitResult.Perfect] + statistics[HitResult.Good] + statistics[HitResult.Meh];
            double totalFruits = fruits + statistics[HitResult.Miss] + statistics[HitResult.Ok];

            if (totalFruits == 0)
                return 1;

            return fruits / totalFruits;
        }
    }
}