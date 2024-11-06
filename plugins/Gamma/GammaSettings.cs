using System;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Gamma.Models;

namespace Gamma
{
    public partial class GammaSettings : UserControl
    {
        private readonly Configuration _configuration;

        public EventHandler SettingUpdated;
        private bool _init = true;
        private BindingList<GammaRange> _gammaRangesBindingList;
        private GammaRange _currentGammaRange;
        private Gamma _gamma;

        private class ScreenValue
        {
            public Screen Screen { get; set; }
            public override string ToString() => $"{Screen.DeviceName} ({Screen.Bounds.Width}x{Screen.Bounds.Height})";

            public ScreenValue(Screen screen)
            {
                Screen = screen;
            }
        }

        public GammaSettings(Configuration configuration)
        {
            _configuration = configuration;
            InitializeComponent();
            var screens = Screen.AllScreens.Select(s => new ScreenValue(s)).ToList();
            comboBox_display.DataSource = screens;
            comboBox_display.SelectedItem = screens.FirstOrDefault(s => s.Screen.DeviceName == _configuration.ScreenDeviceName) ?? screens.First();

            _gammaRangesBindingList = new BindingList<GammaRange>(_configuration.GammaRanges);
            listBox_gammaRanges.DataSource = _gammaRangesBindingList;
            checkBox_enabled.Checked = _configuration.Enabled;

            SetEnabled();
            _init = false;
        }

        public void SetMapAR(double ar)
        {
            if (InvokeRequired)
            {
                Invoke(() => SetMapAR(ar));
                return;
            }
            label_mapAR.Text = $"Current map AR: {ar:0.##}";
        }

        private void checkBox_enabled_CheckedChanged(object sender, EventArgs e)
        {
            if (_init)
                return;

            SetEnabled();
            OnSettingUpdated();
        }

        protected virtual void OnSettingUpdated()
        {
            SettingUpdated?.Invoke(this, null);
        }

        private void SetEnabled()
        {
            panel_main.Enabled = comboBox_display.Enabled = _configuration.Enabled = checkBox_enabled.Checked;
        }

        private void SettingValueUpdated()
        {
            if (_init || _currentGammaRange == null)
                return;

            _currentGammaRange.MinAr = Convert.ToDouble(numericUpDown_arMin.Value);
            _currentGammaRange.MaxAr = Convert.ToDouble(numericUpDown_arMax.Value);
            _currentGammaRange.UserGamma = trackBar_gamma.Value;
            _configuration.SortGammaRanges();
            _gammaRangesBindingList.ResetBindings();
            if (listBox_gammaRanges.SelectedItem != _currentGammaRange)
                listBox_gammaRanges.SelectedItem = _currentGammaRange;

            OnSettingUpdated();
        }

        private void PopulateForm(GammaRange gammaRange)
        {
            _init = true;
            numericUpDown_arMin.Value = Convert.ToDecimal(gammaRange.MinAr);
            numericUpDown_arMax.Value = Convert.ToDecimal(gammaRange.MaxAr);
            trackBar_gamma.Value = gammaRange.UserGamma;
            textBox_gamma.Text = gammaRange.UserGamma.ToString();
            _init = false;
        }

        private void comboBox_display_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_init)
                return;

            _configuration.ScreenDeviceName = ((ScreenValue)comboBox_display.SelectedItem).Screen.DeviceName;
            OnSettingUpdated();
        }

        private void listBox_gammaRanges_SelectedIndexChanged(object sender, EventArgs e)
        {
            _currentGammaRange = (GammaRange)listBox_gammaRanges.SelectedItem;
            panel_rangeSettings.Enabled = _currentGammaRange != null;
            if (panel_rangeSettings.Enabled)
                PopulateForm(_currentGammaRange);
        }

        private void button_addRange_Click(object sender, EventArgs e)
        {
            var newRange = _gammaRangesBindingList.AddNew();
            _configuration.SortGammaRanges();
            _gammaRangesBindingList.ResetBindings();
            listBox_gammaRanges.SelectedItem = newRange;
        }

        private void button_remove_Click(object sender, EventArgs e)
        {
            if (listBox_gammaRanges.SelectedItem == null)
                return;

            _gammaRangesBindingList.Remove((GammaRange)listBox_gammaRanges.SelectedItem);
            SettingUpdated?.Invoke(this, null);
        }

        private void trackBar_gamma_MouseDown(object sender, MouseEventArgs e)
        {
            var selectedDisplayName = ((ScreenValue)comboBox_display.SelectedValue).Screen.DeviceName;
            if (_gamma == null || _gamma.ScreenDeviceName != selectedDisplayName)
            {
                _gamma?.Dispose();
                _gamma = new(selectedDisplayName);
            }

            _gamma.Set(trackBar_gamma.Value);
        }

        private void trackBar_gamma_MouseUp(object sender, MouseEventArgs e)
        {
            _gamma?.Dispose();
            _gamma = null;
        }

        private void trackBar_gamma_ValueChanged(object sender, EventArgs e)
        {
            if (_gamma != null)
                _gamma.Set(trackBar_gamma.Value);

            textBox_gamma.Text = trackBar_gamma.Value.ToString();
            SettingValueUpdated();
        }

        private void numericUpDown_arMin_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDown_arMax.Value < numericUpDown_arMin.Value)
                numericUpDown_arMax.Value = numericUpDown_arMin.Value;

            SettingValueUpdated();
        }

        private void numericUpDown_arMax_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDown_arMax.Value < numericUpDown_arMin.Value)
                numericUpDown_arMin.Value = numericUpDown_arMax.Value;

            SettingValueUpdated();
        }
    }
}
