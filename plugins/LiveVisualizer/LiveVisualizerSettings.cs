using System;
using System.Windows.Forms;
using StreamCompanionTypes.Interfaces.Services;
using DColor = System.Drawing.Color;
using MColor = System.Windows.Media.Color;

namespace LiveVisualizer
{
    public partial class LiveVisualizerSettings : UserControl
    {
        private readonly ISettings _settings;
        private readonly IVisualizerConfiguration _configuration;
        public EventHandler ResetSettings;
        public EventHandler OpenFilesFolder;
        public EventHandler OpenWebUrl;

        public string WebUrl { set => label_webUrl.Text = value; }
        public string FilesLocation { set => label_localFiles.Text = value; }

        public LiveVisualizerSettings(ISettings settings, IVisualizerConfiguration configuration)
        {
            _settings = settings;
            _configuration = configuration;

            InitializeComponent();

            checkBox_simulatePP.Checked = _configuration.SimulatePPWhenListening;
            checkBox_hideDiffText.Checked = _configuration.HideDiffText;
            checkBox_hideMapStats.Checked = _configuration.HideMapStats;

            BindColorPicker(color_chartPrimary, () => _configuration.ChartColor, color => _configuration.ChartColor = color);
            BindColorPicker(color_chartProgress, () => _configuration.ChartProgressColor, color => _configuration.ChartProgressColor = color);
            BindColorPicker(color_imageDimming, () => _configuration.ImageDimColor, color => _configuration.ImageDimColor = color);
            BindColorPicker(color_ppBackground, () => _configuration.PpBackgroundColor, color => _configuration.PpBackgroundColor = color);
            BindColorPicker(color_hit100Background, () => _configuration.Hit100BackgroundColor, color => _configuration.Hit100BackgroundColor = color);
            BindColorPicker(color_hit50Background, () => _configuration.Hit50BackgroundColor, color => _configuration.Hit50BackgroundColor = color);
            BindColorPicker(color_hitMissBackground, () => _configuration.HitMissBackgroundColor, color => _configuration.HitMissBackgroundColor = color);

            checkBox_hideDiffText.CheckedChanged += CheckBox_hideDiffTextOnCheckedChanged;
            checkBox_hideMapStats.CheckedChanged += CheckBox_hideMapStatsOnCheckedChanged;
            checkBox_simulatePP.CheckedChanged += CheckBoxSimulatePpOnCheckedChanged;

            numericUpDown_chartHeight.Value = ((decimal)_configuration.ChartHeight).Clamp(numericUpDown_chartHeight.Minimum, numericUpDown_chartHeight.Maximum);
            numericUpDown_chartHeight.ValueChanged += NumericUpDownChartHeightOnValueChanged;
        }

        private void CheckBox_hideMapStatsOnCheckedChanged(object sender, EventArgs e)
        {
            _configuration.HideMapStats = checkBox_hideMapStats.Checked;
        }

        private void CheckBox_hideDiffTextOnCheckedChanged(object sender, EventArgs e)
        {
            _configuration.HideDiffText = checkBox_hideDiffText.Checked;
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

        private void Button_miniCounter_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("This will modify your existing Live Visualizer settings, are you sure?",
                    "Live Visualizer mini counter mode", MessageBoxButtons.YesNo, MessageBoxIcon.Question) !=
                DialogResult.Yes)
            {
                return;
            }

            numericUpDown_chartHeight.Value = numericUpDown_chartHeight.Minimum;
            checkBox_hideMapStats.Checked = true;
            checkBox_hideDiffText.Checked = true;
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

        private void button_openInBrowser_Click(object sender, EventArgs e)
        {
            OpenWebUrl?.Invoke(this, EventArgs.Empty);
        }

        private void button_openFilesLocation_Click(object sender, EventArgs e)
        {
            OpenFilesFolder?.Invoke(this, EventArgs.Empty);
        }
    }
}
