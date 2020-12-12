using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
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
        public static ConfigEntry TourneyMode = new ConfigEntry("TournamentMode", false);
        public static ConfigEntry ClientCount = new ConfigEntry("TournamentClientCount", 4);
        public static ConfigEntry DataClientId = new ConfigEntry("TournamentDataClientId", 0);

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
        protected List<IOsuMemoryReader> _clientMemoryReaders = new List<IOsuMemoryReader>();
        protected IOsuMemoryReader _memoryReader => _clientMemoryReaders[0];
        private readonly MemoryListener memoryListener;

        protected static readonly object _lockingObject = new object();
        private Task MemoryWorkerTask;
        private CancellationTokenSource cts = new CancellationTokenSource();
        private int _poolingMsDelay = 33;

        protected bool MemoryPoolingIsEnabled = false;

        public OsuMemoryEventSourceBase(IContextAwareLogger logger, ISettings settings,
            IDatabaseController databaseControler, IModParser modParser,
            List<IHighFrequencyDataConsumer> highFrequencyDataConsumers, ISaver saver, Delegates.Exit exiter)
        {
            _settings = settings;
            _databaseController = databaseControler;
            _modParser = modParser;
            _highFrequencyDataConsumers = highFrequencyDataConsumers;
            Logger = logger;
            LiveTokenSetter = Tokens.CreateTokenSetter(Name);
            TokenSetter = Tokens.CreateTokenSetter($"{Name}-Regular");
            var clientCount = _settings.Get<int>(ClientCount);
            if (_settings.Get<bool>(TourneyMode))
            {
                string exitReason = null;
                if (clientCount < 2)
                    exitReason = $"{ClientCount.Name} setting value is invalid. Set value equal or bigger than 2";

                if (_settings.Get<int>(DataClientId) > clientCount - 1)
                    exitReason = $"{DataClientId.Name} can't be bigger than {ClientCount.Name}. Client ids are 0-indexed";

                if (!string.IsNullOrWhiteSpace(exitReason))
                {
                    Logger.Log(exitReason, LogLevel.Warning);
                    MessageBox.Show(exitReason, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    exiter(exitReason);
                    return;
                }

                _clientMemoryReaders.AddRange(Enumerable.Range(0, clientCount)
                    .Select(i => OsuMemoryReader.Instance.GetInstanceForWindowTitleHint($" Tournament Client {i}")));

                //TODO: provide tournament-manager specific data via tokens
                var _tournamentManagerMemoryReader = OsuMemoryReader.Instance.GetInstanceForWindowTitleHint("Tournament Manager");

                Logger.Log($"{_clientMemoryReaders.Count} client readers prepared", LogLevel.Information);
            }
            else
            {
                _clientMemoryReaders.Add(OsuMemoryReader.Instance);
            }

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

            memoryListener = new MemoryListener(settings, saver, logger, clientCount);
            memoryListener.NewOsuEvent += async (s, args) =>
            {
                while (NewOsuEvent == null)
                {
                    await Task.Delay(5);
                }

                NewOsuEvent.Invoke(this, args);
            };
            memoryListener.SetHighFrequencyDataHandlers(_highFrequencyDataConsumers);

            MemoryWorkerTask = Task.Run(MemoryWorker, cts.Token);

            Started = true;
        }

        public Task CreateTokensAsync(IMapSearchResult map, CancellationToken cancellationToken)
        {
            return memoryListener?.CreateTokensAsync(map, cancellationToken) ?? Task.CompletedTask;
        }

        public void SetNewMap(IMapSearchResult map)
        {
            memoryListener?.SetNewMap(map);
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

                    memoryListener?.Tick(_clientMemoryReaders, MemoryPoolingIsEnabled);

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
                memoryListener?.Dispose();
            }
        }
    }
}