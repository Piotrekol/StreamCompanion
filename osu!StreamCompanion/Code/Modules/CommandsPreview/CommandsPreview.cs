using StreamCompanionTypes;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using StreamCompanionTypes.Enums;
using JsonConvert = Newtonsoft.Json.JsonConvert;

namespace osu_StreamCompanion.Code.Modules.CommandsPreview
{
    public class CommandsPreview : IModule, IMapDataParser, ISettingsProvider, IDisposable
    {
        public bool Started { get; set; }
        private CommandsPreviewSettings _commandsPreviewSettings;
        private ISettingsHandler _settings;
        private Dictionary<string, string> tokenFormats;

        public CommandsPreview(ILogger logger, ISettingsHandler settings)
        {
            _settings = settings;
            Start(logger);
        }
        public void Start(ILogger logger)
        {
            tokenFormats = JsonConvert.DeserializeObject<Dictionary<string, string>>(_settings.Get<string>(SettingNames.Instance.TokenFormats));

            Tokens.AllTokensChanged += (sender, args) =>
            {
                //This isn't ideal place to handle this, but it will do for now
                AdjustTokenFormats(Tokens.AllTokens);
            };

            Started = true;
        }

        public List<IOutputPattern> GetFormatedPatterns(Tokens replacements, OsuStatus status)
        {
            if (_commandsPreviewSettings != null && !_commandsPreviewSettings.IsDisposed)
                _commandsPreviewSettings.Add(replacements);

            return null;
        }

        private void AdjustTokenFormats(IReadOnlyDictionary<string, IToken> tokensDictionary)
        {
            if (tokenFormats.Count == 0)
                return;

            foreach (var tokenkv in tokensDictionary)
            {
                if (tokenFormats.ContainsKey(tokenkv.Key))
                    tokenkv.Value.Format = tokenFormats[tokenkv.Key];
            }
        }

        public string SettingGroup { get; } = "Commands Preview";
        public void SetSettingsHandle(ISettingsHandler settings)
        {

        }

        public void Free()
        {
            _commandsPreviewSettings.Dispose();
        }

        public object GetUiSettings()
        {
            if (_commandsPreviewSettings == null || _commandsPreviewSettings.IsDisposed)
            {
                _commandsPreviewSettings = new CommandsPreviewSettings();

                _commandsPreviewSettings.Add(new Dictionary<string, IToken>(Tokens.AllTokens));
            }
            return _commandsPreviewSettings;
        }

        public void Dispose()
        {
            var tokenFormats = Tokens.AllTokens.ToDictionary(k => k.Key, t => t.Value.Format);
            _settings.Add(SettingNames.Instance.TokenFormats.Name, JsonConvert.SerializeObject(tokenFormats));
            _settings.Save();
            _commandsPreviewSettings?.Dispose();

        }
    }
}