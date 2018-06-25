using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Interfaces;

namespace osu_StreamCompanion.Code.Core.Loggers
{
    class EmptyLogger : ILogger
    {
        public void Log(object logMessage, LogLevel loglvevel, params string[] vals)
        {
            
        }
    }
}
