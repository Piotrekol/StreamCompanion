using System.Windows;
using System.Windows.Input;

namespace LiveVisualizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //TODO: Resizing of transparent wpf window is not supported natively.(xaml)
        public MainWindow(IWpfVisualizerData data)
        {
            InitializeComponent();

            this.DataContext = data;

            this.frameholder.Content = new Chart(data, $"{nameof(IWpfVisualizerData.Configuration)}.{nameof(IWpfVisualizerData.Configuration.ChartColor)}", false);
            this.frameholderTimer.Content = new Chart(data, $"{nameof(IWpfVisualizerData.Configuration)}.{nameof(IWpfVisualizerData.Configuration.ChartProgressColor)}", true);
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void MainWindow_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var data = (IWpfVisualizerData)DataContext;
            if (e.HeightChanged)
                data.Configuration.WindowHeight = e.NewSize.Height;
            if (e.WidthChanged)
                data.Configuration.WindowWidth = e.NewSize.Width;
        }
    }
}
