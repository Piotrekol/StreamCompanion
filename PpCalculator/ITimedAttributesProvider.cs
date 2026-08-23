using System.Collections.Generic;
using System.Threading;
using osu.Game.Beatmaps;
using osu.Game.Rulesets;
using osu.Game.Rulesets.Difficulty;
using osu.Game.Rulesets.Mods;

namespace PpCalculator
{
    public interface ITimedAttributesProvider
    {
        List<TimedDifficultyAttributes> GetTimedAttributes(IWorkingBeatmap workingBeatmap, Ruleset ruleset, IReadOnlyList<Mod> mods, bool isFullBeatmap, CancellationToken cancellationToken);
    }
}
