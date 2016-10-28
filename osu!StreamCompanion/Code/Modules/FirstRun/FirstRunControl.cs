using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace osu_StreamCompanion.Code.Modules.FirstRun
{
    public class CompletedEventArgs :EventArgs
    {
        public FirstRunControl.status ControlCompletionStatus { get; set; }
    }
    public class FirstRunControl : UserControl
    {
        public event EventHandler<CompletedEventArgs> Completed;

        protected void OnCompleted(status completionStatus)
        {
            this.Completed?.Invoke(this,new CompletedEventArgs() {ControlCompletionStatus = completionStatus});
        }
        public enum status
        {
            Unknown = 0,
            Ok = 1,
            Fail=2,
        }

        public virtual void GotMsn(string msnString)
        {
        }
        public virtual void GotOsuDirectory(string osuDir)
        {
        }
    }
}
