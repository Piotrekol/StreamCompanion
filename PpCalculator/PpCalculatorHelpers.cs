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
                    return new CatchCalculator();
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

            ppCalculator = GetPpCalculator(workingBeatmap.BeatmapInfo.RulesetID, ppCalculator);
            
            ppCalculator?.PreProcess(workingBeatmap);

            return ppCalculator;
        }

        /// <summary>
        /// Returns performance calculator suitable for given beatmap<para/>
        /// Reuses provided calculator if possible
        /// </summary>
        /// <param name="rulesetId"></param>
        /// <param name="ppCalculator"></param>
        /// <returns></returns>
        public static PpCalculator GetPpCalculator(int rulesetId, PpCalculator ppCalculator)
        {
            if (rulesetId == ppCalculator?.RulesetId)
                return ppCalculator;

            return GetPpCalculator(rulesetId);
        }
        


        /// <summary>
        /// Returns initalized performance calculator for specified ruleset(gamemode)<para/>
        /// Reuses provided calculator if possible
        /// </summary>
        /// <param name="rulesetId"></param>
        /// <param name="file"></param>
        /// <param name="ppCalculator"></param>
        /// <returns></returns>
        public static PpCalculator GetPpCalculator(int rulesetId, string file, PpCalculator ppCalculator)
        {
            if (rulesetId != ppCalculator?.RulesetId)
                ppCalculator = GetPpCalculator(rulesetId);

            ppCalculator?.PreProcess(new ProcessorWorkingBeatmap(file));

            return ppCalculator;
        }


        /// <summary>
        /// Picks valid rulesetId for specified map ruleset
        /// </summary>
        /// <param name="mapRulesetId"></param>
        /// <param name="desiredRulesetId"></param>
        /// <returns></returns>
        public static int GetRulesetId(int mapRulesetId, int? desiredRulesetId)
        {
            if (!desiredRulesetId.HasValue)
                return mapRulesetId;

            if (mapRulesetId != 0)//0=osu!
                return mapRulesetId;

            return desiredRulesetId.Value;
        }
    }
}