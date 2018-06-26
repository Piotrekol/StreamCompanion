using System;
using System.Runtime.InteropServices;
using osu_StreamCompanion.Code.Misc;
using StreamCompanionTypes;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Interfaces;

namespace osu_StreamCompanion.Code.Core.Loggers
{
    class ConsoleLogger : ILogger, IDisposable
    {
        private readonly SettingNames _names = SettingNames.Instance;

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

        public void Log(object logMessage, LogLevel loglvevel, params string[] vals)
        {
            if (_settings.Get<int>(_names.LogLevel) >= loglvevel.GetHashCode())
            {
                string message = logMessage.ToString();
                string prefix = string.Empty;
                while (message.StartsWith(">"))
                {
                    prefix += "\t";
                    message = message.Substring(1);
                }
                message = prefix + message;
                Console.WriteLine(@"{0} - {1}", DateTime.Now.ToString("T"), string.Format(message, vals));
            }
        }
    }
}
