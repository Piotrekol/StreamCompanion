using System;

namespace osu_StreamCompanion.Code.Core.DataTypes
{
    [Flags]
    public enum OsuStatus
    {
        Null = 0,
        Listening = 1 << 0,
        Playing = 1 << 1,
        FalsePlaying = 1 << 2,
        Watching = 1 << 3,
        Editing = 1 << 4,
        ResultsScreen = 1 << 5,

        Listening2 = Listening | ResultsScreen,
        All = Listening | Playing | FalsePlaying | Watching | Editing | ResultsScreen,
    };

}
