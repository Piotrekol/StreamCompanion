using System.Collections.Generic;
using System.Threading;
using osu_StreamCompanion.Code.Core.DataTypes;
using osu_StreamCompanion.Code.Interfaces;
using osu_StreamCompanion.Code.Misc;

namespace osu_StreamCompanion.Code.Core.Maps.Processing
{
    public class MapStringFormatter : IModule,IMsnGetter,ISettings
    {
        private readonly SettingNames _names = SettingNames.Instance;
        private ILogger _logger;
        private readonly MainMapDataGetter _mainMapDataGetter;
        private Settings _settings;
        private string _lastMsnString = "";
        public MapStringFormatter(MainMapDataGetter mainMapDataGetter)
        {
            _mainMapDataGetter = mainMapDataGetter;
        }
        public bool Started { get; set; }
        public void Start(ILogger logger)
        {
            _logger = logger;
        }
        public void SetSettingsHandle(Settings settings)
        {
            _settings = settings;
        }

        public void SetNewMsnString(Dictionary<string, string> osuStatus)
        {
            /*osuStatus["artist"]
            osuStatus["title"]
            osuStatus["diff"]
            osuStatus["status"]*/

            OsuStatus status = osuStatus["status"] == "Listening" ? OsuStatus.Listening
                : osuStatus["status"] == "Playing" ? OsuStatus.Playing
                : osuStatus["status"] == "Watching" ? OsuStatus.Watching
                : osuStatus["status"] == "Editing" ? OsuStatus.Editing
                : OsuStatus.Null;

            osuStatus["raw"] = string.Format("{0} - {1}", osuStatus["title"], osuStatus["artist"]);
            bool isFalsePlay;
            lock (this)
            {
                isFalsePlay = IsFalsePlay(osuStatus["raw"], status, _lastMsnString);
            }
            if (isFalsePlay)
            {
                _logger.Log(">ignoring second MSN string...", LogLevel.Advanced);
            }
            else
            {
                _lastMsnString = osuStatus["raw"];
                _logger.Log("", LogLevel.Advanced);
                string result = ">Got ";
                foreach (var v in osuStatus)
                {
                    if (v.Key != "raw") result = result + $"{v.Key}: \"{v.Value}\" ";
                }
                _logger.Log(result, LogLevel.Basic);
                while (_settings.Get<bool>(_names.LoadingRawBeatmaps))
                {
                    Thread.Sleep(200);
                }
                _mainMapDataGetter.FindMapData(osuStatus, status);

            }
        }

        #region MSN double-send fix
        public class MapArgs
        {
            public string MapName;
            public OsuStatus MapAction;
        }
        //osu! MSN double-send detection
        private readonly string[] _lastListened = new string[2];
        bool IsFalsePlay(string msnString, OsuStatus msnStatus, string lastMapString)
        {
            lock (_lastListened)
            {
                // if we're listening to a song AND it's not already in the first place of our Queue
                if (msnStatus == OsuStatus.Listening && msnString != _lastListened[0])
                {
                    //first process our last listened song "Queue" 
                    _lastListened[1] = _lastListened[0];
                    _lastListened[0] = msnString;
                }
                //we have to be playing for bug to occour...
                if (msnStatus != OsuStatus.Playing)
                    return false;
                //if same string is sent 2 times in a row
                if (msnString == lastMapString)
                {
                    //this is where it gets checked for actual bug- Map gets duplicated only when we just switched from another song
                    //so check if we switched by checking if last listened song has changed 
                    if (_lastListened[0] != _lastListened[1])
                    {
                        //to avoid marking another plays(Retrys) as False- we "break" our Queue until we change song.
                        _lastListened[1] = _lastListened[0];
                        return true;
                    }
                }
                return false;
            }
        }

        #endregion //MSN FIX
    }
}
