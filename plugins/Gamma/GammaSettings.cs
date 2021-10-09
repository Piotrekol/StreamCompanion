using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Gamma.Models;

namespace Gamma
{
    public partial class GammaSettings : UserControl
    {
        private readonly Configuration _configuration;

        public EventHandler OnSettingUpdated;
        private bool _init = true;
        private BindingList<GammaRange> _gammaRangesBindingList;

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
            _gammaRangesBindingList.AddingNew += BindingListOnAddingNew;
            dataGridView.DataSource = _gammaRangesBindingList;
            dataGridView.DataBindingComplete += DataGridViewOnDataBindingComplete;

            checkBox_enabled.Checked = _configuration.Enabled;

            SetEnabled();
            textBox_description.Text = string.Join(Environment.NewLine,
                "MinAr - minimum AR to trigger gamma change (0,01-15,00)",
                "MaxAr - maximum AR to trigger gamma change (0,01-15,00)",
                "Gamma - gamma value to apply (0,228 ~ 4,46)",
                "0,228 being really bright and 5 really dark.",
                "Gamma activates only when playing and both MinAr and MaxAr conditions are satisfied"
                );
            _init = false;
        }

        private void BindingListOnAddingNew(object sender, AddingNewEventArgs e)
        {
            if (dataGridView.Rows.Count == _gammaRangesBindingList.Count)
            {
                _gammaRangesBindingList.RemoveAt(_gammaRangesBindingList.Count - 1);
            }
        }

        private void DataGridViewOnDataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            Validate("MinAr", 0, 15);
            Validate("MaxAr", 0, 15);
            Validate("Gamma", 0.228, 4.46);
            OnSettingUpdated?.Invoke(this, EventArgs.Empty);
        }

        private void Validate(string columnName, double min, double max)
        {
            for (int i = 0; i < dataGridView.Rows.Count; i++)
            {
                var cell = dataGridView.Rows[i].Cells[columnName];
                double value = Convert.ToDouble(cell.Value);
                if (value < min)
                    cell.Value = min;
                if (value > max)
                    cell.Value = max;
            }
        }

        private void checkBox_enabled_CheckedChanged(object sender, EventArgs e)
        {
            if (_init)
                return;

            SetEnabled();
            OnSettingUpdated?.Invoke(this, EventArgs.Empty);
        }

        private void comboBox_display_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_init)
                return;

            _configuration.ScreenDeviceName = ((ScreenValue)comboBox_display.SelectedItem).Screen.DeviceName;
            OnSettingUpdated?.Invoke(this, EventArgs.Empty);
        }

        private void SetEnabled()
        {
            dataGridView.Enabled = comboBox_display.Enabled = _configuration.Enabled = checkBox_enabled.Checked;
        }
    }
}
