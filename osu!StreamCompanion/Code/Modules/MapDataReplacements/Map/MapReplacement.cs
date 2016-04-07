using System.Collections.Generic;
using osu_StreamCompanion.Code.Core.DataTypes;
using osu_StreamCompanion.Code.Helpers;
using osu_StreamCompanion.Code.Interfeaces;

namespace osu_StreamCompanion.Code.Modules.MapDataReplacements.Map
{
    class MapReplacement :IModule,IMapDataReplacements
    {
        public bool Started { get; set; }
        public void Start(ILogger logger){ Started = true; }

        public Dictionary<string, string> GetMapReplacements(MapSearchResult map)
        {
            if (map.FoundBeatmaps)
            {
                var dict = map.BeatmapsFound[0].GetDict(map.Mods);
                return dict;
            }

            return new Dictionary<string, string>();
        }
    }
}
