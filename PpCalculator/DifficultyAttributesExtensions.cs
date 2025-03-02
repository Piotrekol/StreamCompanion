using osu.Game.Beatmaps;
using osu.Game.Rulesets.Catch.Objects;
using osu.Game.Rulesets.Mania.Objects;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Taiko.Objects;
using PpCalculatorTypes;
using System.Collections.Generic;
using System.Linq;

namespace PpCalculator
{
    internal static class DifficultyAttributesExtensions
    {

        public static DifficultyAttributes ConvertToSCAttributes(this osu.Game.Rulesets.Difficulty.DifficultyAttributes attributes, IReadOnlyList<HitObject> hitObjects)
        {
            switch (attributes)
            {
                case osu.Game.Rulesets.Osu.Difficulty.OsuDifficultyAttributes osuAttributes:
                    {
                        return new OsuDifficultyAttributes(osuAttributes.StarRating, osuAttributes.MaxCombo)
                        {
                            AimStrain = osuAttributes.AimDifficulty,
                            SpeedStrain = osuAttributes.SpeedDifficulty,
                            HitCircleCount = osuAttributes.HitCircleCount,
                            SliderCount = osuAttributes.SliderCount,
                            SpinnerCount = osuAttributes.SpinnerCount
                        };
                    }
                case osu.Game.Rulesets.Mania.Difficulty.ManiaDifficultyAttributes maniaAttributes:
                    return new ManiaDifficultyAttributes(maniaAttributes.StarRating, maniaAttributes.MaxCombo)
                    {
                        NoteCount = hitObjects.Count(h => h is Note),
                        HoldNoteCount = hitObjects.Count(h => h is HoldNote),
                    };
                case osu.Game.Rulesets.Taiko.Difficulty.TaikoDifficultyAttributes taikoAttributes:
                    return new TaikoDifficultyAttributes(taikoAttributes.StarRating, taikoAttributes.MaxCombo)
                    {
                        HitCount = hitObjects.Count(h => h is Hit),
                        DrumRollCount = hitObjects.Count(h => h is DrumRoll),
                        SwellCount = hitObjects.Count(h => h is Swell),
                    };
                case osu.Game.Rulesets.Catch.Difficulty.CatchDifficultyAttributes ctbAttributes:
                    return new CatchDifficultyAttributes(ctbAttributes.StarRating, ctbAttributes.MaxCombo)
                    {
                        FruitCount = hitObjects.Count(h => h is Fruit),
                        JuiceStreamCount = hitObjects.Count(h => h is JuiceStream),
                        BananaShowerCount = hitObjects.Count(h => h is BananaShower),
                    };
                default:
                    return new DifficultyAttributes(attributes.StarRating, attributes.MaxCombo);
            }
        }

    }
}
