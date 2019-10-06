using System;
using System.Windows.Forms;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Enums;
using StreamCompanionTypes.Interfaces;

namespace MSNEventSource
{
    public partial class FirstRunMsn : UserControl, IFirstRunUserControl
    {
        public FirstRunMsn()
        {
            InitializeComponent();
            this.pictureBox1.Image = StreamCompanionHelper.StreamCompanionLogo();
        }

        public void GotMsn(string msnString)
        {
            Completed?.Invoke(this, new FirstRunCompletedEventArgs { ControlCompletionStatus = FirstRunStatus.Ok });
        }

        public event EventHandler<FirstRunCompletedEventArgs> Completed;
    }
}
