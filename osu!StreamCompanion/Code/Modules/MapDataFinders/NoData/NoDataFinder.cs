using System.Collections.Generic;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Interfaces;

namespace osu_StreamCompanion.Code.Modules.MapDataFinders.NoData
{
    public class NoDataFinder : IModule, IMapDataFinder
    {
        private ILogger _logger;
        public bool Started { get; set; }

        public NoDataFinder(ILogger logger)
        {
            Start(logger);
        }

        public void Start(ILogger logger)
        {
            Started = true;
            _logger = logger;
        }

        

        public MapSearchResult FindBeatmap(MapSearchArgs searchArgs)
        {

            MapSearchResult mapSearchResult = new MapSearchResult(searchArgs);


            return mapSearchResult;
        }

        public OsuStatus SearchModes { get; } = OsuStatus.Playing|OsuStatus.Watching|OsuStatus.FalsePlaying|OsuStatus.Listening|OsuStatus.Null;
        public string SearcherName { get; } = "~NO DATA~";
    }
}
