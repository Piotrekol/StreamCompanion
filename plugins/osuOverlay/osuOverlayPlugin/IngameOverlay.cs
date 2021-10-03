using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using osuOverlay.Loader;
using StreamCompanionTypes;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Interfaces;
using StreamCompanionTypes.Enums;
using StreamCompanionTypes.Interfaces.Consumers;
using StreamCompanionTypes.Interfaces.Services;
using StreamCompanionTypes.Interfaces.Sources;

namespace osuOverlay
{
    public class IngameOverlay : IPlugin, ISettingsSource, IMapDataConsumer, IDisposable
    {
        public static readonly ConfigEntry EnableIngameOverlay = new ConfigEntry("EnableIngameOverlay", true);

        private ISettings _settings;
        private readonly Delegates.Restart _restarter;
        public string SettingGroup { get; } = "In-game overlay__Text overlay";
        private IngameOverlaySettings _overlaySettings;
        private ILogger _logger;
        private Process _currentOsuProcess;
        Loader.Loader loader = new Loader.Loader();
        private Progress<string> progressReporter;
        private bool _pauseProcessTracking;

        public string Description { get; } = "";
        public string Name { get; } = "TextIngameOverlay";
        public string Author { get; } = "Piotrekol";
        public string Url { get; } = "";
        public string UpdateUrl { get; } = "";
        CancellationTokenSource cancellationToken = new CancellationTokenSource();

