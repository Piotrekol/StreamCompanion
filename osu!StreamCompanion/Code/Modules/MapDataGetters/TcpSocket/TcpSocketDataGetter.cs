using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Newtonsoft.Json;
using osu_StreamCompanion.Code.Core;
using osu_StreamCompanion.Code.Helpers;
using osu_StreamCompanion.Code.Misc;
using StreamCompanionTypes;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Interfaces;

namespace osu_StreamCompanion.Code.Modules.MapDataGetters.TcpSocket
{
    public class TcpSocketDataGetter : IModule, IMapDataGetter, ISettingsProvider, IDisposable
    {
        private readonly SettingNames _names = SettingNames.Instance;

        public bool Started { get; set; }
        private TcpSocketManager _tcpSocketManager;
        private ISettingsHandler _settings;

        public void Start(ILogger logger)
        {
            _tcpSocketManager = new TcpSocketManager();
            _tcpSocketManager.ServerIp = _settings.Get<string>(_names.tcpSocketIp);
            _tcpSocketManager.ServerPort = _settings.Get<int>(_names.tcpSocketPort);
            if (_settings.Get<bool>(_names.tcpSocketEnabled))
            {
                _tcpSocketManager.AutoReconnect = true;
                _tcpSocketManager.Connect();
            }
            _settings.SettingUpdated += SettingUpdated;
            Started = true;
        }

        private void SettingUpdated(object sender, SettingUpdated settingUpdated)
        {
            if (settingUpdated.Name == _names.tcpSocketEnabled.Name)
            {
                var enabled = _settings.Get<bool>(_names.tcpSocketEnabled);
                _tcpSocketManager.AutoReconnect = enabled;
                if (enabled)
                    _tcpSocketManager.Connect();
            }
        }

        public void SetNewMap(MapSearchResult map)
        {
            if (_settings.Get<bool>(_names.tcpSocketEnabled))
            {
                Dictionary<string, string> output = new Dictionary<string, string>();
                foreach (var s in map.FormatedStrings)
                {
                    if (!s.IsMemoryFormat) //memory pattern is handled elsewhere
                        output[s.Name] = s.GetFormatedPattern();
                }
                var json = JsonConvert.SerializeObject(output);
                _tcpSocketManager.Write(json);
            }
        }



        public void SetSettingsHandle(ISettingsHandler settings)
        {
            _settings = settings;
        }

        public string SettingGroup { get; } = "Output patterns";
        private TcpSocketSettings settingsUserControl = null;
        public void Free()
        {
            settingsUserControl?.Dispose();
        }

        public UserControl GetUiSettings()
        {
            if (settingsUserControl == null || settingsUserControl.IsDisposed)
            {
                settingsUserControl = new TcpSocketSettings(_settings);
            }
            return settingsUserControl;
        }

        public void Dispose()
        {
            _settings.SettingUpdated -= SettingUpdated;
            _tcpSocketManager?.Dispose();
            settingsUserControl?.Dispose();
        }
    }
}