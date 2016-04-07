using System.Windows.Forms;

namespace osu_StreamCompanion.Code.Modules.SCGUI
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
            richTextBox1.Text = @"This software is licensed under GNU GPL version 3. You can find the full text of the license inside this link:
https://github.com/Piotrekol/StreamCompanion/blob/master/LICENSE
StreamCompanion is written by Piotrekol
https://github.com/Piotrekol
https://osu.ppy.sh/u/Piotrekol
https://twitch.tv/Piotrekol
            ";
        }

        private void richTextBox1_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.LinkText);
        }

        private void pictureBox2_Click(object sender, System.EventArgs e)
        {
            System.Diagnostics.Process.Start(@"https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=CX2ZC3JKVAK74");
        }
    }
}
