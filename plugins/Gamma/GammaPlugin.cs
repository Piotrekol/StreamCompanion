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
        private double? CurrentAR
        {
            get
            {
                if (!Tokens.AllTokens.TryGetValue("mAR", out var mARToken))
                    return null;

                return (double)mARToken.Value;
            }
        }

        public GammaPlugin(ILogger logger, ISettings settings)
        {
            if (!OperatingSystem.IsWindows())
            {
                logger.Log("Gamma plugin works only under windows - disabling", LogLevel.Warning);
                return;
            }

            _logger = logger;
            _settings = settings;
            LoadGammaValues();
            _originalScreenDeviceName = string.IsNullOrWhiteSpace(_configuration.ScreenDeviceName)
                ? Screen.PrimaryScreen.DeviceName
                : _configuration.ScreenDeviceName;
            _gamma = new Gamma(_originalScreenDeviceName);
            if (!_gamma.ScreenIsValid())
            {
                _gamma.Dispose();
                _gamma = new Gamma(Screen.PrimaryScreen.DeviceName);
            }
        }

        private void LoadGammaValues()
        {
            _configuration = _settings.GetConfiguration<Configuration>(GammaConfiguration);
            foreach (var gamma in _configuration.GammaRanges)
            {
                if (gamma.Gamma != null)
                {
                    gamma.UserGamma = Gamma.GammaToUserValue(gamma.Gamma.Value);
                    gamma.Gamma = null;
                }

                gamma.UserGamma = Math.Clamp(gamma.UserGamma, 0, 100);
            }

            _configuration.SortGammaRanges();
            _settings.SaveConfiguration(GammaConfiguration, _configuration);
        }

        public Task SetNewMapAsync(IMapSearchResult searchResult, CancellationToken cancellationToken)
        {
            var ar = CurrentAR;
            if (ar == null)
                return Task.CompletedTask;

            _gammaSettings?.SetMapAR(ar.Value);
            if (!_configuration.Enabled || !searchResult.BeatmapsFound.Any() || searchResult.Action != OsuStatus.Playing)
            {
                if (!double.IsNaN(_gamma.CurrentGamma))
                    _gamma.Restore();

                return Task.CompletedTask;
            }

            if (_originalScreenDeviceName != _configuration.ScreenDeviceName)
            {
                _gamma.Dispose();
                _gamma = new Gamma(_originalScreenDeviceName = _configuration.ScreenDeviceName);
            }

            var gamma = GetGammaForAr(ar.Value);
            if (gamma.HasValue)
                _gamma.Set(gamma.Value);
            else
                _gamma.Restore();

            return Task.CompletedTask;
        }

        private int? GetGammaForAr(double ar) => _configuration.GammaRanges.FirstOrDefault(x => x.MaxAr >= ar && x.MinAr <= ar)?.UserGamma;

        public void Free()
        {
            _gammaSettings?.Dispose();
        }

        public object GetUiSettings()
        {
            if (_gammaSettings == null || _gammaSettings.IsDisposed)
            {
                _gammaSettings = new GammaSettings(_configuration);
                _gammaSettings.SettingUpdated += OnSettingUpdated;
                _gammaSettings.SetMapAR(CurrentAR ?? 0);
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