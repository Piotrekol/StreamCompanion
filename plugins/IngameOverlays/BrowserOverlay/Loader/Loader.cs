using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BrowserOverlay.Loader
{
    public class Loader : ApplicationContext
    {
        private string _lastMessage = string.Empty;
        public event EventHandler BeforeInjection;

        public bool IsAlreadyInjected(string dllLocation)
            => DllInjector.GetInstance.IsAlreadyLoaded("osu!", Path.GetFileName(dllLocation)).LoadedResult ==
               DllInjector.LoadedResult.Loaded;

        public IEnumerable<string> ListModules()
            => DllInjector.GetInstance.ListModules("osu!");

        public async Task<InjectionResult> Inject(string dllLocation, IProgress<string> progress,
            CancellationToken cancellationToken)
        {
            while (GetOsuProcess() != null)
            {
                await Task.Delay(2000, cancellationToken);
                Report(progress, "Waiting for osu! process to close");
            }

            Process process;
            do
            {
                await Task.Delay(2000, cancellationToken);
                Report(progress, "Waiting for osu! process to start");
            } while (!(
                (process = GetOsuProcess()) != null
                && !process.MainWindowTitle.Contains("osu! updater")
                && !string.IsNullOrEmpty(process.MainWindowTitle))
            );

            BeforeInjection?.Invoke(this, EventArgs.Empty);
            progress?.Report("Injecting");

            DllInjector dllInjector = DllInjector.GetInstance;
            var result = dllInjector.Inject("osu!", dllLocation);

            return new InjectionResult(result.InjectionResult, result.errorCode, result.Win32Error, "_");
        }

        private void Report(IProgress<string> progress, string message)
        {
            if (_lastMessage == message)
                return;

            _lastMessage = message;
            progress?.Report(message);
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
    }
}