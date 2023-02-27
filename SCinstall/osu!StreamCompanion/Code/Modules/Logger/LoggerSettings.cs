using System;
using System.Linq;
using osu_StreamCompanion.Code.Core.Loggers;
using osu_StreamCompanion.Code.Misc;
using StreamCompanionTypes;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Enums;
using StreamCompanionTypes.Interfaces.Services;
using StreamCompanionTypes.Interfaces.Sources;

namespace osu_StreamCompanion.Code.Modules.Logger
{
    internal class LoggerSettings : IModule, ISettingsSource, IDisposable
    {
        private readonly SettingNames _names = SettingNames.Instance;

        private readonly MainLogger mainLogger;
        private readonly ISettings settings;
        private LoggerSettingsUserControl loggerSettingsControl;
        public bool Started { get; set; }
        public void Start(ILogger logger)
        {
            Started = true;
        }

        public LoggerSettings(MainLogger mainLogger, ISettings settings)
        {
            this.mainLogger = mainLogger;
            this.settings = settings;
            settings.SettingUpdated+=SettingUpdated;
        }

        private void SettingUpdated(object sender, SettingUpdated e)
        {
            if (e.Name == _names.Console.Name && OperatingSystem.IsWindows())
            {
                if (settings.Get<bool>(_names.Console))
                {
                    mainLogger.AddLogger(new ConsoleLogger(settings));
                }
                else
                {
                    var logger = mainLogger.Loggers.FirstOrDefault(x => x is ConsoleLogger);
                    if(logger!=null)
                        mainLogger.RemoveLogger(logger);
                }
            }
        }

        public void Free() => loggerSettingsControl.Dispose();

        public object GetUiSettings()
        {
            if (loggerSettingsControl==null || loggerSettingsControl.IsDisposed)
            {
                loggerSettingsControl = new LoggerSettingsUserControl(settings);
            }

            return loggerSettingsControl;
        }

        public string SettingGroup { get; } = "General__Logging";

        public void Dispose()
        {
            loggerSettingsControl?.Dispose();
        }
    }
}