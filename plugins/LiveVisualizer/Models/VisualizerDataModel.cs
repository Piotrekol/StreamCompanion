using System.ComponentModel;

namespace LiveVisualizer
{
    public class VisualizerDataModel : IWpfVisualizerData
    {
        public IVisualizerDisplayData Display { get; set; } = new VisualizerDisplayData();
        public IVisualizerConfiguration Configuration { get; set; } = new VisualizerConfiguration();

#pragma warning disable 0067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore 0067

    }
}