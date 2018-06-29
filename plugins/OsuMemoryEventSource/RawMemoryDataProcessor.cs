using System.IO;
using CollectionManager.DataTypes;
using CollectionManager.Enums;
using OsuMemoryDataProvider;
using Beatmap = StreamCompanionTypes.DataTypes.Beatmap;

namespace OsuMemoryEventSource
{
    public class RawMemoryDataProcessor
    {
        public PlayContainer Play { get; set; } = new PlayContainer();

        private Beatmap _currentBeatmap = null;
        private OppaiSharp.Beatmap _preprocessedBeatmap = null;

        public void SetCurrentMap(Beatmap beatmap, OppaiSharp.Mods mods, string osuFileLocation)
        {
            _currentBeatmap = beatmap;
            if (beatmap == null)
                return;
            try
            {
                if (_currentBeatmap.PlayMode != PlayMode.Osu || !File.Exists(osuFileLocation))
                {
                    _preprocessedBeatmap = null;
                    return;
                }
                _preprocessedBeatmap = Helpers.GetOppaiSharpBeatmap(osuFileLocation);
            }
            catch (FileNotFoundException) { }
        }

        public double PPIfRestFCed()
        {
            return 0d;
        }

        public double PPIfBeatmapWouldEndNow()
        {
            return 0d;
        }

        public double AccIfRestFCed()
        {
            return 0d;
        }
    }
}