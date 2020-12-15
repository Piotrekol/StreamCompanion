using System;
using System.IO;
using System.Linq;
using StreamCompanionTypes;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Enums;
using StreamCompanionTypes.Interfaces.Services;

namespace osu_StreamCompanion.Code.Core.Loggers
{
    public class FileLogger : ILogger
    {
        private readonly SettingNames _names = SettingNames.Instance;
        public static ConfigEntry LogsRetentionDays = new ConfigEntry("LogsRetentionDays", 14);
        private ISaver _saver;
        private readonly Settings _settings;
        private readonly ILogger _parentLogger;
        DateTime startTime = DateTime.Today;
        private string _logsSaveLocation = String.Empty;
        private string _saveDirectory = String.Empty;
        private object _lockingObject = new object();

        private readonly string _logsSaveFolderName = @"Logs\";

        internal FileLogger(ISaver saver, Settings settings, ILogger parentLogger = null)
        {
            _saver = saver;
            _settings = settings;
            _parentLogger = parentLogger;

            CreateLogsDirectory();
            CleanupLogs();
        }

        private void CleanupLogs()
        {
            try
            {
                var deleteDateThreshold = DateTime.UtcNow.AddDays(-_settings.Get<int>(LogsRetentionDays));
                var logFilePaths = Directory.GetFiles(_saveDirectory, "*.txt", SearchOption.TopDirectoryOnly);
                var logFiles = logFilePaths.Select(l => new FileInfo(l))
                    .Where(f => f.LastWriteTimeUtc < deleteDateThreshold);

                foreach (var logFile in logFiles)
                {
                    logFile.Delete();
                }
            }
            catch (Exception ex)
            {
                _parentLogger?.Log(ex, LogLevel.Error);
            }
        }

        private void CreateLogsDirectory()
        {
            _saveDirectory = Path.Combine(_saver.SaveDirectory, _logsSaveFolderName);

            if (!Directory.Exists(_saveDirectory))
                Directory.CreateDirectory(_saveDirectory);

            _logsSaveLocation = Path.Combine(_saveDirectory, $"{startTime:yyyy-MM-dd}.txt");
        }

        public void Log(object logMessage, LogLevel loglvevel, params string[] vals)
            => InternalLog(logMessage, loglvevel, 0, vals);

        private void InternalLog(object logMessage, LogLevel loglvevel, int attemptCount, params string[] vals)
        {
            try
            {
                if (logMessage is Exception ex && ex.Data.Contains("Logger") &&
                    ex.Data["Logger"].ToString() == nameof(FileLogger))
                    return;

                if (_settings.Get<int>(_names.LogLevel) <= loglvevel.GetHashCode())
                {
                    lock (_lockingObject)
                    {
                        File.AppendAllText(_logsSaveLocation, logMessage + Environment.NewLine);
                    }
                }
            }
            catch (Exception exception)
            {
                if (attemptCount >= 3)
                {
                    exception.Data["Logger"] = nameof(FileLogger);
                    _parentLogger?.Log(exception, LogLevel.Error);
                    return;
                }

                lock (_lockingObject)
                    CreateLogsDirectory();

                InternalLog(logMessage, loglvevel, ++attemptCount, vals);
            }
        }

        public void SetSaveHandle(ISaver saver)
        {
            _saver = saver;
        }
    }
}