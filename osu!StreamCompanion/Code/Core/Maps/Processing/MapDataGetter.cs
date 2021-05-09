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

        public IMapSearchResult FindMapData(IMapSearchArgs searchArgs, CancellationToken cancellationToken)
        {
            IMapSearchResult mapSearchResult = null;
            IModsEx foundMods = null;
            for (int i = 0; i < _mapDataFinders.Count; i++)
            {
                if ((_mapDataFinders[i].SearchModes & searchArgs.Status) == 0)
                    continue;
                try
                {
                    mapSearchResult = _mapDataFinders[i].FindBeatmap(searchArgs, cancellationToken);

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
                mapSearchResult.SharedObjects.Add(CreatePpCalculatorTask(mapSearchResult));
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
                SetNewMap(mapSearchResult, cancellationToken);
            }, cancellationToken);
        }

        private CancelableAsyncLazy<IPpCalculator> CreatePpCalculatorTask(IMapSearchResult mapSearchResult) =>
            new CancelableAsyncLazy<IPpCalculator>((cancellationToken) =>
            {
                if (!(mapSearchResult.BeatmapsFound.Any() &&
                      mapSearchResult.BeatmapsFound[0].IsValidBeatmap(_settings, out var mapLocation)))
                    return Task.FromResult<IPpCalculator>(null);
                var desiredGamemode = mapSearchResult.PlayMode.HasValue ? (int?)mapSearchResult.PlayMode : null;
                var playMode = (PlayMode)PpCalculatorHelpers.GetRulesetId((int)mapSearchResult.BeatmapsFound[0].PlayMode, desiredGamemode);
                var ppCalculator = PpCalculatorHelpers.GetPpCalculator((int)playMode, mapLocation, null);
                ppCalculator.Mods = (mapSearchResult.Mods?.WorkingMods ?? "").Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                try
                {
                    ppCalculator.Calculate(cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                //specifically for BeatmapInvalidForRulesetException (beatmap had invalid hitobject with missing position data)
                catch (Exception e)
                {
                    e.Data["PreventedCrash"] = 1;
                    _logger.Log(e, LogLevel.Critical);
                    return Task.FromResult<IPpCalculator>(null);
                }

                return Task.FromResult((IPpCalculator)ppCalculator);
            });

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

        private void SetNewMap(IMapSearchResult map, CancellationToken cancellationToken)
        {
            foreach (var dataGetter in _mapDataConsumers)
            {
                dataGetter.SetNewMap(map, cancellationToken);
            }
        }
    }
}
