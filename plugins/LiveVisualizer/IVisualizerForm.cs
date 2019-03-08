using LiveCharts;
using System;
using System.ComponentModel;

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

    public interface IWpfVisualizerData : IVisualizerData, INotifyPropertyChanged
    {
        double FontsizeTitle { get; set; }
        double FontsizeArtist { get; set; }

        double PixelMapProgress { get; set; }
        string ChartColor { get; set; }
        string ChartProgressColor { get; set; }

        double MaxYValue { get; set; }

        string FillColor { get; set; }
        string Font { get; set; }

        bool ShowAxisYSeparator { get; set; }
        string AxisYSeparatorColor { get; set; }
        double AxisYStep { get; set; }
        bool DisableChartAnimations { get; set; }

        double WindowHeight { get; set; }
        double WindowWidth { get; set; }
        bool EnableResizing { get; set; }
    }

    public interface IWpfChartConfiguration
    {

    }
}