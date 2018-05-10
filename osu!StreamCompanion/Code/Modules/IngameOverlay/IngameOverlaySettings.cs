using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using osu_StreamCompanion.Code.Core;
using osu_StreamCompanion.Code.Misc;

namespace osu_StreamCompanion.Code.Modules.IngameOverlay
{
    public partial class IngameOverlaySettings : UserControl
    {
        private readonly SettingNames _names = SettingNames.Instance;

        private readonly Settings _settings;

        public IngameOverlaySettings(Settings settings)
        {
            _settings = settings;
            InitializeComponent();

            checkBox_ingameOverlay.Checked = _settings.Get<bool>(_names.EnableIngameOverlay);
            checkBox_ingameOverlay.CheckedChanged += CheckBoxIngameOverlayOnCheckedChanged;
        }

        private void CheckBoxIngameOverlayOnCheckedChanged(object sender, EventArgs eventArgs)
        {
            _settings.Add(_names.EnableIngameOverlay.Name, checkBox_ingameOverlay.Checked);
        }

    }
}
