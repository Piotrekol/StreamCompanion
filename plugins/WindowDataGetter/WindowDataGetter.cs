using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CollectionManager.DataTypes;
using CollectionManager.Enums;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Interfaces;

namespace WindowDataGetter
{
    public class WindowDataGetter : IPlugin, IMapDataGetter, IMainWindowUpdater
    {
        private IMainWindowModel _mainwindowHandle;
        public bool Started { get; set; }
        
        public string Description { get; } = "Provides map data for StreamCompanion GUI";
        public string Name { get; } = nameof(WindowDataGetter);
        public string Author { get; } = "Piotrekol";
        public string Url { get; } = "";
        public string UpdateUrl { get; } = "";

        public void Start(ILogger logger)
        {
            Started = true;
        }

        public void SetNewMap(MapSearchResult map)
        {
            if (map.FoundBeatmaps)
            {
                var foundMap = map.BeatmapsFound[0];
                var nowPlaying = string.Format("{0} - {1}", foundMap.ArtistRoman, foundMap.TitleRoman);
                if (map.Action == OsuStatus.Playing || map.Action == OsuStatus.Watching)
                {
                    nowPlaying += string.Format(" [{0}] {1}", foundMap.DiffName, map.Mods?.Item2 ?? "");
                    nowPlaying += string.Format(Environment.NewLine + "NoMod:{0:##.###}", foundMap.StarsNomod);
                    var mods = map.Mods?.Item1 ?? Mods.Omod;
                    nowPlaying += string.Format(" Modded: {0:##.###}", foundMap.Stars(PlayMode.Osu, mods));
                }
                _mainwindowHandle.NowPlaying = nowPlaying;
            }
            else
            {
                _mainwindowHandle.NowPlaying = "map data not found: " + map.MapSearchString;
            }
        }

        public void GetMainWindowHandle(IMainWindowModel mainWindowHandle)
        {
            _mainwindowHandle = mainWindowHandle;
        }

    }
}
