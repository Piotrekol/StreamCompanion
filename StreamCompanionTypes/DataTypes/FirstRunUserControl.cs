using System;
using System.Drawing;
using System.Windows.Forms;

namespace StreamCompanionTypes.DataTypes
{
    public class CompletedEventArgs : EventArgs
    {
        public FirstRunUserControl.status ControlCompletionStatus { get; set; }
    }
    public class FirstRunUserControl : UserControl //Designer doesn't allow for this class to be abstract
    {
        public event EventHandler<CompletedEventArgs> Completed;

        protected void OnCompleted(status completionStatus)
        {
            this.Completed?.Invoke(this, new CompletedEventArgs() { ControlCompletionStatus = completionStatus });
        }

        protected Bitmap GetStreamCompanionLogo()
        {
            return new Bitmap(
                System.Reflection.Assembly.GetEntryAssembly().
                    GetManifestResourceStream("osu_StreamCompanion.Resources.logo_256x256.png"));
        }
        public enum status
        {
            Unknown = 0,
            Ok = 1,
            Fail = 2,
        }
        
    }
}