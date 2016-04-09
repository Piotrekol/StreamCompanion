using System;
using System.Windows.Forms;
using osu_StreamCompanion.Code.Core;
using osu_StreamCompanion.Code.Core.DataTypes;
using osu_StreamCompanion.Code.Helpers;
using osu_StreamCompanion.Code.Interfeaces;

namespace osu_StreamCompanion.Code.Modules.osuPost
{
    public class OsuPost :IModule,ISettingsProvider,IMapDataGetter,IDisposable
    {
        private Settings _settings;
        public bool Started { get; set; }
        OsuPostApi api = new OsuPostApi();
        public void Start(ILogger logger)
        {
            Started = true;
            SwitchApiStatus(_settings.Get("osuPostEnabled", false));
            _settings.SettingUpdated+=SettingUpdated;
        }

        private void SettingUpdated(object sender, SettingUpdated settingUpdated)
        {
            if (settingUpdated.Name == "osuPostEnabled")
                SwitchApiStatus(_settings.Get("osuPostEnabled", false));

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
            _frmSettings.checkBox_osuPostEnabled.Checked = _settings.Get("osuPostEnabled", false);

            return _frmSettings;
        }

        private void SwitchApiStatus(bool enable)
        {
            if (enable)
            {
                api.SetOsuPostLoginData(_settings.Get("osuPostLogin", ""), _settings.Get("osuPostPassword", ""));
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