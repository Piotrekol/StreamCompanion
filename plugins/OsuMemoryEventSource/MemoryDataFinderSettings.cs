﻿using System;
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

            bool isFallback = _settings.Get<bool>(_names.OsuFallback);
            if (isFallback)
            {
                checkBox_EnableMemoryFinder.Enabled = false;
            }
            else
            {
                checkBox_EnableMemoryFinder.Checked = _settings.Get<bool>(_names.EnableMemoryScanner);

                checkBox_EnableMemoryPooling.Checked = _settings.Get<bool>(_names.EnableMemoryPooling);
            }

            init = false;
        }

        private void checkBox_EnableMemoryFinder_CheckedChanged(object sender, EventArgs e)
        {
            if (init) return;
            _settings.Add(_names.EnableMemoryScanner.Name, checkBox_EnableMemoryFinder.Checked, true);
        }

        private void checkBox_EnableMemoryPooling_CheckedChanged(object sender, EventArgs e)
        {
            if (init) return;
            _settings.Add(_names.EnableMemoryPooling.Name, checkBox_EnableMemoryPooling.Checked, true);
        }

        private void checkBox_enableSmoothPp_CheckedChanged(object sender, EventArgs e)
        {
            if (init) return;
            _settings.Add(Helpers.EnablePpSmoothing.Name, checkBox_enableSmoothPp.Checked, true);
        }
    }
}

