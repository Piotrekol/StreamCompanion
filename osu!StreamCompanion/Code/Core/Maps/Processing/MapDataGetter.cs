using System.Collections.Generic;
using osu_StreamCompanion.Code.Core.DataTypes;
using osu_StreamCompanion.Code.Core.Savers;
using osu_StreamCompanion.Code.Interfaces;

namespace osu_StreamCompanion.Code.Core.Maps.Processing
{
    public class MainMapDataGetter
    {
        private readonly List<IMapDataFinder> _mapDataFinders;
        private readonly List<IMapDataParser> _mapDataParsers;
        private readonly List<IMapDataGetter> _mapDataGetters;
        private List<IMapDataReplacements> _mapDataReplacementsGetters;

        private readonly MainSaver _saver;
        private ILogger _logger;

        public MainMapDataGetter(List<IMapDataFinder> mapDataFinders, List<IMapDataGetter> mapDataGetters, List<IMapDataParser> mapDataParsers, List<IMapDataReplacements> mapDataReplacementsGetters, MainSaver saver,ILogger logger)
        {
            _mapDataFinders = mapDataFinders;
            _mapDataParsers = mapDataParsers;
            _mapDataGetters = mapDataGetters;
            _mapDataReplacementsGetters = mapDataReplacementsGetters;
            _saver = saver;
            _logger = logger;
        }

        public MapSearchResult FindMapData(MapSearchArgs searchArgs)
        {
            MapSearchResult mapSearchResult=null;
            for (int i = 0; i < _mapDataFinders.Count; i++)
            {
                if ((_mapDataFinders[i].SearchModes & searchArgs.Status) == 0)
                    continue;

                mapSearchResult = _mapDataFinders[i].FindBeatmap(searchArgs);
                if (mapSearchResult.FoundBeatmaps)
                {
                    _logger.Log(string.Format(">Found data using \"{0}\" ID: {1}", _mapDataFinders[i].SearcherName,mapSearchResult.BeatmapsFound[0]?.MapId),LogLevel.Advanced);
                    break;
                }
            }
            if(mapSearchResult==null)
                mapSearchResult = new MapSearchResult();
            mapSearchResult.Action = searchArgs.Status;
            return mapSearchResult;
        }

        public void ProcessMapResult(MapSearchResult mapSearchResult)
        {
            var mapReplacements = GetMapReplacements(mapSearchResult);
            mapSearchResult.FormatedStrings = GetMapFormatedStrings(mapReplacements, mapSearchResult.Action);
            SaveMapStrings(mapSearchResult.FormatedStrings);
            SetNewMap(mapSearchResult);
        }

        private Dictionary<string,string> GetMapReplacements(MapSearchResult mapSearchResult)
        {
            var ret = new Dictionary<string,string>();
            foreach (var mapDataReplacementsGetter in _mapDataReplacementsGetters)
            {
                var temp = mapDataReplacementsGetter.GetMapReplacements(mapSearchResult);
                if (temp?.Count > 0)
                {
                    foreach (var t in temp)
                    {
                        if (ret.ContainsKey(t.Key))
                            continue;
                        ret.Add(t.Key, t.Value);
                    }
                }

            }
            return ret;
        }

        private void SaveMapStrings(Dictionary<string, string> formatedStrings)
        {
            foreach (var formatedString in formatedStrings)
            {
                _saver.Save(formatedString.Key,formatedString.Value);
            }
        }


        private Dictionary<string, string> GetMapFormatedStrings(Dictionary<string,string> replacements,OsuStatus status)
        {
            var ret = new Dictionary<string, string>();
            foreach (var dataGetter in _mapDataParsers)
            {
                var temp  = dataGetter.GetFormatedMapStrings(replacements,status);
                if (temp?.Count > 0)
                {
                    foreach (var t in temp)
                    {
                        if(ret.ContainsKey(t.Key))
                            continue;
                        ret.Add(t.Key,t.Value);
                    }
                }
            }
            return ret;
        }

        private void SetNewMap(MapSearchResult map)
        {
            foreach (var dataGetter in _mapDataGetters)
            {
                dataGetter.SetNewMap(map);
            }
        }
    }
}
