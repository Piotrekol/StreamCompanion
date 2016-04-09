using System;
using System.Windows.Forms;
using osu_StreamCompanion.Code.Core;
using osu_StreamCompanion.Code.Helpers;

namespace osu_StreamCompanion.Code.Modules.ModParser
{
    public partial class ModParserSettings : UserControl
    {
        private Settings _settings;
        private bool init = true;
        public ModParserSettings(Settings settings)
        {
            _settings = settings;
            _settings.SettingUpdated += SettingUpdated;
            this.Enabled = _settings.Get("EnableMemoryScanner", true);
            InitializeComponent();
            textBox_Mods.Text = _settings.Get("NoModsDisplayText", "None");
            radioButton_longMods.Checked = _settings.Get("UseLongMods", false);
            radioButton_shortMods.Checked = !radioButton_longMods.Checked;
            init = false;
        }

        private void SettingUpdated(object sender, SettingUpdated settingUpdated)
        {
            if (this.IsHandleCreated)
                this.BeginInvoke((MethodInvoker)(() =>
           {
               if (settingUpdated.Name == "EnableMemoryScanner")
                   this.Enabled = _settings.Get("EnableMemoryScanner", true);
           }));
        }

        private void textBox_Mods_TextChanged(object sender, EventArgs e)
        {
            if (init) return;
            _settings.Add("NoModsDisplayText", textBox_Mods.Text);
        }

        private void radioButton_longMods_CheckedChanged(object sender, EventArgs e)
        {
            if (init) return;
            if (((RadioButton)sender).Checked)
            {
                _settings.Add("UseLongMods", radioButton_longMods.Checked);
            }
        }
    }
}
