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
                var nowPlaying = string.Format("{0} - {1}", map.BeatmapsFound[0].ArtistRoman,map.BeatmapsFound[0].TitleRoman);
                if (map.Action == OsuStatus.Playing || map.Action == OsuStatus.Watching)
                    nowPlaying += string.Format(" [{0}] {1}", map.BeatmapsFound[0].DiffName,map.Mods?.Item2 ?? "");
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