using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using EmbedIO;
using EmbedIO.Actions;
using EmbedIO.Files;
using EmbedIO.Utilities;
using Newtonsoft.Json;
using StreamCompanion.Common;
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

        private Dictionary<string, string> OutputPatterns { get; } = new Dictionary<string, string>();
        private HttpServer _server;
        private WebOverlay.WebOverlay _webOverlay;

        public static ConfigEntry HttpServerPort = new ConfigEntry("httpServerPort", 20727);
        public static ConfigEntry HttpServerAddress = new ConfigEntry("httpServerAddress", "http://localhost");

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

        public WebSocketDataGetter(ISettings settings, ILogger logger, ISaver saver, Delegates.Restart restarter, MapStatsModule mapStatsModule)
        {
            _settings = settings;
            _saver = saver;
            _restarter = restarter;
            _logger = logger;
            _webOverlay = new WebOverlay.WebOverlay(settings, saver, restarter);

            var modules = new List<(string Description, IWebModule Module)>
            {
                ("WebSocket stream of output patterns, with can be changed at any point by sending message with serialized JArray, containing case sensitive output pattern names", new WebSocketOutputPatternsEndpoint("/outputPatterns", true, OutputPatterns)),
                ("WebSocket stream of requested tokens, with can be changed at any point by sending message with serialized JArray, containing case sensitive token names", new WebSocketTokenEndpoint("/tokens", true)),
                ("All tokens in form of json objects, prefer usage of one of the websocket endpoints above", new ActionModule("/json",HttpVerbs.Get,SendAllTokens)),
                ("Current beatmap background image, use \"width\" and/or \"height\" query parameters to resize image while keeping its aspect ratio. Set \"crop\" query parameter to true to return image with exact size provided", new ActionModule("/backgroundImage",HttpVerbs.Get,SendCurrentBeatmapImage)),
                ("View into user osu! Songs folder", CreateSongsModule()),
                ("View into user osu! Skins folder", CreateSkinsModule()),
                ("List of available overlays (folder names)", new ActionModule("/overlayList",HttpVerbs.Get,ListOverlays)),
                ("All StreamCompanion settings", new ActionModule("/settings",HttpVerbs.Get,GetSettings)),
            };
            modules.AddRange(mapStatsModule.GetModules());

            _server = new HttpServer(BindAddress(_settings), HttpContentRoot(saver), logger, modules);

            Task.Run(RunServer).HandleExceptions();
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
            var overlaysDir = Path.Combine(HttpContentRoot(_saver), "overlays");
            var overlayIndexFileName = "index.html";
            var fullPaths = Directory.GetFiles(overlaysDir, overlayIndexFileName, SearchOption.AllDirectories);
            var relativePaths = fullPaths.Select(p => p.Substring(overlaysDir.Length + 1, p.Length - overlaysDir.Length - overlayIndexFileName.Length - 2));
            return context.SendStringAsync(
                JsonConvert.SerializeObject(relativePaths),
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
            await using var responseStream = context.OpenResponseStream();
            if (Tokens.AllTokens.TryGetValue("backgroundImageLocation", out var imageToken) &&
                !string.IsNullOrEmpty((string)imageToken.Value))
            {
                var location = (string)imageToken.Value;

                context.Response.ContentType = "image/jpeg";

                if (!File.Exists(location))
                    return;

                await using var fs = new FileStream(location, FileMode.Open, FileAccess.Read);
                if (!(context.Request.QueryString.ContainsKey("width") ||
                    context.Request.QueryString.ContainsKey("height")))
                {
                    await fs.CopyToAsync(responseStream);
                    return;
                }

                if (!OperatingSystem.IsWindows())
                {
                    context.Response.StatusCode = 409;
                    await context.SendStringAsync("Image processing is supported only on windows", "text", Encoding.UTF8);
                    return;
                }

                int.TryParse(context.Request.QueryString["width"], out var desiredWidth);
                int.TryParse(context.Request.QueryString["height"], out var desiredHeight);

                var crop = context.Request.QueryString.ContainsKey("crop") &&
                           new[] { "true", "1" }.Contains(context.Request.QueryString["crop"].ToLowerInvariant());

                if (crop && (desiredWidth == 0 || desiredHeight == 0))
                {
                    context.Response.StatusCode = 422;
                    await context.SendStringAsync("Missing required parameters: \"width\" & \"height\" ",
                        "text", Encoding.UTF8);
                    return;
                }

                using var img = Image.FromStream(fs);
                if (crop)
                {
                    using var croppedImg = img.ResizeAndCropBitmap(desiredWidth, desiredHeight);
                    croppedImg.Save(responseStream, ImageFormat.Jpeg);
                }
                else
                {
                    using var resizedImg = img.ResizeImage(
                        desiredWidth == 0 ? (int?)null : desiredWidth,
                        desiredHeight == 0 ? (int?)null : desiredHeight);
                    resizedImg.Save(responseStream, ImageFormat.Jpeg);
                }
            }
        }

        private IWebModule CreateSongsModule()
        {
            var songsLocation = _settings.GetFullSongsLocation();
            var errorMessage = $"Couldn't find songs folder at \"{songsLocation}\", /songs/ web endpoint has been disabled";
            return CreateFolderModule(songsLocation, "/Songs/", errorMessage);
        }

        private IWebModule CreateSkinsModule()
        {
            var skinsLocation = _settings.GetFullSkinsLocation();
            var errorMessage = $"Couldn't find skins folder at \"{skinsLocation}\", /skins/ web endpoint has been disabled";
            return CreateFolderModule(skinsLocation, "/Skins/", errorMessage);
        }

        private IWebModule CreateFolderModule(string location, string baseRoute, string missingFolderErrorMessage)
        {
            IWebModule module = null;
            if (!Directory.Exists(location))
            {
                _logger.Log(missingFolderErrorMessage, LogLevel.Warning);
                module = new ActionModule(baseRoute, HttpVerbs.Any, c =>
                {
                    c.Response.StatusCode = 500;
                    return c.SendStringAsync(missingFolderErrorMessage, "text", Encoding.UTF8);
                });
            }

            return module ?? new FileModule(baseRoute, new FileSystemProvider(location, false)).WithDirectoryLister(DirectoryLister.Html);
        }

        public void Dispose()
        {
            _server?.Dispose();
        }

        public void Handle(string content)
        {
            //TODO: modify IHighFrequencyDataConsumer to pass dictionary instead
            var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);
            foreach (var kv in dict)
            {
                OutputPatterns[kv.Key] = kv.Value;
            }
        }

        public void Handle(string name, string content)
        {
            // ignored
        }

        public Task SetNewMapAsync(IMapSearchResult map, CancellationToken cancellationToken)
        {
            foreach (var s in map.OutputPatterns)
            {
                if (!s.IsMemoryFormat)
                    OutputPatterns[s.Name] = s.GetFormatedPattern();
            }

            return Task.CompletedTask;
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
