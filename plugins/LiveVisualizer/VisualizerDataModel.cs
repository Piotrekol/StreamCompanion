using LiveCharts;
using LiveVisualizer.Annotations;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LiveVisualizer
{
    public class VisualizerDataModel : IWpfVisualizerData, INotifyPropertyChanged
    {
        //private List<double> _strains = new List<double>(300);
        private double _currentTime;
        public event PropertyChangedEventHandler PropertyChanged;



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


        public double FontsizeTitle { get; set; } = 15;
        public double FontsizeArtist { get; set; } = 15;
        public double PixelMapProgress { get; set; }
        public string ChartColor { get; set; } = "#66FFB2E3";
        public string ChartProgressColor { get; set; } = "#B2FFB2E3";
        public double MaxYValue { get; set; } = 350;
        public string FillColor { get; set; } = "#B2FFB2E3";
        public string Font { get; set; }
        public bool ShowAxisYSeparator { get; set; }
        public string AxisYSeparatorColor { get; set; }
        public double AxisYStep { get; set; } = 100;
    }
}