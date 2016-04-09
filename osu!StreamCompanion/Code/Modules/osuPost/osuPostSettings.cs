using System;
using System.Windows.Forms;
using osu_StreamCompanion.Code.Core;

namespace osu_StreamCompanion.Code.Modules.osuPost
{
    public partial class osuPostSettings : UserControl
    {
        private readonly Settings _settings;
        private bool init = true;
        public osuPostSettings(Settings settings)
        {
            _settings = settings;
            InitializeComponent();
            textBox_userId.Text = _settings.Get("osuPostLogin", "");
            textBox_userPassword.Text = _settings.Get("osuPostPassword", "");
            panel_settings.Enabled = _settings.Get("osuPostEnabled", false);
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
            _settings.Add("osuPostLogin", textBox_userId.Text);
        }
        private void textBox_password_TextChanged(object sender, EventArgs e)
        {
            if (init) return;
            _settings.Add("osuPostPassword", textBox_userPassword.Text);
        }

        private void checkBox_osuPostEnabled_CheckedChanged(object sender, EventArgs e)
        {
            if (init) return;
            _settings.Add("osuPostEnabled", checkBox_osuPostEnabled.Checked, true);
            panel_settings.Enabled = checkBox_osuPostEnabled.Checked;
        }
    }
}
