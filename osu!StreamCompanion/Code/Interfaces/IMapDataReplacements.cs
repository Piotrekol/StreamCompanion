using System.Collections.Generic;
using osu_StreamCompanion.Code.Core.DataTypes;

namespace osu_StreamCompanion.Code.Interfaces
{
    public interface IMapDataReplacements
    {
        Dictionary<string, string> GetMapReplacements(MapSearchResult map);
    }
}