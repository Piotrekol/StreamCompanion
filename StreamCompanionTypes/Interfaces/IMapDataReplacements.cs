using System.Collections.Generic;
using StreamCompanionTypes.DataTypes;

namespace StreamCompanionTypes.Interfaces
{
    public interface IMapDataReplacements
    {
        Dictionary<string, string> GetMapReplacements(MapSearchResult map);
    }
}