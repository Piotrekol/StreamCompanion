using System;
using System.Windows.Forms;
using osu_StreamCompanion.Code.Core;

namespace osu_StreamCompanion.Code.Modules.MapDataFinders.osuMemoryID
{
    public partial class MemoryDataFinderSettings : UserControl
    {
        private readonly Settings _settings;
        private bool init = true;
        public MemoryDataFinderSettings(Settings settings)
        {
            InitializeComponent();

            _settings = settings;

            bool isFallback = _settings.Get("OsuFallback", false);
            if (isFallback)
            {
                checkBox_EnableMemoryFinder.Enabled = false;
            }
            else
                checkBox_EnableMemoryFinder.Checked = _settings.Get("EnableMemoryScanner", true);

            init = false;
        }

        private void checkBox_EnableMemoryFinder_CheckedChanged(object sender, EventArgs e)
        {
            if (init) return;
            _settings.Add("EnableMemoryScanner", checkBox_EnableMemoryFinder.Checked, true);
        }

        private void MemoryDataFinderSettings_Load(object sender, EventArgs e)
        {

        }
    }
}
