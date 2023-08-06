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
        public static readonly ConfigEntry WebOverlayConfig = new ConfigEntry($"WebOverlay_Config", null);
        protected IOverlayConfiguration OverlayConfiguration;
        private WebOverlaySettings _webOverlaySettings;

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
            _settings.Add(WebOverlayConfig.Name, OverlayConfiguration, false);
        }

        private void LoadConfiguration(bool reset = false)
        {
            var configEntry = _settings.GetConfiguration<OverlayConfiguration>(WebOverlayConfig);

            if (reset)
            {
                OverlayConfiguration.PropertyChanged -= OverlayConfigurationPropertyChanged;

                var newConfiguration = new OverlayConfiguration();
                OverlayConfiguration = newConfiguration;
                OverlayConfiguration.PropertyChanged += OverlayConfigurationPropertyChanged;
                OverlayConfigurationPropertyChanged(this, new PropertyChangedEventArgs("dummy"));

            }

            if (reset || configEntry == null)
                return;

            OverlayConfiguration = configEntry;
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
    }
}