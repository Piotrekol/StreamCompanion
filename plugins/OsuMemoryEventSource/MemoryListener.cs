using CollectionManager.Enums;
using OsuMemoryDataProvider;
using StreamCompanionTypes.DataTypes;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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
        private MemoryDataProcessor _memoryDataProcessor;
        private PatternsDispatcher _patternsDispatcher;
        public Tokens Tokens => _memoryDataProcessor.Tokens;
        private ISettings _settings;

        public MemoryListener(ISettings settings, ISaver saver, IContextAwareLogger logger)
        {
            _settings = settings;
            _settings.SettingUpdated += SettingUpdated;

            _memoryDataProcessor = new MemoryDataProcessor(settings, logger);
            _patternsDispatcher = new PatternsDispatcher(settings, saver);
            _memoryDataProcessor.TokensUpdated += (_, status) => _patternsDispatcher.TokensUpdated(status);
            _memoryDataProcessor.ToggleSmoothing(_settings.Get<bool>(Helpers.EnablePpSmoothing));
        }

        public void Tick(IOsuMemoryReader reader, bool sendEvents)
        {
            _currentStatus = reader.GetCurrentStatus(out _);
            if (_lastStatusLog != _currentStatus)
            {
                _lastStatusLog = _currentStatus;
                //Console.WriteLine("status: {0} {1}", _currentStatus, _currentStatus == OsuMemoryStatus.Unknown ? num.ToString() : "");
            }

            if (_currentStatus != OsuMemoryStatus.NotRunning)
            {
                _currentMapId = reader.GetMapId();
                var isReplay = reader.IsReplay();
                OsuStatus status = _currentStatus.Convert();
                status = status == OsuStatus.Playing && isReplay
                    ? OsuStatus.Watching
                    : status;

                var gameMode = reader.ReadSongSelectGameMode();
                var mapHash = reader.GetMapMd5Safe();
                var mapSelectionMods = reader.GetMods();
                var retries = reader.GetRetrys();
                var currentTime = reader.ReadPlayTime();

                var mapHashDiffers = mapHash != null && _lastMapHash != null && _lastMapHash != mapHash;
                var mapIdDiffers = _lastMapId != _currentMapId;
                var memoryStatusDiffers = _lastStatus != _currentStatus;
                var gameModeDiffers = gameMode != _lastGameMode;
                var mapSelectionModsDiffer = mapSelectionMods != _lastMapSelectionMods;

                OsuEventType? osuEventType = null;
                //"good enough" replay retry detection.
                if (isReplay && _currentStatus == OsuMemoryStatus.Playing && _lastTime > currentTime && DateTime.UtcNow > _nextReplayRetryAllowedAt)
                {
                    osuEventType = OsuEventType.PlayChange;
                    _nextReplayRetryAllowedAt = DateTime.UtcNow.AddMilliseconds(500);
                }

                _lastTime = currentTime;

                if (sendEvents && (
                        osuEventType.HasValue
                        || mapIdDiffers || memoryStatusDiffers
                        || mapHashDiffers || gameModeDiffers
                        || mapSelectionModsDiffer
                        || retries != _lastRetries
                        )
                    )
                {
                    if (!osuEventType.HasValue)
                    {
                        osuEventType =
                            mapIdDiffers || mapHashDiffers || gameModeDiffers || mapSelectionModsDiffer ? OsuEventType.MapChange //different mapId/hash/mode/mods(changed stats) = different map
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

                    NewOsuEvent?.Invoke(this, new MapSearchArgs("OsuMemory", osuEventType.Value)
                    {
                        MapId = _currentMapId,
                        Status = status,
                        Raw = reader.GetSongString(),
                        MapHash = mapHash,
                        PlayMode = (PlayMode)gameMode,
                    });

                }


                _memoryDataProcessor.Tick(status, reader);
            }
        }

        public void SetNewMap(IMapSearchResult map)
        {
            _memoryDataProcessor.SetNewMap(map);
            _patternsDispatcher.SetOutputPatterns(map.OutputPatterns);
        }

        public void SetHighFrequencyDataHandlers(List<IHighFrequencyDataConsumer> consumers)
        {
            _patternsDispatcher.HighFrequencyDataConsumers = consumers;
        }

        public void Dispose()
        {
            _memoryDataProcessor?.Dispose();
            _settings.SettingUpdated -= SettingUpdated;
        }

        private void SettingUpdated(object sender, SettingUpdated settingUpdated)
        {
            if (settingUpdated.Name == Helpers.EnablePpSmoothing.Name)
            {
                var enableSmoothing = _settings.Get<bool>(Helpers.EnablePpSmoothing);
                _memoryDataProcessor.ToggleSmoothing(enableSmoothing);
            }
        }

        public Task CreateTokensAsync(IMapSearchResult map, CancellationToken cancellationToken) => _memoryDataProcessor.CreateTokensAsync(map, cancellationToken);
    }
}