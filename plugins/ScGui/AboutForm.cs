using System.Windows.Forms;

namespace ScGui
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
            this.pictureBox1.Image = Helpers.GetStreamCompanionLogo();
            richTextBox1.Text = @"This software is licensed under MIT. You can find the full text of the license inside this link:
https://github.com/Piotrekol/StreamCompanion/blob/master/LICENSE
StreamCompanion is written by Piotrekol
https://github.com/Piotrekol
https://osu.ppy.sh/u/Piotrekol
https://twitch.tv/Piotrekol
Libraries used:
CollectionManager by Piotrekol
SQLite
Newtonsoft.Json by JamesNK

FreeType by David Turner, Robert Wilhelm, and Werner Lemberg
FreeType license is avaliable at http://git.savannah.gnu.org/cgit/freetype/freetype2.git/tree/docs/FTL.TXT";
            richTextBox1.ReadOnly = true;
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
