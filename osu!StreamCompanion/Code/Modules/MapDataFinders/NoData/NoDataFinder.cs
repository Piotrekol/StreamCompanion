using System.Collections.Generic;
using osu_StreamCompanion.Code.Core.DataTypes;
using osu_StreamCompanion.Code.Interfaces;

namespace osu_StreamCompanion.Code.Modules.MapDataFinders.NoData
{
    public class NoDataFinder : IModule,IMapDataFinder
    {
        private ILogger _logger;
        public bool Started { get; set; }

        public void Start(ILogger logger)
        {
            Started = true;
            _logger = logger;
        }

        

        public MapSearchResult FindBeatmap(Dictionary<string, string> mapDictionary)
        {

            MapSearchResult mapSearchResult = new MapSearchResult();
            mapSearchResult.MapSearchString = mapDictionary["raw"];


            return mapSearchResult;
        }

        public OsuStatus SearchModes { get; } = OsuStatus.Playing|OsuStatus.Watching|OsuStatus.FalsePlaying|OsuStatus.Listening|OsuStatus.Null;
        public string SearcherName { get; } = "~NO DATA~";
    }
}
