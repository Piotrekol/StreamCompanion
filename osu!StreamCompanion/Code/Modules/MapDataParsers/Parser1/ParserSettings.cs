using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace osu_StreamCompanion.Code.Modules.MapDataParsers.Parser1
{
    public partial class ParserSettings : UserControl
    {
        private BindingList<OutputPattern> _patterns;

        public ParserSettings()
        {
            InitializeComponent();
            this.patternList.SelectedItemChanged+=SelectedItemChanged;

            this.patternEdit.DeletePattern+=DeletePattern;
            this.patternEdit.AddPattern+=AddPattern;
        }

        private void AddPattern(object sender, EventArgs eventArgs)
        {
            var baseName = "name_";
            string name;
            int i=0;
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
