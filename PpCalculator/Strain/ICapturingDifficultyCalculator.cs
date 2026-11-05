#nullable enable
using osu.Game.Rulesets.Difficulty.Preprocessing;
using osu.Game.Rulesets.Difficulty.Skills;
using System.Collections.Generic;

namespace PpCalculator;

internal interface ICapturingDifficultyCalculator
{
    Skill[] CapturedSkills { get; }

    IReadOnlyList<DifficultyHitObject> CapturedDifficultyHitObjects { get; }
}
