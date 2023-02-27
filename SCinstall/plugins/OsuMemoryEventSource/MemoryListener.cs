using CollectionManager.Enums;
using OsuMemoryDataProvider;
using StreamCompanionTypes.DataTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OsuMemoryDataProvider.OsuMemoryModels.Abstract;
using OsuMemoryDataProvider.OsuMemoryModels.Direct;
using StreamCompanion.Common;
using StreamCompanion.Common.Helpers;
using StreamCompanionTypes;
using StreamCompanionTypes.Enums;
using StreamCompanionTypes.Interfaces.Consumers;
using StreamCompanionTypes.Interfaces.Services;
using StreamCompanionTypes.Interfaces.Sources;

namespace OsuMemoryEventSource
{
    public class MemoryListener : IOsuEventSource, IDisposable
    {
        public EventHandler<IMapSearchArgs> NewOsuEvent { get; set; }

        private int _lastMapId = -1;
        private int _currentMapId = -2;
        private int _lastGameMode = -1;
        private int _lastRetries = -1;
        private int _lastTime = -1;
        private readonly IToken _osuIsRunningToken = OsuMemoryEventSourceBase.LiveTokenSetter("osuIsRunning", 0, TokenType.Live, null, 0);
        private DateTime _nextReplayRetryAllowedAt = DateTime.MinValue;
        private OsuMemoryStatus _lastStatus = OsuMemoryStatus.Unknown;
        private OsuMemoryStatus _lastStatusLog = OsuMemoryStatus.Unknown;
        private OsuMemoryStatus _currentStatus = OsuMemoryStatus.Unknown;
        private string _lastMapHash = "";
        private int _lastSentMods = -2;
        private List<MemoryDataProcessor> _memoryDataProcessors;
        private PatternsDispatcher _patternsDispatcher;
        private ISettings _settings;
        private readonly IContextAwareLogger _logger;

        public MemoryListener(ISettings settings, ISaver saver, IModParser modParser, IContextAwareLogger logger,
            int clientCount = 1)
        {
            _settings = settings;
            _logger = logger;
            _settings.SettingUpdated += SettingUpdated;
            var tournamentMode = clientCount > 1;
            var mainClientId = tournamentMode
                ? _settings.Get<int>(OsuMemoryEventSourceBase.DataClientId)
                : 0;
            _memoryDataProcessors = Enumerable.Range(0, clientCount)
                .Select(x => new MemoryDataProcessor(settings, logger, modParser, x == mainClientId, tournamentMode ? $"client_{x}_" : string.Empty)).ToList();
            _patternsDispatcher = new PatternsDispatcher(settings, saver);
            var mainProcessor = _memoryDataProcessors.First(p => p.IsMainProcessor);
            mainProcessor.TokensUpdated += (sender, status) => _patternsDispatcher.TokensUpdated(status);
            foreach (var memoryDataProcessor in _memoryDataProcessors)
            {
                memoryDataProcessor.ToggleSmoothing(_settings.Get<bool>(Helpers.EnablePpSmoothing));
            }
        }

