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

        public async void Run(string dllLocation)
        {
            if (DllInjector.GetInstance.IsAlreadyLoaded("osu!", Path.GetFileName(dllLocation)) == DllInjector.LoadedResult.Loaded)
                Environment.Exit(0);

            Environment.ExitCode = 1;

            if (!NoConsole)
            {
                AllocConsole();
                Console.Title = "osu!StreamCompanion - ingameOverlay loader";
            }


            Console.CancelKeyPress += (_, __) =>
            {
                Log("Exiting...");
                Environment.Exit(1);
            };

            while (GetOsuProcess() != null)
            {
                Log("osu! is running - close it! Press CTRL+C at any tme to cancel.");
                await Task.Delay(2000);
            }

            do
            {
                Log("Ready to inject - start osu! now. Press CTRL+C at any tme to cancel.");
                await Task.Delay(2000);
            } while (GetOsuProcess() == null);

            var result = Inject(dllLocation);
            Log(result.ToString());
            await Task.Delay(2000);
            Environment.Exit((int)result);
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

        private DllInjectionResult Inject(string dllLocation)
        {
            DllInjector dllInjector = DllInjector.GetInstance;
            return dllInjector.Inject("osu!", dllLocation);
        }


        [DllImport("kernel32")]
        private static extern bool AllocConsole();
    }
}