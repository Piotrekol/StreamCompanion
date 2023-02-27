using System;
using System.Diagnostics;
using System.Windows.Forms;
using StreamCompanion.Common;

namespace osu_StreamCompanion.Code.Modules.Donation
{
    public partial class DonationSettings : UserControl
    {
        public DonationSettings()
        {
            InitializeComponent();

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            ProcessExt.OpenUrl(@"https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=CX2ZC3JKVAK74");
        }
    }
}
