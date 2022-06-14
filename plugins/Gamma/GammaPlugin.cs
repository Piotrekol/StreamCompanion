using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Gamma.Models;
using StreamCompanion.Common;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Enums;
using StreamCompanionTypes.Interfaces;
using StreamCompanionTypes.Interfaces.Consumers;
using StreamCompanionTypes.Interfaces.Services;
using StreamCompanionTypes.Interfaces.Sources;

namespace Gamma
{
    public class GammaPlugin : IPlugin, IMapDataConsumer, ISettingsSource, IDisposable
    {
        public static ConfigEntry GammaConfiguration = new ConfigEntry("GammaConfiguration", "{}");

        public string Description { get; } = "Adjusts gamma depending on AR";
        public string Name { get; } = nameof(GammaPlugin);
        public string Author { get; } = "Piotrekol";
        public string Url { get; } = string.Empty;
        public string UpdateUrl { get; } = string.Empty;
        public string SettingGroup { get; } = "Gamma";

        private readonly ILogger _logger;
        private readonly ISettings _settings;
        private Gamma _gamma;
        private Configuration _configuration;
        private string _originalScreenDeviceName;
        private GammaSettings _gammaSettings;

        public GammaPlugin(ILogger logger, ISettings settings)
        {
            if (!OperatingSystem.IsWindows())
            {
                logger.Log("Gamma plugin works only under windows - disabling", LogLevel.Warning);
                return;
            }

            _logger = logger;
            _settings = settings;
            _configuration = settings.GetConfiguration<Configuration>(GammaConfiguration);
            _configuration.GammaRanges.Sort((r1, r2) => r1.MinAr.CompareTo(r2.MinAr));
            settings.SaveConfiguration(GammaConfiguration, _configuration);
            _originalScreenDeviceName = _configuration.ScreenDeviceName ?? Screen.PrimaryScreen.DeviceName;
            _gamma = new Gamma(_originalScreenDeviceName);
        }

        public Task SetNewMapAsync(IMapSearchResult searchResult, CancellationToken cancellationToken)
        {
            if (!_configuration.Enabled || !searchResult.BeatmapsFound.Any() || searchResult.Action != OsuStatus.Playing)
            {
                if (!float.IsNaN(_gamma.CurrentGamma))
                    _gamma.Restore();

                return Task.CompletedTask;
            }

            if (_originalScreenDeviceName != _configuration.ScreenDeviceName)
            {
                _gamma.Dispose();
                _gamma = new Gamma(_originalScreenDeviceName = _configuration.ScreenDeviceName);
            }

            var ar = (double)Tokens.AllTokens["mAR"].Value;
            var gamma = GetGammaForAr(ar);

            if (gamma.HasValue)
                _gamma.Set(gamma.Value);
            else
                _gamma.Restore();

            return Task.CompletedTask;
        }

        private float? GetGammaForAr(double ar) => _configuration.GammaRanges.FirstOrDefault(x => x.MaxAr >= ar && x.MinAr <= ar)?.Gamma;

        public void Free()
        {
            _gammaSettings?.Dispose();
        }

        public object GetUiSettings()
        {
            if (_gammaSettings == null || _gammaSettings.IsDisposed)
            {
                _gammaSettings = new GammaSettings(_configuration);
                _gammaSettings.OnSettingUpdated += OnSettingUpdated;
            }
            return _gammaSettings;
        }

        private void OnSettingUpdated(object sender, EventArgs e)
        {
            _settings.SaveConfiguration(GammaConfiguration, _configuration);
        }

        public void Dispose()
        {
            _gamma?.Dispose();
        }
    }
}