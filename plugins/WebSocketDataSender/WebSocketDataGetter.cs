using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Interfaces;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace WebSocketDataSender
{
    public class WebSocketDataGetter : IPlugin, IMapDataGetter, ISettingsProvider, IDisposable,
        IHighFrequencyDataHandler
    {
        private ISettingsHandler _settings;
        public string Description { get; } = "Provides beatmap and live map data using websockets";
        public string Name { get; } = nameof(WebSocketDataGetter);
        public string Author { get; } = "Piotrekol";
        public string Url { get; } = "";
        public string UpdateUrl { get; } = "";
        public bool Started { get; set; }
        public string SettingGroup { get; } = "Output patterns";

        public static ConfigEntry Enabled = new ConfigEntry("webSocketEnabled", false);
        public static ConfigEntry WebSocketPort = new ConfigEntry("webSocketPort", 80);
        public static ConfigEntry WebSocketAddress = new ConfigEntry("webSocketAddress", "127.0.0.1");

        private DataContainer _liveDataContainer = new DataContainer();
        private DataContainer _mapDataContainer = new DataContainer();
        private WebSocketServer webSocketServer;

        public WebSocketDataGetter(ISettingsHandler settings)
        {
            _settings = settings;
            if (!_settings.Get<bool>(Enabled))
            {
                return;
            }

            webSocketServer = new WebSocketServer(IPAddress.Parse(_settings.Get<string>(WebSocketAddress)), _settings.Get<int>(WebSocketPort));
            webSocketServer.ReuseAddress = true;
            webSocketServer.WaitTime = TimeSpan.FromSeconds(30);

            webSocketServer.AddWebSocketService("/StreamCompanion/LiveData/Stream", () => new StreamDataProvider(_liveDataContainer));
            webSocketServer.AddWebSocketService("/StreamCompanion/MapData/Stream", () => new StreamDataProvider(_mapDataContainer));

            webSocketServer.Start();
        }
        public void Dispose()
        {
        }

        public void Handle(string content)
        {
            _liveDataContainer.Data = content;
        }

        public void Handle(string name, string content)
        {
            // ignored
        }

        public void SetNewMap(MapSearchResult map)
        {
            Dictionary<string, string> output = new Dictionary<string, string>();
            foreach (var s in map.FormatedStrings)
            {
                if (!s.IsMemoryFormat) //memory pattern is handled elsewhere
                    output[s.Name] = s.GetFormatedPattern();
            }
            var json = JsonConvert.SerializeObject(output);
            _mapDataContainer.Data = json;
        }


        internal class DataContainer
        {
            public volatile string Data;
        }

        internal class DataProvider : WebSocketBehavior
        {
            protected readonly DataContainer DataContainer;

            public DataProvider(DataContainer dataContainer)
            {
                DataContainer = dataContainer;
            }

            protected override Task OnMessage(MessageEventArgs e)
            {
                Send(DataContainer.Data);
                return Task.CompletedTask;
            }
        }

        internal class StreamDataProvider : DataProvider
        {
            public StreamDataProvider(DataContainer dataContainer) : base(dataContainer)
            {
                Task.Run(SendLoop);
            }

            public async Task SendLoop()
            {
                string lastSentData = string.Empty;
                var counter = 0;
                while (true)
                {
                    await Task.Delay(33);
                    if (lastSentData != DataContainer.Data)
                    {
                        lastSentData = DataContainer.Data;
                        await Send(lastSentData);
                    }

                    //check for connection status every ~50s
                    if (counter++ == 1500)
                    {
                        if (State == WebSocketState.Closed)
                        {
                            return;
                        }

                        counter = 0;
                    }
                }
            }
        }

        public void Free()
        {
            _webSocketSettings?.Dispose();
        }

        private WebSocketSettings _webSocketSettings;
        public object GetUiSettings()
        {
            if (_webSocketSettings == null || _webSocketSettings.IsDisposed)
            {
                _webSocketSettings = new WebSocketSettings(_settings);
            }

            return _webSocketSettings;
        }
    }
}
