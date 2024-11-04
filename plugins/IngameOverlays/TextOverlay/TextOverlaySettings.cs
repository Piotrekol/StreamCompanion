using System;
using System.Windows.Forms;
using StreamCompanionTypes;
using StreamCompanionTypes.Interfaces.Services;

namespace TextOverlay
{
    public partial class TextOverlaySettings : UserControl
    {

        private readonly ISettings _settings;
        public event EventHandler<bool> RestartRequested;
        public TextOverlaySettings(ISettings settings)
        {
            _settings = settings;
            InitializeComponent();

            checkBox_ingameOverlay.Checked = _settings.Get<bool>(TextOverlay.EnableIngameOverlay);
            checkBox_ingameOverlay.CheckedChanged += CheckBoxIngameOverlayOnCheckedChanged;

            checkBox_noOsuRestartCheck.Checked = _settings.Get<bool>(TextOverlay.BypassOsuRunningCheck);
            checkBox_noOsuRestartCheck.CheckedChanged += checkBox_noOsuRestartCheck_CheckedChanged;
        }

        private void CheckBoxIngameOverlayOnCheckedChanged(object sender, EventArgs eventArgs)
        {
            _settings.Add(TextOverlay.EnableIngameOverlay.Name, checkBox_ingameOverlay.Checked);
            RequestRestart(checkBox_ingameOverlay.Checked);
        }

        private void checkBox_noOsuRestartCheck_CheckedChanged(object sender, EventArgs e)
        {
            _settings.Add(TextOverlay.BypassOsuRunningCheck.Name, checkBox_noOsuRestartCheck.Checked);
            RequestRestart(checkBox_noOsuRestartCheck.Checked);
        }

        private void RequestRestart(bool value)
            => RestartRequested?.Invoke(this, value);
    }
}
