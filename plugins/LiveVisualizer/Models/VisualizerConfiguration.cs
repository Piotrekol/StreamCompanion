using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media;

namespace LiveVisualizer
{
    public class VisualizerConfiguration : IVisualizerConfiguration
    {
#pragma warning disable 0067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore 0067

        public Color ChartColor { get; set; } = Color.FromArgb(80, 255, 178, 227);
        public Color ChartProgressColor { get; set; } = Color.FromArgb(140, 255, 178, 227);
        public bool SimulatePPWhenListening { get; set; } = true;
        public bool HideDiffText { get; set; } = false;
        public bool HideMapStats { get; set; } = false;
        public string Font { get; set; } = "Arial";
        public double ChartHeight { get; set; } = 150;
        public Color BackgroundColor { get; set; } = Color.FromArgb(255, 0, 0, 0);
        public Color ImageDimColor { get; set; } = Color.FromArgb(102, 0, 0, 0);
        public Color TitleTextColor { get; set; } = Color.FromArgb(255, 232, 232, 232);
        public Color ArtistTextColor { get; set; } = Color.FromArgb(255, 203, 203, 203);
        public Color PpBackgroundColor { get; set; } = Color.FromArgb(102, 0, 0, 0);
        public Color Hit100BackgroundColor { get; set; } = Color.FromArgb(170, 50, 205, 50); //LimeGreen
        public Color Hit50BackgroundColor { get; set; } = Color.FromArgb(170, 138, 43, 226); //BlueViolet
        public Color HitMissBackgroundColor { get; set; } = Color.FromArgb(170, 255, 69, 0); //OrangeRed
    }
}