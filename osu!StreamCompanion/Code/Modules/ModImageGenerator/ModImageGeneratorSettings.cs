using System;
using System.Drawing;
using System.Windows.Forms;
using osu_StreamCompanion.Code.Core;

namespace osu_StreamCompanion.Code.Modules.ModImageGenerator
{
    public partial class ModImageGeneratorSettings : UserControl
    {
        private Settings _settings;
        private bool init = true;
        private Func<string[], Bitmap> _generateImage;

        public ModImageGeneratorSettings(Settings settings, Func<string[], Bitmap> generateImage)
        {
            _generateImage = generateImage;
            _settings = settings;
            InitializeComponent();
            checkBox_enable.Checked = _settings.Get("EnableModImages", true); //checkBox
            nUd_ImageWidth.Value = _settings.Get("ImageWidth", 720); //numericUpDown 1-10000
            nUd_ModImageHeight.Value = _settings.Get("ModHeight", 64); //numericUpDown 1-10000
            nUd_ModImageWidth.Value = _settings.Get("ModWidth", 64); //numericUpDown 1-10000
            nUd_Spacing.Value = _settings.Get("ModImageSpacing", -25); //numericUpDown -10000 - +10000
            nUd_Opacity.Value = _settings.Get("ModImageOpacity", 85); //numericUpDown 0-100
            radioButton_DrawOnRight.Checked = _settings.Get("DrawOnRightSide", false);// radioButton x2
            radioButton_DrawOnLeft.Checked = !radioButton_DrawOnRight.Checked;

            radioButton_DrawFromRightToLeft.Checked = _settings.Get("DrawFromRightToLeft", false);// radioButton x2
            radioButton_DrawFromLeftToRight.Checked = !radioButton_DrawFromRightToLeft.Checked;
            textBox_PreviewMods.Text = "NF,HR,HD,DT,FL,K4,RX"; 
            CreatePreview();
            init = false;
        }

        private void nUd_ValueChanged(object sender, EventArgs e)
        {
            if (init) return;
            if (sender == nUd_ImageWidth)
            {
                _settings.Add("ImageWidth", Convert.ToInt32(nUd_ImageWidth.Value));
            }
            else if (sender == nUd_ModImageWidth)
            {
                _settings.Add("ModWidth", Convert.ToInt32(nUd_ModImageWidth.Value));
            }
            else if (sender == nUd_ModImageHeight)
            {
                _settings.Add("ModHeight", Convert.ToInt32(nUd_ModImageHeight.Value));
            }
            else if (sender == nUd_Spacing)
            {
                _settings.Add("ModImageSpacing", Convert.ToInt32(nUd_Spacing.Value));
            }
            else if (sender == nUd_Opacity)
            {
                _settings.Add("ModImageOpacity", Convert.ToInt32(nUd_Opacity.Value));
            }
            CreatePreview();
        }

        private void checkBox_enable_CheckedChanged(object sender, EventArgs e)
        {
            
            _settings.Add("EnableModImages", checkBox_enable.Checked);
            panel_enabled.Enabled = checkBox_enable.Checked;
        }
        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (init) return;
            if (((RadioButton)sender).Checked)
            {
                _settings.Add("DrawOnRightSide", radioButton_DrawOnRight.Checked);
                _settings.Add("DrawFromRightToLeft", radioButton_DrawFromRightToLeft.Checked);
                CreatePreview();
            }
        }

        private void CreatePreview()
        {
            this.pictureBox_preview.Image?.Dispose();
            this.pictureBox_preview.Image = _generateImage(textBox_PreviewMods.Text.Split(','));
        }

        private void textBox_TextChanged(object sender, EventArgs e)
        {
            CreatePreview();
        }
    }
}
