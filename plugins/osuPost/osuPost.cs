using System;
using System.Windows.Forms;
using StreamCompanionTypes;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Interfaces;

namespace osuPost
{
    public class OsuPost :IPlugin,ISettingsProvider,IMapDataGetter,IDisposable
    {
        private readonly SettingNames _names = SettingNames.Instance;

        private ISettingsHandler _settings;
        private OsuPostApi api;

        public string Description { get; } = "";
        public string Name { get; } = nameof(OsuPost);
        public string Author { get; } = "Piotrekol";
        public string Url { get; } = "";
        public string UpdateUrl { get; } = "";

        public OsuPost(ILogger logger,ISettingsHandler settings)
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

        public void SetNewMap(MapSearchResult map)
        {
            api.NewMap(map);
            
        }

        public void Dispose()
        {
            api.Dispose();
        }

    }
}