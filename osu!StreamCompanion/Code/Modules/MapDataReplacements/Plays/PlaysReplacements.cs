using System.Collections.Generic;
using osu_StreamCompanion.Code.Core.DataTypes;
using osu_StreamCompanion.Code.Interfeaces;

namespace osu_StreamCompanion.Code.Modules.MapDataReplacements.Plays
{
    public class PlaysReplacements : IModule, IMapDataReplacements
    {
        private int Plays, Retrys;
        public bool Started { get; set; }
        public void Start(ILogger logger) { Started = true; }
        private string lastMapSearchString = "";
        public Dictionary<string, string> GetMapReplacements(MapSearchResult map)
        {
            if (map.Action == OsuStatus.Playing)
            {
                if (lastMapSearchString == map.MapSearchString)
                    Retrys++;
                else
                    Plays++;
                lastMapSearchString = map.MapSearchString;
            }
            var ret = new Dictionary<string, string>();
            ret.Add("!Plays!", Plays.ToString());
            ret.Add("!Retrys!", Retrys.ToString());
            return ret;
        }
    }
}