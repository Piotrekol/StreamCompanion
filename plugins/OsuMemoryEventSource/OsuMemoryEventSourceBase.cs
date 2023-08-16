using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Versioning;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using OsuMemoryDataProvider;
using StreamCompanion.Common;
using StreamCompanion.Common.Extensions;
using StreamCompanionTypes;
using StreamCompanionTypes.Attributes;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Enums;
using StreamCompanionTypes.Interfaces;
using StreamCompanionTypes.Interfaces.Consumers;
using StreamCompanionTypes.Interfaces.Services;
using StreamCompanionTypes.Interfaces.Sources;
using Timer = System.Threading.Timer;

namespace OsuMemoryEventSource
{
    [SCPluginDependency("ModsHandler", "1.0.0")]
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

        protected List<Lazy<IHighFrequencyDataConsumer>> _highFrequencyDataConsumers;
        protected IModParser _modParser;
        protected ISettings _settings;
        internal static IContextAwareLogger Logger;
        protected List<StructuredOsuMemoryReader> _clientMemoryReaders = new List<StructuredOsuMemoryReader>();
        protected StructuredOsuMemoryReader MemoryReader => _clientMemoryReaders[0];
        private readonly MemoryListener memoryListener;

        protected static readonly object _lockingObject = new object();
        private Task MemoryWorkerTask;
        private CancellationTokenSource cts = new CancellationTokenSource();
        private int _poolingMsDelay = 33;

        [SupportedOSPlatform("windows")]
        static bool IsElevated =>
            WindowsIdentity.GetCurrent().Owner?
                .IsWellKnown(WellKnownSidType.BuiltinAdministratorsSid) ?? false;

        public OsuMemoryEventSourceBase(IContextAwareLogger logger, ISettings settings,
            IModParser modParser, List<Lazy<IHighFrequencyDataConsumer>> highFrequencyDataConsumers, ISaver saver, Delegates.Exit exiter)
        {
            _settings = settings;
            _modParser = modParser;
            _highFrequencyDataConsumers = highFrequencyDataConsumers;
            Logger = logger;
            LiveTokenSetter = Tokens.CreateTokenSetter(Name);
            TokenSetter = Tokens.CreateTokenSetter($"{Name}-Regular");
            var clientCount = _settings.Get<bool>(TourneyMode)
                ? _settings.Get<int>(ClientCount)
                : 1;

            if (OperatingSystem.IsWindows() && IsElevated)
            {
                Logger.Log("StreamCompanion is running as administrator/in elevated mode. This might cause issues!", LogLevel.Warning);
            }

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
                    .Select(i =>
                    {
                        var instance = StructuredOsuMemoryReader.Instance.GetInstanceForWindowTitleHint(
                                $" Tournament Client {i}");
                        return instance;
                    }));

                //TODO: provide tournament-manager specific data via tokens
                Logger.Log($"{_clientMemoryReaders.Count} client readers prepared", LogLevel.Information);
            }
            else
            {
                _clientMemoryReaders.Add(StructuredOsuMemoryReader.Instance);
                StructuredOsuMemoryReader.Instance.InvalidRead += OnInvalidMemoryRead;
            }

            _settings.SettingUpdated += OnSettingsSettingUpdated;

            _poolingMsDelay = _settings.Get<int>(_names.MemoryPoolingFrequency);
            memoryListener = new MemoryListener(settings, saver, modParser, logger, clientCount);
            memoryListener.NewOsuEvent += async (s, args) =>
            {
                while (NewOsuEvent == null)
                {
                    await Task.Delay(5);
                }

                NewOsuEvent.Invoke(this, args);
            };
            memoryListener.SetHighFrequencyDataHandlers(_highFrequencyDataConsumers);

            MemoryWorkerTask = Task.Run(MemoryWorker, cts.Token).HandleExceptions();

            Started = true;
        }

        private void OnInvalidMemoryRead(object sender, (object readObject, string propPath) e)
        {
            Logger.Log($"Failed to read \"{e.propPath}\" memory value", LogLevel.Warning);
        }

        public Task CreateTokensAsync(IMapSearchResult map, CancellationToken cancellationToken)
        {
            return memoryListener?.CreateTokensAsync(map, cancellationToken) ?? Task.CompletedTask;
        }

        public Task SetNewMapAsync(IMapSearchResult map, CancellationToken cancellationToken)
        {
            memoryListener?.SetNewMap(map, cancellationToken);
            return Task.CompletedTask;
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

                    memoryListener?.Tick(_clientMemoryReaders, true);

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
                cts?.TryCancel();
                memoryListener?.Dispose();
            }
        }
    }
}