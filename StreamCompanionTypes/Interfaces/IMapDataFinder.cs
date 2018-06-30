using StreamCompanionTypes.DataTypes;

namespace StreamCompanionTypes.Interfaces
{
    //TODO: refactor IMapDataFinder to 2 interfaces...
    //One responsible for getting various map metadata
    //Second for getting actual map data based on that mapmetadata
    public interface IMapDataFinder
    {
        MapSearchResult FindBeatmap(MapSearchArgs searchArgs);
        OsuStatus SearchModes { get; }
        string SearcherName { get; }
    }
}