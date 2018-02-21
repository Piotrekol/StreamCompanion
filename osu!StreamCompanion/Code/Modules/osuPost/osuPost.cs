using System;
using System.Windows.Forms;
using osu_StreamCompanion.Code.Core;
using osu_StreamCompanion.Code.Core.DataTypes;
using osu_StreamCompanion.Code.Helpers;
using osu_StreamCompanion.Code.Interfaces;
using osu_StreamCompanion.Code.Misc;

namespace osu_StreamCompanion.Code.Modules.osuPost
{
    public class OsuPost :IModule,ISettingsProvider,IMapDataGetter,IDisposable
    {
        private readonly SettingNames _names = SettingNames.Instance;

        private Settings _settings;
        public bool Started { get; set; }
        private OsuPostApi api = new OsuPostApi();
        public void Start(ILogger logger)
        {
            Started = true;
            api.EndpointUrl = _settings.Get<string>(_names.osuPostEndpoint);
            SwitchApiStatus(_settings.Get<bool>(_names.osuPostEnabled));
            _settings.SettingUpdated+=SettingUpdated;
        }

        private void SettingUpdated(object sender, SettingUpdated settingUpdated)
        {
            if (settingUpdated.Name == _names.osuPostEnabled.Name)
                SwitchApiStatus(_settings.Get<bool>(_names.osuPostEnabled));
            else if (settingUpdated.Name == _names.osuPostEndpoint.Name)
                api.EndpointUrl = _settings.Get<string>(_names.osuPostEndpoint);
        }

        public string SettingGroup { get; } = "osu!Post";
        public void SetSettingsHandle(Settings settings)
        {
            _settings = settings;
        }

        public void Free()
        {
            _frmSettings.Dispose();
        }

        private osuPostSettings _frmSettings;
        public UserControl GetUiSettings()
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