        public void Tick(List<StructuredOsuMemoryReader> clientReaders, bool sendEvents)
        {
            var reader = clientReaders[0];
            var osuData = reader.OsuMemoryAddresses;

            var canRead = reader.CanRead;
            _osuIsRunningToken.Value = canRead ? 1 : 0;

            if (!canRead || !reader.TryRead(osuData.GeneralData))
                return;

            _currentStatus = osuData.GeneralData.OsuStatus;
            if (_lastStatusLog != _currentStatus)
            {
                _lastStatusLog = _currentStatus;
                //Console.WriteLine("status: {0} {1}", _currentStatus, _currentStatus == OsuMemoryStatus.Unknown ? num.ToString() : "");
            }

            if (_currentStatus == OsuMemoryStatus.NotRunning)
                return;

            if (_currentStatus == OsuMemoryStatus.Unknown)
            {
                _logger.Log($"Unknown memory status: {osuData.GeneralData.RawStatus}", LogLevel.Warning);
                return;
            }

            if (!reader.TryRead(osuData.Beatmap) || !TryGetOsuFileLocation(osuData.Beatmap, out var osuFileLocation))
                return;

            _currentMapId = osuData.Beatmap.Id;
            OsuStatus status = _currentStatus.Convert();
            object rawIsReplay = false;
            if (status == OsuStatus.Playing && !reader.TryReadProperty(osuData.Player, nameof(Player.IsReplay), out rawIsReplay))
                return;

            var isReplay = (bool)rawIsReplay;
            status = status == OsuStatus.Playing && isReplay
                ? OsuStatus.Watching
                : status;
            var gameMode = osuData.GeneralData.GameMode;
            if (gameMode < 0 || gameMode > 3)
                return;

            var mapHash = osuData.Beatmap.Md5;
            var mods = osuData.GeneralData.Mods;
            if (status == OsuStatus.Playing || status == OsuStatus.Watching)
            {
                if (!reader.TryReadProperty(osuData.Player, nameof(Player.Mods), out var rawMods) || rawMods == null)
                    return;

                mods = ((Mods)rawMods).Value;
            }
            else if (status == OsuStatus.ResultsScreen)
            {
                if (!reader.TryReadProperty(osuData.ResultsScreen, nameof(ResultsScreen.Mods), out var rawMods) || rawMods == null)
                    return;

                mods = ((Mods)rawMods).Value;
            }

            if (Helpers.IsInvalidCombination((CollectionManager.DataTypes.Mods)mods))
                return;

            var retries = osuData.GeneralData.Retries;
            var currentTime = osuData.GeneralData.AudioTime;

            var mapHashDiffers = mapHash != null && _lastMapHash != null && _lastMapHash != mapHash;
            var mapIdDiffers = _lastMapId != _currentMapId;
            var memoryStatusDiffers = _lastStatus != _currentStatus;
            var gameModeDiffers = gameMode != _lastGameMode;
            var modsDiffer = mods != _lastSentMods;
            OsuEventType? osuEventType = null;
            //"good enough" replay retry detection.
            if (isReplay && _currentStatus == OsuMemoryStatus.Playing && _lastTime > currentTime && DateTime.UtcNow > _nextReplayRetryAllowedAt)
            {
                osuEventType = mapIdDiffers || mapHashDiffers || gameModeDiffers || modsDiffer ? OsuEventType.MapChange : OsuEventType.PlayChange;
                _nextReplayRetryAllowedAt = DateTime.UtcNow.AddMilliseconds(500);
            }

            _lastTime = currentTime;
            var playInitialized = (status != OsuStatus.Watching && status != OsuStatus.Playing) || mods != -1;
            if (sendEvents && playInitialized && (
                    osuEventType.HasValue
                    || mapIdDiffers || memoryStatusDiffers
                    || mapHashDiffers || gameModeDiffers || modsDiffer
                    || retries != _lastRetries
                )
            )
            {
                if (!osuEventType.HasValue || modsDiffer)
                {
                    osuEventType =
                        mapIdDiffers || mapHashDiffers || gameModeDiffers || modsDiffer ? OsuEventType.MapChange //different mapId/hash/mode/mods(changed stats) = different map
                        : memoryStatusDiffers ? OsuEventType.SceneChange //memory scene(status) change = Scene change
                        : _currentStatus == OsuMemoryStatus.Playing ? OsuEventType.PlayChange // map retry
                        : OsuEventType.MapChange; //bail
                }

                _lastMapId = _currentMapId;
                _lastStatus = _currentStatus;
                _lastRetries = retries;
                _lastGameMode = gameMode;
                _lastMapHash = mapHash;
                _lastSentMods = mods;
                var rawString = Retry.RetryMe(
                    () =>
                    {
                        var validRead = reader.TryReadProperty(osuData.Beatmap, nameof(CurrentBeatmap.MapString), out var result);
                        return (validRead, (string)result);
                    },
                    s => (s.validRead, s.Item2), 5) ?? string.Empty;

                NewOsuEvent?.Invoke(this, new MapSearchArgs("OsuMemory",osuEventType.Value)
                {
                    MapId = _currentMapId,
                    Status = status,
                    Raw = rawString,
                    MapHash = mapHash,
                    PlayMode = (PlayMode)gameMode,
                    Mods = (CollectionManager.DataTypes.Mods)mods,
                    OsuFilePath = osuFileLocation
                });
            }

            for (int i = 0; i < clientReaders.Count; i++)
            {
                _memoryDataProcessors[i].Tick(status, _currentStatus, clientReaders[i]);
            }
        }

        private bool TryGetOsuFileLocation(CurrentBeatmap memoryBeatmap, out string osuFileLocation)
        {
            osuFileLocation = null;
            try
            {
                var songsLocation = _settings.GetFullSongsLocation();
                if (string.IsNullOrEmpty(memoryBeatmap.FolderName) || string.IsNullOrEmpty(songsLocation))
                    return false;

                osuFileLocation = Path.Combine(songsLocation, memoryBeatmap.FolderName.TrimEnd(), memoryBeatmap.OsuFileName);
                return true;
            }
            catch (ArgumentException)
            {
                //we have garbage data in either FolderName or OsuFileName
                return false;
            }
        }

        public void SetNewMap(IMapSearchResult map, CancellationToken cancellationToken)
        {
            foreach (var memoryDataProcessor in _memoryDataProcessors)
            {
                _ = memoryDataProcessor.SetNewMap(map, cancellationToken);
            }

            _patternsDispatcher.SetOutputPatterns(map.OutputPatterns);
        }

        public void SetHighFrequencyDataHandlers(List<Lazy<IHighFrequencyDataConsumer>> consumers)
        {
            _patternsDispatcher.HighFrequencyDataConsumers = consumers;
        }

        public void Dispose()
        {
            foreach (var memoryDataProcessor in _memoryDataProcessors)
            {
                memoryDataProcessor?.Dispose();
            }
            _settings.SettingUpdated -= SettingUpdated;
        }

        private void SettingUpdated(object sender, SettingUpdated settingUpdated)
        {
            if (settingUpdated.Name == Helpers.EnablePpSmoothing.Name)
            {
                var enableSmoothing = _settings.Get<bool>(Helpers.EnablePpSmoothing);
                foreach (var memoryDataProcessor in _memoryDataProcessors)
                {
                    memoryDataProcessor.ToggleSmoothing(enableSmoothing);
                }
            }
        }

        public Task CreateTokensAsync(IMapSearchResult map, CancellationToken cancellationToken)
        {
            var tasks = new List<Task>();
            foreach (var memoryDataProcessor in _memoryDataProcessors)
            {
                tasks.Add(memoryDataProcessor.CreateTokensAsync(map, cancellationToken));
            }

            return Task.WhenAll(tasks);
        }
    }
}