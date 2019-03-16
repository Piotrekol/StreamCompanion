using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Text;
using System.Linq;
using System.Linq.Expressions;
using System.Windows.Forms;
using MColor = System.Windows.Media.Color;
using DColor = System.Drawing.Color;

namespace LiveVisualizer
{
    public partial class LiveVisualizerSettings : UserControl
    {
        private readonly ISettingsHandler _settings;
        private readonly IVisualizerConfiguration _configuration;

        public LiveVisualizerSettings(ISettingsHandler settings, IVisualizerConfiguration configuration)
        {
            _settings = settings;
            _configuration = configuration;

            InitializeComponent();


            using (InstalledFontCollection fontsCollection = new InstalledFontCollection())
            {
                var fontNames = fontsCollection.Families.Select(f => f.Name).ToList();
                comboBox_font.DataSource = fontNames;

                var desiredFont = _configuration.Font;

                var font = fontNames.Contains(desiredFont)
                    ? desiredFont
                    : "Arial";

                comboBox_font.SelectedItem = font;
            }


            checkBox_enable.Checked = _configuration.Enable;
            panel1.Enabled = checkBox_enable.Checked;

            checkBox_autosizeChart.Checked = _configuration.AutoSizeAxisY;
            panel_manualChart.Enabled = !checkBox_autosizeChart.Checked;

            BindColorPicker(color_chartPrimary, () => _configuration.ChartColor, color => _configuration.ChartColor = color);
            BindColorPicker(color_chartProgress, () => _configuration.ChartProgressColor, color => _configuration.ChartProgressColor = color);
            BindColorPicker(color_horizontalLegend, () => _configuration.AxisYSeparatorColor, color => _configuration.AxisYSeparatorColor = color);
            BindColorPicker(color_background, () => _configuration.BackgroundColor, color => _configuration.BackgroundColor = color);
            BindColorPicker(color_imageDimming, () => _configuration.ImageDimColor, color => _configuration.ImageDimColor = color);

            textBox_chartCutoffs.Text = string.Join(";", _configuration.ChartCutoffsSet);

            checkBox_showAxisYSeparator.Checked = _configuration.ShowAxisYSeparator;

            numericUpDown_windowHeight.Value = (decimal)_configuration.WindowHeight;
            numericUpDown_windowWidth.Value = (decimal) _configuration.WindowWidth;
            checkBox_enableWindowRezising.Checked = _configuration.EnableResizing;



            checkBox_enable.CheckedChanged += CheckBoxEnableOnCheckedChanged;
            checkBox_autosizeChart.CheckedChanged += checkBox_autosizeChart_CheckedChanged;
            comboBox_font.SelectedValueChanged += ComboBoxFontOnSelectedValueChanged;
            textBox_chartCutoffs.TextChanged += textBox_chartCutoffs_TextChanged;
            checkBox_showAxisYSeparator.CheckedChanged += checkBox_showAxisYSeparator_CheckedChanged;

            numericUpDown_windowHeight.ValueChanged += NumericUpDownWindowHeightOnValueChanged;
            numericUpDown_windowWidth.ValueChanged += NumericUpDownWindowWidthOnValueChanged;
            checkBox_enableWindowRezising.CheckedChanged += CheckBoxEnableWindowRezisingOnCheckedChanged;

        }

        private void BindColorPicker(ColorPickerWithPreview cp, Func<MColor> getter, Action<MColor> setter)
        {
            cp.Color = DColor.FromArgb(getter().A, getter().R, getter().G, getter().B);

            cp.ColorChanged += (sender, color) => setter(MColor.FromArgb(color.A, color.R, color.G, color.B));
        }
        private void CheckBoxEnableWindowRezisingOnCheckedChanged(object sender, EventArgs e)
        {
            _configuration.EnableResizing = checkBox_enableWindowRezising.Checked;
        }

        private void NumericUpDownWindowWidthOnValueChanged(object sender, EventArgs e)
        {
            _configuration.WindowWidth = (double)numericUpDown_windowWidth.Value;
        }

        private void NumericUpDownWindowHeightOnValueChanged(object sender, EventArgs e)
        {
            _configuration.WindowHeight = (double)numericUpDown_windowHeight.Value;
        }

        private void ComboBoxFontOnSelectedValueChanged(object sender, EventArgs e)
        {
            _configuration.Font = (string)comboBox_font.SelectedValue;
        }

        private void CheckBoxEnableOnCheckedChanged(object sender, EventArgs e)
        {
            var enabled = checkBox_enable.Checked;

            _configuration.Enable = enabled;

            panel1.Enabled = enabled;
        }

        private void checkBox_autosizeChart_CheckedChanged(object sender, EventArgs e)
        {
            var enabled = checkBox_autosizeChart.Checked;

            _configuration.AutoSizeAxisY = enabled;

            panel_manualChart.Enabled = !enabled;
        }

        private void textBox_chartCutoffs_TextChanged(object sender, EventArgs e)
        {
            _configuration.ChartCutoffsSet = new SortedSet<int>(textBox_chartCutoffs.Text.Split(';').Select(v => int.TryParse(v, out int num) ? num : 0));
        }

        private void checkBox_showAxisYSeparator_CheckedChanged(object sender, EventArgs e)
        {
            _configuration.ShowAxisYSeparator = checkBox_showAxisYSeparator.Checked;
        }

        private void linkLabel_UICredit1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://osu.ppy.sh/users/9173653");
        }

        private void linkLabel_UICredit2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://osu.ppy.sh/users/4944211");
        }

    }
}
