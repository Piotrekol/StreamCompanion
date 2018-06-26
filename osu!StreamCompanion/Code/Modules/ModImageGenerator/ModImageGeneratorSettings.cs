using System;
using System.Drawing;
using System.Windows.Forms;
using osu_StreamCompanion.Code.Misc;
using StreamCompanionTypes;
using StreamCompanionTypes.Interfaces;

namespace osu_StreamCompanion.Code.Modules.ModImageGenerator
{
    public partial class ModImageGeneratorSettings : UserControl
    {
        private ISettingsHandler _settings;
        private bool init = true;
        private Func<string[], Bitmap> _generateImage;
        private readonly SettingNames _names = SettingNames.Instance;
        public ModImageGeneratorSettings(ISettingsHandler settings, Func<string[], Bitmap> generateImage)
        {
            _generateImage = generateImage;
            _settings = settings;
            InitializeComponent();
            checkBox_enable.Checked = _settings.Get<bool>(_names.EnableModImages); //checkBox
            nUd_ImageWidth.Value = _settings.Get<int>(_names.ImageWidth); //numericUpDown 1-10000
            nUd_ModImageHeight.Value = _settings.Get<int>(_names.ModHeight); //numericUpDown 1-10000
            nUd_ModImageWidth.Value = _settings.Get<int>(_names.ModWidth); //numericUpDown 1-10000
            nUd_Spacing.Value = _settings.Get<int>(_names.ModImageSpacing); //numericUpDown -10000 - +10000
            nUd_Opacity.Value = _settings.Get<int>(_names.ModImageOpacity); //numericUpDown 0-100
            radioButton_DrawOnRight.Checked = _settings.Get<bool>(_names.DrawOnRightSide);// radioButton x2
            radioButton_DrawOnLeft.Checked = !radioButton_DrawOnRight.Checked;

            radioButton_DrawFromRightToLeft.Checked = _settings.Get<bool>(_names.DrawFromRightToLeft);// radioButton x2
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
                _settings.Add(_names.ImageWidth.Name, Convert.ToInt32(nUd_ImageWidth.Value));
            }
            else if (sender == nUd_ModImageWidth)
            {
                _settings.Add(_names.ModWidth.Name, Convert.ToInt32(nUd_ModImageWidth.Value));
            }
            else if (sender == nUd_ModImageHeight)
            {
                _settings.Add(_names.ModHeight.Name, Convert.ToInt32(nUd_ModImageHeight.Value));
            }
            else if (sender == nUd_Spacing)
            {
                _settings.Add(_names.ModImageSpacing.Name, Convert.ToInt32(nUd_Spacing.Value));
            }
            else if (sender == nUd_Opacity)
            {
                _settings.Add(_names.ModImageOpacity.Name, Convert.ToInt32(nUd_Opacity.Value));
            }
            CreatePreview();
        }

        private void checkBox_enable_CheckedChanged(object sender, EventArgs e)
        {

            _settings.Add(_names.EnableModImages.Name, checkBox_enable.Checked);
            panel_enabled.Enabled = checkBox_enable.Checked;
        }
        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (init) return;
            if (((RadioButton)sender).Checked)
            {
                _settings.Add(_names.DrawOnRightSide.Name, radioButton_DrawOnRight.Checked);
                _settings.Add(_names.DrawFromRightToLeft.Name, radioButton_DrawFromRightToLeft.Checked);
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
            foreach (var invalidChar in System.IO.Path.GetInvalidFileNameChars())
            {
                textBox_PreviewMods.Text = textBox_PreviewMods.Text.Replace(invalidChar.ToString(), string.Empty);
            }
            CreatePreview();
        }
    }
}
