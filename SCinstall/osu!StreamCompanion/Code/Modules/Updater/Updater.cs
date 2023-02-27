using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using osu_StreamCompanion.Code.Misc;
using StreamCompanionTypes.Enums;
using StreamCompanionTypes.Interfaces;
using StreamCompanionTypes.Interfaces.Services;

namespace osu_StreamCompanion.Code.Modules.Updater
{
    class Updater : IModule, IDisposable
    {
        private readonly ILogger _logger;
        private IMainWindowModel _mainWindowHandle;
        private const string baseGithubUrl = "https://api.github.com/repos/Piotrekol/StreamCompanion";
        private const string githubUpdateUrl = baseGithubUrl + "/releases/latest";
        private const string githubChangelogUrl = baseGithubUrl + "/releases";
        private DateTime _currentVersion = Helpers.Helpers.GetDateFromVersionString(Program.ScVersion);
        private Exception exception = null;
        private Lazy<bool> portableMode = new(() => File.Exists(".portableMode"));
        public bool Started { get; set; }
        internal static string UpdaterExeName = "StreamCompanion Updater.exe";

        public Updater(ILogger logger, IMainWindowModel mainWindowHandle)
        {
            _logger = logger;
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

        private async Task SetErrorMessage(string baseMsg)
        {
            if (userDelayTask != null)
                await userDelayTask;
            string ret = baseMsg + " ";
            if (exception != null)
                ret += exception.Message;
            _logger.Log($"Error: {ret}", LogLevel.Debug);
            await SetStatus(ret);
        }

        private Task userDelayTask;
        private void CheckForUpdates()
        {
            Task.Run(async () =>
            {
                await SetStatus("Checking for updates...");
                userDelayTask = Task.Delay(500);
                string rawData = GetStringData(githubUpdateUrl);
                if (string.IsNullOrWhiteSpace(rawData))
                {
                    await SetErrorMessage("Could not get update information - rawData");
                    return;
                }

                JObject json;
                try
                {
                    json = JObject.Parse(rawData);
                }
                catch (JsonReaderException)
                {
                    await SetErrorMessage("Could not get update information - invalidJson");
                    return;
                }

                var newestReleaseVersion = json["tag_name"].ToString();
                if (string.IsNullOrWhiteSpace(newestReleaseVersion))
                {
                    await SetErrorMessage("Could not get update information - newestRelease");
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
                            await SetErrorMessage("Could not find file to download");
                            return;
                        }

                        var container = new UpdateContainer()
                        {
                            ExeDownloadUrl = asset["browser_download_url"].ToString(),
                            Version = newestReleaseVersion,
                            ExpectedExeSizeInBytes = asset["size"].ToObject<int>(),
                            DownloadPageUrl = json["html_url"].ToString(),
                            PortableMode = portableMode.Value,
                            Changelog = GetChangelog()
                        };
                        await SetStatus(string.Format("Update to version {0} is available",
                            newestReleaseVersion));

                        ShowUpdateWindow(container);
                    }
                    catch (Exception e)
                    {
                        exception = e;
                        await SetErrorMessage("There was a problem with update information");
                    }
                }
                else
                {
                    await SetStatus("No new updates found");
                }
            });
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

        private async Task SetStatus(string status)
        {
            if (userDelayTask != null)
                await userDelayTask;

            _mainWindowHandle.UpdateText = status;
        }

        private void _mainWindowHandle_OnUpdateTextClicked(object sender, EventArgs e)
        {
            CheckForUpdates();
        }

        public void Dispose()
        {
            userDelayTask?.Dispose();
            _updateForm?.Dispose();
        }
    }
}
