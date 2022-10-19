using System.Collections.Generic;
using System.Linq;
using osu.Game.Beatmaps;
using osu.Game.Rulesets;
using osu.Game.Rulesets.Difficulty;
using osu.Game.Rulesets.Mods;
using osu.Game.Utils;

namespace PpCalculator
{
    internal class LegacyHelper
    {
        /// <summary>
        /// Transforms a given <see cref="IMod"/> combination into one which is applicable to legacy scores.
        /// This is used to match osu!stable/osu!web calculations for the time being, until such a point that these mods do get considered.
        /// </summary>
        public static IMod[] ConvertToLegacyDifficultyAdjustmentMods(Ruleset ruleset, DifficultyCalculator difficultyCalculator, IMod[] mods)
        {
            var allMods = ruleset.AllMods;
            var allowedModAcronyms = ModUtils.FlattenMods(difficultyCalculator.DifficultyAdjustmentMods)
                                      .Select(m => m.Acronym)
                                      .Distinct()
                                      .ToHashSet();

            var result = new List<IMod>();
            var classicMod = allMods.SingleOrDefault(m => m.Acronym == "CL");
            if (classicMod != null)
                result.Add(classicMod);

            result.AddRange(mods.Where(m => allowedModAcronyms.Contains(m.Acronym)));
            return result.ToArray();
        }
    }
}
