using System;
using System.Collections.Generic;
using System.Threading;
using OsuMemoryDataProvider;
using StreamCompanionTypes;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Interfaces;

namespace OsuMemoryEventSource
{
    public abstract class OsuMemoryEventSourceBase : IPlugin, IDisposable, IMapDataGetter,
         IOsuEventSource, IHighFrequencyDataSender, IModParserGetter, ISqliteUser, ISettings
    {
        protected SettingNames _names = SettingNames.Instance;

        public EventHandler<MapSearchArgs> NewOsuEvent { get; set; }

        public OsuStatus SearchModes { get; } = OsuStatus.Playing;
        public string SearcherName { get; } = "Memory";
        public bool Started { get; set; }

        public string Description { get; } = "Provides accurate data directly from osu! memory";
        public string Name { get; } = nameof(OsuMemoryEventSource);
        public string Author { get; } = "Piotrekol";
        public string Url { get; } = "";
        public string UpdateUrl { get; } = "";

        protected List<IHighFrequencyDataHandler> _highFrequencyDataHandlers;
        protected ISqliteControler _sqLiteController;
        protected List<IModParser> _modParser;
        protected ISettingsHandler _settings;
        protected ILogger _logger;
        protected IOsuMemoryReader _memoryReader;
        protected MemoryListener _memoryListener;

        protected static readonly object _lockingObject = new object();
        AutoResetEvent timerDisposed = new AutoResetEvent(false);
        private long _shouldTimerRun = 1;
        private Timer _timer;
        private int _poolingMsDelay = 33;

        public virtual void Start(ILogger logger)
        {
            Started = true;
            _logger = logger;

            OsuMemoryDataProvider.DataProvider.Initalize();
            _memoryReader = OsuMemoryDataProvider.DataProvider.Instance;

            _settings.SettingUpdated += OnSettingsSettingUpdated;

            bool isFallback = _settings.Get<bool>(_names.OsuFallback);
            bool memoryScannerIsEnabled = _settings.Get<bool>(_names.EnableMemoryScanner);
            bool poolingIsEnabled = _settings.Get<bool>(_names.EnableMemoryPooling);

            if (!memoryScannerIsEnabled)
                return;
            if (isFallback)
            {
                _settings.Add(_names.EnableMemoryScanner.Name, false);
                return;
            }

            if (poolingIsEnabled)
            {
                lock (_lockingObject)
                    _timer = new Timer(TimerCallback, null, 250, Int32.MaxValue);
            }

            _memoryListener = new MemoryListener(Helpers.GetFullSongsLocation(_settings));
            _memoryListener.NewOsuEvent += (s, args) => NewOsuEvent?.Invoke(this, args);
            _memoryListener.SetHighFrequencyDataHandlers(_highFrequencyDataHandlers);
        }


        public void SetNewMap(MapSearchResult map)
        {
            lock (_lockingObject)
                _memoryListener?.SetNewMap(map);
        }

        protected virtual void OnSettingsSettingUpdated(object sender, SettingUpdated e) { }

        protected void TimerTick()
        {
            _memoryListener?.Tick(_memoryReader);
        }
        private void TimerCallback(object state)
        {
            try
            {
                lock (_lockingObject)
                    TimerTick();
            }
            finally
            {
                RestartTimer(_poolingMsDelay);
            }
        }

        private void RestartTimer(int msDelay)
        {

            lock (_lockingObject)
            {
                if (timerDisposed.WaitOne(1))
                {
                    return;
                }
                if (Interlocked.Read(ref _shouldTimerRun) == 1)
                    _timer?.Change(msDelay, Int32.MaxValue);
            }
        }
        protected void DisableTimer()
        {
            if (Interlocked.Read(ref _shouldTimerRun) == 1)
                Interlocked.Decrement(ref _shouldTimerRun);
        }

        protected void EnableTimer()
        {
            if (Interlocked.Read(ref _shouldTimerRun) == 0)
            {
                Interlocked.Increment(ref _shouldTimerRun);
                RestartTimer(500);
            }
        }

        public void SetSettingsHandle(ISettingsHandler settings)
        {
            _settings = settings;
        }

        public void SetSqliteControlerHandle(ISqliteControler sqLiteControler)
        {
            _sqLiteController = sqLiteControler;
        }

        public void SetModParserHandle(List<IModParser> modParser)
        {
            _modParser = modParser;
        }

        public void SetHighFrequencyDataHandlers(List<IHighFrequencyDataHandler> handlers)
        {
            _highFrequencyDataHandlers = handlers;
        }

        public void Dispose()
        {
            _settings.SettingUpdated -= OnSettingsSettingUpdated;
            lock (_lockingObject)
            {
                timerDisposed.Set();
                Thread.Sleep(200);
                _timer?.Dispose();
            }
        }
    }
}