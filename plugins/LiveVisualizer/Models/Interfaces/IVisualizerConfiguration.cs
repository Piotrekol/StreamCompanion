using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media;

namespace LiveVisualizer
{
    public interface IVisualizerConfiguration : INotifyPropertyChanged
    {
        bool Enable { get; set; }
        bool SimulatePPWhenListening { get; set; }
        bool EnableRoundedCorners { get; set; }
        string BottomHeight { get; set; }
        string Font { get; set; }
        double FontsizeTitle { get; set; }
        double FontsizeArtist { get; set; }
        double MaxYValue { get; set; }

        bool ShowAxisYSeparator { get; set; }
        double AxisYStep { get; set; }
        bool AutoSizeAxisY { get; set; }

        double WindowHeight { get; set; }
        double WindowWidth { get; set; }
        bool EnableResizing { get; set; }
        double ChartHeight { get; set; }

        SortedSet<int> ChartCutoffsSet { get; set; }

        Color ChartColor { get; set; }
        Color ChartProgressColor { get; set; }
        Color AxisYSeparatorColor { get; set; }
        Color FillColor { get; set; }
        Color BackgroundColor { get; set; }
        Color ImageDimColor { get; set; }
        Color TitleTextColor { get; set; }
        Color ArtistTextColor { get; set; }
    }
}