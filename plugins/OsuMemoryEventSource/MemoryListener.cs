using System;
using System.Collections.Generic;
using OsuMemoryDataProvider;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Interfaces;

namespace OsuMemoryEventSource
{
    public class MemoryListener : IOsuEventSource, IHighFrequencyDataSender, IDisposable, ISettings
    {
        public EventHandler<MapSearchArgs> NewOsuEvent { get; set; }

        private int _lastMapId = -1;
        private int _currentMapId = -2;
        private OsuMemoryStatus _lastStatus = OsuMemoryStatus.Unknown;
        private OsuMemoryStatus _lastStatusLog = OsuMemoryStatus.Unknown;
        private OsuMemoryStatus _currentStatus = OsuMemoryStatus.Unknown;
        private string _lastMapString = "-";
        private string _currentMapString = "";
        private MemoryDataProcessor _memoryDataProcessor;
        public Tokens Tokens => _memoryDataProcessor.Tokens;
        private ISettingsHandler _settings;

        public MemoryListener(string songsFolderLocation)
        {
            _memoryDataProcessor = new MemoryDataProcessor(songsFolderLocation);
        }

        public void Tick(IOsuMemoryReader reader, bool sendEvents)
        {
            int num;
            _currentStatus = reader.GetCurrentStatus(out num);
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

                if (sendEvents && 
                    (_lastMapId != _currentMapId || 
                    _lastStatus != _currentStatus ||
                    _currentMapString != _lastMapString)
                    )
                {
                    _lastMapId = _currentMapId;
                    _lastStatus = _currentStatus;
                    _lastMapString = _currentMapString;
                    var mapHash = reader.GetMapMd5();

                    NewOsuEvent?.Invoke(this, new MapSearchArgs("OsuMemory")
                    {
                        MapId = _currentMapId,
                        Status = status,
                        Raw = _currentMapString,
                        MapHash = mapHash
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