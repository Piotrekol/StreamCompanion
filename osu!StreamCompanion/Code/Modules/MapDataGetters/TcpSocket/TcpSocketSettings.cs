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

namespace osu_StreamCompanion.Code.Modules.MapDataGetters.TcpSocket
{
    public partial class TcpSocketSettings : UserControl
    {
        private readonly Settings _settings;
        private readonly SettingNames _names = SettingNames.Instance;

        public TcpSocketSettings(Settings settings)
        {
            _settings = settings;
            InitializeComponent();
        }

        private void checkBox_EnableTcpOutput_CheckedChanged(object sender, EventArgs e)
        {
            _settings.Add(_names.tcpSocketEnabled.Name, checkBox_EnableTcpOutput.Checked);
        }
    }
}
