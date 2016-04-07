using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace osu_StreamCompanion
{
    public partial class Error : Form
    {
        private readonly string errorTemplate = "Steps to reproduce this error(if known):" + Environment.NewLine +
                                       "" + Environment.NewLine +
                                       "" + Environment.NewLine +
                                       "" + Environment.NewLine +
                                       "~~" + Environment.NewLine +
                                       "Error:" + Environment.NewLine +
                                       "[code]" + Environment.NewLine +
                                       "{0}" + Environment.NewLine +
                                       "[/code]" + Environment.NewLine;
        protected override bool ShowWithoutActivation
        {
            get { return true; }
        }

        public Error(String Message)
        {
            InitializeComponent();
            this.textBox1.Text = Message;
        }

        private void button_message_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Clipboard.SetText(string.Format(errorTemplate, textBox1.Text));
            Process.Start(@"https://osu.ppy.sh/forum/ucp.php?i=pm&mode=compose&u=304520");
        }
    }
}
