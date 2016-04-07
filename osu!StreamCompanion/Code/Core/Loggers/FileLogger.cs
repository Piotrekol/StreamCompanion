using System;
using System.IO;
using osu_StreamCompanion.Code.Core.DataTypes;
using osu_StreamCompanion.Code.Interfeaces;

namespace osu_StreamCompanion.Code.Core.Loggers
{
    public class FileLogger : ILogger
    {
        private ISaver _saver;
        private readonly Settings _settings;
        DateTime startTime = DateTime.Today;
        private readonly string _relativeSaveLocation = @"Logs/";

        public FileLogger(ISaver saver, Settings settings)
        {
            _saver = saver;
            _settings = settings;
            CreateDirectory(_saver.SaveDirectory);
        }

        private void CreateDirectory(string baseFolder)
        {
            string dir = Path.Combine(baseFolder, _relativeSaveLocation);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }

        public void Log(string logMessage, LogLevel loglvevel, params string[] vals)
        {
            if (_settings.Get("LogLevel", LogLevel.Disabled.GetHashCode()) >= loglvevel.GetHashCode())
                _saver.append(_relativeSaveLocation + startTime.ToString("yyyy-MM-dd") + ".txt", string.Format(logMessage, vals) + Environment.NewLine);
        }

        public void SetSaveHandle(ISaver saver)
        {
            _saver = saver;
        }
    }
}