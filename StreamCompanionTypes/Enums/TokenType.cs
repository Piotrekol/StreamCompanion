using System;

namespace StreamCompanionTypes.Enums
{
    [Flags]
    public enum TokenType
    {
        Normal = 0, //Token value changes only on map metadata change.
        Live = 1 << 0, //Token might change over time on same map metadata.
    }
}