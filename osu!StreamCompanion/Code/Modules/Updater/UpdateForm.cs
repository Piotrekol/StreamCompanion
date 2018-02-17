using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Text;
using System.Windows.Forms;

namespace osu_StreamCompanion.Code.Modules.Updater
{
    public partial class UpdateForm : Form
    {
        public UpdateContainer UpdateContainer;
        private string saveDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Files" + Path.DirectorySeparatorChar);
        private string tempFileName = "StreamCompanion.Setup.tmp";
        private string setupFileName = "StreamCompanion.Setup.exe";
        private string UpdaterExeName = "StreamCompanion Updater.exe";
        public UpdateForm(UpdateContainer container)
        {
            UpdateContainer = container;
            InitializeComponent();
            SetLabels();
        }

        private void SetLabels()
        {
            this.label_currentVersion.Text = string.Format("Your version: {0}", Program.ScVersion);
            this.label_newVersion.Text = string.Format("Newest version avaliable: {0}", UpdateContainer.Version);
            this.richTextBox_changelog.Rtf = UpdateContainer.GetChangelog(true);
        }

        private void StartUpdate()
        {
            
            panel_downloadProgress.Visible = true;
            var fullTempSavePath = saveDirectory + tempFileName;
            if(File.Exists(fullTempSavePath))
                File.Delete(fullTempSavePath);
            using (WebClient wc = new WebClient())
            {
                wc.DownloadProgressChanged += WcOnDownloadProgressChanged;
                wc.DownloadFileCompleted+=WcOnDownloadFileCompleted;
                wc.DownloadFileAsync(new System.Uri(UpdateContainer.ExeDownloadUrl), fullTempSavePath);
            }
        }

        private bool isAdmin()
        {
            bool isElevated;
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                isElevated = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            return isElevated;
        }
        private void WcOnDownloadFileCompleted(object sender, AsyncCompletedEventArgs asyncCompletedEventArgs)
        {
            var fullTempSavePath = saveDirectory + tempFileName;
            var fullSavePath = saveDirectory + setupFileName;

            if (File.Exists(fullSavePath))
                File.Delete(fullSavePath);
            if (File.Exists(fullTempSavePath))
            {
                File.Move(fullTempSavePath, fullSavePath);
                Process.Start(UpdaterExeName, string.Format("\"{0}\" \"{1}\"", fullSavePath, " /VERYSILENT /CLOSEAPPLICATIONS /RESTARTAPPLICATIONS"));
                Program.SafeQuit();
            }
        }

        private void WcOnDownloadProgressChanged(object o, DownloadProgressChangedEventArgs args)
        {
            this.progressBar1.Value = args.ProgressPercentage;
            this.label_downloadProgress.Text = string.Format("{0:0.##,9}MB/{1:0.##,9}MB  {2}%", 
                bytesToMbytes(args.BytesReceived), bytesToMbytes(args.TotalBytesToReceive), args.ProgressPercentage);
        }

        private double bytesToMbytes(long bytes)
        {
            if (bytes > 0)
                return bytes / (1024d* 1024d);
            return 0;
        }
        private void button_cancel_Click(object sender, EventArgs e)
        {
            this.Dispose(true);
            this.Close();
        }

        private void button_update_Click(object sender, EventArgs e)
        {
            button_update.Enabled = false;
            StartUpdate();
        }
    }
}
