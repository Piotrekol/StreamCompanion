using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EmbedIO;
using EmbedIO.Actions;
using EmbedIO.Utilities;
using Newtonsoft.Json;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Enums;
using StreamCompanionTypes.Interfaces;
using StreamCompanionTypes.Interfaces.Consumers;
using StreamCompanionTypes.Interfaces.Services;
using StreamCompanionTypes.Interfaces.Sources;


namespace WebSocketDataSender
{
    public class WebSocketDataGetter : IPlugin, IMapDataConsumer, IDisposable,
        IHighFrequencyDataConsumer, ISettingsSource
    {
        private ISettings _settings;
        private readonly ISaver _saver;
        private readonly Delegates.Restart _restarter;
        private ILogger _logger;

        public string Description { get; } = "Provides beatmap and live map data using websockets";
        public string Name { get; } = nameof(WebSocketDataGetter);
        public string Author { get; } = "Piotrekol";
        public string Url { get; } = "";
        public string UpdateUrl { get; } = "";
        public string SettingGroup { get; } = "Web overlay";


        private DataContainer _liveDataContainer = new DataContainer();
        private DataContainer _mapDataContainer = new DataContainer();
        private HttpServer _server;

        public static ConfigEntry HttpServerPort = new ConfigEntry("httpServerPort", 20727);
        public static ConfigEntry HttpServerAddress = new ConfigEntry("httpServerAddress", "http://localhost");
        private WebOverlay.WebOverlay _webOverlay;

        public static string BaseAddress(ISettings settings) => BindAddress(settings).Replace("*", "localhost");
        public static string BindAddress(ISettings settings) => $"{settings.Get<string>(HttpServerAddress)}:{settings.Get<int>(HttpServerPort)}";
        public static bool RemoteAccessEnabled(ISettings settings) => BindAddress(settings).Contains("://*");

        public static string HttpContentRoot(ISaver saver)
        {
            var httpContentRoot = Path.Combine(saver.SaveDirectory, "web");
            if (!Directory.Exists(httpContentRoot))
                Directory.CreateDirectory(httpContentRoot);
#if DEBUG
            //A little hack to grab overlay files directly from git repository
            var newRootPath = Path.Combine(httpContentRoot, "..", "..", "..", "..", "webOverlay");
            if (Directory.Exists(newRootPath))
                httpContentRoot = newRootPath;
#endif
            return httpContentRoot;
        }

        public WebSocketDataGetter(ISettings settings, ILogger logger, ISaver saver, Delegates.Restart restarter)
        {
            _settings = settings;
            _saver = saver;
            _restarter = restarter;
            _logger = logger;
            _webOverlay = new WebOverlay.WebOverlay(settings, saver, restarter);

            var modules = new List<(string Description, IWebModule Module)>
            {
                ("WebSocket stream of output patterns containing live tokens", new WebSocketDataEndpoint("/liveData", true, _liveDataContainer)),
                ("WebSocket stream of output patterns with do not contain live tokens", new WebSocketDataEndpoint("/mapData", true, _mapDataContainer)),
                ("WebSocket stream of requested tokens, with can be changed at any point by sending message with serialized JArray, containing case sensitive token names", new WebSocketTokenEndpoint("/tokens", true, Tokens.AllTokens)),
                ("All tokens in form of json objects, prefer usage of one of the websocket endpoints above", new ActionModule("/json",HttpVerbs.Get,SendAllTokens)),
                ("Current beatmap background image, use \"width\" and/or \"height\" query parameters to resize image while keeping its aspect ratio. Set \"crop\" query parameter to true to return image with exact size provided", new ActionModule("/backgroundImage",HttpVerbs.Get,SendCurrentBeatmapImage)),
                ("List of available overlays (folder names)", new ActionModule("/overlayList",HttpVerbs.Get,ListOverlays)),
                ("All StreamCompanion settings", new ActionModule("/settings",HttpVerbs.Get,GetSettings)),
            };

            _server = new HttpServer(BindAddress(_settings), HttpContentRoot(saver), logger, modules);

            Task.Run(RunServer);
        }

        private async Task RunServer()
        {
            try
            {
                await _server.RunAsync();
            }
            catch (SocketException ex)
            {
                if (ex.SocketErrorCode == SocketError.AddressAlreadyInUse || ex.SocketErrorCode == SocketError.AccessDenied)
                {
                    var currentPort = _settings.Get<int>(HttpServerPort);
                    var newPort = currentPort + 1;
                    if (newPort > ushort.MaxValue)
                        newPort = 20000;


                    var userResult = MessageBox.Show(
                        $"Web overlay couldn't start because there is something already running on port {currentPort} on your system.{Environment.NewLine}" +
                        $"Do you want Stream Companion to automatically change used port to {newPort}?{Environment.NewLine}" +
                        $"You might need to update your overlay URLs in obs/slobs afterwards.", "Stream Companion Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                    if (userResult == DialogResult.Yes)
                    {
                        _settings.Add(HttpServerPort.Name, newPort);

                        userResult = MessageBox.Show(
                            $"Stream Companion restart is necessary to apply web port changes.{Environment.NewLine}" +
                            $"Restart now?", "Stream Companion Error", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (userResult == DialogResult.Yes)
                            _restarter("Updated web overlay port number");
                    }

                    return;
                }

                throw;
            }
        }

        private Task GetSettings(IHttpContext context) => context.SendDataAsync(_settings.SettingsEntries);

        private Task ListOverlays(IHttpContext context)
        {
            return context.SendStringAsync(
                JsonConvert.SerializeObject(Directory.EnumerateDirectories(Path.Combine(HttpContentRoot(_saver), "overlays")).Select(x => Path.GetFileName(x))),
                "application/json", Encoding.UTF8);
        }

        private Task SendAllTokens(IHttpContext context)
        {
            if (context.Request.QueryString.ContainsKey("debug") && context.Request.QueryString["debug"] == "1")
            {
                return context.SendStringAsync(JsonConvert.SerializeObject(Tokens.AllTokens), "application/json", Encoding.UTF8);
            }

            return context.SendStringAsync(JsonConvert.SerializeObject(Tokens.AllTokens.ToDictionary(k => k.Key, v => v.Value.Value)), "application/json", Encoding.UTF8);
        }

        private async Task SendCurrentBeatmapImage(IHttpContext context)
        {
            using (var responseStream = context.OpenResponseStream())
            {
                if (Tokens.AllTokens.TryGetValue("backgroundImageLocation", out var imageToken) &&
                    !string.IsNullOrEmpty((string)imageToken.Value))
                {
                    var location = (string)imageToken.Value;

                    context.Response.ContentType = "image/jpeg";

                    if (File.Exists(location))
                    {
                        using (var fs = new FileStream(location, FileMode.Open, FileAccess.Read))
                        {

                            if (context.Request.QueryString.ContainsKey("width") || context.Request.QueryString.ContainsKey("height"))
                            {
                                int.TryParse(context.Request.QueryString["width"], out var desiredWidth);
                                int.TryParse(context.Request.QueryString["height"], out var desiredHeight);

                                var crop = context.Request.QueryString.ContainsKey("crop") && new[] { "true", "1" }.Contains(context.Request.QueryString["crop"].ToLowerInvariant());

                                using (var img = Image.FromStream(fs))
                                {
                                    if (crop)
                                    {
                                        using (var croppedImg = img.ResizeAndCropBitmap(desiredWidth, desiredHeight))
                                        {
                                            croppedImg.Save(responseStream, ImageFormat.Jpeg);
                                        }
                                    }
                                    else
                                    {
                                        using (var resizedImg = img.ResizeImage(
                                            desiredWidth == 0 ? (int?)null : desiredWidth,
                                            desiredHeight == 0 ? (int?)null : desiredHeight)
                                        )
                                        {
                                            resizedImg.Save(responseStream, ImageFormat.Jpeg);
                                        }
                                    }
                                }

                            }
                            else
                            {
                                await fs.CopyToAsync(responseStream);
                            }
                        }
                    }
                }
            }
        }

        public void Dispose()
        {
            _server?.Dispose();
        }

        public void Handle(string content)
        {
            _liveDataContainer.Data = content;
        }

        public void Handle(string name, string content)
        {
            // ignored
        }

        public void SetNewMap(IMapSearchResult map)
        {
            Dictionary<string, string> output = new Dictionary<string, string>();
            foreach (var s in map.OutputPatterns)
            {
                if (!s.IsMemoryFormat)
                    output[s.Name] = s.GetFormatedPattern();
            }
            var json = JsonConvert.SerializeObject(output);
            _mapDataContainer.Data = json;
        }

        public void Free()
        {
            _webOverlay.Free();
        }

        public object GetUiSettings()
        {
            return _webOverlay.GetUiSettings();
        }

    }
}
