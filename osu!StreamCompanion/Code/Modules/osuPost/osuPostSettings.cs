using System.Windows.Forms;

namespace osu_StreamCompanion.Code.Modules.osuPost
{
    public partial class osuPostSettings : UserControl
    {
        public osuPostSettings()
        {
            InitializeComponent();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(@"http://osupost.givenameplz.de/");
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(@"https://osu.ppy.sh/forum/t/164486");
        }
    }
}
