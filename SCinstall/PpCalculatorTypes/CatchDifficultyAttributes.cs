namespace PpCalculatorTypes
{
    public class CatchDifficultyAttributes : DifficultyAttributes
    {
        public int FruitCount;
        public int JuiceStreamCount;
        public int BananaShowerCount;

        public CatchDifficultyAttributes(double starRating, int maxCombo) : base(starRating, maxCombo)
        {
        }
    }
}