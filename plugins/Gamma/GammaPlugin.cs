using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Threading;
using System.Windows.Forms;
using StreamCompanion.Common;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Enums;
using StreamCompanionTypes.Interfaces;
using StreamCompanionTypes.Interfaces.Consumers;
using StreamCompanionTypes.Interfaces.Services;
using StreamCompanionTypes.Interfaces.Sources;

namespace Gamma
{
    [SupportedOSPlatform("windows")]
    public class GammaPlugin : IPlugin, IMapDataConsumer, ISettingsSource, IDisposable
    {
        public static ConfigEntry GammaConfiguration = new ConfigEntry("GammaConfiguration", "{}");

        private readonly ILogger _logger;
        private readonly ISettings _settings;
        private Gamma _gamma;
        public string Description { get; } = "Adjusts gamma depending on AR";
        public string Name { get; } = nameof(GammaPlugin);
        public string Author { get; } = "Piotrekol";
        public string Url { get; } = string.Empty;
        public string UpdateUrl { get; } = string.Empty;
        private Configuration _configuration;
        public GammaPlugin(ILogger logger, ISettings settings)
        {
            _logger = logger;
            _settings = settings;
            _configuration = settings.GetConfiguration<Configuration>(GammaConfiguration);
            _configuration.GammaRanges.Sort((r1, r2) => r1.MinAr.CompareTo(r2.MinAr));
            settings.SaveConfiguration(GammaConfiguration, _configuration);
            //TODO: screen selection
            _gamma = new Gamma(Screen.PrimaryScreen);
        }

        public void SetNewMap(IMapSearchResult searchResult, CancellationToken cancellationToken)
        {
            if (!searchResult.BeatmapsFound.Any() || searchResult.Action != OsuStatus.Playing)
            {
                _gamma.Restore();
                return;
            }

            var ar = (double)Tokens.AllTokens["mAR"].Value;
            var gamma = GetGammaForAr(ar);

            if (gamma.HasValue)
                _gamma.Set(gamma.Value);
            else
                _gamma.Restore();
        }

        private float? GetGammaForAr(double ar) => _configuration.GammaRanges.FirstOrDefault(x => x.MaxAr >= ar && x.MinAr <= ar)?.Gamma;

        public class GammaRange
        {
            public double MinAr { get; set; }
            public double MaxAr { get; set; }
            public float Gamma { get; set; }
        }

        public class Configuration
        {
            public List<GammaRange> GammaRanges { get; set; } = new List<GammaRange>
            {
                new GammaRange{MinAr = 10, MaxAr = 12, Gamma = 0.7f}
            };
            public bool Enabled { get; set; } = false;

        }

        public void Free()
        {
            throw new System.NotImplementedException();
        }

        public object GetUiSettings()
        {
            throw new System.NotImplementedException();
        }

        public string SettingGroup { get; } = "Gamma";

        public void Dispose()
        {
            _gamma?.Dispose();
        }
    }
}