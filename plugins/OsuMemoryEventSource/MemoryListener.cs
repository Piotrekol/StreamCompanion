using CollectionManager.Enums;
using OsuMemoryDataProvider;
using StreamCompanionTypes.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OsuMemoryDataProvider.OsuMemoryModels.Abstract;
using OsuMemoryDataProvider.OsuMemoryModels.Direct;
using StreamCompanion.Common.Helpers;
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
        private DateTime _nextReplayRetryAllowedAt = DateTime.MinValue;
        private OsuMemoryStatus _lastStatus = OsuMemoryStatus.Unknown;
        private OsuMemoryStatus _lastStatusLog = OsuMemoryStatus.Unknown;
        private OsuMemoryStatus _currentStatus = OsuMemoryStatus.Unknown;
        private string _lastMapHash = "";
        private int _lastMapSelectionMods = -2;
        private int _lastPlayingMods = -2;
        private List<MemoryDataProcessor> _memoryDataProcessors;
        private PatternsDispatcher _patternsDispatcher;
        private ISettings _settings;

        public MemoryListener(ISettings settings, ISaver saver, IContextAwareLogger logger, int clientCount = 1)
        {
            _settings = settings;
            _settings.SettingUpdated += SettingUpdated;
            var tournamentMode = clientCount > 1;
            var mainClientId = tournamentMode
                ? _settings.Get<int>(OsuMemoryEventSourceBase.DataClientId)
                : 0;
            _memoryDataProcessors = Enumerable.Range(0, clientCount)
                .Select(x => new MemoryDataProcessor(settings, logger, x == mainClientId, tournamentMode ? $"client_{x}_" : string.Empty)).ToList();
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
            reader.Read(osuData.GeneralData);
            _currentStatus = osuData.GeneralData.OsuStatus;
            if (_lastStatusLog != _currentStatus)
            {
                _lastStatusLog = _currentStatus;
                //Console.WriteLine("status: {0} {1}", _currentStatus, _currentStatus == OsuMemoryStatus.Unknown ? num.ToString() : "");
            }

            if (_currentStatus == OsuMemoryStatus.NotRunning)
                return;

            reader.Read(osuData.Beatmap);
            _currentMapId = osuData.Beatmap.Id;
            var isReplay = (bool?)reader.ReadProperty(osuData.Player, nameof(Player.IsReplay));
            if (isReplay == null)
                return;

            OsuStatus status = _currentStatus.Convert();
            status = status == OsuStatus.Playing && isReplay.Value
                ? OsuStatus.Watching
                : status;

            var gameMode = osuData.GeneralData.GameMode;
            var mapHash = osuData.Beatmap.Md5;
            var mapSelectionMods = osuData.GeneralData.Mods;
            var playingMods = -1;
            if (status == OsuStatus.Playing || status == OsuStatus.Watching)
            {
                var mods = ((Mods)reader.ReadProperty(osuData.Player, nameof(Player.Mods)))?.Value;
                if (mods == null)
                    return;

                playingMods = mods.Value;
            }
            var retries = osuData.GeneralData.Retries;
            var currentTime = osuData.GeneralData.AudioTime;

            var mapHashDiffers = mapHash != null && _lastMapHash != null && _lastMapHash != mapHash;
            var mapIdDiffers = _lastMapId != _currentMapId;
            var memoryStatusDiffers = _lastStatus != _currentStatus;
            var gameModeDiffers = gameMode != _lastGameMode;
            var mapSelectionModsDiffer = mapSelectionMods != _lastMapSelectionMods;
            var playingModsDiffer = (status == OsuStatus.Watching || status == OsuStatus.Playing) && playingMods != _lastPlayingMods;
            OsuEventType? osuEventType = null;
            //"good enough" replay retry detection.
            if (isReplay.Value && _currentStatus == OsuMemoryStatus.Playing && _lastTime > currentTime && DateTime.UtcNow > _nextReplayRetryAllowedAt)
            {
                osuEventType = OsuEventType.PlayChange;
                _nextReplayRetryAllowedAt = DateTime.UtcNow.AddMilliseconds(500);
            }

            _lastTime = currentTime;
            var playInitialized = (status != OsuStatus.Watching && status != OsuStatus.Playing) || playingMods != -1;
            if (sendEvents && playInitialized && (
                    osuEventType.HasValue
                    || mapIdDiffers || memoryStatusDiffers
                    || mapHashDiffers || gameModeDiffers
                    || mapSelectionModsDiffer
                    || retries != _lastRetries
                )
            )
            {
                if (!osuEventType.HasValue || playingModsDiffer)
                {
                    osuEventType =
                        mapIdDiffers || mapHashDiffers || gameModeDiffers || mapSelectionModsDiffer || playingModsDiffer ? OsuEventType.MapChange //different mapId/hash/mode/mods(changed stats) = different map
                        : memoryStatusDiffers ? OsuEventType.SceneChange //memory scene(status) change = Scene change
                        : _currentStatus == OsuMemoryStatus.Playing ? OsuEventType.PlayChange // map retry
                        : OsuEventType.MapChange; //bail
                }

                _lastMapId = _currentMapId;
                _lastStatus = _currentStatus;
                _lastRetries = retries;
                _lastGameMode = gameMode;
                _lastMapSelectionMods = mapSelectionMods;
                _lastMapHash = mapHash;
                _lastPlayingMods = playingMods;

                var rawString = Retry.RetryMe(() => (string)reader.ReadProperty(osuData.Beatmap, nameof(CurrentBeatmap.Md5)),
                    s => (System.Text.Encoding.UTF8.GetByteCount(s ?? string.Empty) == s?.Length), 5) ?? string.Empty;

                NewOsuEvent?.Invoke(this, new MemoryMapSearchArgs(osuEventType.Value)
                {
                    MapId = _currentMapId,
                    Status = status,
                    Raw = rawString,
                    MapHash = mapHash,
                    PlayMode = (PlayMode)gameMode,
                    Mods = playingMods != -1 ? playingMods : mapSelectionMods
                });
            }

            for (int i = 0; i < clientReaders.Count; i++)
            {
                _memoryDataProcessors[i].Tick(status, _currentStatus, clientReaders[i]);
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