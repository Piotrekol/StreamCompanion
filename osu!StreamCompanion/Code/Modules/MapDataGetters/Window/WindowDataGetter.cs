using System;
using CollectionManager.DataTypes;
using CollectionManager.Enums;
using osu_StreamCompanion.Code.Core.DataTypes;
using osu_StreamCompanion.Code.Interfaces;
using osu_StreamCompanion.Code.Windows;

namespace osu_StreamCompanion.Code.Modules.MapDataGetters.Window
{
    public class WindowDataGetter :IModule,IMapDataGetter,IMainWindowUpdater
    {
        private MainWindowUpdater _mainwindowHandle;
        public bool Started { get; set; }
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
                    nowPlaying += string.Format(Environment.NewLine+"NoMod:{0:##.###}", foundMap.StarsNomod);
                    var mods = map.Mods?.Item1 ?? Mods.Omod;
                    nowPlaying += string.Format(" Modded: {0:##.###}", foundMap.Stars(PlayMode.Osu, mods));
                }
                _mainwindowHandle.NowPlaying = nowPlaying;
            }
            else
            {
                _mainwindowHandle.NowPlaying = "notFound:( " + map.MapSearchString;
            }
        }

        public void GetMainWindowHandle(MainWindowUpdater mainWindowHandle)
        {
            _mainwindowHandle = mainWindowHandle;
        }
    }
}