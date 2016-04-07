using System;
using System.Runtime.InteropServices;
using osu_StreamCompanion.Code.Core.DataTypes;
using osu_StreamCompanion.Code.Interfeaces;

namespace osu_StreamCompanion.Code.Core.Loggers
{
    class ConsoleLogger : ILogger, IDisposable
    {
        private readonly Settings _settings;

        [DllImport("kernel32")]
        private static extern bool AllocConsole();
        [DllImport("Kernel32")]
        public static extern void FreeConsole();

        public ConsoleLogger(Settings settings)
        {
            _settings = settings;
            AllocConsole();
        }


        public void Dispose()
        {
            FreeConsole();
        }

        public void Log(string logMessage, LogLevel loglvevel, params string[] vals)
        {
            if (_settings.Get("LogLevel", LogLevel.Basic.GetHashCode()) >= loglvevel.GetHashCode())
            {
                string prefix = string.Empty;
                while (logMessage.StartsWith(">"))
                {
                    prefix += "\t";
                    logMessage = logMessage.Substring(1);
                }
                logMessage = prefix + logMessage;
                Console.WriteLine(@"{0} - {1}", DateTime.Now.ToString("T"), string.Format(logMessage, vals));
            }
        }
    }
}
