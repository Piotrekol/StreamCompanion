using System;
using System.Collections.Generic;
using System.Windows.Forms;
using osu_StreamCompanion.Code.Misc;
using osu_StreamCompanion.Code.Modules.FirstRun.Phases;
using StreamCompanionTypes.Interfaces;

namespace osu_StreamCompanion.Code.Modules.FirstRun
{
    public partial class FirstRunFrm : Form
    {
        private readonly SettingNames _names = SettingNames.Instance;
        
        private ISettingsHandler _settings;
        private List<FirstRunControl> phases = new List<FirstRunControl>();
        public bool CompletedSuccesfully { get; set; }

        public FirstRunFrm(ISettingsHandler settings)
        {
            _settings = settings;
            phases.Add(new FirstRunMsn());
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

        public void GotMsn(string msnString)
        {
            var osuDir = _settings.Get<string>(_names.MainOsuDirectory);

            foreach (var phase in phases)
            {
                phase.GotMsn(msnString);
                phase.GotOsuDirectory(osuDir);
            }
        }
        
    }
}

