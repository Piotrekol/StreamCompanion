using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using osu_StreamCompanion.Code.Helpers;
using StreamCompanionTypes;
using StreamCompanionTypes.Enums;
using StreamCompanionTypes.Interfaces.Services;

namespace osu_StreamCompanion.Code.Core.Loggers
{
    class ConsoleLogger : ILogger, IDisposable
    {
        private readonly SettingNames _names = SettingNames.Instance;

        private readonly ISettings _settings;

        [DllImport("kernel32")]
        private static extern bool AllocConsole();
        [DllImport("Kernel32")]
        public static extern void FreeConsole();

        public ConsoleLogger(ISettings settings)
        {
            _settings = settings;
            AllocConsole();
            Console.Title = "StreamCompanion logs";
            Console.SetOut(TextWriter.Synchronized(new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true }));
#if !DEBUG
            Console.WindowWidth = Console.LargestWindowWidth - Convert.ToInt32(Console.LargestWindowWidth / 3);
#endif
        }


        public void Dispose()
        {
            FreeConsole();
        }

        public void Log(object logMessage, LogLevel loglvevel, params string[] vals)
        {
            if (_settings.Get<int>(_names.LogLevel) >= loglvevel.GetHashCode())
                Console.WriteLine(logMessage.ToString());
        }
    }
}
