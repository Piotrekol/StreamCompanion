using System;
using System.Windows.Forms;
using StreamCompanionTypes.Interfaces.Services;

namespace WebSocketDataSender
{
    public partial class WebSocketSettings : UserControl
    {
        private readonly ISettings _settings;

        public WebSocketSettings(ISettings settings)
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
