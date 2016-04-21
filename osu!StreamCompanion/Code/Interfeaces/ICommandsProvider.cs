using System.Collections.Generic;
using osu_StreamCompanion.Code.Core.DataTypes;

namespace osu_StreamCompanion.Code.Interfeaces
{
    public interface ICommandsProvider
    {
        Dictionary<string, string> GetCommands(Dictionary<string, string> replacements, OsuStatus status);

    }
}