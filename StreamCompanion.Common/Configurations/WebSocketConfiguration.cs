using Newtonsoft.Json;
using StreamCompanion.Common.Models;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Interfaces.Services;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace StreamCompanion.Common.Configurations
{
    public partial class WebSocketConfiguration
    {
        public int HttpServerPort { get; set; } = 20727;
        public string HttpServerAddress { get; set; } = "http://localhost";

        public string BindAddress() => $"{HttpServerAddress}:{HttpServerPort}";
        public string BaseAddress() => BindAddress().Replace("*", "localhost");
        public bool RemoteAccessEnabled() => BindAddress().Contains("://*");

        public List<string> GetLocalOverlayRelativePaths(ISaver saver) => GetLocalOverlays(saver).Select(o => o.RelativePath).ToList();
        public List<string> GetLocalOverlayURLs(ISaver saver) => GetLocalOverlayRelativePaths(saver).Select(relativePath => $"{BaseAddress()}/overlays/{relativePath}/").ToList();
        public List<WebOverlay> GetLocalOverlays(ISaver saver)
        {
            var overlaysDir = Path.Combine(HttpContentRoot(saver), "overlays");
            var overlayIndexFileName = "index.html";

            return Directory.GetFiles(overlaysDir, overlayIndexFileName, SearchOption.AllDirectories)
                .Select(fullPath =>
                    {
                        var relativePath = fullPath.Substring(overlaysDir.Length + 1, fullPath.Length - overlaysDir.Length - overlayIndexFileName.Length - 2).Replace('\\', '/');
                        return new WebOverlay
                        {
                            FullPath = fullPath.Replace(overlayIndexFileName, ""),
                            RelativePath = relativePath,
                            URL = $"{BaseAddress()}/overlays/{relativePath}/"
                        };
                    }).ToList();
        }

        public string HttpContentRoot(ISaver saver)
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

        public static ConfigEntry ConfigEntry { get; } = new ConfigEntry("WebSocket", null);
        public static WebSocketConfiguration GetConfiguration(ISettings settings)
        {
            return settings.GetConfiguration<WebSocketConfiguration>(ConfigEntry);
        }
    }
}
