using System;
using System.Windows.Forms;
using StreamCompanionTypes.Interfaces;
using StreamCompanionTypes.Interfaces.Services;

namespace osuOverlay
{
    public partial class IngameOverlaySettings : UserControl
    {

        private readonly ISettings _settings;

        public IngameOverlaySettings(ISettings settings)
        {
            _settings = settings;
            InitializeComponent();

            checkBox_ingameOverlay.Checked = _settings.Get<bool>(PluginSettings.EnableIngameOverlay);
            checkBox_ingameOverlay.CheckedChanged += CheckBoxIngameOverlayOnCheckedChanged;
        }

        private void CheckBoxIngameOverlayOnCheckedChanged(object sender, EventArgs eventArgs)
        {
            _settings.Add(PluginSettings.EnableIngameOverlay.Name, checkBox_ingameOverlay.Checked);
        }

    }
}
