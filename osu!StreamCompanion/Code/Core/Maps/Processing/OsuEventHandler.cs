using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using StreamCompanion.Common.Extensions;
using StreamCompanionTypes;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Enums;
using StreamCompanionTypes.Interfaces.Services;
using StreamCompanionTypes.Interfaces.Sources;

namespace osu_StreamCompanion.Code.Core.Maps.Processing
{

    public sealed class OsuEventHandler : IDisposable
    {
        public static ConfigEntry ConserveCpuInterval = new("ConserveCpuInterval", 350);
        public static ConfigEntry ConserveMemory = new("ConserveMemory", true);

        private readonly SettingNames _names = SettingNames.Instance;
        private IContextAwareLogger _logger;
        private readonly Delegates.Exit _exiter;
        private readonly MainMapDataGetter _mainMapDataGetter;
        private ISettings _settings;

        private Task WorkerTask;
        private CancellationTokenSource workerCancellationTokenSource = new CancellationTokenSource();
        private ConcurrentStack<IMapSearchArgs> LegacyOsuTasks = new ConcurrentStack<IMapSearchArgs>();
        private ConcurrentStack<IMapSearchArgs> OsuTasks = new ConcurrentStack<IMapSearchArgs>();

        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly WorkerState _workerState = new WorkerState();

        public OsuEventHandler(MainMapDataGetter mainMapDataGetter, List<IOsuEventSource> osuEventSources, ISettings settings, IContextAwareLogger logger, Delegates.Exit exiter)
        {
            _settings = settings;
            _mainMapDataGetter = mainMapDataGetter;
            foreach (var source in osuEventSources)
            {
                source.NewOsuEvent += NewOsuEvent;
            }

            _logger = logger;
            _exiter = exiter;
            WorkerTask = Task.Run(OsuEventWorkerLoop);
        }

        private void NewOsuEvent(object sender, IMapSearchArgs mapSearchArgs)
        {
            if (mapSearchArgs == null || workerCancellationTokenSource.IsCancellationRequested)
                return;

            var eventData = new
            {
                eventType = mapSearchArgs.EventType,
                mapId = mapSearchArgs.MapId.ToString(),
                raw = mapSearchArgs.Raw,
                hash = mapSearchArgs.MapHash,
                playMode = mapSearchArgs.PlayMode?.ToString() ?? "null",
                sourceName = mapSearchArgs.SourceName
            }.ToString();

            _logger.Log($"Received event: {eventData}", LogLevel.Debug);
            if (mapSearchArgs.SourceName.Contains("Legacy"))
            {
                LegacyOsuTasks.Clear();
                LegacyOsuTasks.Push(mapSearchArgs);
                return;
            }

            _cancellationTokenSource.TryCancel();
            OsuTasks.Clear();
            _logger.SetContextData("Osu_event", eventData);

            OsuTasks.Push(mapSearchArgs);
        }

