namespace PpCalculatorTypes
{
    public class ManiaDifficultyAttributes : DifficultyAttributes
    {
        public int NoteCount;
        public int HoldNoteCount;
        public double GreatHitWindow;

        public ManiaDifficultyAttributes(double starRating, int maxCombo) : base(starRating, maxCombo)
        {
        }
    }
}