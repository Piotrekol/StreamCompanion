using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using osu_StreamCompanion.Code.Core.Savers;
using StreamCompanionTypes;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Enums;
using StreamCompanionTypes.Interfaces;
using StreamCompanionTypes.Interfaces.Consumers;
using StreamCompanionTypes.Interfaces.Services;
using StreamCompanionTypes.Interfaces.Sources;

namespace osu_StreamCompanion.Code.Core.Maps.Processing
{
    public class MainMapDataGetter
    {
        private readonly List<IMapDataFinder> _mapDataFinders;
        private readonly List<IOutputPatternGenerator> _outputPatternGenerators;
        private readonly List<IMapDataConsumer> _mapDataConsumers;
        private List<ITokensSource> _tokenSources;

        private readonly MainSaver _saver;
        private ILogger _logger;
        private readonly Settings _settings;
        private readonly SettingNames _names = SettingNames.Instance;

        public MainMapDataGetter(List<IMapDataFinder> mapDataFinders, List<IMapDataConsumer> mapDataConsumers,
            List<IOutputPatternGenerator> outputPatternGenerators, List<ITokensSource> tokenSources,
            MainSaver saver, ILogger logger, Settings settings)
        {
            _mapDataFinders = mapDataFinders.OrderByDescending(x => x.Priority).ToList();
            _outputPatternGenerators = outputPatternGenerators;
            _mapDataConsumers = mapDataConsumers;
            _tokenSources = tokenSources;
            _saver = saver;
            _logger = logger;
            _settings = settings;
        }

        public IMapSearchResult FindMapData(IMapSearchArgs searchArgs)
        {
            IMapSearchResult mapSearchResult = null;
            IModsEx foundMods = null;
            for (int i = 0; i < _mapDataFinders.Count; i++)
            {
                if ((_mapDataFinders[i].SearchModes & searchArgs.Status) == 0)
                    continue;
                try
                {
                    mapSearchResult = _mapDataFinders[i].FindBeatmap(searchArgs);

                }
                catch (Exception e)
                {
                    _logger.Log(e, LogLevel.Error);
                    mapSearchResult = null;
                }

                if (mapSearchResult != null && mapSearchResult.BeatmapsFound.Any())
                {
                    if (mapSearchResult.Mods == null && foundMods != null)
                        mapSearchResult.Mods = foundMods;
                    _logger.Log(string.Format(">Found data using \"{0}\" ID: {1}", _mapDataFinders[i].SearcherName, mapSearchResult.BeatmapsFound[0]?.MapId), LogLevel.Advanced);
                    break;
                }
                if (mapSearchResult?.Mods != null)
                    foundMods = mapSearchResult.Mods;
            }

            return mapSearchResult ?? new MapSearchResult(searchArgs);
        }

        public Task ProcessMapResult(IMapSearchResult mapSearchResult, CancellationToken cancellationToken)
        {
            return Task.Run(async () =>
            {
                await CreateTokens(mapSearchResult, cancellationToken);
                if (cancellationToken.IsCancellationRequested)
                    return;

                var tokens = new Tokens();
                foreach (var token in Tokens.AllTokens)
                {
                    tokens.Add(token.Key, token.Value);
                }

                mapSearchResult.OutputPatterns.AddRange(GetOutputPatterns(tokens, mapSearchResult.Action));

                if (cancellationToken.IsCancellationRequested)
                    return;

                if (!_settings.Get<bool>(_names.DisableDiskPatternWrite))
                    SaveMapStrings(mapSearchResult.OutputPatterns, mapSearchResult.Action);
                SetNewMap(mapSearchResult);
            }, cancellationToken);
        }

        private Task CreateTokens(IMapSearchResult mapSearchResult, CancellationToken cancellationToken)
        {
            List<Task> tasks = new List<Task>();
            foreach (var mapDataReplacementsGetter in _tokenSources)
            {
                var task = mapDataReplacementsGetter.CreateTokensAsync(mapSearchResult, cancellationToken);
                if(task == null)
                    throw new NullReferenceException($"received null instead of task in ${mapDataReplacementsGetter.GetType().FullName}");
                
                tasks.Add(task);
            }

            return Task.WhenAll(tasks);
        }

        private void SaveMapStrings(List<IOutputPattern> patterns, OsuStatus status)
        {

            foreach (var p in patterns)
            {
                if (!p.IsMemoryFormat)
                {
                    if ((p.SaveEvent & status) != 0)
                        _saver.Save(p.Name + ".txt", p.GetFormatedPattern());
                    else
                        _saver.Save(p.Name + ".txt", "");

                }
            }
        }


        private List<IOutputPattern> GetOutputPatterns(Tokens tokens, OsuStatus status)
        {
            var ret = new List<IOutputPattern>();
            foreach (var dataGetter in _outputPatternGenerators)
            {
                var temp = dataGetter.GetOutputPatterns(tokens, status);
                if (temp?.Count > 0)
                {
                    ret.AddRange(temp);
                }
            }
            return ret;
        }

        private void SetNewMap(IMapSearchResult map)
        {
            foreach (var dataGetter in _mapDataConsumers)
            {
                dataGetter.SetNewMap(map);
            }
        }
    }
}
