using StreamCompanionTypes.Enums;
using StreamCompanionTypes.Interfaces.Services;

namespace osu_StreamCompanion.Code.Core.Loggers
{
    internal class PluginLogger : IContextAwareLogger
    {
        private readonly MainLogger mainLogger;
        private readonly string pluginName;

        public PluginLogger(MainLogger mainLogger,string pluginName)
        {
            this.mainLogger = mainLogger;
            this.pluginName = pluginName;
        }

        public void Log(object logMessage, LogLevel loglvevel, params string[] vals)
            => mainLogger.Log(logMessage, loglvevel, pluginName, vals);

        public void SetContextData(string key, string value)
            => mainLogger.SetContextData(key, value);
    }
}