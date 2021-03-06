using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using StreamCompanionTypes.Interfaces.Services;

namespace BrowserOverlay
{
    public partial class BrowserOverlaySettings : UserControl
    {
        private readonly Configuration _configuration;
        public EventHandler OnSettingUpdated;
        private bool _init = true;
        public BrowserOverlaySettings(Configuration configuration)
        {
            _configuration = configuration;
            var overlayConfiguration = configuration.OverlayConfiguration;
            InitializeComponent();

            numericUpDown_CanvasHeight.Value = overlayConfiguration.Canvas.Height;
            numericUpDown_CanvasWidth.Value = overlayConfiguration.Canvas.Width;
            numericUpDown_positionX.Value = overlayConfiguration.Position.X;
            numericUpDown_positionY.Value = overlayConfiguration.Position.Y;
            textBox_overlayUrl.Text = overlayConfiguration.Url;
            numericUpDown_scale.Value = overlayConfiguration.Scale;

            _init = false;
        }

        private void textBox_overlayUrl_TextChanged(object sender, EventArgs e)
        {
            SettingValueUpdated();
        }

        private void numericUpDown_ValueChanged(object sender, EventArgs e)
        {
            SettingValueUpdated();
        }

        private void SettingValueUpdated()
        {
            if (_init)
                return;
            _configuration.OverlayConfiguration.Canvas.Height = Convert.ToInt32(numericUpDown_CanvasHeight.Value);
            _configuration.OverlayConfiguration.Canvas.Width = Convert.ToInt32(numericUpDown_CanvasWidth.Value);
            _configuration.OverlayConfiguration.Position.X = Convert.ToInt32(numericUpDown_positionX.Value);
            _configuration.OverlayConfiguration.Position.Y = Convert.ToInt32(numericUpDown_positionY.Value);
            _configuration.OverlayConfiguration.Url = textBox_overlayUrl.Text;
            _configuration.OverlayConfiguration.Scale = numericUpDown_scale.Value;
            OnSettingUpdated?.Invoke(this, EventArgs.Empty);
        }

        private void checkBox_enable_CheckedChanged(object sender, EventArgs e)
        {
            if (_init)
                return;

            _configuration.Enabled = checkBox_enable.Checked;
            OnSettingUpdated?.Invoke(this, EventArgs.Empty);
        }
    }
}
