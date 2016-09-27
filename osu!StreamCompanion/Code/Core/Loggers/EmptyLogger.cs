using osu_StreamCompanion.Code.Core.DataTypes;
using osu_StreamCompanion.Code.Interfeaces;

namespace osu_StreamCompanion.Code.Core.Loggers
{
    class EmptyLogger : ILogger
    {
        public void Log(object logMessage, LogLevel loglvevel, params string[] vals)
        {
            
        }
    }
}
