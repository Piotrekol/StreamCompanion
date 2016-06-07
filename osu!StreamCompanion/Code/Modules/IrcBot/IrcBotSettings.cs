using System;
using System.Windows.Forms;
using osu_StreamCompanion.Code.Core;

namespace osu_StreamCompanion.Code.Modules.IrcBot
{
    public partial class IrcBotSettings : UserControl
    {
        private readonly Settings _settings;

        //TODO: create global class for config names.
        private const string CfgEnableTwitchBot = "EnableTwitchBot";
        private const string CfgTwitchUsername = "IrcUsername";
        private const string CfgTwitchPassword = "IrcPassword";
        private const string CfgTwitchChannel = "IrcChannel";

        public IrcBotSettings(Settings settings)
        {
            _settings = settings;
            InitializeComponent();

            field_username.textBox.Text = _settings.Get(CfgTwitchUsername, "");
            field_password.textBox.Text = _settings.Get(CfgTwitchPassword, "");
            field_Channel.textBox.Text = _settings.Get(CfgTwitchChannel, "");


            checkBox_enableTwitchBot.CheckedChanged += CheckBox_CheckedChanged;
            checkBox_enableTwitchBot.Checked = _settings.Get(CfgEnableTwitchBot, false);


            field_username.textBox.TextChanged += TextBox_TextChanged;
            field_password.textBox.TextChanged += TextBox_TextChanged;
            field_Channel.textBox.TextChanged += TextBox_TextChanged;

        }

        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            if (sender == field_username.textBox)
            {
                _settings.Add(CfgTwitchUsername, field_username.textBox.Text);
            }
            else if (sender == field_password.textBox)
            {
                _settings.Add(CfgTwitchPassword, field_password.textBox.Text);
            }
            else if (sender == field_Channel.textBox)
            {
                _settings.Add(CfgTwitchChannel, field_Channel.textBox.Text);
            }
        }

        private void CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (sender == checkBox_enableTwitchBot)
            {
                bool enabled = checkBox_enableTwitchBot.Checked;
                panel_botOptions.Enabled = enabled;
                _settings.Add(CfgEnableTwitchBot, enabled);
            }
        }
    }
}
