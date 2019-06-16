using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using StreamCompanionTypes.Interfaces;

namespace WebSocketDataSender
{
    public partial class WebSocketSettings : UserControl
    {
        private readonly ISettingsHandler _settings;

        public WebSocketSettings(ISettingsHandler settings)
        {
            _settings = settings;
            InitializeComponent();

            checkBox_EnableWebSocketOutput.Checked = settings.Get<bool>(WebSocketDataGetter.Enabled);
            checkBox_EnableWebSocketOutput.CheckedChanged += CheckBoxEnableWebSocketOutputOnCheckedChanged;
        }

        private void CheckBoxEnableWebSocketOutputOnCheckedChanged(object sender, EventArgs e)
        {
            _settings.Add(WebSocketDataGetter.Enabled.Name, checkBox_EnableWebSocketOutput.Checked);
        }
    }
}
