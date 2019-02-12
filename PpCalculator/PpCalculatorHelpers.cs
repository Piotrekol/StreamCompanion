using osu.Game.Rulesets.Catch;
using osu.Game.Rulesets.Mania;
using osu.Game.Rulesets.Osu;
using osu.Game.Rulesets.Taiko;
using System;

namespace PpCalculator
{
    public class PpCalculatorHelpers
    {
        private PpCalculatorHelpers() { }

        /// <summary>
        /// Creates performance calculator suitable for given ruleset(gamemode)
        /// </summary>
        /// <param name="rulesetId">
        /// gamemode id<para/>
        /// 0 = osu<para/>
        /// 1 = Taiko <para/>
        /// 2 = Ctb(unsupported - returns null)<para/>
        /// 3 = Mania
        /// </param>
        /// <returns></returns>
        public static PpCalculator GetPpCalculator(int rulesetId)
        {
            switch (rulesetId)
            {
                default:
                    throw new ArgumentException("Invalid ruleset ID provided.");
                case 0:
                    return new OsuCalculator();
                case 1:
                    return new TaikoCalculator();
                case 2:
                    return null;
                case 3:
                    return new ManiaCalculator();
            }
        }

        /// <summary>
        /// Returns initalized performance calculator suitable for given beatmap<para/>
        /// Reuses provided calculator if possible
        /// </summary>
        /// <param name="file"></param>
        /// <param name="ppCalculator"></param>
        /// <returns></returns>
        public static PpCalculator GetPpCalculator(string file, PpCalculator ppCalculator)
        {
            var workingBeatmap = new ProcessorWorkingBeatmap(file);

            if (workingBeatmap.BeatmapInfo.RulesetID == ppCalculator?.RulesetId)
            {
                ppCalculator.PreProcess(workingBeatmap);
                return ppCalculator;
            }

            ppCalculator = GetPpCalculator(workingBeatmap.BeatmapInfo.RulesetID);

            if (ppCalculator == null)
                return null;

            ppCalculator.PreProcess(workingBeatmap);
            return ppCalculator;
        }

    }
}