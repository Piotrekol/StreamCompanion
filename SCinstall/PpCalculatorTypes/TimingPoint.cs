using Newtonsoft.Json;

namespace PpCalculatorTypes
{
    public class TimingPoint
    {
        public TimingPoint(double startTime, double bpm, double beatLength)
        {
            StartTime = startTime;
            BPM = bpm;
            BeatLength = beatLength;
        }

        [JsonProperty("startTime")]
        public double StartTime { get; }
        [JsonProperty("bpm")]
        public double BPM { get; }
        [JsonProperty("beatLength")]
        public double BeatLength { get; }
    }
}