using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Interfaces;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Windows.Forms;

namespace LiveVisualizer
{
    public partial class LiveVisualizerSettings : UserControl
    {
        private readonly ISettingsHandler _settings;

        public LiveVisualizerSettings(ISettingsHandler settings)
        {
            _settings = settings;

            InitializeComponent();


            using (InstalledFontCollection fontsCollection = new InstalledFontCollection())
            {
                var fontNames = fontsCollection.Families.Select(f => f.Name).ToList();
                comboBox_font.DataSource = fontNames;

                var desiredFont = _settings.Get<string>(ConfigEntrys.Font);

                var font = fontNames.Contains(desiredFont)
                    ? desiredFont
                    : "Arial";

                comboBox_font.SelectedItem = font;
            }


            checkBox_enable.Checked = _settings.Get<bool>(ConfigEntrys.Enable);
            panel1.Enabled = checkBox_enable.Checked;

            checkBox_autosizeChart.Checked = _settings.Get<bool>(ConfigEntrys.AutoSizeAxisY);
            panel_manualChart.Enabled = !checkBox_autosizeChart.Checked;

            color_chartPrimary.Color = ColorHelpers.GetColor(_settings, ConfigEntrys.ChartColor);
            color_chartProgress.Color = ColorHelpers.GetColor(_settings, ConfigEntrys.ChartProgressColor);
            color_horizontalLegend.Color = ColorHelpers.GetColor(_settings, ConfigEntrys.AxisYSeparatorColor);

            textBox_chartCutoffs.Text = _settings.Get<string>(ConfigEntrys.ManualAxisCutoffs);

            checkBox_showAxisYSeparator.Checked = _settings.Get<bool>(ConfigEntrys.ShowAxisYSeparator);

            numericUpDown_windowHeight.Value = (decimal)_settings.Get<double>(ConfigEntrys.WindowHeight);
            numericUpDown_windowWidth.Value = (decimal)_settings.Get<double>(ConfigEntrys.WindowWidth);
            checkBox_enableWindowRezising.Checked = _settings.Get<bool>(ConfigEntrys.EnableResizing);



            color_chartPrimary.ColorChanged += Color_chartPrimary_ColorChanged;
            color_chartProgress.ColorChanged += ColorChartProgressOnColorChanged;
            color_horizontalLegend.ColorChanged += ColorHorizontalLegendOnColorChanged;

            checkBox_enable.CheckedChanged += CheckBoxEnableOnCheckedChanged;
            checkBox_autosizeChart.CheckedChanged += checkBox_autosizeChart_CheckedChanged;
            comboBox_font.SelectedValueChanged += ComboBoxFontOnSelectedValueChanged;
            textBox_chartCutoffs.TextChanged += textBox_chartCutoffs_TextChanged;
            checkBox_showAxisYSeparator.CheckedChanged += checkBox_showAxisYSeparator_CheckedChanged;

            numericUpDown_windowHeight.ValueChanged+=NumericUpDownWindowHeightOnValueChanged;
            numericUpDown_windowWidth.ValueChanged+=NumericUpDownWindowWidthOnValueChanged;
            checkBox_enableWindowRezising.CheckedChanged+=CheckBoxEnableWindowRezisingOnCheckedChanged;

        }

        private void CheckBoxEnableWindowRezisingOnCheckedChanged(object sender, EventArgs e)
        {
            _settings.Add(ConfigEntrys.EnableResizing.Name, checkBox_enableWindowRezising.Checked, true);
        }

        private void NumericUpDownWindowWidthOnValueChanged(object sender, EventArgs e)
        {
            _settings.Add(ConfigEntrys.WindowWidth.Name, (double)numericUpDown_windowWidth.Value, true);
        }

        private void NumericUpDownWindowHeightOnValueChanged(object sender, EventArgs e)
        {
            _settings.Add(ConfigEntrys.WindowHeight.Name, (double)numericUpDown_windowHeight.Value, true);
        }

        private void ColorHorizontalLegendOnColorChanged(object sender, Color e)
        {
            ColorHelpers.SaveColor(_settings, ConfigEntrys.AxisYSeparatorColor, e);
        }

        private void ComboBoxFontOnSelectedValueChanged(object sender, EventArgs e)
        {
            _settings.Add(ConfigEntrys.Font.Name, (string)comboBox_font.SelectedValue, true);
        }

        private void ColorChartProgressOnColorChanged(object sender, Color e)
        {
            ColorHelpers.SaveColor(_settings, ConfigEntrys.ChartProgressColor, e);
        }

        private void Color_chartPrimary_ColorChanged(object sender, Color e)
        {
            ColorHelpers.SaveColor(_settings, ConfigEntrys.ChartColor, e);
        }

        private void CheckBoxEnableOnCheckedChanged(object sender, EventArgs e)
        {
            var enabled = checkBox_enable.Checked;
            _settings.Add(ConfigEntrys.Enable.Name, enabled, true);

            panel1.Enabled = enabled;
        }

        private void checkBox_autosizeChart_CheckedChanged(object sender, EventArgs e)
        {
            var enabled = checkBox_autosizeChart.Checked;
            _settings.Add(ConfigEntrys.AutoSizeAxisY.Name, enabled, true);

            panel_manualChart.Enabled = !enabled;
        }

        private void textBox_chartCutoffs_TextChanged(object sender, EventArgs e)
        {
            _settings.Add(ConfigEntrys.ManualAxisCutoffs.Name, textBox_chartCutoffs.Text, true);
        }

        private void checkBox_showAxisYSeparator_CheckedChanged(object sender, EventArgs e)
        {
            _settings.Add(ConfigEntrys.ShowAxisYSeparator.Name, checkBox_showAxisYSeparator.Checked, true);
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
