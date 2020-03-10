using System;
using System.Windows.Forms;
using StreamCompanionTypes;
using StreamCompanionTypes.Interfaces;
using StreamCompanionTypes.Interfaces.Services;

namespace osuPost
{
    public partial class osuPostSettings : UserControl
    {
        private readonly SettingNames _names = SettingNames.Instance;

        private readonly ISettings _settings;
        private bool init = true;
        public osuPostSettings(ISettings settings)
        {
            _settings = settings;
            InitializeComponent();
            textBox_userId.Text = _settings.Get<string>(_names.osuPostLogin);
            textBox_userPassword.Text = _settings.Get<string>(_names.osuPostPassword);
            textBox_endpointUrl.Text = _settings.Get<string>(_names.osuPostEndpoint);
            panel_settings.Enabled = _settings.Get<bool>(_names.osuPostEnabled);
            init = false;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(@"http://osupost.givenameplz.de/");
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(@"https://osu.ppy.sh/forum/t/164486");
        }
        private void textBox_login_TextChanged(object sender, EventArgs e)
        {
            if (init) return;
            _settings.Add(_names.osuPostLogin.Name, textBox_userId.Text);
        }
        private void textBox_password_TextChanged(object sender, EventArgs e)
        {
            if (init) return;
            _settings.Add(_names.osuPostPassword.Name, textBox_userPassword.Text);
        }

        private void checkBox_osuPostEnabled_CheckedChanged(object sender, EventArgs e)
        {
            if (init) return;
            _settings.Add(_names.osuPostEnabled.Name, checkBox_osuPostEnabled.Checked, true);
            panel_settings.Enabled = checkBox_osuPostEnabled.Checked;
        }


        private void checkBox_advanced_CheckedChanged(object sender, EventArgs e)
        {
            panel_advanced.Visible = checkBox_advanced.Checked;
        }

        private void textBox_endpointUrl_TextChanged(object sender, EventArgs e)
        {
            if (init) return;
            Uri uriResult;
            bool valid = Uri.TryCreate(textBox_endpointUrl.Text, UriKind.Absolute, out uriResult)
                          && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            if (valid)
                _settings.Add(_names.osuPostEndpoint.Name, textBox_endpointUrl.Text, true);
            else
                MessageBox.Show("Invalid url - I recommend copy-pasting it if you have one", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void button_resetEndpoint_Click(object sender, EventArgs e)
        {
            textBox_endpointUrl.Text = _names.osuPostEndpoint.Default<string>();
        }
    }
}
