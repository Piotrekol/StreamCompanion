using StreamCompanionTypes;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Interfaces;

namespace ModsHandler
{
    public class ModParser : CollectionManager.Modules.ModParser.ModParser, IModParser, ISettingsProvider
    {
        private readonly SettingNames _names = SettingNames.Instance;

        private ISettingsHandler _settings;
        private ModParserSettings _modParserSettings;
        public string SettingGroup { get; } = "Map matching";

        public ModParser(ISettingsHandler settings)
        {
            _settings = settings;
        }


        private void UpdateNoModText()
        {
            string noneText = _settings?.Get<string>(_names.NoModsDisplayText) ?? "None";
            if (LongNoModText != noneText)
                LongNoModText = noneText;
            if (ShortNoModText != noneText)
                ShortNoModText = noneText;
        }

        public IModsEx GetModsFromEnum(int modsEnum)
        {
            UpdateNoModText();

            var useShortMod = !_settings?.Get<bool>(_names.UseLongMods) ?? true;

            var mods = new ModsEx(useShortMod,
                mods: GetModsFromInt(modsEnum),
                shortMods: GetModsFromEnum(modsEnum, true),
                longMods: GetModsFromEnum(modsEnum, false)
            );

            return mods;
        }

        public void Free()
        {
            _modParserSettings.Dispose();
        }

        public object GetUiSettings()
        {
            if (_modParserSettings == null || _modParserSettings.IsDisposed)
            {
                _modParserSettings = new ModParserSettings(_settings);
            }
            return _modParserSettings;
        }

    }
}