        public IngameOverlay(ILogger logger, ISettings settings, Delegates.Restart restarter)
        {
            _logger = logger;
            _settings = settings;
            _restarter = restarter;
            var enabled = _settings.Get<bool>(EnableIngameOverlay);
            if (enabled && BrowserOverlayIsEnabled(_settings))
            {
                _settings.Add(EnableIngameOverlay.Name, false);

                var infoText =
                    $"TextIngameOverlay and BrowserIngameOverlay can't be ran at the same time.{Environment.NewLine} TextIngameOverlay was disabled in order to prevent osu! crash.";
                _logger.Log(infoText, LogLevel.Warning);
                MessageBox.Show(infoText, "TextIngameOverlay Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            if (_settings.Get<bool>(EnableIngameOverlay))
            {
                CopyFreeType();
                progressReporter = new Progress<string>(s => _logger.Log(s, LogLevel.Debug));
                Task.Run(() => WatchForProcessStart(cancellationToken.Token), cancellationToken.Token);
            }
        }

        private bool BrowserOverlayIsEnabled(ISettings settings)
        {
            if (settings.SettingsEntries.TryGetValue("BrowserOverlay", out var rawBrowserOverlayConfig))
            {
                var config = JsonConvert.DeserializeObject<JObject>(rawBrowserOverlayConfig?.ToString() ?? "");
                if (config.TryGetValue("Enabled", out var enabledToken) && bool.TryParse(enabledToken?.ToString() ?? "", out var browserOverlayEnabled))
                {
                    return browserOverlayEnabled;
                }
            }

            return false;
        }

        private Process GetOsuProcess()
        {
            foreach (var process in Process.GetProcesses())
            {
                if (process.ProcessName == "osu!")
                {
                    return process;
                }
            }

            return null;
        }

        private bool IsAlreadyInjected()
        {
            return loader.IsAlreadyInjected(GetFullDllLocation());
        }

        private async Task<InjectionResult> Inject()
        {
            try
            {
                return await loader.Inject(GetFullDllLocation(), progressReporter,
                    cancellationToken.Token);
            }
            catch (TaskCanceledException)
            {
                return new InjectionResult(DllInjectionResult.Cancelled, 0, 0, "Task cancelled");
            }
        }
        public async Task WatchForProcessStart(CancellationToken token)
        {
            try
            {
                var lastResult = DllInjectionResult.GameProcessNotFound;
                while (true)
                {
                    if (token.IsCancellationRequested)
                        return;

                    if (_currentOsuProcess == null || SafeHasExited(_currentOsuProcess))
                    {
                        _logger.Log("Checking osu! overlay injection status.", LogLevel.Debug);
                        var resultCode = DllInjectionResult.Success;
                        if (IsAlreadyInjected())
                        {
                            _logger.Log("Already injected & running.", LogLevel.Debug);
                        }
                        else
                        {
                            if (_currentOsuProcess == null && GetOsuProcess() != null && lastResult != DllInjectionResult.Timeout)
                            {
                                _ = Task.Run(() =>
                                {
                                    MessageBox.Show(
                                        "In order to load StreamCompanion osu! overlay you need to restart your osu!",
                                        "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                });
                            }
                            _logger.Log("Not injected - waiting for either osu! start or restart.", LogLevel.Information);

                            var result = await Inject();
                            resultCode = result.ResultCode;
                            HandleInjectionResult(result, true);
                            lastResult = result.ResultCode;
                        }

                        if (resultCode == DllInjectionResult.Success)
                        {
                            if (_currentOsuProcess == null || SafeHasExited(_currentOsuProcess))
                            {
                                _currentOsuProcess = GetOsuProcess();
                            }
                        }
                    }

                    while (_pauseProcessTracking)
                    {
                        await Task.Delay(1000);
                    }

                    await Task.Delay(2000);
                }
            }
            catch (TaskCanceledException)
            {
            }
        }
        private void HandleInjectionResult(InjectionResult helperProcessResult, bool showErrors = false)
        {
            string message = null;
            switch (helperProcessResult.ResultCode)
            {
                case DllInjectionResult.DllNotFound:
                    message =
                        "Could not find osuOverlay file to add to osu!... this shouldn't happen, if it does(you see this message) please report this.";
                    break;
                case DllInjectionResult.InjectionFailed:
                    {
                        //ERROR_ACCESS_DENIED
                        if (helperProcessResult.Win32ErrorCode == 5)
                        {
                            message = $"Your antivirus has blocked an attempt to add ingame overlay to osu!.";
                        }
                        else
                        {
                            message = "Could not add overlay to osu! most likely SC doesn't have enough premissions - restart SC as administrator and try again. If that doesn't solve it - please report ";
                        }
                        break;
                    }
                case DllInjectionResult.Cancelled:
                case DllInjectionResult.GameProcessNotFound:
                case DllInjectionResult.Timeout:
                    return;
                case DllInjectionResult.Success:
                    _logger.Log("Injection success.", LogLevel.Information);
                    return;
            }
            _logger.Log($"Injection failed: {message}", LogLevel.Information);
            _logger.Log($"{helperProcessResult}", LogLevel.Debug);

            if (showErrors && helperProcessResult.ResultCode != DllInjectionResult.GameProcessNotFound && !cancellationToken.IsCancellationRequested)
            {
                MessageBox.Show(message + Environment.NewLine + Environment.NewLine + $"Ingame Overlay result: {helperProcessResult}", "StreamCompanion - ingameOverlay Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void CopyFreeType()
        {
            var osuFolderDirectory = _settings.Get<string>(SettingNames.Instance.MainOsuDirectory);
            if (Directory.Exists(osuFolderDirectory))
            {
                var newFreeTypeLocation = Path.Combine(osuFolderDirectory, "FreeType.dll");
                if (File.Exists(newFreeTypeLocation))
                    return;

                File.Copy(GetFullFreeTypeLocation(), newFreeTypeLocation);
            }
        }
        public bool SafeHasExited(Process process)
        {

            try
            {
                return process.HasExited;
            }
            catch
            {
                return true;
            }
        }

        private string GetFilesFolder() => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins", "Dlls");

        private string GetFullFreeTypeLocation() => Path.Combine(GetFilesFolder(), "FreeType.dll");
        private string GetFullDllLocation() => Path.Combine(GetFilesFolder(), "osuOverlay.dll");
        public void Free()
        {
            _overlaySettings?.Dispose();
        }

        public object GetUiSettings()
        {
            if (_overlaySettings == null || _overlaySettings.IsDisposed)
            {
                _overlaySettings = new IngameOverlaySettings(_settings);
                _overlaySettings.OverlayToggled += (_, value) => _restarter($"Text overlay was toggled. isEnabled:{value}");
            }
            return _overlaySettings;
        }

        public Task SetNewMapAsync(IMapSearchResult map, CancellationToken cancellationToken)
        {
            try
            {
                _pauseProcessTracking = (map.Action & (OsuStatus.Playing | OsuStatus.Watching)) != 0;
            }
            catch
            {
                // ignored
            }

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _overlaySettings?.Dispose();
            cancellationToken.Cancel();
            _currentOsuProcess?.Dispose();
        }
    }
}