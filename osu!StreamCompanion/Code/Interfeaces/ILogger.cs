using osu_StreamCompanion.Code.Core.DataTypes;

namespace osu_StreamCompanion.Code.Interfeaces
{
    public interface ILogger
    {
        void Log(string logMessage, LogLevel loglvevel, params string[] vals);
    }
}
