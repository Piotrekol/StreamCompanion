using Newtonsoft.Json;

namespace PpCalculatorTypes
{
    public class KiaiPoint
    {
        public KiaiPoint(double startTime,double duration)
        {
            StartTime = startTime;
            Duration = duration;
        }

        [JsonProperty("startTime")]
        public double StartTime { get; }
        [JsonProperty("duration")]
        public double Duration { get; }
    }
}