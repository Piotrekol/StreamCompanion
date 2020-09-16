using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media;

namespace LiveVisualizer
{
    public interface IVisualizerConfiguration : INotifyPropertyChanged
    {
        bool SimulatePPWhenListening { get; set; }
        bool HideDiffText { get; set; }
        bool HideMapStats { get; set; }
        bool HideChartLegend { get; set; }
        string Font { get; set; }
        double ChartHeight { get; set; }

        Color ChartColor { get; set; }
        Color ChartProgressColor { get; set; }
        Color BackgroundColor { get; set; }
        Color ImageDimColor { get; set; }
        Color TitleTextColor { get; set; }
        Color ArtistTextColor { get; set; }
        Color PpBackgroundColor { get; set; }
        Color Hit100BackgroundColor { get; set; }
        Color Hit50BackgroundColor { get; set; }
        Color HitMissBackgroundColor { get; set; }
    }
}