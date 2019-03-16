using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media;

namespace LiveVisualizer
{
    public class VisualizerConfiguration : IVisualizerConfiguration
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public double FontsizeTitle { get; set; } = 15;
        public double FontsizeArtist { get; set; } = 15;
        public Color ChartColor { get; set; } = Color.FromArgb(102, 255, 178, 227);
        public Color ChartProgressColor { get; set; } = Color.FromArgb(178, 255, 178, 227);
        public double MaxYValue { get; set; } = 350;
        public Color FillColor { get; set; } = Color.FromArgb(178, 255, 178, 227);
        public bool Enable { get; set; } = true;
        public string Font { get; set; } = "Arial";
        public bool ShowAxisYSeparator { get; set; } = true;
        public Color AxisYSeparatorColor { get; set; } = Color.FromArgb(102, 255, 178, 227);
        public double AxisYStep { get; set; } = 100;
        public bool AutoSizeAxisY { get; set; } = true;
        public double WindowHeight { get; set; } = 350;
        public double WindowWidth { get; set; } = 700;
        public bool EnableResizing { get; set; } = false;
        public Color BackgroundColor { get; set; } = Color.FromArgb(255, 0, 0, 0);
        public Color ImageDimColor { get; set; } = Color.FromArgb(102, 0, 0, 0);
        public SortedSet<int> ChartCutoffsSet { get; set; } = new SortedSet<int>();
    }
}