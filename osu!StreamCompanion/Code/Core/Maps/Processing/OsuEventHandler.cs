using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using StreamCompanionTypes;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Enums;
using StreamCompanionTypes.Interfaces.Services;
using StreamCompanionTypes.Interfaces.Sources;

namespace osu_StreamCompanion.Code.Core.Maps.Processing
{
    
    public sealed class OsuEventHandler : IDisposable
    {
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
                _cancellationTokenSource.Cancel();
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
                    _cancellationTokenSource?.Cancel();
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

        private Task HandleMapSearchArgs(IMapSearchArgs mapSearchArgs)
        {
            if (mapSearchArgs == null)
                return Task.CompletedTask;

            _cancellationTokenSource = new CancellationTokenSource();
            var mapSearchResult = FindBeatmaps(mapSearchArgs);
            if (mapSearchResult == null)
                return Task.CompletedTask;

            if (!mapSearchResult.BeatmapsFound.Any() && mapSearchArgs.SourceName.Contains("OsuMemory"))
            {
                _workerState.MemorySearchFailed = true;
                return Task.CompletedTask;
            }

            mapSearchResult.MapSource = mapSearchArgs.SourceName;
            return HandleMapSearchResult(mapSearchResult);
        }

        private IMapSearchResult FindBeatmaps(IMapSearchArgs mapSearchArgs)
        {
            if (mapSearchArgs.MapId == 0 && string.IsNullOrEmpty(mapSearchArgs.MapHash) && string.IsNullOrEmpty(mapSearchArgs.Raw))
                return null;

            if (mapSearchArgs.EventType == OsuEventType.MapChange || _workerState.LastMapSearchResult == null || !_workerState.LastMapSearchResult.BeatmapsFound.Any())
            {
                _logger.SetContextData("SearchingForBeatmaps", "1");
                _workerState.LastMapSearchResult = _mainMapDataGetter.FindMapData(mapSearchArgs, _cancellationTokenSource.Token);
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
        private async Task HandleMapSearchResult(IMapSearchResult mapSearchResult)
        {
            _logger.SetContextData("searchResult", new
            {
                mods = mapSearchResult.Mods?.Mods.ToString() ?? "null",
                rawName = $"{mapSearchResult.BeatmapsFound[0]?.Artist} - {mapSearchResult.BeatmapsFound[0]?.Title} [{mapSearchResult.BeatmapsFound[0]?.DiffName}]",
                mapId = mapSearchResult.BeatmapsFound[0]?.MapId.ToString(),
                action = mapSearchResult.Action.ToString()
            }.ToString());

            await _mainMapDataGetter.ProcessMapResult(mapSearchResult, _cancellationTokenSource.Token);
        }

        public void Dispose()
        {
            workerCancellationTokenSource.Cancel();
            workerCancellationTokenSource.Dispose();
            _cancellationTokenSource.Cancel();
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
