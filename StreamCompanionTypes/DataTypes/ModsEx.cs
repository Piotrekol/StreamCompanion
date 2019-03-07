using CollectionManager.DataTypes;

namespace StreamCompanionTypes.DataTypes
{
    public class ModsEx
    {
        private readonly bool _showShortMod;

        public ModsEx(bool showShortMod)
        {
            _showShortMod = showShortMod;
        }
        public Mods Mods { get; set; }
        public string ShortMods { get; set; }
        public string LongMods { get; set; }
        public string ShownMods => _showShortMod ? ShortMods : LongMods;
    }
}