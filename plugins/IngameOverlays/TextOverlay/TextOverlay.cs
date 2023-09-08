using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Interfaces;
using StreamCompanionTypes.Enums;
using StreamCompanionTypes.Interfaces.Services;
using StreamCompanionTypes.Interfaces.Sources;
using Overlay.Common.Loader;
using Overlay.Common;
using StreamCompanion.Common;
using StreamCompanionTypes.Attributes;

namespace TextOverlay
{
    [SCPluginDependency("FileMapDataSender", "1.0.0")]
    [SCPlugin("Text ingame overlay", "Basic text based ingame overlay. Data is provided using Output patterns", Consts.SCPLUGIN_AUTHOR, Consts.SCPLUGIN_BASEURL, Consts.SCPLUGIN_GUIDE_INGAMEOVERLAYURL)]
    public class TextOverlay : IPlugin, ISettingsSource, IDisposable
    {
        public static readonly ConfigEntry EnableIngameOverlay = new ConfigEntry("EnableIngameOverlay", true);

        private ISettings _settings;
        private readonly Delegates.Restart _restarter;
        public string SettingGroup { get; } = "In-game overlay__Text overlay";
        private TextOverlaySettings _overlaySettings;
        private ILogger _logger;
        private LoaderWatchdog _loaderWatchdog;
        CancellationTokenSource cancellationToken = new CancellationTokenSource();

        public TextOverlay(ILogger logger, ISettings settings, Delegates.Restart restarter)
        {
            _logger = logger;
            _settings = settings;
            _restarter = restarter;

            if (_settings.Get<bool>(EnableIngameOverlay))
            {
                _loaderWatchdog = new LoaderWatchdog(_logger, GetFullDllLocation(), new Progress<OverlayReport>(HandleOverlayReport));
                _ = _loaderWatchdog.WatchForProcessStart(CancellationToken.None).HandleExceptions();
            }
        }

        private void HandleOverlayReport(OverlayReport report)
        {
            const string messageBoxTitle = "StreamCompanion - Text overlay";
            switch (report.ReportType)
            {
                case ReportType.Information:
                    MessageBox.Show(report.Message, messageBoxTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
                case ReportType.Error:
                    MessageBox.Show(report.Message, messageBoxTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                case ReportType.Log:
                    _logger.Log(report.Message, LogLevel.Information);
                    break;
            }
        }

        private string GetFilesFolder() => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins", "Dlls");
        private string GetFullDllLocation() => Path.Combine(GetFilesFolder(), "textOverlay.dll");

        public void Free() => _overlaySettings?.Dispose();
        public object GetUiSettings()
        {
            if (_overlaySettings == null || _overlaySettings.IsDisposed)
            {
                _overlaySettings = new TextOverlaySettings(_settings);
                _overlaySettings.OverlayToggled += (_, value) => _restarter($"Text overlay was toggled. isEnabled:{value}");
            }
            return _overlaySettings;
        }

        public void Dispose()
        {
            _overlaySettings?.Dispose();
            cancellationToken.Cancel();
        }
    }
}