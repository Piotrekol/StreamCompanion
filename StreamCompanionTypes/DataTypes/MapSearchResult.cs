using CollectionManager.Enums;
using System.Collections.Generic;

namespace StreamCompanionTypes.DataTypes
{
    public class MapSearchResult
    {
        private readonly MapSearchArgs _searchArgs;
        public List<Beatmap> BeatmapsFound { get { return _beatmapsFound; } set { } }
        readonly List<Beatmap> _beatmapsFound = new List<Beatmap>();
        public List<OutputPattern> FormatedStrings = new List<OutputPattern>();
        public bool FoundBeatmaps => _beatmapsFound.Count > 0;
        public string MapSearchString => _searchArgs.Raw;
        public ModsEx Mods = null;
        public OsuStatus Action => _searchArgs.Status;
        public PlayMode? PlayMode => _searchArgs.PlayMode;
        public string EventSource { get; set; }

        public MapSearchResult(MapSearchArgs searchArgs)
        {
            _searchArgs = searchArgs;
        }
    }
}
