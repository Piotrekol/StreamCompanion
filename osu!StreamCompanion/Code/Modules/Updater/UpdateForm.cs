using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows.Forms;
using StreamCompanion.Common;

namespace osu_StreamCompanion.Code.Modules.Updater
{
    public partial class UpdateForm : Form
    {
        public UpdateContainer UpdateContainer;
        private string saveDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Files" + Path.DirectorySeparatorChar);
        private string tempFileName = "StreamCompanion.Setup.tmp";
        private string setupFileName = "StreamCompanion.Setup.exe";
        public UpdateForm(UpdateContainer container)
        {
            UpdateContainer = container;
            InitializeComponent();
            SetLabels();
        }

        private void SetLabels()
        {
            label_currentVersion.Text = string.Format("Your version: {0}", Program.ScVersion);
            label_newVersion.Text = string.Format("Newest version avaliable: {0}", UpdateContainer.Version);
            richTextBox_changelog.Rtf = UpdateContainer.GetChangelog(true);
            if (UpdateContainer.PortableMode)
            {
                button_update.Text = "Releases";
            }
        }

        private void StartUpdate()
        {
            panel_downloadProgress.Visible = true;
            var fullTempSavePath = saveDirectory + tempFileName;
            if (File.Exists(fullTempSavePath))
                File.Delete(fullTempSavePath);
            using (WebClient wc = new WebClient())
            {
                wc.DownloadProgressChanged += WcOnDownloadProgressChanged;
                wc.DownloadFileCompleted += WcOnDownloadFileCompleted;
                wc.DownloadFileAsync(new System.Uri(UpdateContainer.ExeDownloadUrl), fullTempSavePath);
            }
        }

        private void WcOnDownloadFileCompleted(object sender, AsyncCompletedEventArgs args)
        {
            var fullTempSavePath = saveDirectory + tempFileName;
            var fullSavePath = saveDirectory + setupFileName;

            if (args.Error != null)
            {
                MessageBox.Show("There was a problem with download. \n " + args.Error.Message);
                if (File.Exists(fullTempSavePath))
                    File.Delete(fullTempSavePath);
                return;
            }

            if (args.Cancelled)
                return;

            FileInfo file = new FileInfo(fullTempSavePath);
            var downloadedSize = file.Length;
            var expectedSize = UpdateContainer.ExpectedExeSizeInBytes;

            if (downloadedSize != expectedSize)
            {
                MessageBox.Show("Downloaded file was corrupt, either try again or try updating manually :(",
                    "Download error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (File.Exists(fullSavePath))
                File.Delete(fullSavePath);

            if (File.Exists(fullTempSavePath))
            {
                File.Move(fullTempSavePath, fullSavePath);
                Process.Start(Updater.UpdaterExeName, string.Format("\"{0}\" \"{1}\"", fullSavePath, " /VERYSILENT /CLOSEAPPLICATIONS /RESTARTAPPLICATIONS"));
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
                return bytes / (1024d * 1024d);
            return 0;
        }
        private void button_cancel_Click(object sender, EventArgs e)
        {
            this.Dispose(true);
            this.Close();
        }

        private void button_update_Click(object sender, EventArgs e)
        {
            if (UpdateContainer.PortableMode)
            {
                ProcessExt.OpenUrl(UpdateContainer.DownloadPageUrl);
                return;
            }

            button_update.Enabled = false;
            StartUpdate();
        }
    }
}
