using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using osu_StreamCompanion.Code.Core;
using osu_StreamCompanion.Code.Misc;

namespace osu_StreamCompanion.Code.Modules.MapDataParsers.Parser1
{
    public partial class ParserSettings : UserControl
    {
        private readonly Settings _settings;
        private BindingList<OutputPattern> _patterns;
        private readonly SettingNames _names = SettingNames.Instance;

        public ParserSettings(Settings settings)
        {
            _settings = settings;
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

        private void AddPattern(object sender, EventArgs eventArgs)
        {
            var baseName = "name_";
            string name;
            int i = 0;
            do
            {
                name = baseName + i++;
            } while (_patterns.Any(p => p.Name == name));
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
    }
}
