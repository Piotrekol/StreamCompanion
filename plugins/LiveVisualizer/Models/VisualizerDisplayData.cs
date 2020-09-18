using System.ComponentModel;
using LiveCharts;

namespace LiveVisualizer
{
    public class VisualizerDisplayData : IVisualizerDisplayData
    {
#pragma warning disable 0067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore 0067
        public ChartValues<double> Strains { get; set; }

        public string Title { get; set; }
        public string Artist { get; set; }
        public string ImageLocation { get; set; }
        public double Pp { get; set; }
        public int Hit300 { get; set; }
        public int Hit100 { get; set; }
        public int Hit50 { get; set; }
        public int HitMiss { get; set; }

        public double Acc { get; set; }

        public double CurrentTime { get; set; }

        public double TotalTime { get; set; }
        public double PixelMapProgress { get; set; }
        public bool DisableChartAnimations { get; set; } = false;
    }
}