using System.ComponentModel;
using ScottPlot;

namespace LiveVisualizer
{
    public class DataPlotBinding : INotifyPropertyChanged
    {
#pragma warning disable 0067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore 0067
        public WpfPlot WpfPlot { get; set; }
        public IWpfVisualizerData Data { get; set; }
    }
}