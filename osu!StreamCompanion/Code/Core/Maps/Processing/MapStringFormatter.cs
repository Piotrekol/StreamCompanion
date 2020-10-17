using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using StreamCompanionTypes;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Enums;
using StreamCompanionTypes.Interfaces.Services;
using StreamCompanionTypes.Interfaces.Sources;

namespace osu_StreamCompanion.Code.Core.Maps.Processing
{
    public class MapStringFormatter : IDisposable
    {
        private readonly SettingNames _names = SettingNames.Instance;
        private IContextAwareLogger _logger;
        private readonly MainMapDataGetter _mainMapDataGetter;
        private ISettings _settings;

        private Thread ConsumerThread;
        private ConcurrentStack<IMapSearchArgs> TasksMsn = new ConcurrentStack<IMapSearchArgs>();
        private ConcurrentStack<IMapSearchArgs> TasksMemory = new ConcurrentStack<IMapSearchArgs>();

        public MapStringFormatter(MainMapDataGetter mainMapDataGetter, List<IOsuEventSource> osuEventSources, ISettings settings, IContextAwareLogger logger)
        {
            _settings = settings;
            _mainMapDataGetter = mainMapDataGetter;
            foreach (var source in osuEventSources)
            {
                source.NewOsuEvent += NewOsuEvent;
            }
            ConsumerThread = new Thread(ConsumerTask);

            _logger = logger;
            ConsumerThread.Start();
        }

        private void NewOsuEvent(object sender, IMapSearchArgs mapSearchArgs)
        {
            if (mapSearchArgs == null)
                return;
            _isPoolingEnabled = _settings.Get<bool>(_names.EnableMemoryPooling);
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
            //TODO: priority system for IOsuEventSource 
            if (mapSearchArgs.SourceName.Contains("OsuMemory"))
            {
                _cancellationTokenSource.Cancel();
                TasksMemory.Clear();
                _logger.SetContextData("OsuMemory_event", eventData);

                TasksMemory.Push(mapSearchArgs);
            }
            else
            {
                TasksMsn.Clear();
                TasksMsn.Push(mapSearchArgs);
            }
        }

        private bool _isPoolingEnabled = true;
        CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        public async void ConsumerTask()
        {
            try
            {
                IMapSearchArgs memorySearchArgs, msnSearchArgs;
                IMapSearchResult searchResult, lastSearchResult = null;
                var memorySearchFailed = false;
                while (true)
                {
                    if (_isPoolingEnabled)
                    {
                        //Prioritize Memory events over MSN/other.
                        if (TasksMemory.TryPop(out memorySearchArgs))
                        {
                            _cancellationTokenSource = new CancellationTokenSource();
                            if (memorySearchArgs.MapId == 0 && string.IsNullOrEmpty(memorySearchArgs.MapHash))
                            {
                                memorySearchFailed = true;
                            }
                            else
                            {
                                if (memorySearchArgs.EventType == OsuEventType.MapChange || lastSearchResult == null || !lastSearchResult.BeatmapsFound.Any())
                                {
                                    _logger.SetContextData("OsuMemory_searchingForBeatmaps", "1");
                                    lastSearchResult = searchResult = _mainMapDataGetter.FindMapData(memorySearchArgs);
                                    _logger.SetContextData("OsuMemory_searchingForBeatmaps", "0");
                                }
                                else
                                {
                                    searchResult = new MapSearchResult(memorySearchArgs)
                                    {
                                        Mods = lastSearchResult.Mods
                                    };
                                    searchResult.BeatmapsFound.AddRange(lastSearchResult.BeatmapsFound);
                                }


                                if (searchResult.BeatmapsFound.Any())
                                {
                                    memorySearchFailed = false;
                                    searchResult.MapSource = memorySearchArgs.SourceName;
                                    _logger.SetContextData("OsuMemory_searchResult", new
                                    {
                                        mods = searchResult.Mods?.Mods.ToString() ?? "null",
                                        rawName = $"{searchResult.BeatmapsFound[0].Artist} - {searchResult.BeatmapsFound[0].Title} [{searchResult.BeatmapsFound[0].DiffName}]",
                                        mapId = searchResult.BeatmapsFound[0].MapId.ToString(),
                                        action = searchResult.Action.ToString()
                                    }.ToString());
                                    try
                                    {
                                        await _mainMapDataGetter.ProcessMapResult(searchResult,
                                            _cancellationTokenSource.Token);
                                    }
                                    catch(TaskCanceledException) { }
                                }
                                else
                                    memorySearchFailed = true;
                            }
                        }
                        if (memorySearchFailed)
                        {
                            if (TasksMsn.TryPop(out msnSearchArgs))
                            {
                                var status = memorySearchArgs?.Status ?? OsuStatus.Null;

                                msnSearchArgs.Status = status != OsuStatus.Null
                                    ? status
                                    : msnSearchArgs.Status;

                                searchResult = _mainMapDataGetter.FindMapData(msnSearchArgs);
                                searchResult.MapSource = msnSearchArgs.SourceName;
                                try
                                {
                                    await _mainMapDataGetter.ProcessMapResult(searchResult,
                                        _cancellationTokenSource.Token);
                                }
                                catch (TaskCanceledException) { }
                            }
                        }
                    }
                    else
                    {
                        //Use MSN/other events only
                        if (TasksMsn.TryPop(out msnSearchArgs))
                        {
                            searchResult = _mainMapDataGetter.FindMapData(msnSearchArgs);
                            searchResult.MapSource = msnSearchArgs.SourceName;
                            await _mainMapDataGetter.ProcessMapResult(searchResult, _cancellationTokenSource.Token);
                        }
                    }
                    Thread.Sleep(5);
                }
            }
            catch (ThreadAbortException)
            {
            }
            finally
            {
            }
        }


        public void Dispose()
        {
            ConsumerThread?.Abort();
        }
    }
}
