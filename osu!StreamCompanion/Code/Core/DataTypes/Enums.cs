using System;

namespace osu_StreamCompanion.Code.Core.DataTypes
{
    [Flags]
    public enum OsuStatus
    {
        Null=0,
        Listening =1,
        Playing =2,
        FalsePlaying =4,
        Watching =8,
        Editing =16,
        All = Listening|Playing|FalsePlaying|Watching|Editing
    };

}
