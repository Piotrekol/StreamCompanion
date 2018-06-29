using System;
using System.Collections.Generic;
using CollectionManager.DataTypes;

namespace StreamCompanionTypes.DataTypes
{
    public class MapSearchResult
    {
        public List<Beatmap> BeatmapsFound { get { return _beatmapsFound;} set { } }
        readonly List<Beatmap> _beatmapsFound = new List<Beatmap>();
        public List<OutputPattern> FormatedStrings = new List<OutputPattern>(); 
        public bool FoundBeatmaps => _beatmapsFound.Count > 0;
        public string MapSearchString;
        public Tuple<Mods,string> Mods = null;
        public OsuStatus Action=OsuStatus.Null;
        public string EventSource { get; set; }
        public MapSearchResult(List<Beatmap> beatmaps)
        {
            _beatmapsFound = beatmaps;
        }

        public MapSearchResult(Beatmap beatmap)
        {
            _beatmapsFound.Add(beatmap);
        }

        public MapSearchResult()
        {
            
        }
    }
}
