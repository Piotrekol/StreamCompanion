using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace osuOverlayLoader
{
    public class Main : ApplicationContext
    {
        public bool NoConsole = false;

        private void Log(string message)
        {
            if (NoConsole)
                return;

            Console.WriteLine(message);
        }

        public async void Run(string dllLocation, bool checkInjectionStatus)
        {
            if (!NoConsole)
            {
                AllocConsole();
                Console.Title = "osu!StreamCompanion - ingameOverlay loader";

                Console.CancelKeyPress += (_, __) =>
                {
                    Log("Exiting...");
                    Environment.Exit(1);
                };
            }

            if (DllInjector.GetInstance.IsAlreadyLoaded("osu!", Path.GetFileName(dllLocation)).LoadedResult ==
                DllInjector.LoadedResult.Loaded)
            {
                Log("Already loaded");
                BeforeExit();
                Environment.Exit(0);
            }

            if (checkInjectionStatus)
            {
                Log("Not loaded");
                BeforeExit();
                Environment.Exit(1);
            }

            Environment.ExitCode = 1;

            while (GetOsuProcess() != null)
            {
                Log("osu! is running - close it! Press CTRL+C at any time to cancel.");
                await Task.Delay(2000);
            }

            Process process;
            do
            {
                Log("Ready to inject - start osu! now. Press CTRL+C at any time to cancel.");
                await Task.Delay(2000);
            } while (!(
                (process = GetOsuProcess()) != null
                && !process.MainWindowTitle.Contains("osu! updater")
                && !string.IsNullOrEmpty(process.MainWindowTitle))
                );

            var result = Inject(dllLocation);
            Log(result.ToString());
            await Task.Delay(2000);
            BeforeExit();
            Environment.Exit((int)result.InjectionResult);
        }

        private void BeforeExit()
        {
            if (!NoConsole)
            {
                Log("Press any key to exit");
                Console.ReadKey();
                Log("Exiting...");
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

        private (DllInjectionResult InjectionResult, int ErrorCode, int Win32Error) Inject(string dllLocation)
        {
            DllInjector dllInjector = DllInjector.GetInstance;
            return dllInjector.Inject("osu!", dllLocation);
        }


        [DllImport("kernel32")]
        private static extern bool AllocConsole();
    }
}