        private async Task OsuEventWorkerLoop()
        {
            while (true)
            {
                if (workerCancellationTokenSource.IsCancellationRequested)
                {
                    _cancellationTokenSource?.TryCancel();
                    return;
                }

                try
                {
                    await HandleMapSearchArgs(GetSearchArgs());
                }
                catch (OperationCanceledException)
                {
                    _workerState.LastProcessingCancelled = true;
                }
                catch (MissingMethodException ex)
                {
                    ex.Data["PreventedCrash"] = 1;
                    _logger.Log(ex, LogLevel.Critical);
                    MessageBox.Show($"Looks like one or more files required to run StreamCompanion are corrupted. Run StreamCompanion setup again to repair. Closing now.", "StreamCompanion", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    _exiter("MissingMethodException");
                }
                catch (Exception ex)
                {
                    ex.Data["PreventedCrash"] = 1;
                    _logger.Log(ex, LogLevel.Critical);
                    _logger.Log("Prevented crash in event worker, token data for last event might be incorrect! This exception has been automatically reported.", LogLevel.Warning);
                }

                await Task.Delay(5);
            }
        }

        private IMapSearchArgs GetSearchArgs()
        {
            IMapSearchArgs mapSearchArgs;

            if (!_workerState.MapSearchFailed)
            {
                OsuTasks.TryPop(out mapSearchArgs);
                return mapSearchArgs;
            }

            _workerState.MapSearchFailed = false;
            LegacyOsuTasks.TryPop(out mapSearchArgs);
            return mapSearchArgs;
        }

        private async Task HandleMapSearchArgs(IMapSearchArgs mapSearchArgs)
        {
            if (mapSearchArgs == null)
                return;

            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = _cancellationTokenSource.Token;
            var mapSearchResult = await FindBeatmaps(mapSearchArgs, cancellationToken);
            if (_settings.Get<bool>(ConserveMemory))
                GC.Collect();

            if (mapSearchResult == null)
                return;

            if (!mapSearchResult.BeatmapsFound.Any() && !mapSearchArgs.SourceName.Contains("Legacy"))
            {
                _workerState.MapSearchFailed = true;
                return;
            }

            mapSearchResult.MapSource = mapSearchArgs.SourceName;
            await HandleMapSearchResult(mapSearchResult, cancellationToken);
        }

        private async Task<IMapSearchResult> FindBeatmaps(IMapSearchArgs mapSearchArgs, CancellationToken token)
        {
            if (mapSearchArgs.MapId <= 0 && string.IsNullOrEmpty(mapSearchArgs.MapHash) && string.IsNullOrEmpty(mapSearchArgs.Raw))
                return null;

            var performMapSearch = true;
            if (_workerState.LastProcessingCancelled)
            {
                _workerState.LastProcessingCancelled = false;
                //preserve previous search results if we have same playMode & same map _file hash_ & same mods
                performMapSearch = _workerState.LastMapSearchResult == null
                                   || _workerState.LastMapSearchResult.PlayMode != mapSearchArgs.PlayMode
                                   || !_workerState.LastMapSearchResult.BeatmapsFound.Any()
                                   || string.IsNullOrWhiteSpace(mapSearchArgs.MapHash)
                                   || _workerState.LastMapSearchResult.SearchArgs.MapHash != mapSearchArgs.MapHash
                                   || _workerState.LastMapSearchResult.SearchArgs.Mods != mapSearchArgs.Mods;
                mapSearchArgs.EventType = OsuEventType.MapChange;
            }

            if (performMapSearch && (mapSearchArgs.EventType == OsuEventType.MapChange || _workerState.LastMapSearchResult == null || !_workerState.LastMapSearchResult.BeatmapsFound.Any()))
            {
                await DelayBeatmapSearch(token);
                _logger.SetContextData("SearchingForBeatmaps", "1");
                _workerState.LastMapSearchResult = await _mainMapDataGetter.FindMapData(mapSearchArgs, token);
                _logger.SetContextData("SearchingForBeatmaps", "0");
                return _workerState.LastMapSearchResult;
            }

            _logger.Log((!performMapSearch ? "Skipped map search & " : "") + "Reusing last search result", LogLevel.Trace);

            var searchResult = new MapSearchResult(mapSearchArgs)
            {
                Mods = _workerState.LastMapSearchResult.Mods
            };
            searchResult.BeatmapsFound.AddRange(_workerState.LastMapSearchResult.BeatmapsFound);
            searchResult.SharedObjects.AddRange(_workerState.LastMapSearchResult.SharedObjects);
            return _workerState.LastMapSearchResult = searchResult;
        }

        private Task DelayBeatmapSearch(CancellationToken token)
        {
            var interval = _settings.Get<int>(ConserveCpuInterval);
            return interval > 0
                ? Task.Delay(interval, token)
                : Task.CompletedTask;
        }

        private async Task HandleMapSearchResult(IMapSearchResult mapSearchResult, CancellationToken token)
        {
            _logger.SetContextData("searchResult", new
            {
                mods = mapSearchResult.Mods?.Mods.ToString() ?? "null",
                rawName = $"{mapSearchResult.BeatmapsFound[0]?.Artist} - {mapSearchResult.BeatmapsFound[0]?.Title} [{mapSearchResult.BeatmapsFound[0]?.DiffName}]",
                mapId = mapSearchResult.BeatmapsFound[0]?.MapId.ToString(),
                action = mapSearchResult.Action.ToString()
            }.ToString());

            await _mainMapDataGetter.ProcessMapResult(mapSearchResult, token);
        }

        public void Dispose()
        {
            workerCancellationTokenSource.TryCancel();
            workerCancellationTokenSource.Dispose();
            _cancellationTokenSource.TryCancel();
            _cancellationTokenSource.Dispose();
        }

        private class WorkerState
        {
            public bool MapSearchFailed { get; set; }
            public IMapSearchResult LastMapSearchResult { get; set; }
            public bool LastProcessingCancelled { get; set; }
        }
    }
}
