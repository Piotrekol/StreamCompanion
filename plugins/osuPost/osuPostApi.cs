﻿using System;
using System.Net;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Enums;
using StreamCompanionTypes.Interfaces;
using StreamCompanionTypes.Interfaces.Services;

namespace osuPost
{
    public class OsuPostApi : IDisposable
    {
        private readonly ILogger _logger;

        #region variables
        string _userId = "-1";
        string _userKey = "";
        bool _isLoginDataSet;
        public string EndpointUrl = @"http://osupost.givenameplz.de/input.php?u=";

        public OsuPostApi(ILogger logger)
        {
            _logger = logger;
        }

        public bool IsEnabled => _isLoginDataSet;

        public class ErrorReport : EventArgs
        {
            public string Message { get; set; }
            public string Caption { get; set; }
        }
        #endregion

        #region Events
        public event EventHandler<ErrorReport> OnError;
        #endregion

        #region Initalizers

        private bool IsInitDataCorrect(string userId, string userKey)
        {
            if (userId == null || userId == "-1" || userId.Trim() == "")
                return false;
            if (userKey.Trim() == "" || userKey.Length < 10)
                return false;
            return true;

        }

        public bool SetOsuPostLoginData(string userId, string userKey)
        {
            if (!IsInitDataCorrect(userId, userKey))
            {
                Error("Osu!Post login data seems to be incorrect" + Environment.NewLine + "If you believe that this is an error please report it!", "Osu!Post Error");

                return false;
            }
            _userId = userId;
            _userKey = userKey;
            _isLoginDataSet = true;
            return true;

        }
        #endregion //Initalizers

        #region Event Calls
        private void Error(string message, string caption)
        {
            if (OnError != null)
            {
                var args = new ErrorReport()
                {
                    Message = message,
                    Caption = caption
                };
                OnError(this, args);
            }
        }


        #endregion // Event Calls

        #region Callable Functions
        public void Disable()
        {
            logOut();
            _isLoginDataSet = false;
        }

        private void logOut()
        {
            if (_isLoginDataSet)
                SendRequestToServer(FormatRequest(new MapSearchResult(new MapSearchArgs("FakeSource", OsuEventType.MapChange)), false));
        }
        public void NewMap(IMapSearchResult map, bool isOnline = true)
        {
            if (!_isLoginDataSet) return;


            string postData;
            string response;
            if (map.Action == OsuStatus.Playing || map.Action == OsuStatus.Watching)
            {
                postData = FormatRequest(map, isOnline);
                response = SendRequestToServer(postData);

            }
            else
            {
                postData = FormatRequest(map, isOnline);
                response = SendRequestToServer(postData);
            }
            if (response == "ERROR")
            {
                Error("There was a problem while contacting server.", "osu!Post Error");
            }
            else
            {
                switch (response)
                {
                    case "true":
                        break;
                    case "false":
                        Error("Key Provided is incorrect.", "osu!Post Error");
                        break;
                    case "null":
                        Error("User not found.", "osu!Post Error");
                        break;
                }
            }
        }
        #endregion

        #region Request Formating

        private class OsuPostApiMapNameTooLongException : Exception
        {
            public OsuPostApiMapNameTooLongException(string mapName) : base(mapName) { }
        }
        private string FormatRequest(IMapSearchResult map, bool isOnline)
        {
            var output = "key=" + _userKey;
            output += "&isOnline=" + (isOnline ? "true" : "false");
            var mapName = GetMapName(map);
            if (mapName.Length >= 2000)
            {
                var ex = new OsuPostApiMapNameTooLongException(mapName);
                _logger.Log(ex, LogLevel.Error);
                mapName = mapName.Substring(0, 2000);
            }
            output += "&mapName=" + Uri.EscapeDataString(mapName);
            output += "&mapID=" + GetMapId(map);
            output += "&mapSetID=" + GetMapSetId(map);
            output += "&userAction=" + GetMapAction(map);
            return output;
        }


        private string GetMapAction(IMapSearchResult map)
        {
            var act = map.Action;

            if (act == OsuStatus.Playing)
                return "Playing";
            if (act == OsuStatus.Watching)
                return "Watching";
            if (act == OsuStatus.Listening)
                return "Listening";
            if (act == OsuStatus.Editing)
                return "Editing";
            if (act == OsuStatus.Null)
                return "NoAction";

            return "NoAction";
        }
        private int GetMapSetId(IMapSearchResult map)
        {
            try
            {
                if (map?.BeatmapsFound[0]?.MapSetId > 0)
                    return map.BeatmapsFound[0].MapSetId;
            }
            catch { return -1; }
            return -1;
        }
        private int GetMapId(IMapSearchResult map)
        {
            try
            {
                if (map?.BeatmapsFound[0]?.MapId > 0)
                    return map.BeatmapsFound[0].MapId;
            }
            catch { return -1; }
            return -1;
        }
        private string GetMapName(IMapSearchResult map)
        {
            if (!string.IsNullOrWhiteSpace(map.MapSearchString))
                return map.MapSearchString;
            else
                return "NoMap";
        }


        #endregion

        #region Server Requests
        private string SendRequestToServer(string dataToSend)
        {
            var uri = EndpointUrl + _userId;
            var postParams = dataToSend;

            string htmlResult;
            try
            {
#pragma warning disable SYSLIB0014 // WebClient is deprecated, use HttpClient instead
                using (WebClient wc = new WebClient())
#pragma warning restore SYSLIB0014 // WebClient is deprecated, use HttpClient instead
                {
                    wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                    htmlResult = wc.UploadString(uri, postParams);
                }
            }
            catch
            {
                return "ERROR";//doesn't matter what error
            }
            return htmlResult;
        }
        #endregion //Server Requests

        public void Dispose()
        {
            logOut();
        }


    }
}
