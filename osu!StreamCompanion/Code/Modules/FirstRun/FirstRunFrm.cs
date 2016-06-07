using System;
using System.Windows.Forms;
using osu_StreamCompanion.Code.Core;
using osu_StreamCompanion.Code.Misc;

namespace osu_StreamCompanion.Code.Modules.FirstRun
{
    public partial class FirstRunFrm : Form
    {
        private readonly SettingNames _names = SettingNames.Instance;

        private Phase1 _phase1;
        private Phase2 _phase2;
        private Settings _settings;

        public bool completedSuccesfully
        {
            get { return _phase2 != null; }
        }

        public FirstRunFrm(Settings settings)
        {
            _settings = settings;
            InitializeComponent();
        }

        public void StartSetup()
        {
            _phase1 = new Phase1();
            this.Controls.Add(_phase1);
            this.ShowDialog();
        }
        public void GotMsn(string msnString)
        {
            if (_phase1 != null)
            {
                this.BeginInvoke((MethodInvoker)delegate
                {
                    this.Controls.RemoveAt(0);
                    _phase1.Dispose();
                    _phase1 = null;

                    _phase2 = new Phase2();
                    _phase2.button_end.Click += ButtonEndOnClick;
                    _phase2.label_msnString.Text = msnString;
                    _phase2.label_osuDirectory.Text = _settings.Get<string>(_names.MainOsuDirectory);
                    this.Controls.Add(_phase2);
                });
            }
            if (_phase2 != null)
                if (_phase2.Created)
                {
                    _phase2.BeginInvoke((MethodInvoker)delegate
                    {
                        _phase2.label_msnString.Text = msnString;
                    });
                }
        }

        private void ButtonEndOnClick(object sender, EventArgs eventArgs)
        {
            this.BeginInvoke((MethodInvoker) delegate()
            {
                this.Close();
            });
        }
    }
}

