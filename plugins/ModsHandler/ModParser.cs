using StreamCompanionTypes;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Interfaces;

namespace ModsHandler
{
    public class ModParser : CollectionManager.Modules.ModParser.ModParser, IModule, IModParser, ISettingsProvider
    {
        private readonly SettingNames _names = SettingNames.Instance;
   
        private ISettingsHandler _settings;
        private ModParserSettings _modParserSettings;
        public bool Started { get; set; }
        public string SettingGroup { get; } = "Map matching";
        
        public void Start(ILogger logger)
        {
            Started = true;
        }

        private void UpdateNoModText()
        {
            string noneText = _settings?.Get<string>(_names.NoModsDisplayText) ?? "None";
            if (LongNoModText != noneText)
                LongNoModText = noneText;
            if (ShortNoModText != noneText)
                ShortNoModText = noneText;
        }

        public ModsEx GetModsFromEnum(int modsEnum)
        {
            UpdateNoModText();

            var useShortMod = !_settings?.Get<bool>(_names.UseLongMods) ?? true;

            var mods = new ModsEx(useShortMod);
            mods.Mods = GetModsFromInt(modsEnum);
            mods.LongMods = GetModsFromEnum(modsEnum, false);
            mods.ShortMods = GetModsFromEnum(modsEnum, true);

            return mods;
        }

        public void SetSettingsHandle(ISettingsHandler settings)
        {
            _settings = settings;
        }

        public void Free()
        {
            _modParserSettings.Dispose();
        }

        public System.Windows.Forms.UserControl GetUiSettings()
        {
            if (_modParserSettings == null || _modParserSettings.IsDisposed)
            {
                _modParserSettings = new ModParserSettings(_settings);
            }
            return _modParserSettings;
        }

    }
}
