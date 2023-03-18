using System.Collections.Generic;
using System.Runtime.CompilerServices;

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

        public void SortGammaRanges()
            => GammaRanges.Sort((r1, r2) => r1.MinAr.CompareTo(r2.MinAr));
    }
}