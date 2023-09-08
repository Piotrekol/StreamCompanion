using System;
using System.Threading;
using System.Threading.Tasks;
using StreamCompanion.Common;
using StreamCompanionTypes;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Interfaces;
using StreamCompanionTypes.Interfaces.Consumers;
using StreamCompanionTypes.Interfaces.Services;
using StreamCompanionTypes.Interfaces.Sources;

namespace osuPost
{
    [SCPlugin("Osu Post", "OsuPost integration", Consts.SCPLUGIN_AUTHOR, Consts.SCPLUGIN_BASEURL)]
    public class OsuPost : IPlugin, ISettingsSource, IMapDataConsumer, IDisposable
    {
        private readonly SettingNames _names = SettingNames.Instance;

        private ISettings _settings;
        private OsuPostApi api;

        public OsuPost(ILogger logger, ISettings settings)
        {
            _settings = settings;

            api = new OsuPostApi(logger);
            api.EndpointUrl = _settings.Get<string>(_names.osuPostEndpoint);
            SwitchApiStatus(_settings.Get<bool>(_names.osuPostEnabled));
            _settings.SettingUpdated += SettingUpdated;
        }

        private void SettingUpdated(object sender, SettingUpdated settingUpdated)
        {
            if (settingUpdated.Name == _names.osuPostEnabled.Name)
                SwitchApiStatus(_settings.Get<bool>(_names.osuPostEnabled));
            else if (settingUpdated.Name == _names.osuPostEndpoint.Name)
                api.EndpointUrl = _settings.Get<string>(_names.osuPostEndpoint);
        }

        public string SettingGroup { get; } = "osu!Post";
        public void Free()
        {
            _frmSettings.Dispose();
        }

        private osuPostSettings _frmSettings;
        public object GetUiSettings()
        {
            if (_frmSettings == null || _frmSettings.IsDisposed)
            {
                _frmSettings = new osuPostSettings(_settings);
            }
            _frmSettings.checkBox_osuPostEnabled.Checked = _settings.Get<bool>(_names.osuPostEnabled);

            return _frmSettings;
        }

        private void SwitchApiStatus(bool enable)
        {
            if (enable)
            {
                api.SetOsuPostLoginData(_settings.Get<string>(_names.osuPostLogin), _settings.Get<string>(_names.osuPostPassword));
            }
            else
            {
                api.Disable();
            }
        }

        public Task SetNewMapAsync(IMapSearchResult map, CancellationToken cancellationToken)
        {
            api.NewMap(map);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            api.Dispose();
        }

    }
}