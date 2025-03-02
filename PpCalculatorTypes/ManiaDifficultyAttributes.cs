namespace PpCalculatorTypes
{
    public class ManiaDifficultyAttributes : DifficultyAttributes
    {
        public int NoteCount;
        public int HoldNoteCount;

        public ManiaDifficultyAttributes(double starRating, int maxCombo) : base(starRating, maxCombo)
        {
        }
    }
}