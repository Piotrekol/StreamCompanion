using StreamCompanionTypes.DataTypes;

namespace MSNEventSource
{
    public partial class FirstRunMsn : FirstRunUserControl
    {
        public FirstRunMsn()
        {
            InitializeComponent();
            this.pictureBox1.Image = GetStreamCompanionLogo();
        }

        public void GotMsn(string msnString)
        {
            this.OnCompleted(FirstRunUserControl.status.Ok);
        }
    }
}
