using Newtonsoft.Json;

namespace PpCalculatorTypes
{
    public class BreakPeriod
    {
        [JsonProperty("startTime")]
        public double StartTime { get; }
        [JsonProperty("endTime")]
        public double EndTime { get; }
        [JsonProperty("hasEffect")]
        public bool HasEffect { get; }

        public BreakPeriod(double startTime, double endTime, bool hasEffect)
        {
            StartTime = startTime;
            EndTime = endTime;
            HasEffect = hasEffect;
        }

    }
}