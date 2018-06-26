using System;
using System.Collections.Generic;
using System.Windows.Forms;
using StreamCompanionTypes;
using StreamCompanionTypes.Interfaces;

namespace ScGui
{
    class MainWindow : IPlugin, ISettingsProvider, IMainWindowUpdater, ISettingsGetter,IExiter
    {
        private readonly SettingNames _names = SettingNames.Instance;

        private ISettingsHandler _settings;
        private MainForm _mainForm;
        private IMainWindowModel _mainWindowHandle;
        private List<ISettingsProvider> _settingsList;
        private Action<object> exitAction;
        public bool Started { get; set; }

        public string Description { get; } = "";
        public string Name { get; } = "StreamCompanion GUI";
        public string Author { get; } = "Piotrekol";
        public string Url { get; } = "";
        public string UpdateUrl { get; } = "";

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
                    exitAction?.Invoke("User pressed exit");
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
            return (UserControl)null;
        }

        public void GetMainWindowHandle(IMainWindowModel mainWindowHandle)
        {
            _mainWindowHandle = mainWindowHandle;
        }

        public void SetSettingsListHandle(List<ISettingsProvider> settingsList)
        {
            _settingsList = settingsList;
        }

        public void SetExitHandle(Action<object> exiter)
        {
            this.exitAction = exiter;
        }
    }
}
