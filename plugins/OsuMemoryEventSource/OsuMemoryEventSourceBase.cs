using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using OsuMemoryDataProvider;
using StreamCompanionTypes;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Interfaces;

namespace OsuMemoryEventSource
{
    public abstract class OsuMemoryEventSourceBase : IPlugin, IDisposable, IMapDataGetter,
         IOsuEventSource, IHighFrequencyDataSender, IModParserGetter, ISqliteUser, ISettings, ITokensProvider
    {
        protected SettingNames _names = SettingNames.Instance;

        public EventHandler<MapSearchArgs> NewOsuEvent { get; set; }
        internal static Tokens.TokenSetter TokenSetter;

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
        internal static ILogger Logger;
        protected IOsuMemoryReader _memoryReader;
        protected MemoryListener _memoryListener;

        protected static readonly object _lockingObject = new object();
        AutoResetEvent timerDisposed = new AutoResetEvent(false);
        private long _shouldTimerRun = 1;
        private long _timerIsRunning = 0;
        private Timer _timer;
        private int _poolingMsDelay = 33;

        protected bool MemoryPoolingIsEnabled=false;

        public virtual void Start(ILogger logger)
        {
            Logger = logger;
            TokenSetter = Tokens.CreateTokenSetter(Name);

            _memoryReader = OsuMemoryReader.Instance;

            _settings.SettingUpdated += OnSettingsSettingUpdated;

            bool isFallback = _settings.Get<bool>(_names.OsuFallback);
            bool memoryScannerIsEnabled = _settings.Get<bool>(_names.EnableMemoryScanner);
            MemoryPoolingIsEnabled = _settings.Get<bool>(_names.EnableMemoryPooling);

            _poolingMsDelay = _settings.Get<int>(_names.MemoryPoolingFrequency);
            if (!memoryScannerIsEnabled)
                return;
            if (isFallback)
            {
                _settings.Add(_names.EnableMemoryScanner.Name, false);
                return;
            }
            
            lock (_lockingObject)
                _timer = new Timer(TimerCallback, null, 250, Int32.MaxValue);
            

            _memoryListener = new MemoryListener(Helpers.GetFullSongsLocation(_settings));
            _memoryListener.NewOsuEvent += (s, args) => NewOsuEvent?.Invoke(this, args);
            _memoryListener.SetHighFrequencyDataHandlers(_highFrequencyDataHandlers);
            _memoryListener.SetSettingsHandle(_settings);
            
            Started = true;
        }

        public void CreateTokens(MapSearchResult map)
        {
            //No need to do anything, tokens are created in MemoryDataProcessor.InitLiveTokens() and are constantly updated(live)
        }

        public void SetNewMap(MapSearchResult map)
        {
            lock (_lockingObject)
                _memoryListener?.SetNewMap(map);
        }

        protected virtual void OnSettingsSettingUpdated(object sender, SettingUpdated e) { }

        protected void TimerTick()
        {
            if (timerDisposed.WaitOne(1))
            {
                return;
            }

            _memoryListener?.Tick(_memoryReader, MemoryPoolingIsEnabled);
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
                _timer?.Dispose();
                _memoryListener?.Dispose();
            }
        }
    }
}