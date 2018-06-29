using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using StreamCompanionTypes.Interfaces;

namespace ScGui
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            this.BackgroundImage = Helpers.GetStreamCompanionBackground();
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

        private void label_MouseDown(object sender, MouseEventArgs e)
        {
            MainForm_MouseDown(sender, e);
            if (((Label) sender).Tag != null)
            {
                OnUpdateTextClicked?.Invoke(sender, e);
            }
        }
        #endregion


        #region startup Code
        public event EventHandler OnUpdateTextClicked;

        public void SetDataBindings(IMainWindowModel bindingSource)
        {
            NowPlaying.DataBindings.Add(AsyncBindingHelper.GetBinding(NowPlaying, "Text", bindingSource, "NowPlaying"));
            UpdateText.DataBindings.Add(AsyncBindingHelper.GetBinding(UpdateText, "Text", bindingSource, "UpdateText"));
            BeatmapsLoaded.DataBindings.Add(AsyncBindingHelper.GetBinding(BeatmapsLoaded, "Text", bindingSource, "BeatmapsLoaded"));

            OnUpdateTextClicked += bindingSource.UpdateTextClicked;
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

        private void button_hide_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
        }

        private void showToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Show();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
