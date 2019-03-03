using LiveCharts;
using LiveCharts.Helpers;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;

namespace LiveVisualizer
{
    /// <summary>
    /// Interaction logic for Chart.xaml
    /// </summary>
    public partial class Chart : UserControl
    {
        public Chart(IWpfVisualizerData data)
        {
            InitializeComponent();
            DataContext = data;
        }
    }
}
