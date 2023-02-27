using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using StreamCompanionTypes.Interfaces;
using StreamCompanionTypes.Interfaces.Services;

namespace ScGui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ISettings _settings;
        public event EventHandler OnOpenSettingsClicked;
        public event EventHandler OnOpenInfoClicked;
        public event EventHandler OnUpdateClicked;
        public event EventHandler OnPpClicked;
        public event EventHandler OnWikiClicked;

        public MainWindow(IMainWindowModel data, ISettings settings)
        {
            SourceInitialized += Window_SourceInitialized;

            _settings = settings;
            RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;

            InitializeComponent();

            Title = $"StreamCompanion ({(Environment.Is64BitProcess ? "x64" : "x86")})";
            SetTheme(_settings.Get<string>(MainWindowPlugin.Theme));
            Style = (Style)FindResource(typeof(Window));
            DataContext = data;
            StateChanged += OnStateChanged;
        }

        public void SetTheme(string themeName)
        {
            if (themeName == "System default" && OperatingSystem.IsWindows())
                themeName = WindowsThemeHelper.GetWindowsTheme() == WindowsThemeHelper.WindowsTheme.Light
                    ? "Light"
                    : "Dark";

            Resources.MergedDictionaries[0].Source = new Uri($"./themes/{themeName}.xaml", UriKind.Relative);
        }

        private void OnStateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Minimized && _settings.Get<bool>(MainWindowPlugin.minimizeToTaskbar))
                Hide();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void ButtonSettings_OnClick(object sender, RoutedEventArgs e)
        {
            OnOpenSettingsClicked?.Invoke(this, EventArgs.Empty);
        }

        private void ButtonInfo_OnClick(object sender, RoutedEventArgs e)
        {
            OnOpenInfoClicked?.Invoke(this, EventArgs.Empty);
        }

        private void ButtonPp_OnClick(object sender, RoutedEventArgs e)
        {
            OnPpClicked?.Invoke(this, EventArgs.Empty);
        }

        private void LabelUpdate_OnClick(object sender, RoutedEventArgs e)
        {
            OnUpdateClicked?.Invoke(this, EventArgs.Empty);
        }

        private void ButtonWiki_OnClick(object sender, RoutedEventArgs e)
        {
            OnWikiClicked?.Invoke(this, EventArgs.Empty);
        }

        #region Disable Maximize button

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        private const int GWL_STYLE = -16;
        private const int WS_MAXIMIZEBOX = 0x10000;

        private void Window_SourceInitialized(object sender, EventArgs e)
        {
            var hwnd = new WindowInteropHelper((Window)sender).Handle;
            var value = GetWindowLong(hwnd, GWL_STYLE);
            SetWindowLong(hwnd, GWL_STYLE, (int)(value & ~WS_MAXIMIZEBOX));
        }

        #endregion
    }
}
