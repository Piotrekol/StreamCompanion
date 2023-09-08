using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Overlay.Common;
using Overlay.Common.Loader;
using StreamCompanion.Common;
using StreamCompanionTypes.Attributes;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Enums;
using StreamCompanionTypes.Interfaces;
using StreamCompanionTypes.Interfaces.Consumers;
using StreamCompanionTypes.Interfaces.Services;
using StreamCompanionTypes.Interfaces.Sources;

namespace BrowserOverlay
{
    [SCPluginDependency("FileMapDataSender", "1.0.0")]
    [SCPlugin("Browser ingame overlay", "Allows for displaying any web overlay in fullscreened osu!. No borderless/windowed shenanigans.", Consts.SCPLUGIN_AUTHOR, Consts.SCPLUGIN_BASEURL, Consts.SCPLUGIN_GUIDE_INGAMEOVERLAYURL)]
    public class BrowserOverlay : IPlugin, ISettingsSource
    {
        public static ConfigEntry BrowserOverlayConfigurationConfigEntry = new ConfigEntry("BrowserOverlay", null);
        public string SettingGroup { get; } = "In-game overlay__Browser overlay";
        private const string messageBoxTitle = "StreamCompanion - Browser overlay";

        private readonly IContextAwareLogger _logger;
        private readonly ISettings _settings;
        private readonly ISaver _saver;
        private readonly List<Lazy<IHighFrequencyDataConsumer>> _dataConsumers;
        private readonly Delegates.Restart _restarter;
        private readonly Configuration _browserOverlayConfiguration;
        private LoaderWatchdog _loaderWatchdog;
        private OverlayDownload _overlayDownloadForm;
        private BrowserOverlaySettings _browserOverlaySettings;

        private string GetFilesFolder => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins", "Dlls");
        private string GetFullDllLocation(ISaver saver) => Path.Combine(GetAssetsLocation(saver), "browserIngameOverlay.dll");
        private string GetAssetsLocation(ISaver saver) => Path.Combine(saver.SaveDirectory, "BrowserOverlay");

        public BrowserOverlay(IContextAwareLogger logger, ISettings settings, ISaver saver, List<Lazy<IHighFrequencyDataConsumer>> dataConsumers, Delegates.Restart restarter)
        {
            _logger = logger;
            _settings = settings;
            _saver = saver;
            _dataConsumers = dataConsumers;
            _restarter = restarter;
            _browserOverlayConfiguration = _settings.GetConfiguration<Configuration>(BrowserOverlayConfigurationConfigEntry);
            _browserOverlayConfiguration.OverlayTabs ??= new List<OverlayTab> { new OverlayTab() };

            if (_browserOverlayConfiguration.Enabled)
                Initialize().HandleExceptions();

            SendConfiguration();
        }

        public void SendConfiguration()
        {
            _dataConsumers.ForEach(x => x.Value.Handle("Sc-webOverlayConfiguration", JsonConvert.SerializeObject(_browserOverlayConfiguration.OverlayTabs)));
        }

        public void Free()
        {
            _browserOverlaySettings?.Dispose();
        }

        public object GetUiSettings()
        {
            if (_browserOverlaySettings == null || _browserOverlaySettings.IsDisposed)
            {
                _browserOverlaySettings = new BrowserOverlaySettings(_browserOverlayConfiguration);
                _browserOverlaySettings.SettingUpdated += OnSettingUpdated;
            }
            return _browserOverlaySettings;
        }

        private void OnSettingUpdated(object sender, (string SettingName, object Value)? eventData)
        {
            _settings.Add(BrowserOverlayConfigurationConfigEntry, _browserOverlayConfiguration);
            SendConfiguration();

            if (eventData != null && eventData.Value.SettingName == "enable")
                _restarter($"Browser overlay was toggled. isEnabled:{eventData.Value.Value}");
        }

