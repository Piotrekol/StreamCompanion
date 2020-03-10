using System;
using System.Windows.Forms;
using StreamCompanionTypes;
using StreamCompanionTypes.Interfaces;
using StreamCompanionTypes.Interfaces.Services;

namespace TcpSocketDataSender
{
    public partial class TcpSocketSettings : UserControl
    {
        private readonly ISettings _settings;
        private readonly SettingNames _names = SettingNames.Instance;

        public TcpSocketSettings(ISettings settings)
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
