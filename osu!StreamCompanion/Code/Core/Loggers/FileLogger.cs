using System;
using System.IO;
using osu_StreamCompanion.Code.Core.DataTypes;
using osu_StreamCompanion.Code.Interfeaces;
using osu_StreamCompanion.Code.Misc;

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
            CurrentLogSaveLocation = GetRelativeSaveLocation();

            CreateLogsDirectory();
        }

        private void CreateLogsDirectory()
        {
            string dir = Path.Combine(_saver.SaveDirectory, _logsSaveFolderName);

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            CurrentLogSaveLocation = GetRelativeSaveLocation();
        }


        private bool SaveDirectoryExists()
        {
            return Directory.Exists(Path.Combine(_saver.SaveDirectory, _logsSaveFolderName));
        }

        private string GetRelativeSaveLocation()
        {
            return Path.Combine(_logsSaveFolderName, startTime.ToString("yyyy-MM-dd") + ".txt");
        }
        public void Log(string logMessage, LogLevel loglvevel, params string[] vals)
        {
            try
            {
                if (_settings.Get<int>(_names.LogLevel) >= loglvevel.GetHashCode())
                    _saver.append(CurrentLogSaveLocation, string.Format(logMessage, vals) + Environment.NewLine);
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