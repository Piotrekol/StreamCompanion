using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using osu_StreamCompanion.Code.Core.DataTypes;
using osu_StreamCompanion.Code.Helpers;

namespace osu_StreamCompanion.Code.Modules.MapDataParsers.Parser1
{
    public partial class PatternEdit : UserControl
    {
        private OutputPattern _current;
        public Dictionary<string, OsuStatus> SaveEvents = new Dictionary<string, OsuStatus>
        {
            {"All",OsuStatus.All },
            {"Listening",OsuStatus.Listening },
            {"Playing",OsuStatus.Playing },
            {"Watching",OsuStatus.Watching },
            {"Editing",OsuStatus.Editing },
            {"Never",OsuStatus.Null }
        };
        public OutputPattern Current
        {
            get { return _current; }
            set
            {
                if (!this.IsHandleCreated || this.IsDisposed)
                    return;
                this.BeginInvoke((MethodInvoker)(() =>
                {
                    _current = value;
                    if (value != null)
                        comboBox_saveEvent.SelectedItem = SaveEvents.First(s => s.Value == value.SaveEvent).Key;
                    textBox_formating.Text = value?.Pattern ?? "";
                    textBox_FileName.Text = value?.Name ?? "";

                }));
            }
        }

        public EventHandler<OutputPattern> DeletePattern;
        public EventHandler AddPattern;
        private Dictionary<string, string> _replacements;
        private Dictionary<string, string> _liveReplacements = new Dictionary<string, string>
        {
            { "!acc!", "98,05%" },
            { "!300!", "301" },
            { "!100!", "101" },
            { "!50!", "51" },
            { "!miss!", "15" },
            { "!time!", "116,5" },
            { "!combo!", "124" },
            { "!CurrentMaxCombo!", "1000" },
            { "!PpIfMapEndsNow!", "99,52pp" },
            { "!PpIfRestFced!", "257,27pp" },
            { "!AccIfRestFced!", "99,54%" }
        };

        public PatternEdit()
        {
            InitializeComponent();
            comboBox_saveEvent.DataSource = SaveEvents.Select(v => v.Key).ToList();

        }

        private void Save()
        {
            if (Current != null)
            {
                Current.Name = textBox_FileName.Text;
                Current.Pattern = textBox_formating.Text;
                Current.SaveEvent = SaveEvents.First(s => s.Key == (string)comboBox_saveEvent.SelectedItem).Value;
            }
        }

        private void button_Click(object sender, EventArgs e)
        {
            if (sender == button_save)
            {
                Save();
            }
            else if (sender == button_addNew)
            {
                AddPattern?.Invoke(this, EventArgs.Empty);
            }
            else if (sender == button_delete)
            {
                DeletePattern?.Invoke(this, Current);
            }
        }

        private void textBox_FileName_KeyPress(object sender, KeyPressEventArgs e)
        {
            foreach (var c in Path.GetInvalidFileNameChars())
            {
                textBox_FileName.Text = textBox_FileName.Text.Replace(c.ToString(), "");
            }
        }

        public void SetPreview(Dictionary<string, string> replacements)
        {
            _replacements = replacements;
        }
        private void textBox_formating_TextChanged(object sender, EventArgs e)
        {
            if (!this.IsHandleCreated || this.IsDisposed || Current == null)
                return;
            var isMemoryPattern = Current.MemoryFormatTokens.Any(textBox_formating.Text.Contains);
            label_warning.Visible = isMemoryPattern;
            comboBox_saveEvent.SelectedItem = isMemoryPattern ? "Playing" : comboBox_saveEvent.SelectedItem;
            comboBox_saveEvent.Enabled = !isMemoryPattern;


            if (_replacements == null)
                textBox_preview.Text = "Change map in osu! to see preview";
            else
            {
                var toFormat = textBox_formating.Text;
                foreach (var r in _replacements)
                {
                    toFormat = toFormat.Replace(r.Key, r.Value, StringComparison.InvariantCultureIgnoreCase);
                }
                foreach (var r in _liveReplacements)
                {
                    toFormat = toFormat.Replace(r.Key, r.Value, StringComparison.InvariantCultureIgnoreCase);
                }
                textBox_preview.Text = toFormat;
            }
        }
    }
}
