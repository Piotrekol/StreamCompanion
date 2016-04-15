using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace osu_StreamCompanion.Code.Modules.FileSaveLocation
{
    public partial class FileSaveLocationSettings : UserControl
    {
        public FileSaveLocationSettings()
        {
            InitializeComponent();
        }

        private void button_OpenFilesFolder_Click(object sender, System.EventArgs e)
        {
            string dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Files\");
            Process.Start("explorer.exe", dir);
        }
    }
}
