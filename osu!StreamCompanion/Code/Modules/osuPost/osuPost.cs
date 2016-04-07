using System;
using System.Windows.Forms;
using osu_StreamCompanion.Code.Core;
using osu_StreamCompanion.Code.Core.DataTypes;
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
                _frmSettings = new osuPostSettings();
                _frmSettings.checkBox1.CheckedChanged += CheckBox1_CheckedChanged;
            }
            _frmSettings.checkBox1.Checked = _settings.Get("osuPostEnabled", false);

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
        private void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            _settings.Add("osuPostEnabled",_frmSettings.checkBox1.Checked);
            SwitchApiStatus(_frmSettings.checkBox1.Checked);
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