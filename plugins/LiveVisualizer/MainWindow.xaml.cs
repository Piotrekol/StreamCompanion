using System.Runtime.Versioning;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using ScottPlot;
using Color = System.Drawing.Color;

namespace LiveVisualizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [SupportedOSPlatform("windows7.0")]
    public partial class MainWindow : Window
    {
        private PlottableSignal BackgroundPlottable;
        private PlottableSignal ForegroundPlottable;
        public MainWindow(IWpfVisualizerData data)
        {
            RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;
            InitializeComponent();
            this.frameholder.Visibility = Visibility.Hidden;
            this.frameholderTimer.Visibility = Visibility.Hidden;
            this.DataContext = data;

            var backgroundFill = ColorHelpers.Convert(data.Configuration.FillColor);
            BackgroundPlottable = data.Display.BackgroundDataPlot.plt.PlotSignal(new double[] { 1, 10, 5, 15, 10, 5, 0 }, lineWidth: 3, markerSize: 0, color: backgroundFill);
            BackgroundPlottable.fillColor1 = backgroundFill;
            BackgroundPlottable.fillType = FillType.FillBelow;

            var foregroundFill = ColorHelpers.Convert(data.Configuration.ChartProgressColor);
            ForegroundPlottable = data.Display.MainDataPlot.plt.PlotSignal(new double[] { 1, 10, 5, 15, 10, 5, 0 }, lineWidth: 3, markerSize: 0, color: foregroundFill);
            ForegroundPlottable.fillColor1 = foregroundFill;
            ForegroundPlottable.fillType = FillType.FillBelow;

            this.frameholder.Content = new Chart(new DataPlotBinding { WpfPlot = data.Display.BackgroundDataPlot, Data = data });
            this.frameholderTimer.Content = new Chart(new DataPlotBinding { WpfPlot = data.Display.MainDataPlot, Data = data });
        }

        public void UpdateChart()
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(UpdateChart);
                return;
            }
            var data = (IWpfVisualizerData)DataContext;

            var strains = data.Display.Strains.ToArray();
            if (strains.Length == 0)
            {
                this.frameholder.Visibility = Visibility.Hidden;
                this.frameholderTimer.Visibility = Visibility.Hidden;
                return;
            }
            this.frameholder.Visibility = Visibility.Visible;
            this.frameholderTimer.Visibility = Visibility.Visible;

            var backgroundFill = ColorHelpers.Convert(data.Configuration.ChartColor);
            var foregroundFill = ColorHelpers.Convert(data.Configuration.ChartProgressColor);

            BackgroundPlottable.ys = ForegroundPlottable.ys = strains;
            BackgroundPlottable.maxRenderIndex = ForegroundPlottable.maxRenderIndex = BackgroundPlottable.ys.Length - 1;
            BackgroundPlottable.samplePeriod = ForegroundPlottable.samplePeriod = 1.0 / BackgroundPlottable.sampleRate;
            BackgroundPlottable.fillColor1 = BackgroundPlottable.color = backgroundFill;
            ForegroundPlottable.fillColor1 = ForegroundPlottable.color = foregroundFill;

            data.Display.MainDataPlot.plt.Axis(0, data.Display.Strains.Count, 0, data.Configuration.MaxYValue);
            data.Display.MainDataPlot.plt.Layout(0, 0, 0, 0, 0, 0, 0);
            data.Display.MainDataPlot.Render();
            data.Display.BackgroundDataPlot.plt.Axis(0, data.Display.Strains.Count, 0, data.Configuration.MaxYValue);
            data.Display.BackgroundDataPlot.plt.Layout(0, 0, 0, 0, 0, 0, 0);
            data.Display.BackgroundDataPlot.Render();
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
