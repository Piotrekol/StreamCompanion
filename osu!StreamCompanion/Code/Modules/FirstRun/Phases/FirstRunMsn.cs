using System.Drawing;
using System.Windows.Forms;

namespace osu_StreamCompanion.Code.Modules.FirstRun.Phases
{
    public partial class FirstRunMsn : FirstRunControl
    {
        public FirstRunMsn()
        {
            InitializeComponent();
            Bitmap bmp = new Bitmap(
                System.Reflection.Assembly.GetEntryAssembly().
                    GetManifestResourceStream("osu_StreamCompanion.Resources.logo_256x256.png"));
            this.pictureBox1.Image = bmp;
        }

        public override void GotMsn(string msnString)
        {
            this.OnCompleted(status.Ok);
        }
    }
}
