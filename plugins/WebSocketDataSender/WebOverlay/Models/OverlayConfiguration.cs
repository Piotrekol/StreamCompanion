using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace WebSocketDataSender.WebOverlay.Models
{
    public class OverlayConfiguration : IOverlayConfiguration
    {
#pragma warning disable 0067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore 0067

        private Color chartColor { get; set; } = Color.FromArgb(80, 255, 178, 227);
        private Color chartProgressColor { get; set; } = Color.FromArgb(140, 255, 178, 227);
        private bool simulatePPWhenListening { get; set; } = true;
        private bool hideDiffText { get; set; } = false;
        private bool hideMapStats { get; set; } = false;
        private bool hideChartLegend { get; set; } = false;
        private string font { get; set; } = "Arial";
        private double chartHeight { get; set; } = 150;
        private Color backgroundColor { get; set; } = Color.FromArgb(255, 0, 0, 0);
        private Color imageDimColor { get; set; } = Color.FromArgb(102, 0, 0, 0);
        private Color titleTextColor { get; set; } = Color.FromArgb(255, 232, 232, 232);
        private Color artistTextColor { get; set; } = Color.FromArgb(255, 203, 203, 203);
        private Color ppBackgroundColor { get; set; } = Color.FromArgb(102, 0, 0, 0);
        private Color hit100BackgroundColor { get; set; } = Color.FromArgb(170, 50, 205, 50); //LimeGreen
        private Color hit50BackgroundColor { get; set; } = Color.FromArgb(170, 138, 43, 226); //BlueViolet
        private Color hitMissBackgroundColor { get; set; } = Color.FromArgb(170, 255, 69, 0); //OrangeRed

        #region propertychanged boilerplate
        public Color ChartColor
        {
            get => chartColor;
            set
            {
                chartColor = value;
                OnPropertyChanged();
            }
        }
        public Color ChartProgressColor
        {
            get => chartProgressColor;
            set
            {
                chartProgressColor = value;
                OnPropertyChanged();
            }
        }
        public bool SimulatePPWhenListening
        {
            get => simulatePPWhenListening;
            set
            {
                simulatePPWhenListening = value;
                OnPropertyChanged();
            }
        }
        public bool HideDiffText
        {
            get => hideDiffText;
            set
            {
                hideDiffText = value;
                OnPropertyChanged();
            }
        }
        public bool HideMapStats
        {
            get => hideMapStats;
            set
            {
                hideMapStats = value;
                OnPropertyChanged();
            }
        }
        public bool HideChartLegend
        {
            get => hideChartLegend;
            set
            {
                hideChartLegend = value;
                OnPropertyChanged();
            }
        }
        public string Font
        {
            get => font;
            set
            {
                font = value;
                OnPropertyChanged();
            }
        }
        public double ChartHeight
        {
            get => chartHeight;
            set
            {
                chartHeight = value;
                OnPropertyChanged();
            }
        }
        public Color BackgroundColor
        {
            get => backgroundColor;
            set
            {
                backgroundColor = value;
                OnPropertyChanged();
            }
        }
        public Color ImageDimColor
        {
            get => imageDimColor;
            set
            {
                imageDimColor = value;
                OnPropertyChanged();
            }
        }
        public Color TitleTextColor
        {
            get => titleTextColor;
            set
            {
                titleTextColor = value;
                OnPropertyChanged();
            }
        }
        public Color ArtistTextColor
        {
            get => artistTextColor;
            set
            {
                artistTextColor = value;
                OnPropertyChanged();
            }
        }
        public Color PpBackgroundColor
        {
            get => ppBackgroundColor;
            set
            {
                ppBackgroundColor = value;
                OnPropertyChanged();
            }
        }
        public Color Hit100BackgroundColor
        {
            get => hit100BackgroundColor;
            set
            {
                hit100BackgroundColor = value;
                OnPropertyChanged();
            }
        }
        public Color Hit50BackgroundColor
        {
            get => hit50BackgroundColor;
            set
            {
                hit50BackgroundColor = value;
                OnPropertyChanged();
            }
        }
        public Color HitMissBackgroundColor
        {
            get => hitMissBackgroundColor;
            set
            {
                hitMissBackgroundColor = value;
                OnPropertyChanged();
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion
    }
}