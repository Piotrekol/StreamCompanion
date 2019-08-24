using System;
using System.Windows.Forms;
using osu_StreamCompanion.Code.Core;
using StreamCompanionTypes;

namespace osu_StreamCompanion.Code.Modules.MapDataFinders.osuMemoryID
{
    public partial class MemoryDataFinderSettings : UserControl
    {
        private readonly SettingNames _names = SettingNames.Instance;

        private readonly Settings _settings;
        private bool init = true;
        public MemoryDataFinderSettings(Settings settings)
        {
            InitializeComponent();

            _settings = settings;

            bool isFallback = _settings.Get<bool>(_names.OsuFallback);
            if (isFallback)
            {
                checkBox_EnableMemoryFinder.Enabled = false;
            }
            else
                checkBox_EnableMemoryFinder.Checked = _settings.Get<bool>(_names.EnableMemoryScanner);

            init = false;
        }

        private void checkBox_EnableMemoryFinder_CheckedChanged(object sender, EventArgs e)
        {
            if (init) return;
            _settings.Add(_names.EnableMemoryScanner.Name, checkBox_EnableMemoryFinder.Checked, true);
        }

        private void MemoryDataFinderSettings_Load(object sender, EventArgs e)
        {

        }
    }
}
