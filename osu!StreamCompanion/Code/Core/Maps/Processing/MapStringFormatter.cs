using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using osu_StreamCompanion.Code.Misc;
using StreamCompanionTypes;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Interfaces;

namespace osu_StreamCompanion.Code.Core.Maps.Processing
{
    //TODO: refactor to use list of IOsuEventSource providers instead.
    public class MapStringFormatter : IModule, ISettings,IDisposable
    {
        private readonly SettingNames _names = SettingNames.Instance;
        private ILogger _logger;
        private readonly MainMapDataGetter _mainMapDataGetter;
        private readonly List<IOsuEventSource> _osuEventSources;
        private ISettingsHandler _settings;
        private string _lastMsnString = "";
        private Thread ConsumerThread;
        private ConcurrentStack<MapSearchArgs> Tasks = new ConcurrentStack<MapSearchArgs>();

        public MapStringFormatter(MainMapDataGetter mainMapDataGetter,List<IOsuEventSource> osuEventSources)
        {
            _mainMapDataGetter = mainMapDataGetter;
            _osuEventSources = osuEventSources;
            foreach (var source in osuEventSources)
            {
                source.NewOsuEvent+=NewOsuEvent;
            }
            ConsumerThread = new Thread(ConsumerTask);
        }

        private void NewOsuEvent(object sender, MapSearchArgs mapSearchArgs)
        {
            Tasks.Clear();
            Tasks.Push(mapSearchArgs);
        }

        public bool Started { get; set; }
        public void Start(ILogger logger)
        {
            _logger = logger;
            ConsumerThread.Start();
        }
        public void SetSettingsHandle(ISettingsHandler settings)
        {
            _settings = settings;
        }

        
        public void ConsumerTask()
        {
            try
            {
                MapSearchArgs searchArgs;
                MapSearchResult searchResult;
                while (true)
                {
                    if (Tasks.TryPop(out searchArgs))
                    {
                        searchResult = _mainMapDataGetter.FindMapData(searchArgs);
                        _mainMapDataGetter.ProcessMapResult(searchResult);
                    }
                    Thread.Sleep(5);
                }
            }
            catch (ThreadAbortException ex)
            {
                //Console.WriteLine("Consumer thread aborted");
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
