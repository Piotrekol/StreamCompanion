using System.ComponentModel;

namespace LiveVisualizer
{
    public interface IWpfVisualizerData : INotifyPropertyChanged
    {
        IVisualizerDisplayData Display { get; set; }
        IVisualizerConfiguration Configuration { get; set; }
    }
}