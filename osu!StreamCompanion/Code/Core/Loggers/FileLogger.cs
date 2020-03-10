using System;
using System.IO;
using StreamCompanionTypes;
using StreamCompanionTypes.Enums;
using StreamCompanionTypes.Interfaces.Services;

namespace osu_StreamCompanion.Code.Core.Loggers
{
    public class FileLogger : ILogger
    {
        private readonly SettingNames _names = SettingNames.Instance;
        private ISaver _saver;
        private readonly Settings _settings;
        DateTime startTime = DateTime.Today;
        private string CurrentLogSaveLocation = "";


        private readonly string _logsSaveFolderName = @"Logs\";

        public FileLogger(ISaver saver, Settings settings)
        {
            _saver = saver;
            _settings = settings;

            CreateLogsDirectory();
        }

        private void CreateLogsDirectory()
        {
            string dir = Path.Combine(_saver.SaveDirectory, _logsSaveFolderName);

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            CurrentLogSaveLocation = Path.Combine(dir, $"{startTime:yyyy-MM-dd}.txt");
        }


        private bool SaveDirectoryExists()
        {
            return Directory.Exists(Path.Combine(_saver.SaveDirectory, _logsSaveFolderName));
        }

        public void Log(object logMessage, LogLevel loglvevel, params string[] vals)
        {
            try
            {
                string message = logMessage.ToString();
                if (_settings.Get<int>(_names.LogLevel) >= loglvevel.GetHashCode())
                    File.AppendAllText(CurrentLogSaveLocation, string.Format(message, vals) + Environment.NewLine);
            }
            catch
            {
                if (SaveDirectoryExists())
                    throw;

                CreateLogsDirectory();
                Log(logMessage, loglvevel, vals);
            }
        }

        public void SetSaveHandle(ISaver saver)
        {
            _saver = saver;
        }
    }
}