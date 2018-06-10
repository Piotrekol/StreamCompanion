using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Updater
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                TryStartSc();
            }
            if (args.Length != 2)
                return;
            var setupExe= args[0];
            var setupExeArgs = args[1];

            if (!File.Exists(setupExe))
            {
                Environment.ExitCode = -1;
                return;
            }
            //Console.WriteLine("Updating...");

            var p = Process.Start(setupExe, setupExeArgs);
        }

        public static bool TryStartSc()
        {
            var scExe = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "osu!StreamCompanion.exe");
            if (File.Exists(scExe))
            {
                Process.Start(scExe);
                return true;
            }
            return false;
        }
    }
}
