using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using osu_StreamCompanion.Code.Misc;
using StreamCompanionTypes.Interfaces;
using StreamCompanionTypes.Interfaces.Services;

namespace osu_StreamCompanion.Code.Modules.Updater
{
    class Updater : IModule
    {
        private IMainWindowModel _mainWindowHandle;
        private const string baseGithubUrl = "https://api.github.com/repos/Piotrekol/StreamCompanion";
        private const string githubUpdateUrl = baseGithubUrl + "/releases/latest";
        private const string githubChangelogUrl = baseGithubUrl + "/releases";
        private DateTime _currentVersion = Helpers.Helpers.GetDateFromVersionString(Program.ScVersion);
        private Exception exception = null;
        public bool Started { get; set; }

        public Updater(ILogger logger, IMainWindowModel mainWindowHandle)
        {
            _mainWindowHandle = mainWindowHandle;
            _mainWindowHandle.OnUpdateTextClicked += _mainWindowHandle_OnUpdateTextClicked;
            Start(logger);
        }
        public void Start(ILogger logger)
        {
            //TODO: handle not supported exception
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072; //Force usage of TLS1.2 when possible.
            Started = true;
            CheckForUpdates();
        }
        
        private Dictionary<string, string> GetChangelog()
        {
            Dictionary<string, string> log = null;
            var rawData = GetStringData(githubChangelogUrl);
            if (!string.IsNullOrWhiteSpace(rawData))
            {
                var json = JArray.Parse(rawData);
                log = json.Where(j =>
                        Helpers.Helpers.GetDateFromVersionString(j["tag_name"].ToString()) > _currentVersion)
                    .ToDictionary(j => j["tag_name"].ToString(), j => j["body"].ToString());
            }
            return log;
        }

        private void SetErrorMessage(string baseMsg)
        {
            string ret = baseMsg+" ";
            if (exception != null)
                ret += exception.Message;
            setStatus(ret);
        }
        private void CheckForUpdates()
        {
            new Thread(() =>
            {
                setStatus("Checking for updates...");
                string rawData = GetStringData(githubUpdateUrl);
                if (string.IsNullOrWhiteSpace(rawData))
                {
                    SetErrorMessage("Could not get update information. - rawData");
                    return;
                }
                JObject json;
                try
                {
                    json = JObject.Parse(rawData);
                }
                catch (JsonReaderException)
                {
                    SetErrorMessage("Could not get update information. - invalidJson");
                    return;
                }
                var newestReleaseVersion = json["tag_name"].ToString();
                if (string.IsNullOrWhiteSpace(newestReleaseVersion))
                {
                    SetErrorMessage("Could not get update information. - newestRelease");
                }
                else if (Helpers.Helpers.GetDateFromVersionString(newestReleaseVersion) > _currentVersion)
                {
                    try
                    {
                        JToken asset = null;
                        foreach (var a in json["assets"])
                        {
                            if (a["name"].ToString() == "StreamCompanion.Setup.exe")
                            {
                                asset = a;
                                break;
                            }

                        }
                        if (asset == null)
                        {
                            SetErrorMessage("Could not find file to download!");
                            return;
                        }

                        var container = new UpdateContainer()
                        {
                            ExeDownloadUrl = asset["browser_download_url"].ToString(),
                            Version = newestReleaseVersion,
                            ExpectedExeSizeInBytes = asset["size"].ToObject<int>(),
                            DownloadPageUrl = json["html_url"].ToString(),
                            Changelog = GetChangelog()
                        };
                        setStatus(string.Format("Update is avaliable! running: {0} , avaliable: {1}",
                            Program.ScVersion, newestReleaseVersion));

                        ShowUpdateWindow(container);
                    }
                    catch(Exception e)
                    {
                        exception = e;
                        SetErrorMessage("There was a problem with update information.");
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
            var ret = Helpers.Helpers.ExecWithTimeout(token =>
            {
                string contents = string.Empty;
                try
                {
                    using (var wc = new ImpatientWebClient())
                    {
                        wc.Headers.Add("user-agent", "StreamCompanion_Updater_" + Program.ScVersion);
                        contents = wc.DownloadString(url);
                    }
                }
                catch (Exception e)
                {
                    exception = e;
                }
                return contents;
            });
            return ret ?? string.Empty;
        }
        
        private void setStatus(string status)
        {
            _mainWindowHandle.UpdateText = status;
        }

        private void _mainWindowHandle_OnUpdateTextClicked(object sender, EventArgs e)
        {
            CheckForUpdates();
        }
    }
}
