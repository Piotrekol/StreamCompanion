using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Newtonsoft.Json.Linq;
using osu_StreamCompanion.Code.Interfaces;
using osu_StreamCompanion.Code.Misc;
using osu_StreamCompanion.Code.Windows;
namespace osu_StreamCompanion.Code.Modules.Updater
{
    class Updater : IModule, IMainWindowUpdater
    {
        private MainWindowUpdater _mainWindowHandle;
        private const string baseGithubUrl = "https://api.github.com/repos/Piotrekol/StreamCompanion";
        private const string githubUpdateUrl = baseGithubUrl + "/releases/latest";
        private const string githubChangelogUrl = baseGithubUrl + "/releases";
        DateTime _currentVersion = Helpers.Helpers.GetDateFromVersionString(Program.ScVersion);
        public bool Started { get; set; }
        public void Start(ILogger logger)
        {
            Started = true;
            CheckForUpdates();
        }

        private Dictionary<string, string> GetChangelog()
        {
            var rawData = GetStringData(githubChangelogUrl);
            var json = JArray.Parse(rawData);
            var log = json.Where(j =>
                    Helpers.Helpers.GetDateFromVersionString(j["tag_name"].ToString()) > _currentVersion)
                .ToDictionary(j => j["tag_name"].ToString(), j => j["body"].ToString());

            return log;
        }
        private void CheckForUpdates()
        {
            new Thread(() =>
            {
                setStatus("Checking for updates...");
                string rawData = GetStringData(githubUpdateUrl);
                if (string.IsNullOrWhiteSpace(rawData))
                {
                    setStatus("Could not get update information. - rawData");
                }
                var json = JObject.Parse(rawData);
                var newestReleaseVersion = json["tag_name"].ToString();
                if (string.IsNullOrWhiteSpace(newestReleaseVersion))
                {
                    setStatus("Could not get update information. - newestRelease");
                }
                else if (Helpers.Helpers.GetDateFromVersionString(newestReleaseVersion) > _currentVersion)
                {
                    try
                    {
                        var container = new UpdateContainer()
                        {
                            ExeDownloadUrl = json["assets"][0]["browser_download_url"].ToString(),
                            Version = newestReleaseVersion,
                            ExpectedExeSizeInBytes = json["assets"][0]["size"].ToObject<int>(),
                            DownloadPageUrl = json["html_url"].ToString(),
                            Changelog = GetChangelog()
                        };
                        setStatus(string.Format("Update is avaliable! running: {0} , avaliable: {1}",
                            Program.ScVersion, newestReleaseVersion));

                        ShowUpdateWindow(container);
                    }
                    catch
                    {
                        setStatus("There was a problem with update information.");
                    }
                }
                else
                {
                    setStatus(string.Format("No new updates found ({0})", Program.ScVersion));
                }
            }).Start();
        }

        private UpdateForm _updateForm = null;
        private void ShowUpdateWindow(UpdateContainer container)
        {
            if (_updateForm == null || _updateForm.IsDisposed)
            {
                _updateForm = new UpdateForm(container);
            }
            else
            {
                _updateForm.UpdateContainer = container;
            }
            _updateForm.ShowDialog();
            _updateForm.Dispose();

        }
        private string GetStringData(string url)
        {
            var ret = Helpers.Helpers.ExecWithTimeout(() =>
            {
                string contents = string.Empty;
                try
                {
                    using (var wc = new ImpatientWebClient())
                    {
                        wc.Headers.Add("user-agent", "StreamCompanion_Updater");
                        contents = wc.DownloadString(url);
                    }
                }
                catch
                {
                }
                return contents;
            });
            return ret ?? string.Empty;
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
