using System;
using System.Windows.Forms;
using StreamCompanionTypes;
using StreamCompanionTypes.Interfaces.Services;

namespace TextOverlay
{
    public partial class TextOverlaySettings : UserControl
    {

        private readonly ISettings _settings;
        public event EventHandler<bool> OverlayToggled;
        public TextOverlaySettings(ISettings settings)
        {
            _settings = settings;
            InitializeComponent();

            checkBox_ingameOverlay.Checked = _settings.Get<bool>(TextOverlay.EnableIngameOverlay);
            checkBox_ingameOverlay.CheckedChanged += CheckBoxIngameOverlayOnCheckedChanged;
        }

        private void CheckBoxIngameOverlayOnCheckedChanged(object sender, EventArgs eventArgs)
        {
            _settings.Add(TextOverlay.EnableIngameOverlay.Name, checkBox_ingameOverlay.Checked);
            OnOverlayToggled(checkBox_ingameOverlay.Checked);
        }

        protected virtual void OnOverlayToggled(bool value)
        {
            OverlayToggled?.Invoke(this, value);
        }
    }
}
