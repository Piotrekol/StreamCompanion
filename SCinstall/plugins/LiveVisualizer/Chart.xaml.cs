using System.Runtime.Versioning;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace LiveVisualizer
{

    /// <summary>
    /// Interaction logic for Chart.xaml
    /// </summary>
    [SupportedOSPlatform("windows7.0")]
    public partial class Chart : UserControl
    {
        public Chart(DataPlotBinding data)
        {
            InitializeComponent();

            DataContext = data;
        }
        
    }
}
