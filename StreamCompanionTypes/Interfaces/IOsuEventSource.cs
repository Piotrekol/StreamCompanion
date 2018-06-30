using System;
using StreamCompanionTypes.DataTypes;

namespace StreamCompanionTypes.Interfaces
{
    public interface IOsuEventSource
    {
        EventHandler<MapSearchArgs> NewOsuEvent { get; set; }
    }
}