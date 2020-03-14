using System;
using System.Collections.Generic;
using StreamCompanionTypes.Enums;
using StreamCompanionTypes.Interfaces.Services;

namespace osu_StreamCompanion.Code.Core.Loggers
{
    class MainLogger : IContextAwareLogger
    {
        private List<ILogger> _loggers = new List<ILogger>();

        public void AddLogger(ILogger logger)
        {
            _loggers.Add(logger);
        }
        public void Log(object logMessage, LogLevel logLevel, params string[] vals)
        {
            for (int i = 0; i < _loggers.Count; i++)
            {
                _loggers[i].Log(logMessage, logLevel, vals);
            }
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
