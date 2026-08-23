using System.Collections.Generic;
using System.Threading;
using osu.Game.Beatmaps;
using osu.Game.Rulesets;
using osu.Game.Rulesets.Difficulty;
using osu.Game.Rulesets.Mods;

namespace PpCalculator
{
    public sealed class CalculatingTimedAttributesProvider : ITimedAttributesProvider
    {
        public static readonly CalculatingTimedAttributesProvider Instance = new();

        public List<TimedDifficultyAttributes> GetTimedAttributes(IWorkingBeatmap workingBeatmap, Ruleset ruleset, IReadOnlyList<Mod> mods, bool isFullBeatmap, CancellationToken cancellationToken)
            => ruleset.CreateDifficultyCalculator(workingBeatmap).CalculateTimed(mods, cancellationToken);
    }
}