        private bool TryRemoveOldAssets()
        {
            string[] oldFileNames = { "BrowserOverlay.zip" };
            foreach (var filename in oldFileNames)
            {
                var filePath = Path.Combine(_saver.SaveDirectory, filename);
                if (File.Exists(filePath))
                {
                    try
                    {
                        Directory.Delete(GetAssetsLocation(_saver), true);
                    }
                    catch (DirectoryNotFoundException e)
                    {
                        //Fresh install or assets directory deleted manually
                        _logger.Log("Could not find directory to delete", LogLevel.Warning);
                        _logger.Log(e, LogLevel.Warning);
                    }
                    catch (UnauthorizedAccessException e)
                    {
                        //Assets are locked by running osu!
                        var errorMessage = "Failed to update Browser overlay assets - close osu! and restart SC to try again";
                        _logger.Log(errorMessage, LogLevel.Error);
                        _logger.Log(e, LogLevel.Error);
                        MessageBox.Show(errorMessage, messageBoxTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }

                    File.Delete(filePath);
            }
        }

            return true;
        }

        private async Task Initialize()
        {
            if (!TryRemoveOldAssets())
                return;

            var assetsLocation = GetAssetsLocation(_saver);
            var zipFileLocation = Path.Combine(_saver.SaveDirectory, "BrowserOverlayAssetsV2.zip");
            var overlayFilesMissing = true;
            if (File.Exists(zipFileLocation))
            {
                try
                {
                    var files = ZipFile.OpenRead(zipFileLocation).Entries.Where(e => !e.FullName.EndsWith('/')).Select(e => e.FullName);
                    overlayFilesMissing = files.Any(relativePath => !File.Exists(Path.Combine(assetsLocation, relativePath)));
                }
                catch (Exception e)
                {
                    _logger.Log("Could not load browser overlay assets zip, redownloading", LogLevel.Warning);
                    _logger.Log(e, LogLevel.Debug);
                    File.Delete(zipFileLocation);
                }
            }

            if (overlayFilesMissing)
            {
                if (await DownloadAndUnpackOverlay(zipFileLocation, assetsLocation))
                    _restarter("browser overlay installation/update finished");

                return;
            }

            _loaderWatchdog = new LoaderWatchdog(_logger, GetFullDllLocation(_saver), new Progress<OverlayReport>(HandleOverlayReport));
            _ = _loaderWatchdog.WatchForProcessStart(CancellationToken.None).HandleExceptions();
            return;
        }

        private void HandleOverlayReport(OverlayReport report)
        {
            switch (report.ReportType)
            {
                case ReportType.Information:
                    MessageBox.Show(report.Message, messageBoxTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
                case ReportType.Error:
                    MessageBox.Show(report.Message, messageBoxTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                case ReportType.Log:
                    _logger.Log(report.Message, LogLevel.Debug);
                    break;
            }
        }

        private async Task<bool> DownloadAndUnpackOverlay(string zipFileLocation, string assetsLocation)
        {
            _overlayDownloadForm = new OverlayDownload();
            _overlayDownloadForm.Show();
            if (!File.Exists(zipFileLocation))
            {
                var tempFileLocation = $"{zipFileLocation}.tmp";
                if (File.Exists(tempFileLocation))
                    File.Delete(tempFileLocation);

                try
                {
                    await DownloadOverlay(tempFileLocation);
                }
                catch (WebException ex)
                {
                    _overlayDownloadForm.SetStatus("problem during download - restart SC to try again");
                    _logger.Log(ex, LogLevel.Error);
                    return false;
                }

                if (!File.Exists(tempFileLocation))
                {
                    _overlayDownloadForm.SetStatus("Failed to download assets - restart SC to try again");
                    return false;
                }

                File.Move(tempFileLocation, zipFileLocation);
            }

            _overlayDownloadForm.SetStatus("unpacking files...");
            Directory.CreateDirectory(assetsLocation);
            ZipFile.ExtractToDirectory(zipFileLocation, assetsLocation, true);
            _overlayDownloadForm.Close();
            return true;
        }

        private async Task DownloadOverlay(string destination)
        {
            const string browserOverlayUrl = @"https://pioo.space/StreamCompanion/BrowserOverlayAssetsV2.zip";
#pragma warning disable SYSLIB0014 // WebClient is deprecated, use HttpClient instead
            using var client = new WebClient();
#pragma warning restore SYSLIB0014
            client.DownloadProgressChanged += ClientOnDownloadProgressChanged;
            await client.DownloadFileTaskAsync(browserOverlayUrl, destination);
        }

        private void ClientOnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            if (_overlayDownloadForm == null)
                return;
            _overlayDownloadForm.SetProgress(e.ProgressPercentage);
            _overlayDownloadForm.SetStatus($"{e.BytesReceived / 1024 / 1024}MB / {e.TotalBytesToReceive / 1024 / 1024}MB");
        }
    }

    public class Configuration
    {
        public bool Enabled { get; set; } = true;
        public List<OverlayTab> OverlayTabs { get; set; }
    }
    public class OverlayTab
    {
        public string Url { get; set; } = "http://localhost:20727/overlays/SC_PP%20Counter/";
        public decimal Scale { get; set; } = 1;
        public Canvas Canvas { get; set; } = new Canvas();
        public Position Position { get; set; } = new Position();
        public override string ToString() => $"{Url}, {Canvas}, {Position}, Scale:{Scale:0.###}";
    }

    public class Position
    {
        public int X = 0;
        public int Y = 0;
        public override string ToString() => $"X:{X} Y:{Y}";
    }

    public class Canvas
    {
        public int Width = 500;
        public int Height = 90;
        public override string ToString() => $"{Width}x{Height}";
    }

}
