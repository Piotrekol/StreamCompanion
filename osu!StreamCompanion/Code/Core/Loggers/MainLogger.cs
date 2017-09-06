using System;
using System.Collections.Generic;
using osu_StreamCompanion.Code.Core.DataTypes;
using osu_StreamCompanion.Code.Interfaces;

namespace osu_StreamCompanion.Code.Core.Loggers
{
    class MainLogger : ILogger
    {
        private ILogger _logger = new EmptyLogger();
        private List<ILogger> _loggers = new List<ILogger>();
        public void ChangeLogger(ILogger logger)
        {
            if (_logger is IDisposable)
                ((IDisposable)_logger).Dispose();
            _logger = null;

            _logger = logger;
        }

        public void AddLogger(ILogger logger)
        {
            _loggers.Add(logger);
        }
        public void Log(object logMessage, LogLevel logLevel, params string[] vals)
        {
            _logger?.Log(logMessage, logLevel, vals);
            for (int i = 0; i < _loggers.Count; i++)
            {
                _loggers[i].Log(logMessage, logLevel, vals);
            }
        }
    }
}
