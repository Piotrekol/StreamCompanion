using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using OsuMemoryDataProvider;
using StreamCompanionTypes;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Enums;
using StreamCompanionTypes.Interfaces;
using StreamCompanionTypes.Interfaces.Consumers;
using StreamCompanionTypes.Interfaces.Services;
using StreamCompanionTypes.Interfaces.Sources;
using Timer = System.Threading.Timer;

namespace OsuMemoryEventSource
{
    public abstract class OsuMemoryEventSourceBase : IPlugin, IDisposable, IMapDataConsumer,
         IOsuEventSource, ITokensSource
    {
        public static ConfigEntry SaveLiveTokensOnDisk = new ConfigEntry(nameof(SaveLiveTokensOnDisk), false);

        protected SettingNames _names = SettingNames.Instance;
        public EventHandler<IMapSearchArgs> NewOsuEvent { get; set; }
        internal static Tokens.TokenSetter LiveTokenSetter;
        internal static Tokens.TokenSetter TokenSetter;

        public OsuStatus SearchModes { get; } = OsuStatus.All;
        public string SearcherName { get; } = "Memory";
        public bool Started { get; set; }

        public string Description { get; } = "Provides accurate data directly from osu! memory";
        public string Name { get; } = nameof(OsuMemoryEventSource);
        public string Author { get; } = "Piotrekol";
        public string Url { get; } = "";
        public string UpdateUrl { get; } = "";

        protected List<IHighFrequencyDataConsumer> _highFrequencyDataConsumers;
        protected IDatabaseController _databaseController;
        protected IModParser _modParser;
        protected ISettings _settings;
        internal static IContextAwareLogger Logger;
        protected IOsuMemoryReader _memoryReader;
        protected MemoryListener _memoryListener;

        protected static readonly object _lockingObject = new object();
        private Task MemoryWorkerTask;
        private CancellationTokenSource cts = new CancellationTokenSource();
        private int _poolingMsDelay = 33;

        protected bool MemoryPoolingIsEnabled = false;

        public OsuMemoryEventSourceBase(IContextAwareLogger logger, ISettings settings,
            IDatabaseController databaseControler, IModParser modParser,
            List<IHighFrequencyDataConsumer> highFrequencyDataConsumers, ISaver saver)
        {
            _settings = settings;
            _databaseController = databaseControler;
            _modParser = modParser;
            _highFrequencyDataConsumers = highFrequencyDataConsumers;
            Logger = logger;
            LiveTokenSetter = Tokens.CreateTokenSetter(Name);
            TokenSetter = Tokens.CreateTokenSetter($"{Name}-Regular");
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

            _memoryListener = new MemoryListener(settings, saver, logger);
            _memoryListener.NewOsuEvent += async (s, args) =>
            {
                while (NewOsuEvent == null)
                {
                    await Task.Delay(5);
                }

                NewOsuEvent.Invoke(this, args);
            };
            _memoryListener.SetHighFrequencyDataHandlers(_highFrequencyDataConsumers);

            MemoryWorkerTask = Task.Run(MemoryWorker, cts.Token);

            Started = true;
        }

        public Task CreateTokensAsync(IMapSearchResult map, CancellationToken cancellationToken)
        {
            return _memoryListener?.CreateTokensAsync(map, cancellationToken);
        }

        public void SetNewMap(IMapSearchResult map)
        {
            _memoryListener?.SetNewMap(map);
        }

        protected virtual void OnSettingsSettingUpdated(object sender, SettingUpdated e) { }
        private async Task MemoryWorker()
        {
            try
            {
                while (true)
                {
                    if (cts.IsCancellationRequested)
                        return;

                    _memoryListener?.Tick(_memoryReader, MemoryPoolingIsEnabled);

                    //Note that anything below ~20ms will result in wildly inaccurate delays
                    await Task.Delay(_poolingMsDelay);
                }
            }
            catch (Win32Exception ex)
            {
                //ERROR_ACCESS_DENIED
                if (ex.NativeErrorCode == 5)
                {
                    var nl = Environment.NewLine;
                    MessageBox.Show("StreamCompanion doesn't have enough permissions to access osu! data." + nl +
                                    "Majority of live tokens, pp counts, modded star rating will not work." + nl +
                                    "Reading of live osu! data has been disabled for this StreamCompanion run, restart to try again.",
                        "StreamCompanion - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    Logger.SetContextData("ErrorCode", ex.ErrorCode.ToString());
                    Logger.SetContextData("NativeErrorCode", ex.NativeErrorCode.ToString());
                    throw;
                }
            }
        }

        public void Dispose()
        {
            _settings.SettingUpdated -= OnSettingsSettingUpdated;
            lock (_lockingObject)
            {
                cts?.Cancel();
                _memoryListener?.Dispose();
            }
        }
    }
}