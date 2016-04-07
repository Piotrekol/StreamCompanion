using System.Collections.Generic;
using osu_StreamCompanion.Code.Core.DataTypes;

namespace osu_StreamCompanion.Code.Interfeaces
{
    public interface IMapDataFinder
    {
        MapSearchResult FindBeatmap(Dictionary<string,string> mapDictionary);
        OsuStatus SearchModes { get; }
        string SearcherName { get; }
    }
}