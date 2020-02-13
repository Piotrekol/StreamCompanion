using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Interfaces;
using StreamCompanionTypes.Enums;

namespace osuOverlay
{
    public class IngameOverlay : IPlugin, ISettingsProvider, IMapDataGetter, IDisposable
    {
        private ISettingsHandler _settings;
        public string SettingGroup { get; } = "General";
        private IngameOverlaySettings _overlaySettings;
        private ILogger _logger;
        private Process _osuLoaderProcess;

        private Process _currentOsuProcess;
        private bool _pauseProcessTracking;

        public string Description { get; } = "";
        public string Name { get; } = nameof(IngameOverlay);
        public string Author { get; } = "Piotrekol";
        public string Url { get; } = "";
        public string UpdateUrl { get; } = "";
        CancellationTokenSource cancellationToken = new CancellationTokenSource();

        public IngameOverlay(ILogger logger, ISettingsHandler settings, Delegates.Exit exiter)
        {
            _logger = logger;
            _settings = settings;

            try
            {
                SetNewMap(new MapSearchResult(new MapSearchArgs("dummy")));
            }
            catch (Exception)
            {
                MessageBox.Show(
                    $"IngameOverlay plugin version is not valid for this version of StreamCompanion. {Environment.NewLine} Either update or remove it from plugins folder",
                    "osu!StreamCompanion Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                exiter("plugin version is invalid for current StreamCompanion version.");
            }

            if (_settings.Get<bool>(PluginSettings.EnableIngameOverlay))
            {
                CopyFreeType();

                Task.Run(() => WatchForProcessStart(cancellationToken.Token), cancellationToken.Token);
            }
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
        public async Task WatchForProcessStart(CancellationToken token)
        {
            int RunHelperProcess(bool silent, bool checkInjectionStatus = false)
            {
                var proc = new ProcessStartInfo(Path.Combine("Plugins", "bin", "osuOverlayLoader.exe"),
                    $"\"{GetFullDllLocation()}\" {silent.ToString().ToLower()} {checkInjectionStatus.ToString().ToLower()}");
                _osuLoaderProcess = Process.Start(proc);
                while (_osuLoaderProcess?.WaitForExit(100) == false)
                {
                    if (token.IsCancellationRequested)
                    {
                        _osuLoaderProcess?.Kill();
                        return -2;
                    }
                }
                return _osuLoaderProcess?.ExitCode ?? -1;
            }

            try
            {
                while (true)
                {
                    if (token.IsCancellationRequested)
                        return;

                    if (_currentOsuProcess == null || SafeHasExited(_currentOsuProcess))
                    {
                        var isAlreadyInjected = RunHelperProcess(true, true) == 0;
                        int exitCode = 0;
                        if (!isAlreadyInjected)
                        {
                            if (_currentOsuProcess == null && GetOsuProcess() != null)
                            {
                                _ = Task.Run(() =>
                                {
                                    MessageBox.Show(
                                        "In order to load StreamCompanion osu! overlay you need to restart your osu!",
                                        "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                });
                            }

                            exitCode = RunHelperProcess(true);
                            HandleExitCode(exitCode, true);
                        }

                        if (exitCode == 0)
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
        private void HandleExitCode(int exitCode, bool showErrors = false)
        {
            string message = null;
            switch (exitCode)
            {
                case -1:
                    message = "Could not spawn helper process";
                    break;
                case (int)InjectionResult.DllNotFound:
                    message =
                        "Could not find osuOverlay file to add to osu!... this shouldn't happen, if it does(you see this message) please report this.";
                    break;
                case (int)InjectionResult.InjectionFailed:
                    message =
                        "Could not add overlay to osu! most likely SC doesn't have enough premissions - restart SC as administrator and try again. If that doesn't solve it - please report ";
                    break;
                case (int)InjectionResult.GameProcessNotFound:
                case (int)InjectionResult.Success:
                    return;
            }

            if (showErrors && exitCode != (int)InjectionResult.GameProcessNotFound && !cancellationToken.IsCancellationRequested)
            {
                MessageBox.Show(message ?? $"Ingame Overlay exit code: {(InjectionResult)exitCode} ({exitCode})", "StreamCompanion - ingameOverlay Error!", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void CopyFreeType()
        {
            var osuFolderDirectory = _settings.Get<string>(PluginSettings.MainOsuDirectory);
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
            }
            return _overlaySettings;
        }

        public void SetNewMap(MapSearchResult map)
        {
            try
            {
                _pauseProcessTracking = map.Action == OsuStatus.Playing;
            }
            catch
            {
                // ignored
            }
        }

        public void Dispose()
        {
            _overlaySettings?.Dispose();

            _osuLoaderProcess?.Kill();
            cancellationToken.Cancel();
            _currentOsuProcess?.Dispose();
        }
    }
}