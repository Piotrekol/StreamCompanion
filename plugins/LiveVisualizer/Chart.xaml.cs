using LiveCharts;
using LiveCharts.Helpers;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Data;
using LiveCharts.Wpf;
using LiveVisualizer.Annotations;

namespace LiveVisualizer
{
    /// <summary>
    /// Interaction logic for Chart.xaml
    /// </summary>
    public partial class Chart : UserControl
    {
        public Chart(IWpfVisualizerData data, string bindingName)
        {
            InitializeComponent();

            var binding = new Binding(bindingName);
            binding.Source = data;
            LineSeries.SetBinding(Series.FillProperty, binding);

            DataContext = data;
        }
    }
}
