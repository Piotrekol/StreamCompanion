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
using StreamCompanionTypes;
using StreamCompanionTypes.Interfaces;

namespace osu_StreamCompanion.Code.Modules.MapDataGetters.TcpSocket
{
    public partial class TcpSocketSettings : UserControl
    {
        private readonly ISettingsHandler _settings;
        private readonly SettingNames _names = SettingNames.Instance;

        public TcpSocketSettings(ISettingsHandler settings)
        {
            _settings = settings;
            InitializeComponent();
            checkBox_EnableTcpOutput.Checked = _settings.Get<bool>(_names.tcpSocketEnabled);


            checkBox_EnableTcpOutput.CheckedChanged += checkBox_EnableTcpOutput_CheckedChanged;

        }

        private void checkBox_EnableTcpOutput_CheckedChanged(object sender, EventArgs e)
        {
            _settings.Add(_names.tcpSocketEnabled.Name, checkBox_EnableTcpOutput.Checked, true);
        }
    }
}
