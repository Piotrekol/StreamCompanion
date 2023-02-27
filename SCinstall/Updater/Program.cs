using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Updater
{
    class Program
    {
        public static string ProgramName = "osu!StreamCompanion";
        public static string ProgramExe = "osu!StreamCompanion.exe";
        static void Main(string[] args)
        {
            switch (args.Length)
            {
                case 1:
                    TryStartSc(Convert.ToInt32(args[0]));
                    return;
                case 2:
                    break;
                default:
                    TryStartSc(null);
                    return;
            }

            var setupExe= args[0];
            var setupExeArgs = args[1];

            if (!File.Exists(setupExe))
            {
                Environment.ExitCode = -1;
                return;
            }

            Process.Start(setupExe, setupExeArgs);
        }

        public static bool TryStartSc(int? processId)
        {
            if (processId.HasValue)
            {
                var runningProcess = Process.GetProcessById(processId.Value);
                runningProcess.WaitForExit();
            }

            var scExe = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ProgramExe);
            if (File.Exists(scExe))
            {
                Process.Start(scExe);
                return true;
            }
            return false;
        }
    }
}
