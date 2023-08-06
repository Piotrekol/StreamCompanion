using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CollectionManager.Enums;
using osu_StreamCompanion.Code.Core.Savers;
using PpCalculator;
using PpCalculatorTypes;
using StreamCompanion.Common;
using StreamCompanionTypes;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Enums;
using StreamCompanionTypes.Interfaces;
using StreamCompanionTypes.Interfaces.Consumers;
using StreamCompanionTypes.Interfaces.Services;
using StreamCompanionTypes.Interfaces.Sources;
using StreamCompanion.Common.Helpers.Tokens;

namespace osu_StreamCompanion.Code.Core.Maps.Processing
{
    public class MainMapDataGetter
    {
        private readonly List<IMapDataFinder> _mapDataFinders;
        private readonly List<IOutputPatternSource> _outputPatternSources;
        private readonly List<IMapDataConsumer> _mapDataConsumers;
        private List<ITokensSource> _tokenSources;

        private readonly MainSaver _saver;
        private ILogger _logger;
        private readonly Settings _settings;
        private readonly SettingNames _names = SettingNames.Instance;

        public MainMapDataGetter(List<IMapDataFinder> mapDataFinders, List<IMapDataConsumer> mapDataConsumers,
            List<IOutputPatternSource> outputPatternSources, List<ITokensSource> tokenSources,
            MainSaver saver, ILogger logger, Settings settings)
        {
            _mapDataFinders = mapDataFinders.OrderByDescending(x => x.Priority).ToList();
            _outputPatternSources = outputPatternSources;
            _mapDataConsumers = mapDataConsumers;
            _tokenSources = tokenSources;
            _saver = saver;
            _logger = logger;
            _settings = settings;
        }

        public async Task<IMapSearchResult> FindMapData(IMapSearchArgs searchArgs, CancellationToken cancellationToken)
        {
            IMapSearchResult mapSearchResult = null;
            IModsEx foundMods = null;
            for (int i = 0; i < _mapDataFinders.Count; i++)
            {
                if ((_mapDataFinders[i].SearchModes & searchArgs.Status) == 0)
                    continue;
                try
                {
                    mapSearchResult = await _mapDataFinders[i].FindBeatmap(searchArgs, cancellationToken);

                }
                catch (OperationCanceledException)
                {
                    throw;
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
                    _logger.Log($">Found data using \"{_mapDataFinders[i].SearcherName}\" ID: {mapSearchResult.BeatmapsFound[0]?.MapId}", LogLevel.Debug);
                    break;
                }
                if (mapSearchResult?.Mods != null)
                    foundMods = mapSearchResult.Mods;
            }

            if (mapSearchResult == null || !mapSearchResult.BeatmapsFound.Any())
            {
                _logger.Log("Couldn't find map data for specified map search args", LogLevel.Warning);
            }

            return mapSearchResult ?? new MapSearchResult(searchArgs);
        }

        public Task ProcessMapResult(IMapSearchResult mapSearchResult, CancellationToken cancellationToken)
        {
            return Task.Run(async () =>
            {
                using var tokenBulkUpdateContext = TokensBulkUpdate.StartBulkUpdate(BulkTokenUpdateType.MainPipeline);
                await CreateTokens(mapSearchResult, cancellationToken);
                if (cancellationToken.IsCancellationRequested)
                    return;

                var tokens = new Tokens();
                foreach (var token in Tokens.AllTokens)
                {
                    tokens.Add(token.Key, token.Value);
                }

                mapSearchResult.OutputPatterns.AddRange(await GetOutputPatterns(mapSearchResult, tokens, mapSearchResult.Action));

                if (cancellationToken.IsCancellationRequested)
                    return;

                if (!_settings.Get<bool>(_names.DisableDiskPatternWrite))
                    SaveMapStrings(mapSearchResult.OutputPatterns, mapSearchResult.Action);
                
                await SetNewMap(mapSearchResult, cancellationToken);
            }, cancellationToken);
        }

        private Task CreateTokens(IMapSearchResult mapSearchResult, CancellationToken cancellationToken)
        {
            List<Task> tasks = new List<Task>();
            foreach (var mapDataReplacementsGetter in _tokenSources)
            {
                var task = mapDataReplacementsGetter.CreateTokensAsync(mapSearchResult, cancellationToken);
                if (task == null)
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


        private async Task<List<IOutputPattern>> GetOutputPatterns(IMapSearchResult map, Tokens tokens, OsuStatus status)
        {
            var ret = new List<IOutputPattern>();
            foreach (var dataGetter in _outputPatternSources)
            {
                var temp = await dataGetter.GetOutputPatterns(map, tokens, status);
                if (temp?.Count > 0)
                {
                    ret.AddRange(temp);
                }
            }
            return ret;
        }

        private async Task SetNewMap(IMapSearchResult map, CancellationToken cancellationToken)
        {
            foreach (var dataGetter in _mapDataConsumers)
            {
                await dataGetter.SetNewMapAsync(map, cancellationToken);
            }
        }
    }
}
