using System;

namespace osu_StreamCompanion.Code.Core.DataTypes
{
    [Flags]
    public enum LogLevel
    {
        Disabled = 0, Basic = 1, Advanced = 2, Debug = 4
    }

}