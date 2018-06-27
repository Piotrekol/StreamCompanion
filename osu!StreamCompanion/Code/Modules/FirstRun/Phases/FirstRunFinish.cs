using System.Drawing;
using System.Windows.Forms;

namespace osu_StreamCompanion.Code.Modules.FirstRun.Phases
{
    public partial class FirstRunFinish : FirstRunControl
    {
        public FirstRunFinish()
        {
            InitializeComponent();

            Bitmap bmp = new Bitmap(
                System.Reflection.Assembly.GetEntryAssembly().
                    GetManifestResourceStream("osu_StreamCompanion.Resources.logo_256x256.png"));
            this.pictureBox1.Image = bmp;

            this.HandleCreated += Finish_HandleCreated;
        }

        private void Finish_HandleCreated(object sender, System.EventArgs e)
        {
            this.label_msnString.Text = msnString;
            this.label_osuDirectory.Text = osuDir;
        }

        private string msnString;
        private string osuDir;
        public override void GotMsn(string msnString)
        {
            if (this.IsHandleCreated)
            {
                this.BeginInvoke((MethodInvoker) delegate
                {
                    this.label_msnString.Text = msnString;
                });
            }
            else
            {
                this.msnString = msnString;
            }
        }

        public override void GotOsuDirectory(string osuDir)
        {
            if (this.IsHandleCreated)
            {
                this.BeginInvoke((MethodInvoker)delegate
                {
                    this.label_osuDirectory.Text = osuDir;
                });
            }
            else
            {
                this.osuDir = osuDir;
            }
        }

        private void button_end_Click(object sender, System.EventArgs e)
        {
            this.OnCompleted(status.Ok);
        }
    }
}
