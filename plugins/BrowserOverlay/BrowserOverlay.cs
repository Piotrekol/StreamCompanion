using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
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
        public string Name { get; } = nameof(BrowserOverlay);
        public string Author { get; } = "Piotrekol";
        public string Url { get; } = string.Empty;
        public string UpdateUrl { get; } = string.Empty;
        public string SettingGroup { get; } = "browser overlay";

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
            _browserOverlayConfiguration.OverlayConfiguration ??= new OverlayConfiguration();
            if (_browserOverlayConfiguration.Enabled)
                Initialize().HandleExceptions();

            SendConfiguration();
        }

        public void SendConfiguration()
        {
            _dataConsumers.ForEach(x => x.Value.Handle("Sc-webOverlayConfiguration", JsonConvert.SerializeObject(_browserOverlayConfiguration.OverlayConfiguration)));
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
                _browserOverlaySettings.OnSettingUpdated += OnSettingUpdated;
            }
            return _browserOverlaySettings;
        }

        private void OnSettingUpdated(object? sender, EventArgs e)
        {
            _settings.Add(BrowserOverlayConfigurationConfigEntry.Name, JsonConvert.SerializeObject(_browserOverlayConfiguration));
            SendConfiguration();
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
                _ = _loaderWatchdog.WatchForProcessStart(CancellationToken.None).HandleExceptions();
                return;
            }

            _overlayDownloadForm = new OverlayDownload();
            _overlayDownloadForm.Show();
            if (!File.Exists(zipFileLocation))
            {
                await DownloadOverlay(zipFileLocation);
            }

            if (!File.Exists(zipFileLocation))
                return;

            _overlayDownloadForm.SetStatus("unpacking files...");
            ZipFile.ExtractToDirectory(zipFileLocation, osuFolderDirectory, true);
            _overlayDownloadForm.Close();
            _restarter("browser overlay installation/update finished");
        }

        private async Task DownloadOverlay(string destination)
        {
            const string browserOverlayUrl = @"https://pioo.space/StreamCompanion/BrowserOverlayAssets.zip";
            using var client = new WebClient();
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
        public OverlayConfiguration OverlayConfiguration { get; set; }
    }
    public class OverlayConfiguration
    {
        public string Url { get; set; } = "http://localhost:20727/overlays/SC_PP%20Counter/";
        public decimal Scale { get; set; } = 1;
        public Canvas Canvas { get; set; } = new Canvas();
        public Position Position { get; set; } = new Position();
    }

    public class Position
    {
        public int X = 0;
        public int Y = 0;
    }

    public class Canvas
    {
        public int Width = 500;
        public int Height = 90;
    }

}
