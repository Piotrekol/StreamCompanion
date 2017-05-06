using System;
using System.Windows.Forms;
using osu_StreamCompanion.Code.Core;
using osu_StreamCompanion.Code.Helpers;
using osu_StreamCompanion.Code.Misc;

namespace osu_StreamCompanion.Code.Modules.ModParser
{
    public partial class ModParserSettings : UserControl
    {
        private readonly SettingNames _names = SettingNames.Instance;
        private Settings _settings;
        private bool init = true;
        public ModParserSettings(Settings settings)
        {
            _settings = settings;
            _settings.SettingUpdated += SettingUpdated;
            this.Enabled = _settings.Get<bool>(_names.EnableMemoryScanner);
            InitializeComponent();
            textBox_Mods.Text = _settings.Get<string>(_names.NoModsDisplayText);
            radioButton_longMods.Checked = _settings.Get<bool>(_names.UseLongMods);
            radioButton_shortMods.Checked = !radioButton_longMods.Checked;
            init = false;
        }

        private void SettingUpdated(object sender, SettingUpdated settingUpdated)
        {
            if (this.IsHandleCreated)
                this.BeginInvoke((MethodInvoker)(() =>
           {
               if (settingUpdated.Name != _names.EnableMemoryScanner.Name)
                   this.Enabled = _settings.Get<bool>(_names.EnableMemoryScanner);
           }));
        }

        private void textBox_Mods_TextChanged(object sender, EventArgs e)
        {
            if (init) return;
            _settings.Add(_names.NoModsDisplayText.Name, textBox_Mods.Text);
        }

        private void radioButton_longMods_CheckedChanged(object sender, EventArgs e)
        {
            if (init) return;
            if (((RadioButton)sender).Checked)
            {
                _settings.Add(_names.UseLongMods.Name, radioButton_longMods.Checked);
            }
        }
    }
}
