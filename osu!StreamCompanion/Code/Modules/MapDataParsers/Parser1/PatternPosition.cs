using System;
using System.Drawing;
using System.Windows.Forms;

namespace osu_StreamCompanion.Code.Modules.MapDataParsers.Parser1
{
    public partial class PatternPosition : UserControl
    {
        public PatternPosition()
        {
            InitializeComponent();
            ui_osuHeight.Value = Screen.PrimaryScreen.Bounds.Height;
            ui_osuWidth.Value = Screen.PrimaryScreen.Bounds.Width;
        }


        public int X =>Convert.ToInt32(ui_textX.Value);
        public int Y => Convert.ToInt32(ui_textY.Value);
        private float XScaleFactor => Convert.ToInt32(ui_osuWidth.Value) / panel_OsuScreen.Width;
        private float YScaleFactor => Convert.ToInt32(ui_osuHeight.Value) / panel_OsuScreen.Height;
        private bool _dragging = false;

        
        private void label_text_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Console.WriteLine(label_text.Location.X);
                Console.WriteLine(label_text.Location.Y);
                
                panel_OsuScreen_MouseMove(sender,
                    new MouseEventArgs(MouseButtons.Left, 1, label_text.Location.X+e.X, label_text.Location.Y+e.Y, 0));
            }
        }

        private void panel_OsuScreen_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _dragging = true;
                var x = e.X >= 0 ? (e.X <= panel_OsuScreen.Width ? e.X : 0) : 0;
                var y = e.Y >= 0 ? (e.Y <= panel_OsuScreen.Height ? e.Y : 0) : 0;
                this.label_text.Location = new Point(x, y);
                UpdateCoords();
                _dragging = false;

            }
        }

        private void UpdateCoords()
        {
            var x = label_text.Location.X;
            var y = label_text.Location.Y;
            ui_textX.Value = Convert.ToInt32(Math.Round(x * XScaleFactor));
            ui_textY.Value = Convert.ToInt32(Math.Round((panel_OsuScreen.Height - y) * YScaleFactor));
        }

        private void UpdateDisplay()
        {
            var x = Convert.ToInt32(ui_textX.Value);
            var y = Convert.ToInt32(ui_textY.Value);
            var newX = Convert.ToInt32(Math.Round(x / XScaleFactor));
            var newY= Convert.ToInt32(Math.Round(panel_OsuScreen.Height-(y / YScaleFactor)));
            this.label_text.Location = new Point(newX, newY);

        }
        private void TextPositionValueChanged(object sender, EventArgs e)
        {
            if (_dragging)
                return;
            UpdateDisplay();

        }

        private void OsuRectChanged(object sender, EventArgs e)
        {
            if (_dragging)
                return;
            UpdateDisplay();
        }

    }
}
