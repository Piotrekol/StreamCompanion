using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Versioning;
using System.Text;
using osu_StreamCompanion.Code.Helpers;
using osu_StreamCompanion.Code.Misc;
using StreamCompanionTypes;
using StreamCompanionTypes.Enums;
using StreamCompanionTypes.Interfaces.Services;

namespace osu_StreamCompanion.Code.Core.Loggers
{
    [SupportedOSPlatform("windows")]
    class ConsoleLogger : IContextAwareLogger, IDisposable
    {
        private readonly SettingNames _names = SettingNames.Instance;

        private readonly ISettings _settings;
        public static Dictionary<string, string> ContextData { get; } = new();
        private object _lockingObject = new();

        public ConsoleLogger(ISettings settings)
        {
            _settings = settings;
            NativeMethods.AllocConsole();
            Console.Title = "StreamCompanion logs";
            Console.SetOut(TextWriter.Synchronized(new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true }));

            if (Console.LargestWindowWidth > 0)
                Console.WindowWidth = Console.LargestWindowWidth - Convert.ToInt32(Console.LargestWindowWidth / 3);
        }


        public void Dispose()
        {
            NativeMethods.FreeConsole();
        }

        public void Log(object logMessage, LogLevel loglvevel, params string[] vals)
        {
            if (_settings.Get<int>(_names.LogLevel) <= loglvevel.GetHashCode())
            {
                if (logMessage is Exception)
                {
                    lock (_lockingObject)
                        Console.WriteLine(logMessage + Environment.NewLine + string.Join(Environment.NewLine, ContextData));
                }
                else
                    Console.WriteLine(logMessage);
            }
        }

        public void SetContextData(string key, string value)
        {
            lock (_lockingObject)
                ContextData[key] = value;
        }
    }
}
