using System;

namespace StreamCompanionTypes.DataTypes
{
    [Flags]
    public enum LogLevel
    {
        Disabled = 0, Basic = 1, Advanced = 2, Debug = 3, Error = 4
    }

}