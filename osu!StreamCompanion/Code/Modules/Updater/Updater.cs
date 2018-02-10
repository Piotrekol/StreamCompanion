using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using osu_StreamCompanion.Code.Interfaces;
using osu_StreamCompanion.Code.Misc;
using osu_StreamCompanion.Code.Windows;

namespace osu_StreamCompanion.Code.Modules.Updater
{
    class Updater : IModule, IMainWindowUpdater
    {
        private MainWindowUpdater _mainWindowHandle;
        private string _onlineVersion = string.Empty;
        private const string UpdateUrl = "http://osustats.ppy.sh/api/sc/version";
        private string _downloadLink = "";
        public bool Started { get; set; }
        public void Start(ILogger logger)
        {
            Started = true;
#if DEBUG
#else
            CheckForUpdates();
#endif
        }

        private void CheckForUpdates()
        {
            new Thread((() =>
            {
                setStatus("Checking for updates...");
                UpdateVersion();
                if (string.IsNullOrWhiteSpace(_onlineVersion))
                {
                    setStatus(string.Format("Could not get update information."));
                }
                else if (_onlineVersion != Program.ScVersion)
                {
                    setStatus(string.Format("Update is avaliable! running: {0} , avaliable: {1}",
                        Program.ScVersion, _onlineVersion));
                    var result = MessageBox.Show(
                        "StreamCompanion update is avaliable." + Environment.NewLine +
                        "Would you like to open download page right now?",
                        "Update is avaliable", MessageBoxButtons.YesNo);
                    if (result == DialogResult.Yes)
                    {
                        Process.Start(_downloadLink);
                    }
                }
                else
                {
                    setStatus(string.Format("No new updates found ({0})", Program.ScVersion));
                }
            })).Start();

        }
        private void UpdateVersion()
        {
            string contents = string.Empty;
            try
            {
                using (var wc = new ImpatientWebClient())
                    contents = wc.DownloadString(UpdateUrl);
                var splited = contents.Split(new[] { ',' }, 2);
                _downloadLink = splited[1];
                _onlineVersion = splited[0];

            }
            catch (Exception)
            {
            }

        }
        private void setStatus(string status)
        {
            _mainWindowHandle.UpdateText = status;
        }
        public void GetMainWindowHandle(MainWindowUpdater mainWindowHandle)
        {
            _mainWindowHandle = mainWindowHandle;
            _mainWindowHandle.OnUpdateTextClicked += _mainWindowHandle_OnUpdateTextClicked;
        }

        private void _mainWindowHandle_OnUpdateTextClicked(object sender, EventArgs e)
        {
            CheckForUpdates();
        }
    }
}
