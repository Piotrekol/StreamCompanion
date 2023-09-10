using Newtonsoft.Json;
using System.IO;

namespace StreamCompanion.Common.Models
{
    public class WebOverlay
    {
        public string RelativePath { get; set; }
        public string FullPath { get; set; }
        public string URL { get; set; }
        private WebOverlayRecommendedSettings _webOverlayRecommendedSettings;
        private bool _webOverlayRecommendedSettingsChecked = false;
        public WebOverlayRecommendedSettings WebOverlayRecommendedSettings
        {
            get
            {
                if (_webOverlayRecommendedSettingsChecked)
                    return _webOverlayRecommendedSettings;

                _webOverlayRecommendedSettingsChecked = true;
                var settingsPath = Path.Combine(FullPath, "settings.json");
                if (File.Exists(settingsPath))
                    _webOverlayRecommendedSettings = JsonConvert.DeserializeObject<WebOverlayRecommendedSettings>(File.ReadAllText(settingsPath));

                return _webOverlayRecommendedSettings;
            }
        }

        public override string ToString()
        {
            return RelativePath;
        }
    }
}
