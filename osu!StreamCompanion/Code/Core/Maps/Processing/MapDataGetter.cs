using System.Collections.Generic;
using osu_StreamCompanion.Code.Core.DataTypes;
using osu_StreamCompanion.Code.Core.Savers;
using osu_StreamCompanion.Code.Interfeaces;

namespace osu_StreamCompanion.Code.Core.Maps.Processing
{
    public class MainMapDataGetter
    {
        private readonly List<IMapDataFinder> _mapDataFinders;
        private readonly List<IMapDataParser> _mapDataParsers;
        private readonly List<IMapDataGetter> _mapDataGetters;
        private List<IMapDataReplacements> _mapDataReplacementsGetters;

        private readonly MainSaver _saver;
        private MapSearchResult _mapSearchResult;
        private ILogger _logger;

        public MainMapDataGetter(List<IMapDataFinder> mapDataFinders, List<IMapDataGetter> mapDataGetters, List<IMapDataParser> mapDataParsers, List<IMapDataReplacements> mapDataReplacementsGetters, MainSaver saver, ILogger logger)
        {
            _mapDataFinders = mapDataFinders;
            _mapDataParsers = mapDataParsers;
            _mapDataGetters = mapDataGetters;
            _mapDataReplacementsGetters = mapDataReplacementsGetters;
            _saver = saver;
            _logger = logger;
        }

        public void FindMapData(Dictionary<string, string> mapDict, OsuStatus status)
        {
            for (int i = 0; i < _mapDataFinders.Count; i++)
            {
                if ((_mapDataFinders[i].SearchModes & status) == 0)
                    continue;

                _mapSearchResult = _mapDataFinders[i].FindBeatmap(mapDict);
                if (_mapSearchResult.FoundBeatmaps)
                {
                    _logger.Log(string.Format(">Found data using \"{0}\" ID: {1}", _mapDataFinders[i].SearcherName, _mapSearchResult.BeatmapsFound[0]?.MapId), LogLevel.Advanced);
                    break;
                }
            }
            if (_mapSearchResult == null)
                _mapSearchResult = new MapSearchResult();
            _mapSearchResult.Action = status;


            var mapReplacements = GetMapReplacements(_mapSearchResult);
            _mapSearchResult.FormatedStrings = GetMapFormatedStrings(mapReplacements, _mapSearchResult.Action);
            _mapSearchResult.Commands = GetCommandStrings(mapReplacements, _mapSearchResult.Action);

            SaveMapStrings(_mapSearchResult.FormatedStrings);
            SetNewMap(_mapSearchResult);
        }

        private Dictionary<string, string> GetMapReplacements(MapSearchResult mapSearchResult)
        {
            var ret = new Dictionary<string, string>();
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
                _saver.Save(formatedString.Key, formatedString.Value);
            }
        }


        private Dictionary<string, string> GetMapFormatedStrings(Dictionary<string, string> replacements, OsuStatus status)
        {
            var ret = new Dictionary<string, string>();
            foreach (var dataGetter in _mapDataParsers)
            {
                var temp = dataGetter.GetFormatedMapStrings(replacements, status);
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

        private Dictionary<string, string> GetCommandStrings(Dictionary<string, string> replacements, OsuStatus status)
        {
            var ret = new Dictionary<string, string>();
            foreach (var dataGetter in _mapDataParsers)
            {
                if (dataGetter is ICommandsProvider)
                {
                    var temp = ((ICommandsProvider)dataGetter).GetCommands(replacements, status);
                    if (temp?.Count > 0)
                    {
                        foreach (var t in temp)
                        {
                            string key = t.Key;
                            if (!key.StartsWith("!"))
                                key = "!" + key;

                            if (ret.ContainsKey(key))
                                continue;

                            ret.Add(key, t.Value);
                        }
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
