using StreamCompanionTypes.DataTypes;

namespace osu_StreamCompanion.Code.Modules.FirstRun.Phases
{
    public partial class FirstRunFinish : FirstRunUserControl
    {
        public FirstRunFinish()
        {
            InitializeComponent();
            
            this.pictureBox1.Image = GetStreamCompanionLogo();
        }
        
        
        private void button_end_Click(object sender, System.EventArgs e)
        {
            this.OnCompleted(status.Ok);
        }
    }
}
