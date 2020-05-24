using System;
using System.Collections.Generic;
using osu_StreamCompanion.Code.Helpers;
using StreamCompanionTypes.Enums;
using StreamCompanionTypes.Interfaces.Services;

namespace osu_StreamCompanion.Code.Core.Loggers
{
    internal class MainLogger : IContextAwareLogger
    {
        public static MainLogger Instance = new MainLogger();
        private List<ILogger> _loggers = new List<ILogger>();
        public IReadOnlyList<ILogger> Loggers => _loggers.AsReadOnly();

        private MainLogger()
        { }

        public void AddLogger(ILogger logger)
        {
            _loggers.Add(logger);
        }

        public void RemoveLogger(ILogger logger)
        {
            if (_loggers.Contains(logger))
            {
                if (logger is IDisposable disposableLogger)
                    disposableLogger.Dispose();

                _loggers.Remove(logger);
            }
        }

        public void Log(object logMessage, LogLevel logLevel, string pluginName, params string[] vals)
        {
            var (message, prefix) = GetPrefix(logMessage.ToString(), logLevel, pluginName);
            InternalLog(message, logLevel, prefix, vals);
        }

        private (string NewMessage, string Prefix) GetPrefix(string logMessage, LogLevel logLevel, string pluginName)
        {
            string message = logMessage.ToString();
            string prefix = string.Empty;

            while (message.StartsWith(">"))
            {
                prefix += "\t";
                message = message.Substring(1);
            }

            pluginName = pluginName != null ? $" [{pluginName,20}]" : string.Empty;
            prefix = string.Format(@"[{0}] {1:T}{2} - {3}", logLevel.ToString().Substring(0, 3), DateTime.Now, pluginName, prefix);

            return (message, prefix);
        }
        protected void InternalLog(object logMessage, LogLevel logLevel, string messagePrefix, params string[] vals)
        {
            string message = logMessage.ToString();


            if (message.TryFormat(out var result, vals))
                message = result;

            message = string.Format(@"{0}{1}", messagePrefix, message);

            for (int i = 0; i < _loggers.Count; i++)
            {
                _loggers[i].Log(message, logLevel, vals);
            }

        }

        public void Log(object logMessage, LogLevel logLevel, params string[] vals)
        {
            var (message, prefix) = GetPrefix(logMessage.ToString(), logLevel, null);
            InternalLog(message, logLevel, prefix, vals);
        }

        public void SetContextData(string key, string value)
        {
            for (int i = 0; i < _loggers.Count; i++)
            {
                if (_loggers[i] is IContextAwareLogger logger)
                    logger.SetContextData(key, value);
            }
        }
    }
}
