using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using CollectionManager.DataTypes;
using OsuMemoryDataProvider;
using StreamCompanionTypes;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Interfaces;
using Timer = System.Threading.Timer;

namespace OsuMemoryEventSource
{
    //TODO: OsuMemoryEventSource should get refactored into several interface-scoped classes
    public class OsuMemoryEventSource : IPlugin, ISettingsProvider, ISqliteUser,
        IModParserGetter, IMapDataGetter, IDisposable, 
        IMapDataFinder, IOsuEventSource, IHighFrequencyDataSender
    {
        private SettingNames _names = SettingNames.Instance;
        private ISettingsHandler _settings;
        private ILogger _logger;
        private ISqliteControler _sqLiteController;
        private List<IModParser> _modParser;
        private IOsuMemoryReader _memoryReader;
        private List<IHighFrequencyDataHandler> _highFrequencyDataHandlers;

        public EventHandler<MapSearchArgs> NewOsuEvent { get; set; }

        public OsuStatus SearchModes { get; } = OsuStatus.Playing;
        public string SearcherName { get; } = "Memory";
        public string SettingGroup { get; } = "Map matching";
        public bool Started { get; set; }

        public string Description { get; } = "Provides accurate data directly from osu! memory";
        public string Name { get; } = nameof(OsuMemoryEventSource);
        public string Author { get; } = "Piotrekol";
        public string Url { get; } = "";
        public string UpdateUrl { get; } = "";

        private Timer _timer;
        private MemoryListener _memoryListener;
        private static readonly object _lockingObject = new object();
        private int _poolingMsDelay = 33;
        AutoResetEvent timerDisposed = new AutoResetEvent(false);
        private long _shouldTimerRun = 1;

        public void Start(ILogger logger)
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
                    _timer = new Timer(Callback, null, 250, Int32.MaxValue);
            }

            _memoryListener = new MemoryListener();
            _memoryListener.NewOsuEvent += (s, args) => NewOsuEvent.Invoke(this, args);
        }

        private void OnSettingsSettingUpdated(object sender, SettingUpdated e)
        {
            if (e.Name == _names.EnableMemoryPooling.Name)
            {
                if (_settings.Get<bool>(_names.EnableMemoryPooling))
                {
                    EnableTimer();
                }
                else
                    DisableTimer();
            }
        }

        private void Callback(object state)
        {
            try
            {
                lock (_lockingObject)
                    _memoryListener.Tick(_memoryReader);
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
        private void DisableTimer()
        {
            if (Interlocked.Read(ref _shouldTimerRun) == 1)
                Interlocked.Decrement(ref _shouldTimerRun);
        }

        private void EnableTimer()
        {
            if (Interlocked.Read(ref _shouldTimerRun) == 0)
            {
                Interlocked.Increment(ref _shouldTimerRun);
                RestartTimer(500);
            }
        }

        public void Free()
        {

        }

        public UserControl GetUiSettings()
        {
            return null;
        }


        public void SetNewMap(MapSearchResult map)
        {
            //TODO:
        }

        

        public MapSearchResult FindBeatmap(MapSearchArgs searchArgs)
        {
            var result = new MapSearchResult();
            result.MapSearchString = searchArgs.Raw;
            if (!_settings.Get<bool>(_names.EnableMemoryScanner))
                return result;

            int mapId = _memoryReader.GetMapId();
            int mods = 0;

            if (searchArgs.Status == OsuStatus.Playing)
            {
                Thread.Sleep(250);
                mods = _memoryReader.GetMods();
                result.Mods = new Tuple<Mods, string>((Mods)mods, _modParser[0].GetModsFromEnum(mods));
            }

            _logger.Log(">Got {0} & {1} from memory", LogLevel.Advanced, mapId.ToString(), mods.ToString());

            Mods eMods = result?.Mods?.Item1 ?? Mods.Omod;
            if (mapId > 2000000 || mapId < 0 || Helpers.IsInvalidCombination(eMods))
            {
                _logger.Log("Sanity check tiggered - invalidating last result", LogLevel.Advanced);
                result.Mods = null;
                return result;
            }

            var b = _sqLiteController.GetBeatmap(mapId);
            if (b != null)
            {
                result.BeatmapsFound.Add(b);
            }

            return result;
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
    }
}
