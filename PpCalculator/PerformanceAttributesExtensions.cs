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
                case osu.Game.Rulesets.Taiko.Difficulty.TaikoPerformanceAttributes osuAttribs:
                    return new TaikoPerformanceAttributes
                    {
                        Accuracy = osuAttribs.Accuracy,
                        Difficulty = osuAttribs.Difficulty,
                        EffectiveMissCount = osuAttribs.EffectiveMissCount,
                        Total = osuAttribs.Total,
                    };
                case osu.Game.Rulesets.Catch.Difficulty.CatchPerformanceAttributes osuAttribs:
                    return new CatchPerformanceAttributes
                    {
                        Total = osuAttribs.Total,
                    };
                case osu.Game.Rulesets.Mania.Difficulty.ManiaPerformanceAttributes osuAttribs:
                    return new ManiaPerformanceAttributes
                    {
                        Difficulty = osuAttribs.Difficulty,
                        Total = osuAttribs.Total,
                    };
                default:
                    return null;
            }
        }
    }
}
