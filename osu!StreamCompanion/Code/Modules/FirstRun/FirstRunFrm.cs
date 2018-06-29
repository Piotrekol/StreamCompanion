using System.Collections.Generic;
using System.Windows.Forms;
using osu_StreamCompanion.Code.Modules.FirstRun.Phases;
using StreamCompanionTypes.DataTypes;

namespace osu_StreamCompanion.Code.Modules.FirstRun
{
    public partial class FirstRunFrm : Form
    {        
        private List<FirstRunUserControl> phases = new List<FirstRunUserControl>();
        public bool CompletedSuccesfully { get; set; }

        public FirstRunFrm(List<FirstRunUserControl> firstRunControls)
        {
            foreach (var control in firstRunControls)
            {
                phases.Add(control);
            }
            phases.Add(new FirstRunFinish());
            InitializeComponent();
        }

        private int _currentPhase = 0;
        public void StartSetup()
        {
            if (phases.Count > 0)
            {
                var first = phases[0];
                first.Completed += Phase_Completed;
                this.Controls.Add(first);
                this.ShowDialog();
            }
            else
            {
                CompletedSuccesfully = true;
            }
        }

        private void Phase_Completed(object sender, CompletedEventArgs e)
        {
            this.BeginInvoke((MethodInvoker)delegate
            {
                phases[_currentPhase].Completed -= Phase_Completed;
                this.Controls.RemoveAt(0);
                phases[_currentPhase].Dispose();
                if (phases.Count > _currentPhase+1)
                {
                    _currentPhase++;
                    var current = phases[_currentPhase];
                    current.Completed += Phase_Completed;
                    this.Controls.Add(current);
                }
                else
                {
                    CompletedSuccesfully = true;
                    this.BeginInvoke((MethodInvoker)delegate ()
                    {
                        this.Close();
                    });
                }
            });
        }
        
    }
}

