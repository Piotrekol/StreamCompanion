#nullable enable
using osu.Game.Beatmaps;
using osu.Game.Rulesets;
using osu.Game.Rulesets.Difficulty.Preprocessing;
using osu.Game.Rulesets.Difficulty.Skills;
using osu.Game.Rulesets.Mania.Difficulty;
using osu.Game.Rulesets.Mods;
using System;
using System.Collections.Generic;

namespace PpCalculator;

public static partial class StrainValuesCalculator
{
    private sealed class CapturingManiaDifficultyCalculator : ManiaDifficultyCalculator, ICapturingDifficultyCalculator
    {
        public Skill[] CapturedSkills { get; private set; } = [];
        public IReadOnlyList<DifficultyHitObject> CapturedDifficultyHitObjects { get; private set; } = [];

        public CapturingManiaDifficultyCalculator(IRulesetInfo ruleset, IWorkingBeatmap beatmap) : base(ruleset, beatmap)
        {
        }

        protected override IEnumerable<DifficultyHitObject> CreateDifficultyHitObjects(IBeatmap beatmap, Mod[] mods)
        {
            CapturedDifficultyHitObjects = [.. base.CreateDifficultyHitObjects(beatmap, mods)];
            return CapturedDifficultyHitObjects;
        }

        protected override Skill[] CreateSkills(IBeatmap beatmap, Mod[] mods)
        {
            CapturedSkills = base.CreateSkills(beatmap, mods);
            return CapturedSkills;
        }
    }
}
