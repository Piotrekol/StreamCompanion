using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using osu_StreamCompanion.Code.Helpers;
using osu_StreamCompanion.Code.Windows;

namespace osu_StreamCompanion.Code.Modules.SCGUI
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

        }
        #region Form dragging
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        private void MainForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
        #endregion
       

        #region startup Code

        public void SetDataBindings(MainWindowUpdater bindingSource)
        {
            NowPlaying.DataBindings.Add(AsyncBindingHelper.GetBinding(NowPlaying, "Text", bindingSource, "NowPlaying"));
            UpdateText.DataBindings.Add(AsyncBindingHelper.GetBinding(UpdateText, "Text", bindingSource, "UpdateText"));
            BeatmapsLoaded.DataBindings.Add(AsyncBindingHelper.GetBinding(BeatmapsLoaded, "Text", bindingSource, "BeatmapsLoaded"));
            UpdateText.Click += bindingSource.UpdateTextClicked;
        }
        #endregion




        private void button_About_Click(object sender, EventArgs e)
        {
            var aboutFrm = new AboutForm();
            aboutFrm.ShowDialog();
        }

        private void exit_button_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
