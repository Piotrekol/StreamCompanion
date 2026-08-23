#nullable enable
using osu.Game.Beatmaps;
using osu.Game.Rulesets;
using osu.Game.Rulesets.Difficulty.Skills;
using osu.Game.Rulesets.Mania.Difficulty;
using osu.Game.Rulesets.Mods;
using System;

namespace PpCalculator;

public static partial class StrainValuesCalculator
{
    private sealed class CapturingManiaDifficultyCalculator : ManiaDifficultyCalculator
    {
        public Skill[] CapturedSkills { get; private set; } = [];

        public CapturingManiaDifficultyCalculator(IRulesetInfo ruleset, IWorkingBeatmap beatmap) : base(ruleset, beatmap)
        {
        }

        protected override Skill[] CreateSkills(IBeatmap beatmap, Mod[] mods)
        {
            CapturedSkills = base.CreateSkills(beatmap, mods);
            return CapturedSkills;
        }
    }
}
