using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using StreamCompanionTypes.Enums;
using StreamCompanionTypes.Interfaces.Services;

namespace Overlay.Common.Loader
{
    public class LoaderWatchdog
    {
        private readonly ILogger _logger;
        private readonly string _processName;
        public string DllLocation { get; set; }
        private IProgress<OverlayReport> _statusReporter;
        private readonly Loader _loader = new Loader();
        private Process? _currentOsuProcess;
        private IProgress<string> _injectionProgressReporter;
        public event EventHandler BeforeInjection
        {
            add => _loader.BeforeInjection += value;
            remove => _loader.BeforeInjection -= value;
        }

        public LoaderWatchdog(ILogger logger, string dllLocation, Progress<OverlayReport> statusReporter, string processName = "osu!")
        {
            _logger = logger;
            _processName = processName;
            DllLocation = dllLocation;
            _statusReporter = statusReporter;
            _injectionProgressReporter = new Progress<string>(message => _statusReporter.Report(new(ReportType.Log, message)));
            BeforeInjection += OnBeforeInjection;
        }

        private void OnBeforeInjection(object sender, EventArgs e)
        {
            var moduleList = _loader.ListModules().Select(m => m.ToLowerInvariant()).ToList();
            var unknownModules = moduleList.Except(KnownOsuModules.Modules).ToList();
            var troublesomeModules = KnownOsuModules.TroubleMakers.Select(m => m.Key).Intersect(moduleList).ToList();
            if (unknownModules.Any())
                _logger.Log($"This is a list of unknown files loaded in osu!. If you are experiencing startup osu! crashes or overlay just not appearing ingame, these will help with finding conflicting application:{Environment.NewLine}{string.Join(Environment.NewLine, unknownModules)}", LogLevel.Debug);
            else
                _logger.Log("osu! module list is clean", LogLevel.Debug);

            if (troublesomeModules.Any())
                _logger.Log($"Detected modules that could potentialy prevent overlay from starting properly:{Environment.NewLine}{string.Join(Environment.NewLine, troublesomeModules.Select(m => $"{m} - {KnownOsuModules.TroubleMakers[m]}"))}", LogLevel.Warning);
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
                        if (_loader.IsAlreadyInjected(DllLocation))
                        {
                            _logger.Log("Already injected & running.", LogLevel.Debug);
                        }
                        else
                        {
                            if (_currentOsuProcess == null && GetProcess() != null && lastResult != DllInjectionResult.Timeout)
                            {
                                _ = Task.Run(() => _statusReporter.Report(new(ReportType.Information, "In order to load ingame overlay you need to restart your osu!")));
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

                    await Task.Delay(500, token);
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
                return await _loader.Inject(DllLocation, new Progress<string>(), token);
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
                        "Could not find overlay file to add to osu!... this shouldn't happen, if it does(you see this message) please report this.";
                    break;
                case DllInjectionResult.HelperProcessFailed:
                case DllInjectionResult.InjectionFailed:
                    {
                        //ERROR_ACCESS_DENIED
                        if (helperProcessResult.Win32ErrorCode == 5 || helperProcessResult.Win32ErrorCode == 0 && helperProcessResult.ErrorCode == -2)
                        {
                            message = $"Your antivirus has blocked an attempt to add overlay to osu!. Adding an antivirus exception to SC folder & installing overlay again might help.";
                        }
                        else
                        {
                            message = "Could not add overlay to osu!. Most likely SC doesn't have enough premissions - restart SC as administrator and try again. If that doesn't solve it - please report ";
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
                _statusReporter.Report(new(ReportType.Error, message + Environment.NewLine + $"Raw error data: {helperProcessResult}"));
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