namespace osu_StreamCompanion.Code.Modules.FirstRun.Phases
{
    public partial class FirstRunMsn : FirstRunControl
    {
        public FirstRunMsn()
        {
            InitializeComponent();
        }

        public override void GotMsn(string msnString)
        {
            this.OnCompleted(status.Ok);
        }
    }
}
