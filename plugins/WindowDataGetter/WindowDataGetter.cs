using System;
using System.Linq;
using CollectionManager.DataTypes;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Enums;
using StreamCompanionTypes.Interfaces;
using StreamCompanionTypes.Interfaces.Consumers;

namespace WindowDataGetter
{
    public class WindowDataGetter : IPlugin, IMapDataConsumer
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
                if (map.Action == OsuStatus.Playing || map.Action == OsuStatus.Watching || map.EventSource != "Msn")
                {
                    nowPlaying += string.Format(" [{0}] {1}", foundMap.DiffName, map.Mods?.ShownMods ?? "");
                    nowPlaying += string.Format(Environment.NewLine + "NoMod:{0:##.###} ", foundMap.StarsNomod);
                    var mods = map.Mods?.Mods ?? Mods.Omod;
                    var token = Tokens.AllTokens.FirstOrDefault(t => t.Key.ToLower() == "mstars").Value;
                    if (mods != Mods.Omod && token != null)
                    {
                        nowPlaying += $"Modded: {token.Value:##.###}, {map.Action}";
                    }
                    else
                        nowPlaying += $"{map.Action}";
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
