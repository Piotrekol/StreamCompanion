using System.Collections.Generic;
using System.Windows.Forms;
using osu_StreamCompanion.Code.Misc;
using StreamCompanionTypes;
using StreamCompanionTypes.Interfaces;

namespace osu_StreamCompanion.Code.Modules.SCGUI
{
    class MainWindow : IModule, ISettingsProvider, IMainWindowUpdater, ISettingsGetter
    {
        private readonly SettingNames _names = SettingNames.Instance;

        private ISettingsHandler _settings;
        private MainForm _mainForm;
        private IMainWindowModel _mainWindowHandle;
        private List<ISettingsProvider> _settingsList;
        public bool Started { get; set; }
        public void Start(ILogger logger)
        {
            Started = true;
            if (!_settings.Get<bool>(_names.StartHidden))
                ShowWindow();
        }

        private void ShowWindow()
        {
            if (_mainForm == null)
            {
                _mainForm = new MainForm();
                _mainForm.Shown += (sender, args) =>
                {
                    _mainForm.BeginInvoke((MethodInvoker)(() => { _mainForm.SetDataBindings(_mainWindowHandle); }));
                };
                _mainForm.Closed += (sender, args) =>
                {
                    _settings.Save();
                    Program.SafeQuit();
                };
                _mainForm.button_OpenSettings.Click += (sender, args) =>
                {
                    using (SettingsForm settingsForm = new SettingsForm(_settingsList))
                    {
                        settingsForm.ShowDialog();
                    }
                };
            }
            _mainForm.Show();
        }

        private void HideWindow(bool dispose = true)
        {
            _mainForm.Hide();
            if (dispose)
            {
                _mainForm.Dispose();
                _mainForm = null;
            }
        }




        public string SettingGroup { get; } = "General";
        public void SetSettingsHandle(ISettingsHandler settings)
        {
            _settings = settings;
        }

        public void Free()
        {

        }

        public UserControl GetUiSettings()
        {
            return null;
        }

        public void GetMainWindowHandle(IMainWindowModel mainWindowHandle)
        {
            _mainWindowHandle = mainWindowHandle;
        }

        public void SetSettingsListHandle(List<ISettingsProvider> settingsList)
        {
            _settingsList = settingsList;
        }
    }
}
