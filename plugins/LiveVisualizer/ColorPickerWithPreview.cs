using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LiveVisualizer
{
    public partial class ColorPickerWithPreview : UserControl
    {
        public event EventHandler<Color> ColorChanged;
        public Color Color
        {
            get => panel_colorPreview.BackColor;
            set
            {
                panel_colorPreview.BackColor = value;
                numericUpDown_alpha.Value = value.A;
            }
        }
        [
            Category("Appearance"),
            Browsable(true),
            Description("innerLabel"),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
        ]
        public Label LabelDesigner
        {
            get => Label;
            set => Label = value;
        }


        public ColorPickerWithPreview()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            colorDialog.AllowFullOpen = true;
            colorDialog.ShowHelp = true;
            colorDialog.Color = Color;
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                colorDialog.Color = Color.FromArgb((int)numericUpDown_alpha.Value, colorDialog.Color);
                panel_colorPreview.BackColor = colorDialog.Color;
                ColorChanged?.Invoke(this, colorDialog.Color);
            }
        }

        private void panel_ColorPreview_Click(object sender, EventArgs e)
        {
            button_Click(sender, null);
        }

        private void numericUpDown_alpha_ValueChanged(object sender, EventArgs e)
        {
            Color = Color.FromArgb((int)numericUpDown_alpha.Value, Color);
            ColorChanged?.Invoke(this, Color);
        }
    }
}
