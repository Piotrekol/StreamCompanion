using StreamCompanionTypes.DataTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using StreamCompanion.Common;
using StreamCompanionTypes;
using StreamCompanionTypes.Enums;

namespace osu_StreamCompanion.Code.Modules.MapDataParsers.Parser1
{
    public partial class PatternEdit : UserControl
    {
        private OutputPattern _current;
        private bool _inGameOverlayIsAvailable;

        public bool InGameOverlayIsAvailable
        {
            get => _inGameOverlayIsAvailable;
            set
            {
                _inGameOverlayIsAvailable = value;
                checkBox_ShowIngame.Visible = value;
                panel_showInOsu.Visible = value && checkBox_ShowIngame.Checked;
            }
        }

        public Dictionary<string, OsuStatus> SaveEvents = new Dictionary<string, OsuStatus>
        {
            //TODO: replace this madness with checkboxes to pick individual events
            {"All",OsuStatus.All },
            {"Listening",OsuStatus.Listening },
            {"Playing",OsuStatus.Playing },
            {"Watching",OsuStatus.Watching },
            {"Editing",OsuStatus.Editing },
            {"Results screen",OsuStatus.ResultsScreen },
            {"Playing & Watching",OsuStatus.Playing | OsuStatus.Watching },
            {"Listening & Results screen",OsuStatus.Listening | OsuStatus.ResultsScreen },
            {"Listening & Editing",OsuStatus.Editing | OsuStatus.Listening},
            {"Listening & Editing & Results screen",OsuStatus.Listening | OsuStatus.Editing | OsuStatus.ResultsScreen},
            {"Playing & Results screen",OsuStatus.ResultsScreen | OsuStatus.Playing},
            {"Watching & Results screen",OsuStatus.Watching | OsuStatus.ResultsScreen },
            {"Watching & Playing & Results screen",OsuStatus.Watching | OsuStatus.Playing | OsuStatus.ResultsScreen },
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
                    checkBox_ShowIngame.Checked = value?.ShowInOsu ?? false;
                    if (value?.Color != null)
                    {
                        panel_ColorPreview.BackColor = value.Color;
                        label_TestText.ForeColor = value.Color;
                    }
                    numericUpDown_XPosition.Value = value?.XPosition ?? 200;
                    numericUpDown_YPosition.Value = value?.YPosition ?? 200;
                    if (value?.FontName != null)
                        comboBox_font.SelectedItem = value.FontName;
                    numericUpDown_fontSize.Value = value?.FontSize ?? 12;
                    numericUpDown_colorAlpha.Value = value?.Color.A ?? 255;
                    comboBox_align.SelectedIndex = value?.Aligment ?? 0;
                }));
            }
        }

        public EventHandler<OutputPattern> DeletePattern;
        public EventHandler AddPattern;
        private Tokens _replacements;

        private OsuStatus _currentStatus;

        public PatternEdit()
        {
            InitializeComponent();
            comboBox_saveEvent.DataSource = SaveEvents.Select(v => v.Key).ToList();
            comboBox_align.DataSource = new List<string>
            {
                "To Left",
                "To Center",
                "To Right"
            };
            if (OperatingSystem.IsWindows())
            {
                using (InstalledFontCollection fontsCollection = new InstalledFontCollection())
                {
#pragma warning disable CA1416 // Validate platform compatibility
                    var fontNames = fontsCollection.Families.Select(f => f.Name).ToList();
#pragma warning restore CA1416 // Validate platform compatibility
                    comboBox_font.DataSource = fontNames;
                    comboBox_font.SelectedItem = fontNames.First(f => f == "Arial");
                }
            }
        }

        private void Save()
        {
            if (Current != null)
            {
                if (string.IsNullOrEmpty(textBox_FileName.Text))
                {
                    MessageBox.Show("Pattern name can't be empty. Pattern was not saved.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                Current.Name = textBox_FileName.Text;
                Current.Pattern = textBox_formating.Text;
                Current.SaveEvent = SaveEvents.First(s => s.Key == (string)comboBox_saveEvent.SelectedItem).Value;
                Current.ShowInOsu = checkBox_ShowIngame.Checked;
                Current.XPosition = Convert.ToInt32(numericUpDown_XPosition.Value);
                Current.YPosition = Convert.ToInt32(numericUpDown_YPosition.Value);
                Current.Color = panel_ColorPreview.BackColor;
                Current.FontName = (string)comboBox_font.SelectedItem;
                Current.FontSize = Convert.ToInt32(numericUpDown_fontSize.Value);
                Current.Aligment = comboBox_align.SelectedIndex;
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

            textBox_FileName.Text = textBox_FileName.Text.RemoveWhitespace();
        }

        private bool longFormat = false;
        public async void textBoxUpdateLoop(Tokens replacements)
        {
            var _replacements = replacements;
            try
            {
                while (!IsDisposed && Created && ReferenceEquals(_replacements, replacements))
                {
                    Invoke((MethodInvoker)(() => { textBox_formating_TextChanged(null, EventArgs.Empty); }));
                    await Task.Delay(longFormat ? 1000 : 33);
                }
            }
            catch (ObjectDisposedException) { }
            catch (InvalidAsynchronousStateException) { }
            catch (InvalidOperationException) { }
        }

        private string Sanitize(string inputFormat)
        {
            if (string.IsNullOrEmpty(inputFormat) || inputFormat.Length < 250)
                return inputFormat;

            return $"{inputFormat.Substring(0, 250)}... (preview truncated)";
        }
        public void SetPreview(Tokens replacements, OsuStatus status)
        {
            _replacements = replacements;
            _currentStatus = status;
            if (Created)
            {
                Invoke((MethodInvoker)(() => { textBox_formating_TextChanged(null, EventArgs.Empty); }));
                Task.Run(() => textBoxUpdateLoop(replacements));
            }
        }

        private void textBox_formating_TextChanged(object sender, EventArgs e)
        {
            if (!this.IsHandleCreated || this.IsDisposed || Current == null)
                return;

            var isMemoryPattern = Current.MemoryFormatTokens.Select(s => s.ToLower()).Any(textBox_formating.Text.ToLower().Contains);

            if (_replacements == null)
                textBox_preview.Text = "Change map in osu! to see preview";
            else
            {
                var toFormat = Sanitize(textBox_formating.Text ?? "");
                var saveEvent = SaveEvents.First(s => s.Key == (string)comboBox_saveEvent.SelectedItem).Value;
                OsuStatus currentStatus = _currentStatus;
                if ((_currentStatus & saveEvent) != 0)
                {
                    currentStatus = OsuStatus.All;
                }

                var result = string.Empty;
                if (OutputPattern.CanSave(toFormat, _replacements, saveEvent, currentStatus))
                {
                    var formulas = JaceEngine.Instance.CompileFormulas(toFormat);
                    result = OutputPattern.FormatPattern(toFormat, _replacements, formulas);
                    isMemoryPattern |= formulas.Any(kv => kv.Value.TokensUsed.Any(x => Current.MemoryFormatTokens.Contains($"!{x}!")));
                }
                longFormat = result.Length > 250;
                result = Sanitize(result);

                if (string.IsNullOrEmpty(result) && !string.IsNullOrEmpty(toFormat))
                {
                    label_statusInfo.Visible = true;
                    label_statusInfo.Text = $"This pattern will not save anything with current save event under current osu status ({_currentStatus})";
                }
                else
                {
                    label_statusInfo.Visible = false;
                }

                textBox_preview.Text = result;
            }

            label_warning.Visible = isMemoryPattern;
        }

        private void checkBox_ShowIngame_CheckedChanged(object sender, EventArgs e)
        {
            panel_showInOsu.Visible = checkBox_ShowIngame.Checked;
        }

        private void panel_ColorPreview_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();

            colorDialog.AllowFullOpen = true;
            colorDialog.ShowHelp = true;
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                panel_ColorPreview.BackColor = colorDialog.Color;
                label_TestText.ForeColor = colorDialog.Color;
            }
        }

        private void comboBox_font_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!OperatingSystem.IsWindows())
                return;

            var font = (string)comboBox_font.SelectedItem;
            try
            {
                label_TestText.Font = new Font(font, 10);
            }
            catch (ArgumentException)
            { }
        }

        private void numericUpDown_colorAlpha_ValueChanged(object sender, EventArgs e)
        {
            panel_ColorPreview.BackColor = label_TestText.ForeColor =
                Color.FromArgb((int)numericUpDown_colorAlpha.Value, panel_ColorPreview.BackColor);
        }
    }
}
