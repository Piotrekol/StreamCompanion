using StreamCompanionTypes.DataTypes;

namespace StreamCompanionTypes.Interfaces
{
    public interface IMapDataFinder
    {
        MapSearchResult FindBeatmap(MapSearchArgs searchArgs);
        OsuStatus SearchModes { get; }
        string SearcherName { get; }
    }
}