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
        private readonly OverlayConfiguration _configuration;
        public EventHandler OnSettingUpdated;
        private bool _init = true;
        public BrowserOverlaySettings(OverlayConfiguration configuration)
        {
            _configuration = configuration;
            InitializeComponent();

            numericUpDown_CanvasHeight.Value = configuration.Canvas.Height;
            numericUpDown_CanvasWidth.Value = configuration.Canvas.Width;
            numericUpDown_positionX.Value = configuration.Position.X;
            numericUpDown_positionY.Value = configuration.Position.Y;
            textBox_overlayUrl.Text = configuration.Url;
            numericUpDown_scale.Value = configuration.Scale;

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
            _configuration.Canvas.Height = Convert.ToInt32(numericUpDown_CanvasHeight.Value);
            _configuration.Canvas.Width = Convert.ToInt32(numericUpDown_CanvasWidth.Value);
            _configuration.Position.X = Convert.ToInt32(numericUpDown_positionX.Value);
            _configuration.Position.Y = Convert.ToInt32(numericUpDown_positionY.Value);
            _configuration.Url = textBox_overlayUrl.Text;
            _configuration.Scale = numericUpDown_scale.Value;
            OnSettingUpdated?.Invoke(null, EventArgs.Empty);
        }
    }
}
