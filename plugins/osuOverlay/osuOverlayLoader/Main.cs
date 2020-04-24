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
                Console.Write($@"0,0,Loaded");
                Environment.Exit(0);
            }

            if (checkInjectionStatus)
            {
                Log("Not loaded");
                BeforeExit();
                Console.Write($@"1,0,NotLoaded");
                Environment.Exit(1);
            }

            Environment.ExitCode = 1;
            if (NoConsole)
                _ = Task.Run(TimeoutCheck);

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
            if (!NoConsole)
                await Task.Delay(2000);
            else
                Console.Write($@"{result.ErrorCode},{result.Win32Error},{result.InjectionResult}");

            BeforeExit();
            Environment.Exit((int)result.InjectionResult);
        }

        private async Task TimeoutCheck()
        {
            if (!NoConsole)
                return;
            var stopTime = Process.GetCurrentProcess().StartTime.AddSeconds(30);
            while (true)
            {
                await Task.Delay(500);
                if (DateTime.Now > stopTime)
                {
                    Console.Write($@"0,0,{DllInjectionResult.Timeout}");
                    Environment.Exit((int)DllInjectionResult.Timeout);
                }
            }
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