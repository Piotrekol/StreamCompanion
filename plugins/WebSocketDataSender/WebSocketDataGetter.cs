using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Threading.Tasks;
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
    public class WebSocketDataGetter : IPlugin, IMapDataConsumer, ISettingsSource, IDisposable,
        IHighFrequencyDataConsumer
    {
        private ISettings _settings;
        public string Description { get; } = "Provides beatmap and live map data using websockets";
        public string Name { get; } = nameof(WebSocketDataGetter);
        public string Author { get; } = "Piotrekol";
        public string Url { get; } = "";
        public string UpdateUrl { get; } = "";
        public bool Started { get; set; }
        public string SettingGroup { get; } = "Output patterns";

        public static ConfigEntry Enabled = new ConfigEntry("httpServerEnabled", false);
        public static ConfigEntry WebSocketPort = new ConfigEntry("httpServerPort", 28390);
        public static ConfigEntry WebSocketAddress = new ConfigEntry("httpServerAddress", "http://*");

        private DataContainer _liveDataContainer = new DataContainer();
        private DataContainer _mapDataContainer = new DataContainer();
        private HttpServer _server;
        public WebSocketDataGetter(ISettings settings, ILogger logger, ISaver saver)
        {
            _settings = settings;
            if (!_settings.Get<bool>(Enabled))
            {
                return;
            }

            var baseAddress = $"{_settings.Get<string>(WebSocketAddress)}:{_settings.Get<int>(WebSocketPort)}";
            var saveDir = Path.Combine(saver.SaveDirectory, "web");
            if (!Directory.Exists(saveDir))
                Directory.CreateDirectory(saveDir);

            var modules = new List<(string Description, IWebModule Module)>
            {
                ("WebSocket stream of output patterns containing live tokens", new WebSocketDataEndpoint("/liveData", true, _liveDataContainer)),
                ("WebSocket stream of output patterns with do not contain live tokens", new WebSocketDataEndpoint("/mapData", true, _mapDataContainer)),
                ("WebSocket stream of requested tokens, with can be changed at any point by sending message with serialized JArray, containing case sensitive token names", new WebSocketTokenEndpoint("/tokens", true, Tokens.AllTokens)),
                ("Current beatmap background image", new ActionModule("/backgroundImage",HttpVerbs.Get,SendCurrentBeatmapImage)),
            };

            _server = new HttpServer(baseAddress, saveDir, logger, modules);
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
                                    using (var resizedImg = ResizeImage(img, 
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
        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        public static Bitmap ResizeImage(Image image, int? width, int? height)
        {
            if((!width.HasValue || width == 0) && (!height.HasValue || height == 0))
                throw new ArgumentNullException("width","Both width and height cannot be null. Provide value for at least one of them.");
                
            double ratioX = (double)(width ?? double.MaxValue)/ (double)image.Width;
            double ratioY = (double)(height ?? double.MaxValue) / (double)image.Height;
            // use whichever multiplier is smaller
            double ratio = ratioX < ratioY ? ratioX : ratioY;

            // now we can get the new height and width
            int newHeight = Convert.ToInt32(image.Height * ratio);
            int newWidth = Convert.ToInt32(image.Width * ratio);

            var destRect = new Rectangle(0, 0, newWidth, newHeight);
            var destImage = new Bitmap(newWidth, newHeight);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
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
