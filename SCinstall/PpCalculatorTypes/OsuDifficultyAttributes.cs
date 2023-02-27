namespace PpCalculatorTypes
{
    public class OsuDifficultyAttributes : DifficultyAttributes
    {
        public double AimStrain;
        public double SpeedStrain;
        public double ApproachRate;
        public double OverallDifficulty;
        public int HitCircleCount;
        public int SpinnerCount;
        public int SliderCount;

        public OsuDifficultyAttributes(double starRating, int maxCombo) : base(starRating, maxCombo)
        {
        }
    }
}