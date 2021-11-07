using System;
using System.Windows.Forms;
using StreamCompanionTypes;
using StreamCompanionTypes.Interfaces.Services;

namespace OsuMemoryEventSource
{
    public partial class MemoryDataFinderSettings : UserControl
    {
        private readonly SettingNames _names = SettingNames.Instance;

        private readonly ISettings _settings;
        private bool init = true;
        public MemoryDataFinderSettings(ISettings settings)
        {
            InitializeComponent();

            _settings = settings;

            checkBox_enableSmoothPp.Checked = _settings.Get<bool>(Helpers.EnablePpSmoothing);
            checkBox_saveLiveTokensToDisk.Checked = _settings.Get<bool>(OsuMemoryEventSourceBase.SaveLiveTokensOnDisk);

            init = false;
        }

        private void checkBox_enableSmoothPp_CheckedChanged(object sender, EventArgs e)
        {
            if (init) return;
            _settings.Add(Helpers.EnablePpSmoothing.Name, checkBox_enableSmoothPp.Checked, true);
        }

        private void checkBox_saveLiveTokensToDisk_CheckedChanged(object sender, EventArgs e)
        {
            if (init) return;
            _settings.Add(OsuMemoryEventSourceBase.SaveLiveTokensOnDisk.Name, checkBox_saveLiveTokensToDisk.Checked, true);
        }
    }
}

