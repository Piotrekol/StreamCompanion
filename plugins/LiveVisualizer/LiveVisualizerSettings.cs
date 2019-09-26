using StreamCompanionTypes.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Text;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using DColor = System.Drawing.Color;
using MColor = System.Windows.Media.Color;

namespace LiveVisualizer
{
    public partial class LiveVisualizerSettings : UserControl
    {
        private readonly ISettingsHandler _settings;
        private readonly IVisualizerConfiguration _configuration;
        public EventHandler ResetSettings;

        public LiveVisualizerSettings(ISettingsHandler settings, IVisualizerConfiguration configuration)
        {
            _settings = settings;
            _configuration = configuration;

            InitializeComponent();

            label_oneScreenWarning.Visible = Screen.AllScreens.Length <= 1;

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
            checkBox_simulatePP.Checked = _configuration.SimulatePPWhenListening;

            checkBox_autosizeChart.Checked = _configuration.AutoSizeAxisY;
            checkBox_enableRoundedCorners.Checked = _configuration.EnableRoundedCorners;
            panel_manualChart.Enabled = !checkBox_autosizeChart.Checked;

            BindColorPicker(color_chartPrimary, () => _configuration.ChartColor, color => _configuration.ChartColor = color);
            BindColorPicker(color_chartProgress, () => _configuration.ChartProgressColor, color => _configuration.ChartProgressColor = color);
            BindColorPicker(color_horizontalLegend, () => _configuration.AxisYSeparatorColor, color => _configuration.AxisYSeparatorColor = color);
            BindColorPicker(color_background, () => _configuration.BackgroundColor, color => _configuration.BackgroundColor = color);
            BindColorPicker(color_imageDimming, () => _configuration.ImageDimColor, color => _configuration.ImageDimColor = color);

            BindColorPicker(color_textArtist, () => _configuration.ArtistTextColor, color => _configuration.ArtistTextColor = color);
            BindColorPicker(color_textTitle, () => _configuration.TitleTextColor, color => _configuration.TitleTextColor = color);

            BindColorPicker(color_ppBackground, () => _configuration.PpBackgroundColor, color => _configuration.PpBackgroundColor = color);
            BindColorPicker(color_hit100Background, () => _configuration.Hit100BackgroundColor, color => _configuration.Hit100BackgroundColor = color);
            BindColorPicker(color_hit50Background, () => _configuration.Hit50BackgroundColor, color => _configuration.Hit50BackgroundColor = color);
            BindColorPicker(color_hitMissBackground, () => _configuration.HitMissBackgroundColor, color => _configuration.HitMissBackgroundColor = color);


            textBox_chartCutoffs.Text = string.Join(";", _configuration.ChartCutoffsSet);

            checkBox_showAxisYSeparator.Checked = _configuration.ShowAxisYSeparator;

            numericUpDown_windowHeight.Value = ((decimal)_configuration.WindowHeight).Clamp(numericUpDown_windowHeight.Minimum, numericUpDown_windowHeight.Maximum);
            numericUpDown_windowWidth.Value = ((decimal)_configuration.WindowWidth).Clamp(numericUpDown_windowWidth.Minimum, numericUpDown_windowWidth.Maximum);
            numericUpDown_chartHeight.Value = ((decimal)_configuration.ChartHeight).Clamp(numericUpDown_chartHeight.Minimum, numericUpDown_chartHeight.Maximum);
            numericUpDown_bottomHeight.Value = decimal.Parse(_configuration.BottomHeight.Replace("*", "").Replace(".", ",")).Clamp(numericUpDown_bottomHeight.Minimum, numericUpDown_bottomHeight.Maximum);


            checkBox_enable.CheckedChanged += CheckBoxEnableOnCheckedChanged;
            checkBox_autosizeChart.CheckedChanged += checkBox_autosizeChart_CheckedChanged;
            comboBox_font.SelectedValueChanged += ComboBoxFontOnSelectedValueChanged;
            textBox_chartCutoffs.TextChanged += textBox_chartCutoffs_TextChanged;
            checkBox_showAxisYSeparator.CheckedChanged += checkBox_showAxisYSeparator_CheckedChanged;

            numericUpDown_windowHeight.ValueChanged += NumericUpDownWindowHeightOnValueChanged;
            numericUpDown_windowWidth.ValueChanged += NumericUpDownWindowWidthOnValueChanged;
            numericUpDown_chartHeight.ValueChanged += NumericUpDownChartHeightOnValueChanged;
            numericUpDown_bottomHeight.ValueChanged += NumericUpDownBottomHeightOnValueChanged;

            checkBox_simulatePP.CheckedChanged += CheckBoxSimulatePpOnCheckedChanged;
            checkBox_enableRoundedCorners.CheckedChanged += CheckBoxEnableRoundedCornersOnCheckedChanged;
        }

        private void NumericUpDownBottomHeightOnValueChanged(object sender, EventArgs e)
        {
            _configuration.BottomHeight = numericUpDown_bottomHeight.Value.ToString(CultureInfo.InvariantCulture) + "*";
        }

        private void CheckBoxEnableRoundedCornersOnCheckedChanged(object sender, EventArgs e)
        {
            _configuration.EnableRoundedCorners = checkBox_enableRoundedCorners.Checked;
        }

        private void CheckBoxSimulatePpOnCheckedChanged(object sender, EventArgs e)
        {
            _configuration.SimulatePPWhenListening = checkBox_simulatePP.Checked;
        }

        private void NumericUpDownChartHeightOnValueChanged(object sender, EventArgs e)
        {
            _configuration.ChartHeight = (double)numericUpDown_chartHeight.Value;
        }

        private void BindColorPicker(ColorPickerWithPreview cp, Func<MColor> getter, Action<MColor> setter)
        {
            cp.Color = DColor.FromArgb(getter().A, getter().R, getter().G, getter().B);

            cp.ColorChanged += (sender, color) => setter(MColor.FromArgb(color.A, color.R, color.G, color.B));
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

        private void Button_miniCounter_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("This will modify your existing Live Visualizer settings, are you sure?",
                    "Live Visualizer mini counter mode", MessageBoxButtons.YesNo, MessageBoxIcon.Question) !=
                DialogResult.Yes)
            {
                return;
            }

            numericUpDown_windowWidth.Value = numericUpDown_windowWidth.Minimum;
            numericUpDown_windowHeight.Value = numericUpDown_windowHeight.Minimum;
            numericUpDown_chartHeight.Value = numericUpDown_chartHeight.Minimum;
            checkBox_enableRoundedCorners.Checked = false;
            numericUpDown_bottomHeight.Value = 100;
        }

        private void Button_reset_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("This will reset all Live Visualizer settings, are you sure?",
                    "Live Visualizer settings reset", MessageBoxButtons.YesNo, MessageBoxIcon.Question) ==
                DialogResult.Yes)
            {
                ResetSettings?.Invoke(this, EventArgs.Empty);
                if (Parent?.Parent?.Parent is Form frm)
                {
                    frm.Close();
                }
            }
        }
    }
}
