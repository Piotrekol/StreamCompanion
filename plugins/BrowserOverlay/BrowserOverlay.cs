using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using BrowserOverlay.Loader;
using Newtonsoft.Json;
using StreamCompanion.Common;
using StreamCompanionTypes;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Enums;
using StreamCompanionTypes.Interfaces;
using StreamCompanionTypes.Interfaces.Consumers;
using StreamCompanionTypes.Interfaces.Services;
using StreamCompanionTypes.Interfaces.Sources;

namespace BrowserOverlay
{
    public class BrowserOverlay : IPlugin, ISettingsSource
    {
        public static ConfigEntry BrowserOverlayConfigurationConfigEntry = new ConfigEntry("BrowserOverlay", "{}");

        public string Description { get; } = string.Empty;
        public string Name { get; } = "BrowserIngameOverlay";
        public string Author { get; } = "Piotrekol";
        public string Url { get; } = string.Empty;
        public string UpdateUrl { get; } = string.Empty;
        public string SettingGroup { get; } = "In-game overlay__Browser overlay";

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
        private string GetFullDllLocation() => Path.Combine(GetFilesFolder, "osuBrowserOverlay.dll");

        public BrowserOverlay(IContextAwareLogger logger, ISettings settings, ISaver saver, List<Lazy<IHighFrequencyDataConsumer>> dataConsumers, Delegates.Restart restarter)
        {
            _logger = logger;
            _settings = settings;
            _saver = saver;
            _dataConsumers = dataConsumers;
            _restarter = restarter;
            _browserOverlayConfiguration = _settings.GetConfiguration<Configuration>(BrowserOverlayConfigurationConfigEntry);
            _browserOverlayConfiguration.OverlayTabs ??= new List<OverlayTab> { new OverlayTab() };

            if (_browserOverlayConfiguration.Enabled && TextOverlayIsEnabled(_settings))
            {
                _browserOverlayConfiguration.Enabled = false;
                var infoText = $"TextIngameOverlay and BrowserIngameOverlay can't be ran at the same time.{Environment.NewLine} BrowserIngameOverlay was disabled in order to prevent osu! crash.";
                _logger.Log(infoText, LogLevel.Warning);
                MessageBox.Show(infoText, "BrowserIngameOverlay Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            if (_browserOverlayConfiguration.Enabled)
                Initialize().HandleExceptions();

            SendConfiguration();
        }

        public void SendConfiguration()
        {
            _dataConsumers.ForEach(x => x.Value.Handle("Sc-webOverlayConfiguration", JsonConvert.SerializeObject(_browserOverlayConfiguration.OverlayTabs)));
        }

        public static bool TextOverlayIsEnabled(ISettings settings)
        {
            if (settings.SettingsEntries.TryGetValue("EnableIngameOverlay", out var rawTextOverlayEnabled)
                && bool.TryParse(rawTextOverlayEnabled?.ToString() ?? "", out var textOverlayEnabled))
            {
                return textOverlayEnabled;
            }

            return false;
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
            _settings.Add(BrowserOverlayConfigurationConfigEntry.Name, JsonConvert.SerializeObject(_browserOverlayConfiguration));
            SendConfiguration();

            if (eventData != null && eventData.Value.SettingName == "enable")
                _restarter($"Browser overlay was toggled. isEnabled:{eventData.Value.Value}");
        }


        private async Task Initialize()
        {
            var osuFolderDirectory = _settings.Get<string>(SettingNames.Instance.MainOsuDirectory);
            if (!Directory.Exists(osuFolderDirectory))
                return;

            var zipFileLocation = Path.Combine(_saver.SaveDirectory, "BrowserOverlay.zip");
            //Check one of the files included in overlay assets
            if (File.Exists(Path.Combine(osuFolderDirectory, "chrome_elf.dll")))
            {
                //everything seems to be in place - start watching for osu! process
                _loaderWatchdog = new LoaderWatchdog(_logger, GetFullDllLocation())
                {
                    InjectionProgressReporter = new Progress<string>(s => _logger.Log(s, LogLevel.Debug))
                };
                _loaderWatchdog.BeforeInjection += async (_, __) => await DownloadAndUnpackOverlay(zipFileLocation, osuFolderDirectory);
                _ = _loaderWatchdog.WatchForProcessStart(CancellationToken.None).HandleExceptions();
                return;
            }

            if (await DownloadAndUnpackOverlay(zipFileLocation, osuFolderDirectory))
                _restarter("browser overlay installation/update finished");
        }

        private async Task<bool> DownloadAndUnpackOverlay(string zipFileLocation, string osuFolderDirectory)
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
            ZipFile.ExtractToDirectory(zipFileLocation, osuFolderDirectory, true);
            _overlayDownloadForm.Close();
            return true;
        }

        private async Task DownloadOverlay(string destination)
        {
            const string browserOverlayUrl = @"https://pioo.space/StreamCompanion/BrowserOverlayAssets.zip";
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
