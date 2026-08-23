#nullable enable
using osu.Game.Beatmaps;
using osu.Game.Rulesets;
using osu.Game.Rulesets.Difficulty.Skills;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Osu.Difficulty;

namespace PpCalculator;

public static partial class StrainValuesCalculator
{
    private sealed class CapturingOsuDifficultyCalculator : OsuDifficultyCalculator
    {
        public Skill[] CapturedSkills { get; private set; } = [];

        public CapturingOsuDifficultyCalculator(IRulesetInfo ruleset, IWorkingBeatmap beatmap) : base(ruleset, beatmap)
        {
        }

        protected override Skill[] CreateSkills(IBeatmap beatmap, Mod[] mods)
        {
            CapturedSkills = base.CreateSkills(beatmap, mods);
            return CapturedSkills;
        }
    }
}
