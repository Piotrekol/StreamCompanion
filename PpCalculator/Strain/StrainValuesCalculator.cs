#nullable enable
using osu.Game.Beatmaps;
using osu.Game.Rulesets;
using osu.Game.Rulesets.Difficulty;
using osu.Game.Rulesets.Difficulty.Preprocessing;
using osu.Game.Rulesets.Difficulty.Skills;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PpCalculator;

public static partial class StrainValuesCalculator
{
    public static Dictionary<int, double> GetStrains(Ruleset ruleset, IWorkingBeatmap workingBeatmap, IReadOnlyList<Mod> mods, int targetAmount)
    {
        DifficultyCalculator calculator = ruleset.RulesetInfo.OnlineID switch
        {
            0 => new CapturingOsuDifficultyCalculator(ruleset.RulesetInfo, workingBeatmap),
            1 => new CapturingTaikoDifficultyCalculator(ruleset.RulesetInfo, workingBeatmap),
            2 => new CapturingCatchDifficultyCalculator(ruleset.RulesetInfo, workingBeatmap),
            3 => new CapturingManiaDifficultyCalculator(ruleset.RulesetInfo, workingBeatmap),
            _ => throw new ArgumentException($"No strain graph available for ruleset \"{ruleset.RulesetInfo.OnlineID}\""),
        };

        if (calculator is not ICapturingDifficultyCalculator capturingCalculator)
            throw new InvalidOperationException("difficulty calculator is missing calculation capture");

        _ = calculator.Calculate(mods.ToArray());

        return BuildStrains(capturingCalculator.CapturedSkills, capturingCalculator.CapturedDifficultyHitObjects, workingBeatmap.Beatmap.HitObjects, targetAmount);
    }

    private static Dictionary<int, double> BuildStrains(Skill[] capturedSkills, IReadOnlyList<DifficultyHitObject> difficultyHitObjects, IReadOnlyList<HitObject> hitObjects, int targetAmount)
    {
        if (capturedSkills.Length == 0)
        {
            return [];
        }

        double[][] normalizedSkills = capturedSkills
            .Select(Normalize)
            .Where(normalized => normalized != null)
            .ToArray()!;

        if (normalizedSkills.Length == 0)
        {
            return [];
        }

        double[] valueTimes = [.. difficultyHitObjects.Select(difficultyHitObject => difficultyHitObject.StartTime)];
        double clockRate = difficultyHitObjects.Count > 0 ? difficultyHitObjects[^1].ClockRate : 1;
        double interval = (hitObjects[^1].GetEndTime() / clockRate + 1) / targetAmount;
        double[] bucketSums = new double[targetAmount];
        int[] bucketCounts = new int[targetAmount];
        int objectCount = Math.Min(valueTimes.Length, normalizedSkills.Min(normalizedSkill => normalizedSkill.Length));

        for (int objectIndex = 0; objectIndex < objectCount; objectIndex++)
        {
            double value = 0;
            foreach (double[] normalizedSkill in normalizedSkills)
            {
                value = Math.Max(value, normalizedSkill[objectIndex]);
            }

            int bucket = Math.Min(targetAmount - 1, (int)(valueTimes[objectIndex] / interval));
            bucketSums[bucket] += value;
            bucketCounts[bucket]++;
        }

        Dictionary<int, double> strains = new(targetAmount);
        for (int bucket = 0; bucket < targetAmount; bucket++)
        {
            strains[(int)(bucket * interval)] = bucketCounts[bucket] > 0 ? bucketSums[bucket] / bucketCounts[bucket] : 0;
        }

        return strains;
    }

    private static double[]? Normalize(Skill skill)
    {
        IReadOnlyList<double> values = skill.GetObjectDifficulties();
        if (values.Count == 0)
        {
            return null;
        }

        double max = values.Max();
        if (max <= 0)
        {
            return null;
        }

        return [.. values.Select(value => value / max)];
    }
}
