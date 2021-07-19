using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
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
        private readonly MainMapDataGetter _mainMapDataGetter;
        private ISettings _settings;

        private Task WorkerTask;
        private CancellationTokenSource workerCancellationTokenSource = new CancellationTokenSource();
        private ConcurrentStack<IMapSearchArgs> TasksMsn = new ConcurrentStack<IMapSearchArgs>();
        private ConcurrentStack<IMapSearchArgs> TasksMemory = new ConcurrentStack<IMapSearchArgs>();

        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly WorkerState _workerState = new WorkerState();

        public OsuEventHandler(MainMapDataGetter mainMapDataGetter, List<IOsuEventSource> osuEventSources, ISettings settings, IContextAwareLogger logger)
        {
            _settings = settings;
            _mainMapDataGetter = mainMapDataGetter;
            foreach (var source in osuEventSources)
            {
                source.NewOsuEvent += NewOsuEvent;
            }

            _logger = logger;
            WorkerTask = Task.Run(OsuEventWorkerLoop);
        }

        private void NewOsuEvent(object sender, IMapSearchArgs mapSearchArgs)
        {
            if (mapSearchArgs == null || workerCancellationTokenSource.IsCancellationRequested)
                return;

            _workerState.IsMemoryPoolingEnabled = _settings.Get<bool>(_names.EnableMemoryPooling);
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
            if (mapSearchArgs.SourceName.Contains("OsuMemory"))
            {
                _cancellationTokenSource.TryCancel();
                TasksMemory.Clear();
                _logger.SetContextData("OsuMemory_event", eventData);

                TasksMemory.Push(mapSearchArgs);
                return;
            }

            TasksMsn.Clear();
            TasksMsn.Push(mapSearchArgs);
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
                catch (TaskCanceledException)
                {
                }
                catch (OperationCanceledException)
                {
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

            if (_workerState.IsMemoryPoolingEnabled && !_workerState.MemorySearchFailed)
            {
                TasksMemory.TryPop(out mapSearchArgs);
                return mapSearchArgs;
            }

            _workerState.MemorySearchFailed = false;
            TasksMsn.TryPop(out mapSearchArgs);
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

            if (!mapSearchResult.BeatmapsFound.Any() && mapSearchArgs.SourceName.Contains("OsuMemory"))
            {
                _workerState.MemorySearchFailed = true;
                return;
            }

            mapSearchResult.MapSource = mapSearchArgs.SourceName;
            await HandleMapSearchResult(mapSearchResult, cancellationToken);
        }

        private async Task<IMapSearchResult> FindBeatmaps(IMapSearchArgs mapSearchArgs, CancellationToken token)
        {
            if (mapSearchArgs.MapId == 0 && string.IsNullOrEmpty(mapSearchArgs.MapHash) && string.IsNullOrEmpty(mapSearchArgs.Raw))
                //TODO: TEST
                return null;

            if (mapSearchArgs.EventType == OsuEventType.MapChange || _workerState.LastMapSearchResult == null || !_workerState.LastMapSearchResult.BeatmapsFound.Any())
            {
                await DelayBeatmapSearch(token);
                _logger.SetContextData("SearchingForBeatmaps", "1");
                _workerState.LastMapSearchResult = await _mainMapDataGetter.FindMapData(mapSearchArgs, token);
                _logger.SetContextData("SearchingForBeatmaps", "0");
                return _workerState.LastMapSearchResult;
            }

            var searchResult = new MapSearchResult(mapSearchArgs)
            {
                Mods = _workerState.LastMapSearchResult.Mods
            };
            searchResult.BeatmapsFound.AddRange(_workerState.LastMapSearchResult.BeatmapsFound);
            return searchResult;
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
            public bool IsMemoryPoolingEnabled { get; set; }
            public bool MemorySearchFailed { get; set; }
            public IMapSearchResult LastMapSearchResult { get; set; }
        }
    }
}
