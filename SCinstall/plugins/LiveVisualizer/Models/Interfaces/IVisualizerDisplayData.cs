using System.Collections.Generic;
using System.ComponentModel;

namespace LiveVisualizer
{
    public interface IVisualizerDisplayData : INotifyPropertyChanged
    {
        List<double> Strains { get; set; }
        ScottPlot.WpfPlot MainDataPlot { get; }
        ScottPlot.WpfPlot BackgroundDataPlot { get; }
        
        string Title { get; set; }
        string Artist { get; set; }
        string ImageLocation { get; set; }

        int Hit300 { get; set; }
        int Hit100 { get; set; }
        int Hit50 { get; set; }
        int HitMiss { get; set; }
        double Pp { get; set; }
        double Acc { get; set; }

        double CurrentTime { get; set; }
        double TotalTime { get; set; }
        double PixelMapProgress { get; set; }
        bool DisableChartAnimations { get; set; }
    }
}