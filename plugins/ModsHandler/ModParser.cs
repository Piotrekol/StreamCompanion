using StreamCompanionTypes;
using StreamCompanionTypes.Interfaces;

namespace ModsHandler
{
    public class ModParser : CollectionManager.Modules.ModParser.ModParser, IPlugin, IModParser, ISettingsProvider
    {
        private readonly SettingNames _names = SettingNames.Instance;
   
        private ISettingsHandler _settings;
        private ModParserSettings _modParserSettings;
        public bool Started { get; set; }
        public string SettingGroup { get; } = "Map matching";


        public string Description { get; } = "";
        public string Name { get; } = nameof(ModParser);
        public string Author { get; } = "Piotrekol";
        public string Url { get; } = "";
        public string UpdateUrl { get; } = "";

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

        public string GetModsFromEnum(int modsEnum)
        {
            UpdateNoModText();
            var shortMod = !_settings?.Get<bool>(_names.UseLongMods) ?? true;
            return GetModsFromEnum(modsEnum, shortMod);
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
