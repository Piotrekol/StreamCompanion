using osu_StreamCompanion.Code.Core.DataTypes;

namespace osu_StreamCompanion.Code.Interfaces
{
    public interface ILogger
    {
        void Log(object logMessage, LogLevel loglvevel, params string[] vals);
    }
}
