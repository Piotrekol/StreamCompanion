using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using osu_StreamCompanion.Code.Misc;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Interfaces;

namespace osu_StreamCompanion.Code.Modules.MapDataParsers.Parser1
{
    public partial class ParserSettings : UserControl
    {
        private readonly ISettingsHandler _settings;
        private readonly Action _resetPatterns;
        private BindingList<OutputPattern> _patterns;
        private readonly SettingNames _names = SettingNames.Instance;

        public ParserSettings(ISettingsHandler settings, Action resetPatterns)
        {
            _settings = settings;
            _resetPatterns = resetPatterns;
            InitializeComponent();
            this.patternList.SelectedItemChanged += SelectedItemChanged;
            this.checkBox_disableDiskSaving.Checked = _settings.Get<bool>(_names.DisableDiskPatternWrite);
            this.patternEdit.DeletePattern += DeletePattern;
            this.patternEdit.AddPattern += AddPattern;
            checkBox_disableDiskSaving.CheckedChanged += CheckBox_disableDiskSaving_CheckedChanged;
        }

        private void CheckBox_disableDiskSaving_CheckedChanged(object sender, EventArgs e)
        {
            _settings.Add(_names.DisableDiskPatternWrite.Name, checkBox_disableDiskSaving.Checked);
        }

        public static string GenerateFileName(IList<string> currentFileNames, string baseName = "name_")
        {
            string name;
            int i = 0;
            do
            {
                name = baseName + i++;
            } while (currentFileNames.Any(fileName => fileName == name));
            return name;
        }
        private void AddPattern(object sender, EventArgs eventArgs)
        {
            var name = GenerateFileName(_patterns.Select(p => p.Name).ToList());
            _patterns.Add(new OutputPattern()
            {
                Name = name
            });
        }

        private void DeletePattern(object sender, OutputPattern pattern)
        {
            _patterns?.Remove(pattern);
        }


        private void SelectedItemChanged(object sender, OutputPattern outputPattern)
        {
            this.patternEdit.Current = outputPattern;
        }

        public void SetPatterns(BindingList<OutputPattern> patterns)
        {
            _patterns = patterns;
            this.patternList.SetPatterns(patterns);
        }
        public void SetPreview(Dictionary<string, string> replacements)
        {
            this.patternEdit.SetPreview(replacements);
        }

        private void button_reset_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("All of your patterns WILL BE REMOVED" + Environment.NewLine + "Are you sure?",
                "Stream Companion - Pattern reset", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk);
            if (result == DialogResult.Yes)
                _resetPatterns?.Invoke();
        }
    }
}
