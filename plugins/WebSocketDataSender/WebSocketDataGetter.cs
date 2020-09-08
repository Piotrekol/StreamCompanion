using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmbedIO;
using EmbedIO.Actions;
using EmbedIO.Utilities;
using Newtonsoft.Json;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Interfaces;
using StreamCompanionTypes.Interfaces.Consumers;
using StreamCompanionTypes.Interfaces.Services;


namespace WebSocketDataSender
{
    public class WebSocketDataGetter : IPlugin, IMapDataConsumer, IDisposable,
        IHighFrequencyDataConsumer
    {
        private ISettings _settings;
        private readonly ISaver _saver;
        public string Description { get; } = "Provides beatmap and live map data using websockets";
        public string Name { get; } = nameof(WebSocketDataGetter);
        public string Author { get; } = "Piotrekol";
        public string Url { get; } = "";
        public string UpdateUrl { get; } = "";
        public bool Started { get; set; }

        private DataContainer _liveDataContainer = new DataContainer();
        private DataContainer _mapDataContainer = new DataContainer();
        private HttpServer _server;

        public static ConfigEntry HttpServerPort = new ConfigEntry("httpServerPort", 28390);
        public static ConfigEntry HttpServerAddress = new ConfigEntry("httpServerAddress", "http://localhost");

        public static string BaseAddress(ISettings settings) => BindAddress(settings).Replace("*", "localhost");
        public static string BindAddress(ISettings settings) => $"{settings.Get<string>(HttpServerAddress)}:{settings.Get<int>(HttpServerPort)}";

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

        public WebSocketDataGetter(ISettings settings, ILogger logger, ISaver saver)
        {
            _settings = settings;
            _saver = saver;


            var modules = new List<(string Description, IWebModule Module)>
            {
                ("WebSocket stream of output patterns containing live tokens", new WebSocketDataEndpoint("/liveData", true, _liveDataContainer)),
                ("WebSocket stream of output patterns with do not contain live tokens", new WebSocketDataEndpoint("/mapData", true, _mapDataContainer)),
                ("WebSocket stream of requested tokens, with can be changed at any point by sending message with serialized JArray, containing case sensitive token names", new WebSocketTokenEndpoint("/tokens", true, Tokens.AllTokens)),
                ("All tokens in form of json objects, prefer usage of one of the websocket endpoints above", new ActionModule("/json",HttpVerbs.Get,SendAllTokens)),
                ("Current beatmap background image, use \"width\" and/or \"height\" query parameters to resize image while keeping its aspect ratio", new ActionModule("/backgroundImage",HttpVerbs.Get,SendCurrentBeatmapImage)),
                ("List of available overlays (folder names)", new ActionModule("/overlayList",HttpVerbs.Get,ListOverlays)),
                ("All StreamCompanion settings", new ActionModule("/settings",HttpVerbs.Get,GetSettings)),
            };

            var configurationLocation = Path.Combine(HttpContentRoot(saver), "lib", "consts.js");
            if (File.Exists(configurationLocation))
                SetWebOverlayJavaScriptConfiguration(configurationLocation, new Uri(BaseAddress(_settings)));

            _server = new HttpServer(BindAddress(_settings), HttpContentRoot(saver), logger, modules);
        }

        private Task GetSettings(IHttpContext context)
        {
            //TODO: remove dynamic call after upgrading StreamCompanionTypes
            var allSettings = ((dynamic)_settings).SettingsEntries as IReadOnlyDictionary<string, object>;
            return context.SendDataAsync(allSettings);
        }

        private void SetWebOverlayJavaScriptConfiguration(string configurationFileLocation, Uri uri)
        {
            var fileContents = File.ReadAllLines(configurationFileLocation);
            for (int i = 0; i < fileContents.Length; i++)
            {
                if (fileContents[i].StartsWith("let autoConfig"))
                {
                    fileContents[i] = $"let autoConfig = {JsonConvert.SerializeObject(new { uri.Scheme, uri.Host, uri.Port })};";
                    break;
                }
            }
            File.WriteAllLines(configurationFileLocation, fileContents);
        }
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

                                using (var img = Image.FromStream(fs))
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
    }
}
