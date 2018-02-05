using System.Collections.Generic;
using osu_StreamCompanion.Code.Core.DataTypes;

namespace osu_StreamCompanion.Code.Interfaces
{
    public interface IMapDataFinder
    {
        MapSearchResult FindBeatmap(MapSearchArgs searchArgs);
        OsuStatus SearchModes { get; }
        string SearcherName { get; }
    }
}