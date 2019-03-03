using System;
using LiveCharts;

namespace LiveVisualizer
{
    public interface IVisualizerData
    {
        ChartValues<double> Strains { get; set; }

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
    }

    public interface IWpfVisualizerData : IVisualizerData
    {
        double FontsizeTitle { get; set; }
        double FontsizeArtist { get; set; }

        double PixelMapProgress { get; set; }
        string ChartColor { get; set; }
        string ChartProgressColor { get; set; }

        double MaxYValue { get; set; }

        string FillColor { get; set; }
    }

    public interface IWpfChartConfiguration
    {

    }
}