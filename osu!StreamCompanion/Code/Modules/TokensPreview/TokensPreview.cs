﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using osu_StreamCompanion.Code.Misc;
using StreamCompanionTypes;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Enums;
using StreamCompanionTypes.Interfaces;
using StreamCompanionTypes.Interfaces.Services;
using StreamCompanionTypes.Interfaces.Sources;
using JsonConvert = Newtonsoft.Json.JsonConvert;

namespace osu_StreamCompanion.Code.Modules.TokensPreview
{
    public sealed class TokensPreview : IModule, IOutputPatternSource, ISettingsSource, IDisposable
    {
        public bool Started { get; set; }
        private TokensPreviewSettings _commandsPreviewSettings;
        private ISettings _settings;
        private Dictionary<string, string> tokenFormats;

        public TokensPreview(ILogger logger, ISettings settings)
        {
            _settings = settings;
            Start(logger);
        }
        public void Start(ILogger logger)
        {
            try
            {
                tokenFormats = JsonConvert.DeserializeObject<Dictionary<string, string>>(_settings.Get<string>(SettingNames.Instance.TokenFormats));
            }
            catch (Exception ex)
            {
                logger.Log("Error while parsing token formats, value was reset to default", LogLevel.Warning);
                logger.Log(ex, LogLevel.Error);
                tokenFormats = JsonConvert.DeserializeObject<Dictionary<string, string>>(SettingNames.Instance.TokenFormats.Default<string>());
            }

            Tokens.AllTokensChanged += (sender, args) =>
            {
                //This isn't ideal place to handle this, but it will do for now
                AdjustTokenFormats(Tokens.AllTokens);
            };

            Started = true;
        }

        public Task<List<IOutputPattern>> GetOutputPatterns(IMapSearchResult map, Tokens tokens, OsuStatus status)
        {
            if (_commandsPreviewSettings != null && !_commandsPreviewSettings.IsDisposed)
                _commandsPreviewSettings.Add(tokens);

            return Task.FromResult<List<IOutputPattern>>(null);
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

        public string SettingGroup { get; } = "Tokens Preview";

        public void Free()
        {
            _commandsPreviewSettings.Dispose();
        }

        public object GetUiSettings()
        {
            if (_commandsPreviewSettings == null || _commandsPreviewSettings.IsDisposed)
            {
                _commandsPreviewSettings = new TokensPreviewSettings();

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