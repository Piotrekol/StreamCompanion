using System;
using System.IO;

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
        /// 2 = Ctb<para/>
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
                    return new CtbCalculator();
                case 3:
                    return new ManiaCalculator();
            }
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
        /// <param name="rulesetId">osu! gamemode id</param>
        /// <param name="file">.osu file to read</param>
        /// <param name="ppCalculator">Existing <see cref="PpCalculator"/> instance, if any.</param>
        /// <returns></returns>
        public static PpCalculator GetPpCalculator(int rulesetId, string file, PpCalculator ppCalculator)
            => InternalGetPpCalculator(rulesetId, file, ppCalculator, 0);

        private static PpCalculator InternalGetPpCalculator(int rulesetId, string file, PpCalculator ppCalculator,
            int retryCount)
        {
            if (rulesetId != ppCalculator?.RulesetId)
                ppCalculator = GetPpCalculator(rulesetId);
            try
            {
                ppCalculator?.PreProcess(new ProcessorWorkingBeatmap(file));
            }
            catch (IOException)
            {
                //file is being used by another process..
                if (retryCount < 5)
                    return InternalGetPpCalculator(rulesetId, file, ppCalculator, ++retryCount);
            }

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