﻿using System;
using CollectionManager.DataTypes;
using CollectionManager.Enums;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Interfaces;

namespace WindowDataGetter
{
    public class WindowDataGetter : IPlugin, IMapDataGetter
    {
        private readonly IMainWindowModel _mainWindowHandle;
        public bool Started { get; set; }
        
        public string Description { get; } = "Provides map data for StreamCompanion GUI";
        public string Name { get; } = nameof(WindowDataGetter);
        public string Author { get; } = "Piotrekol";
        public string Url { get; } = "";
        public string UpdateUrl { get; } = "";

        public WindowDataGetter(IMainWindowModel mainWindowHandle)
        {
            _mainWindowHandle = mainWindowHandle;
        }

        public void SetNewMap(MapSearchResult map)
        {
            if (map.FoundBeatmaps)
            {
                var foundMap = map.BeatmapsFound[0];
                var nowPlaying = string.Format("{0} - {1}", foundMap.ArtistRoman, foundMap.TitleRoman);
                if (map.Action == OsuStatus.Playing || map.Action == OsuStatus.Watching || map.EventSource!="Msn")
                {
                    nowPlaying += string.Format(" [{0}] {1}", foundMap.DiffName, map.Mods?.ShownMods ?? "");
                    nowPlaying += string.Format(Environment.NewLine + "NoMod:{0:##.###}", foundMap.StarsNomod);
                    var mods = map.Mods?.Mods ?? Mods.Omod;
                    nowPlaying += string.Format(" Modded: {0:##.###}", foundMap.Stars(PlayMode.Osu, mods));
                }
                _mainWindowHandle.NowPlaying = nowPlaying;
            }
            else
            {
                _mainWindowHandle.NowPlaying = "map data not found: " + map.MapSearchString;
            }
        }
    }
}
