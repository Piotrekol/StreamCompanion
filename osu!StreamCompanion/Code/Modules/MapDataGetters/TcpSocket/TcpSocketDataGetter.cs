using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using osu_StreamCompanion.Code.Core;
using osu_StreamCompanion.Code.Core.DataTypes;
using osu_StreamCompanion.Code.Interfaces;
using osu_StreamCompanion.Code.Misc;

namespace osu_StreamCompanion.Code.Modules.MapDataGetters.TcpSocket
{
    public class TcpSocketDataGetter : IModule, IMapDataGetter, ISettings, IDisposable
    {
        private readonly SettingNames _names = SettingNames.Instance;

        public bool Started { get; set; }
        private TcpSocketManager _tcpSocketManager;
        private Settings _settings;

        public void Start(ILogger logger)
        {
            _tcpSocketManager = new TcpSocketManager();
            _tcpSocketManager.ServerIp = _settings.Get<string>(_names.tcpSocketIp);
            _tcpSocketManager.ServerPort = _settings.Get<int>(_names.tcpSocketPort);
            _tcpSocketManager.Connect();
            Started = true;
        }

        public void SetNewMap(MapSearchResult map)
        {
            Dictionary<string, string> output = new Dictionary<string, string>();
            foreach (var s in map.FormatedStrings)
            {
                if (!s.IsMemoryFormat)//memory pattern is handled elsewhere
                    output.Add(s.Name, s.GetFormatedPattern());
            }
            var json = JsonConvert.SerializeObject(output);
            _tcpSocketManager.Write(json);
        }

        public void Dispose()
        {
            _tcpSocketManager?.Dispose();
        }

        public void SetSettingsHandle(Settings settings)
        {
            _settings = settings;
        }
    }
}