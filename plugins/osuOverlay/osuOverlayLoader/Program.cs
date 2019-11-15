using System;
using System.Windows.Forms;

namespace osuOverlayLoader
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length < 2)
                Environment.Exit(-1);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var main = new Main();
            var silent = args[1] == "true";
            var checkInjectionStatus = args.Length>=3 && args[2] == "true";
            main.NoConsole = silent;
            main.Run(args[0], checkInjectionStatus);

            Application.Run(main);
        }
    }
}
