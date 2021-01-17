using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using Newtonsoft.Json;
using StreamCompanion.Common;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Interfaces.Services;
using WebSocketDataSender.WebOverlay.Models;

namespace WebSocketDataSender.WebOverlay
{
    public class WebOverlay
    {
        private readonly ISettings _settings;
        private readonly ISaver _saver;
        private readonly Delegates.Restart _restarter;
        public static readonly ConfigEntry WebOverlayConfig = new ConfigEntry($"WebOverlay_Config", "{}");
        protected IOverlayConfiguration OverlayConfiguration;
        private WebOverlaySettings _webOverlaySettings;
        private static ColorConverter colorConverter = new ColorConverter();
        public WebOverlay(ISettings settings, ISaver saver, Delegates.Restart restarter)
        {
            _settings = settings;
            _saver = saver;
            _restarter = restarter;
            OverlayConfiguration = new OverlayConfiguration();
            LoadConfiguration();
            SaveConfiguration();
            OverlayConfiguration.PropertyChanged += OverlayConfigurationPropertyChanged;
        }


        private void ToggleRemoteAccess()
        {
            var remoteAccessEnabled = WebSocketDataGetter.RemoteAccessEnabled(_settings);
            var newValue = remoteAccessEnabled
                ? WebSocketDataGetter.HttpServerAddress.Default<string>()
                : "http://*";
            _settings.Add(WebSocketDataGetter.HttpServerAddress.Name, newValue);
            _restarter($"Applying web overlay remote access settings ({newValue})");
        }

        private void SaveConfiguration()
        {
            var serializedConfig = JsonConvert.SerializeObject(OverlayConfiguration, Formatting.None, colorConverter);
            _settings.Add(WebOverlayConfig.Name, serializedConfig, false);
        }

        private void LoadConfiguration(bool reset = false)
        {
            var rawConfig = _settings.Get<string>(WebOverlayConfig);

            if (reset)
            {
                OverlayConfiguration.PropertyChanged -= OverlayConfigurationPropertyChanged;

                var newConfiguration = new OverlayConfiguration();
                OverlayConfiguration = newConfiguration;
                OverlayConfiguration.PropertyChanged += OverlayConfigurationPropertyChanged;
                OverlayConfigurationPropertyChanged(this, new PropertyChangedEventArgs("dummy"));

            }

            if (reset || rawConfig == WebOverlayConfig.Default<string>())
                return;

            var config = JsonConvert.DeserializeObject<OverlayConfiguration>(rawConfig, colorConverter);

            OverlayConfiguration = config;
        }

        private void OverlayConfigurationPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            SaveConfiguration();
        }
        public void Free()
        {
            _webOverlaySettings?.Dispose();
        }

        public object GetUiSettings()
        {
            if (_webOverlaySettings == null || _webOverlaySettings.IsDisposed)
            {
                var filesLocation = WebSocketDataGetter.HttpContentRoot(_saver);
                var webUrl = WebSocketDataGetter.BaseAddress(_settings);
                _webOverlaySettings = new WebOverlaySettings(_settings, OverlayConfiguration);
                _webOverlaySettings.ResetSettings += (_, __) => ResetSettings();
                _webOverlaySettings.OpenWebUrl += (_, __) => ProcessExt.OpenUrl(webUrl);
                _webOverlaySettings.OpenFilesFolder += (_, __) => Process.Start("explorer.exe", filesLocation);
                _webOverlaySettings.FilesLocation = filesLocation;
                _webOverlaySettings.WebUrl = webUrl;
                _webOverlaySettings.RemoteAccessEnabled = WebSocketDataGetter.RemoteAccessEnabled(_settings);
                _webOverlaySettings.ToggleRemoteAccess += (_, __) => ToggleRemoteAccess();
            }

            return _webOverlaySettings;
        }
        protected void ResetSettings()
        {
            LoadConfiguration(true);
        }

        private class ColorConverter : JsonConverter<Color>
        {
            public override void WriteJson(JsonWriter writer, Color value, JsonSerializer serializer)
            {
                writer.WriteValue(ColorTranslator.ToHtml(value) + value.A.ToString("X2"));
            }

            public override Color ReadJson(JsonReader reader, Type objectType, Color existingValue, bool hasExistingValue,
                JsonSerializer serializer)
            {
                var rgbaColor = (string)reader.Value;
                var alpha = Convert.ToInt32(rgbaColor.Substring(7), 16);
                var color = ColorTranslator.FromHtml(rgbaColor.Substring(0, 7));
                return Color.FromArgb(alpha, color);
            }
        }
    }
}