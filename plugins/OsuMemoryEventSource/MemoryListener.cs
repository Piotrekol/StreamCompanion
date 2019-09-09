using CollectionManager.Enums;
using OsuMemoryDataProvider;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Interfaces;
using System;
using System.Collections.Generic;

namespace OsuMemoryEventSource
{
    public class MemoryListener : IOsuEventSource, IDisposable
    {
        public EventHandler<MapSearchArgs> NewOsuEvent { get; set; }

        private int _lastMapId = -1;
        private int _currentMapId = -2;
        private int _lastGameMode = -1;
        private OsuMemoryStatus _lastStatus = OsuMemoryStatus.Unknown;
        private OsuMemoryStatus _lastStatusLog = OsuMemoryStatus.Unknown;
        private OsuMemoryStatus _currentStatus = OsuMemoryStatus.Unknown;
        private string _lastMapString = "-";
        private string _currentMapString = "";
        private string _lastMapHash = "";
        private MemoryDataProcessor _memoryDataProcessor;
        public Tokens Tokens => _memoryDataProcessor.Tokens;
        private ISettingsHandler _settings;

        public MemoryListener()
        {
            _memoryDataProcessor = new MemoryDataProcessor();
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
                _currentMapString = reader.GetSongString();
                OsuStatus status = _currentStatus.Convert();
                var gameMode = reader.ReadSongSelectGameMode();
                var mapHash = reader.GetMapMd5Safe();
                var mapHashDiffers = mapHash != null && _lastMapHash != null && _lastMapHash != mapHash;

                if (sendEvents &&
                    (_lastMapId != _currentMapId ||
                    _lastStatus != _currentStatus ||
                    _currentMapString != _lastMapString ||
                     gameMode != _lastGameMode ||
                     mapHashDiffers)
                    )
                {
                    _lastMapId = _currentMapId;
                    _lastStatus = _currentStatus;
                    _lastMapString = _currentMapString;
                    _lastGameMode = gameMode;
                    _lastMapHash = mapHash;


                    NewOsuEvent?.Invoke(this, new MapSearchArgs("OsuMemory")
                    {
                        MapId = _currentMapId,
                        Status = status,
                        Raw = _currentMapString,
                        MapHash = mapHash,
                        PlayMode = (PlayMode)gameMode
                    });

                }

                _memoryDataProcessor.Tick(status, reader);
            }
        }
        public void SetNewMap(MapSearchResult map)
        {
            _memoryDataProcessor.SetNewMap(map);
        }
        public void SetHighFrequencyDataHandlers(List<IHighFrequencyDataHandler> handlers)
        {
            _memoryDataProcessor.SetHighFrequencyDataHandlers(handlers);
        }

        public void Dispose()
        {
            _memoryDataProcessor?.Dispose();
            _settings.SettingUpdated -= SettingUpdated;
        }

        public void SetSettingsHandle(ISettingsHandler settings)
        {
            _settings = settings;
            _settings.SettingUpdated += SettingUpdated;
            _memoryDataProcessor.ToggleSmoothing(_settings.Get<bool>(Helpers.EnablePpSmoothing));
            _memoryDataProcessor.SetSettingsHandle(_settings);
        }

        private void SettingUpdated(object sender, SettingUpdated settingUpdated)
        {
            if (settingUpdated.Name == Helpers.EnablePpSmoothing.Name)
            {
                var enableSmoothing = _settings.Get<bool>(Helpers.EnablePpSmoothing);
                _memoryDataProcessor.ToggleSmoothing(enableSmoothing);
            }
        }
    }
}