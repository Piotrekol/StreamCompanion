using CollectionManager.DataTypes;

namespace StreamCompanionTypes.DataTypes
{
    public class ModsEx
    {
        private readonly bool _showShortMod;

        public ModsEx(bool showShortMod, Mods mods, string shortMods, string longMods)
        {
            _showShortMod = showShortMod;
            ShortMods = shortMods;
            LongMods = longMods;
            Mods = mods;
        }

        public readonly Mods Mods;
        private string _shortMods;
        private string _filteredShortMods;
        private string ShortMods
        {
            get => _shortMods;
            set
            {
                _shortMods = value;
                _filteredShortMods = value
                    .Replace("AU", "")
                    .Replace("SV2", "")
                    .Replace("RL", "")
                    .Replace("CO", "DS"); //Coop is named Dual Stages in osu!lazer
            }
        }
        private readonly string LongMods;
        /// <summary>
        /// Mods value shown to user
        /// </summary>
        public string ShownMods => _showShortMod ? ShortMods : LongMods;

        /// <summary>
        /// Mods used for processing (eg. pp calculation)
        /// </summary>
        public string WorkingMods => Mods == Mods.Omod ? "" : _filteredShortMods;
    }
}