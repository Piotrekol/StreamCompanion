using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using StreamCompanionTypes.Enums;
using StreamCompanionTypes.Interfaces.Services;

namespace BrowserOverlay.Loader
{
    public class LoaderWatchdog
    {
        private readonly ILogger _logger;
        private readonly string _processName;
        public string DllLocation { get; set; }
        public Progress<string> InjectionProgressReporter;
        private readonly Loader _loader = new Loader();
        private Process _currentOsuProcess;

        public LoaderWatchdog(ILogger logger, string dllLocation, string processName = "osu!")
        {
            _logger = logger;
            _processName = processName;
            DllLocation = dllLocation;
        }

        private bool IsAlreadyInjected()
        {
            return _loader.IsAlreadyInjected(DllLocation);
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
                            if (_currentOsuProcess == null && GetProcess() != null && lastResult != DllInjectionResult.Timeout)
                            {
                                _ = Task.Run(() =>
                                {
                                    MessageBox.Show(
                                        "In order to load browser overlay you need to restart your osu!",
                                        "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                });
                            }
                            _logger.Log("Not injected - waiting for either osu! start or restart.", LogLevel.Information);

                            var result = await Inject(token);
                            resultCode = result.ResultCode;
                            if (token.IsCancellationRequested)
                                return;
                            HandleInjectionResult(result, true);
                            lastResult = result.ResultCode;
                        }

                        if (resultCode == DllInjectionResult.Success)
                        {
                            if (_currentOsuProcess == null || SafeHasExited(_currentOsuProcess))
                            {
                                _currentOsuProcess = GetProcess();
                            }
                        }
                    }

                    if (_currentOsuProcess != null)
                        await _currentOsuProcess.WaitForExitAsync(token);

                    await Task.Delay(1000, token);
                }
            }
            catch (TaskCanceledException)
            {
            }
        }

        private async Task<InjectionResult> Inject(CancellationToken token)
        {
            try
            {
                return await _loader.Inject(DllLocation, InjectionProgressReporter,
                    token);
            }
            catch (TaskCanceledException)
            {
                return new InjectionResult(DllInjectionResult.Cancelled, 0, 0, "Task cancelled");
            }
        }

        private Process GetProcess()
        {
            foreach (var process in Process.GetProcesses())
            {
                if (process.ProcessName == _processName)
                {
                    return process;
                }
            }

            return null;
        }

        private void HandleInjectionResult(InjectionResult helperProcessResult, bool showErrors = false)
        {
            string message = null;
            switch (helperProcessResult.ResultCode)
            {
                case DllInjectionResult.DllNotFound:
                    message =
                        "Could not find browser overlay file to add to osu!... this shouldn't happen, if it does(you see this message) please report this.";
                    break;
                case DllInjectionResult.InjectionFailed:
                    {
                        //ERROR_ACCESS_DENIED
                        if (helperProcessResult.Win32ErrorCode == 5)
                        {
                            message = $"Your antivirus has blocked an attempt to add browser overlay to osu!.";
                        }
                        else
                        {
                            message = "Could not add browser overlay to osu!. Most likely SC doesn't have enough premissions - restart SC as administrator and try again. If that doesn't solve it - please report ";
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

            if (showErrors && helperProcessResult.ResultCode != DllInjectionResult.GameProcessNotFound)
            {
                MessageBox.Show(message + Environment.NewLine + Environment.NewLine + $"browser Overlay result: {helperProcessResult}", "StreamCompanion - browser Overlay Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
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
    }
}