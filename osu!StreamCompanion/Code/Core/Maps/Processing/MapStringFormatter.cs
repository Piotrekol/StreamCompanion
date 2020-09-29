using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using StreamCompanionTypes;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Interfaces;
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
        private ConcurrentStack<MapSearchArgs> TasksMsn = new ConcurrentStack<MapSearchArgs>();
        private ConcurrentStack<MapSearchArgs> TasksMemory = new ConcurrentStack<MapSearchArgs>();

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

        private void NewOsuEvent(object sender, MapSearchArgs mapSearchArgs)
        {
            if (mapSearchArgs == null)
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
            //TODO: priority system for IOsuEventSource 
            if (mapSearchArgs.SourceName.Contains("OsuMemory"))
            {
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



        public void ConsumerTask()
        {
            try
            {
                bool isPoolingEnabled = _settings.Get<bool>(_names.EnableMemoryPooling);
                int counter = 0;
                MapSearchArgs memorySearchArgs;
                MapSearchArgs msnSearchArgs;
                MapSearchResult searchResult, lastSearchResult = null;
                var memorySearchFailed = false;
                while (true)
                {
                    if (counter % 400 == 0)
                    {//more or less every 2 seconds given 5ms delay at end.
                        counter = 0;
                        isPoolingEnabled = _settings.Get<bool>(_names.EnableMemoryPooling);
                    }
                    counter++;
                    if (isPoolingEnabled)
                    {
                        //Here we prioritize Memory events over MSN/other.
                        if (TasksMemory.TryPop(out memorySearchArgs))
                        {
                            if (memorySearchArgs.MapId == 0 && string.IsNullOrEmpty(memorySearchArgs.MapHash))
                            {
                                memorySearchFailed = true;
                            }
                            else
                            {
                                if (memorySearchArgs.EventType == OsuEventType.MapChange || lastSearchResult == null || !lastSearchResult.FoundBeatmaps)
                                {
                                    _logger.SetContextData("OsuMemory_searchingForBeatmaps", "1");
                                    lastSearchResult = searchResult = _mainMapDataGetter.FindMapData(memorySearchArgs);
                                    _logger.SetContextData("OsuMemory_searchingForBeatmaps", "0");
                                }
                                else
                                    searchResult = lastSearchResult;


                                if (searchResult.FoundBeatmaps)
                                {
                                    memorySearchFailed = false;
                                    searchResult.EventSource = memorySearchArgs.SourceName;
                                    _logger.SetContextData("OsuMemory_searchResult", new
                                    {
                                        mods = searchResult.Mods?.Mods.ToString() ?? "null",
                                        rawName = $"{searchResult.BeatmapsFound[0].Artist} - {searchResult.BeatmapsFound[0].Title} [{searchResult.BeatmapsFound[0].DiffName}]",
                                        mapId = searchResult.BeatmapsFound[0].MapId.ToString(),
                                        action = searchResult.Action.ToString()
                                    }.ToString());

                                    _mainMapDataGetter.ProcessMapResult(searchResult);
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
                                searchResult.EventSource = msnSearchArgs.SourceName;
                                _mainMapDataGetter.ProcessMapResult(searchResult);
                            }
                        }
                    }
                    else
                    {
                        //Use MSN/other events only
                        if (TasksMsn.TryPop(out msnSearchArgs))
                        {
                            searchResult = _mainMapDataGetter.FindMapData(msnSearchArgs);
                            searchResult.EventSource = msnSearchArgs.SourceName;
                            _mainMapDataGetter.ProcessMapResult(searchResult);
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
