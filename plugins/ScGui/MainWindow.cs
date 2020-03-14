using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using StreamCompanionTypes;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Interfaces;
using StreamCompanionTypes.Interfaces.Services;
using StreamCompanionTypes.Interfaces.Sources;

namespace ScGui
{
    class MainWindow : IPlugin
    {
        private readonly SettingNames _names = SettingNames.Instance;

        private ISettings _settings;
        private MainForm _mainForm;
        private IMainWindowModel _mainWindowHandle;
        private readonly Delegates.Exit _exitAction;
        private List<ISettingsSource> _settingsList;

        public string Description { get; } = "";
        public string Name { get; } = "StreamCompanion GUI";
        public string Author { get; } = "Piotrekol";
        public string Url { get; } = "";
        public string UpdateUrl { get; } = "";

        public MainWindow(ISettings settings, IMainWindowModel mainWindowHandle, IEnumerable<ISettingsSource> settingsSources,Delegates.Exit exitAction)
        {
            _settings = settings;
            _mainWindowHandle = mainWindowHandle;
            _exitAction = exitAction;
            _settingsList = settingsSources.ToList();

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
                    _exitAction?.Invoke("User pressed exit");
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
    }
}
