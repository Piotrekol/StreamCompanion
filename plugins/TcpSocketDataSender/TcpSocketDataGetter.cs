using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Newtonsoft.Json;
using StreamCompanionTypes;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Interfaces;

namespace TcpSocketDataSender
{
    public class TcpSocketDataGetter : IPlugin, IMapDataGetter, ISettingsProvider, IDisposable, IHighFrequencyDataHandler
    {
        private readonly SettingNames _names = SettingNames.Instance;

        public bool Started { get; set; }
        private BlockedTcpSocketManager _liveTcpSocketManager;
        private BlockedTcpSocketManager _tcpSocketManager;

        private ISettingsHandler _settings;

        private bool tcpSocketIsEnabled = false;
        
        public string Description { get; } = "";
        public string Name { get; } = nameof(TcpSocketDataGetter);
        public string Author { get; } = "Piotrekol";
        public string Url { get; } = "";
        public string UpdateUrl { get; } = "";

        public void Start(ILogger logger)
        {
            _liveTcpSocketManager = new BlockedTcpSocketManager();
            _liveTcpSocketManager.ServerIp = _settings.Get<string>(_names.tcpSocketIp);
            _liveTcpSocketManager.ServerPort = _settings.Get<int>(_names.tcpSocketLiveMapDataPort);
            
            _tcpSocketManager = new BlockedTcpSocketManager();
            _tcpSocketManager.ServerIp = _settings.Get<string>(_names.tcpSocketIp);
            _tcpSocketManager.ServerPort = _settings.Get<int>(_names.tcpSocketPort);
            

            tcpSocketIsEnabled = _settings.Get<bool>(_names.tcpSocketEnabled);
            if (tcpSocketIsEnabled)
            {
                _liveTcpSocketManager.AutoReconnect = true;
                _liveTcpSocketManager.Connect();

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
                tcpSocketIsEnabled = _settings.Get<bool>(_names.tcpSocketEnabled);
                _liveTcpSocketManager.AutoReconnect = tcpSocketIsEnabled;
                _tcpSocketManager.AutoReconnect = tcpSocketIsEnabled;
                if (tcpSocketIsEnabled)
                {
                    _liveTcpSocketManager.Connect();
                    _tcpSocketManager.Connect();
                }
            }
        }

        public void SetNewMap(MapSearchResult map)
        {
            if (tcpSocketIsEnabled)
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
            _liveTcpSocketManager?.Dispose();
            _tcpSocketManager?.Dispose();
            settingsUserControl?.Dispose();
        }

        public void Handle(string content)
        {
            if (!tcpSocketIsEnabled) return;

            _liveTcpSocketManager.Write(content);

        }

        public void Handle(string name, string content)
        {
            //Ignored
        }
    }
}