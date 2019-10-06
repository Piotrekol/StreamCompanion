using System;
using System.Windows.Forms;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Enums;
using StreamCompanionTypes.Interfaces;

namespace osu_StreamCompanion.Code.Modules.FirstRun.Phases
{
    public partial class FirstRunFinish : UserControl, IFirstRunUserControl
    {
        public FirstRunFinish()
        {
            InitializeComponent();

            this.pictureBox1.Image = StreamCompanionHelper.StreamCompanionLogo();
        }


        private void button_end_Click(object sender, System.EventArgs e)
        {
            Completed?.Invoke(this,new FirstRunCompletedEventArgs{ ControlCompletionStatus = FirstRunStatus.Ok });
        }

        public event EventHandler<FirstRunCompletedEventArgs> Completed;
    }
}
