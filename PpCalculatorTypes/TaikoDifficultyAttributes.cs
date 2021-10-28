namespace PpCalculatorTypes
{
    public class TaikoDifficultyAttributes : DifficultyAttributes
    {
        public int HitCount;
        public int DrumRollCount;
        public int SwellCount;
        public double GreatHitWindow;

        public TaikoDifficultyAttributes(double starRating, int maxCombo) : base(starRating, maxCombo)
        {
        }
    }
}