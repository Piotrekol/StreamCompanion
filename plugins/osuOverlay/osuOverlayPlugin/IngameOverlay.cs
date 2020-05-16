using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
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
        private ISettings _settings;
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

        public IngameOverlay(ILogger logger, ISettings settings, Delegates.Exit exiter)
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

        private class HelperProcessResult
        {
            public int ExitCode { get; }
            public int ErrorCode { get; }
            public int Win32ErrorCode { get; }
            public string Result { get; }

            public HelperProcessResult(int ExitCode, int ErrorCode, int Win32ErrorCode, string Result)
            {
                this.ExitCode = ExitCode;
                this.ErrorCode = ErrorCode;
                this.Win32ErrorCode = Win32ErrorCode;
                this.Result = Result;
            }

            public override string ToString() => $"{(InjectionResult)ExitCode}({ExitCode}),{ErrorCode},{Win32ErrorCode},{Result}";
        }

        public async Task WatchForProcessStart(CancellationToken token)
        {
            HelperProcessResult RunHelperProcess(bool silent, bool checkInjectionStatus = false)
            {
                var proc = new ProcessStartInfo
                {
                    FileName = Path.Combine("Plugins", "bin", "osuOverlayLoader.exe"),
                    Arguments = $"\"{GetFullDllLocation()}\" {silent.ToString().ToLower()} {checkInjectionStatus.ToString().ToLower()}",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true
                };
                _osuLoaderProcess = Process.Start(proc);
                while (_osuLoaderProcess?.WaitForExit(100) == false)
                {
                    if (token.IsCancellationRequested)
                    {
                        try
                        {
                            _osuLoaderProcess?.Kill();
                        }
                        catch (Win32Exception ex)
                        {
                            _logger.Log(ex, LogLevel.Debug);
                        }

                        return new HelperProcessResult(-2, 0, 0, string.Empty);
                    }
                }

                var resultData = _osuLoaderProcess.StandardOutput.ReadToEnd().Split(',');
                if (resultData.Length != 3)
                    return new HelperProcessResult(-3, 0, 0, string.Empty);

                return new HelperProcessResult(_osuLoaderProcess?.ExitCode ?? -1, Convert.ToInt32(resultData[0]), Convert.ToInt32(resultData[1]), resultData[2]);
            }

            try
            {
                InjectionResult lastResult = InjectionResult.GameProcessNotFound;
                while (true)
                {
                    if (token.IsCancellationRequested)
                        return;

                    if (_currentOsuProcess == null || SafeHasExited(_currentOsuProcess))
                    {
                        var helperProcessResult = RunHelperProcess(true, true);
                        var isAlreadyInjected = helperProcessResult.ExitCode == 0;
                        int exitCode = 0;
                        if (!isAlreadyInjected)
                        {
                            if (_currentOsuProcess == null && GetOsuProcess() != null && lastResult != InjectionResult.Timeout)
                            {
                                _ = Task.Run(() =>
                                {
                                    MessageBox.Show(
                                        "In order to load StreamCompanion osu! overlay you need to restart your osu!",
                                        "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                });
                            }

                            helperProcessResult = RunHelperProcess(true);
                            exitCode = helperProcessResult.ExitCode;
                            HandleHelperProcessResult(helperProcessResult, true);
                            lastResult = (InjectionResult)helperProcessResult.ExitCode;
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
        private void HandleHelperProcessResult(HelperProcessResult helperProcessResult, bool showErrors = false)
        {
            string message = null;
            switch (helperProcessResult.ExitCode)
            {
                case -1:
                    message = "Could not spawn helper process";
                    break;
                case -3:
                    message = "helper process exited without returning any data. Consider reinstalling ingameOverlay plugin.";
                    break;
                case (int)InjectionResult.DllNotFound:
                    message =
                        "Could not find osuOverlay file to add to osu!... this shouldn't happen, if it does(you see this message) please report this.";
                    break;
                case (int)InjectionResult.InjectionFailed:
                    {
                        //ERROR_ACCESS_DENIED
                        if (helperProcessResult.Win32ErrorCode == 5)
                        {
                            message =
                                $"Your antivirus has blocked an attempt to add ingame overlay to osu!. in order to fix this you need to add \"{GetFullLoaderExeLocation()}\" to your antivirus exceptions";
                        }
                        else
                            message =
                                "Could not add overlay to osu! most likely SC doesn't have enough premissions - restart SC as administrator and try again. If that doesn't solve it - please report ";
                        break;
                    }

                case (int)InjectionResult.GameProcessNotFound:
                case (int)InjectionResult.Timeout:
                case (int)InjectionResult.Success:
                    return;
            }

            if (showErrors && helperProcessResult.ExitCode != (int)InjectionResult.GameProcessNotFound && !cancellationToken.IsCancellationRequested)
            {
                MessageBox.Show(message + Environment.NewLine + Environment.NewLine + $"Ingame Overlay result: {helperProcessResult}", "StreamCompanion - ingameOverlay Error", MessageBoxButtons.OK,
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
        private string GetFullBinLocation() => Path.Combine(GetFilesFolder(), "bin");
        private string GetFullLoaderExeLocation() => Path.Combine(GetFullBinLocation(), "osuOverlayLoader.exe");
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