using Newtonsoft.Json;
using System.IO;

namespace StreamCompanion.Common.Models
{
    public class WebOverlay
    {
        public string RelativePath { get; set; }
        public string FullPath { get; set; }
        public string URL { get; set; }
        private WebOverlayRecommendedSettings _recommendedSettings;
        private bool _recommendedSettingsChecked = false;
        public WebOverlayRecommendedSettings RecommendedSettings
        {
            get
            {
                if (_recommendedSettingsChecked)
                    return _recommendedSettings;

                _recommendedSettingsChecked = true;
                var settingsPath = Path.Combine(FullPath, "settings.json");
                if (File.Exists(settingsPath))
                    _recommendedSettings = JsonConvert.DeserializeObject<WebOverlayRecommendedSettings>(File.ReadAllText(settingsPath));

                return _recommendedSettings;
            }
        }

        public override string ToString()
        {
            return RelativePath;
        }
    }
}
