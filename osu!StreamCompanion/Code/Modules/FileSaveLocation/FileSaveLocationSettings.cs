﻿using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using StreamCompanionTypes.Interfaces;

namespace osu_StreamCompanion.Code.Modules.FileSaveLocation
{
    public partial class FileSaveLocationSettings : UserControl
    {
        private ISaver _saver;

        public FileSaveLocationSettings()
        {
            InitializeComponent();
        }

        private void button_OpenFilesFolder_Click(object sender, System.EventArgs e)
        {
            string dir = Path.Combine(_saver.SaveDirectory);
            Process.Start("explorer.exe", dir);
        }

        public void SetSaveHandle(ISaver saver)
        {
            _saver = saver;
        }
    }
}
