using System.Collections.Generic;

namespace Gamma.Models
{
    public class Configuration
    {
        public List<GammaRange> GammaRanges { get; set; } = new List<GammaRange>
        {
            new GammaRange{MinAr = 10.01, MaxAr = 14, Gamma = 0.7f}
        };
        public bool Enabled { get; set; } = false;
        public string ScreenDeviceName { get; set; } = string.Empty;
    }
}