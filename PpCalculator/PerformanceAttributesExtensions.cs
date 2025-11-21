using PpCalculatorTypes;

namespace PpCalculator
{
    internal static class PerformanceAttributesExtensions
    {
        public static PerformanceAttributes ConvertToSCAttributes(this osu.Game.Rulesets.Difficulty.PerformanceAttributes attributes)
        {
            switch (attributes)
            {
                case osu.Game.Rulesets.Osu.Difficulty.OsuPerformanceAttributes osuAttribs:
                    return new OsuPerformanceAttributes
                    {
                        Accuracy = osuAttribs.Accuracy,
                        Aim = osuAttribs.Aim,
                        EffectiveMissCount = osuAttribs.EffectiveMissCount,
                        Flashlight = osuAttribs.Flashlight,
                        Speed = osuAttribs.Speed,
                        Total = osuAttribs.Total,
                    };
                case osu.Game.Rulesets.Taiko.Difficulty.TaikoPerformanceAttributes taikoAttribs:
                    return new TaikoPerformanceAttributes
                    {
                        Accuracy = taikoAttribs.Accuracy,
                        Difficulty = taikoAttribs.Difficulty,
                        EstimatedUnstableRate = taikoAttribs.EstimatedUnstableRate,
                        Total = taikoAttribs.Total,
                    };
                case osu.Game.Rulesets.Catch.Difficulty.CatchPerformanceAttributes ctbAttribs:
                    return new CatchPerformanceAttributes
                    {
                        Total = ctbAttribs.Total,
                    };
                case osu.Game.Rulesets.Mania.Difficulty.ManiaPerformanceAttributes maniaAttribs:
                    return new ManiaPerformanceAttributes
                    {
                        Difficulty = maniaAttribs.Difficulty,
                        Total = maniaAttribs.Total,
                    };
                default:
                    return null;
            }
        }
    }
}
