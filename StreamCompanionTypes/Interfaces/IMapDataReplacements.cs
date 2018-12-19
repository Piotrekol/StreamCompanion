using System.Collections.Generic;
using StreamCompanionTypes.DataTypes;

namespace StreamCompanionTypes.Interfaces
{
    public interface IMapDataReplacements
    {
        Tokens GetMapReplacements(MapSearchResult map);
    }
}