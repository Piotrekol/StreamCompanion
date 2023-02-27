using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using CollectionManager.DataTypes;
using StreamCompanion.Common;
using StreamCompanionTypes;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Enums;
using StreamCompanionTypes.Interfaces;
using StreamCompanionTypes.Interfaces.Consumers;
using StreamCompanionTypes.Interfaces.Services;
using StreamCompanionTypes.Interfaces.Sources;

namespace ScGui
{
    class MainWindowPlugin : IPlugin, IMapDataConsumer, ISettingsSource
    {
        private readonly SettingNames _names = SettingNames.Instance;
        public static ConfigEntry minimizeToTaskbar = new ConfigEntry($"{nameof(ScGui)}_minimizeToTaskbar", false);
        public static ConfigEntry Theme = new ConfigEntry($"{nameof(ScGui)}_theme", "System default");
        private ISettings _settings;
        private MainWindow _mainWindow;
        private SettingsForm _settingsForm = null;
        private NotifyIcon _notifyIcon;

        private IMainWindowModel _mainWindowModel;
        private readonly Delegates.Exit _exitAction;
        private readonly ILogger _logger;
        private List<Lazy<ISettingsSource>> _settingsList;

        public string Description { get; } = "";
        public string Name { get; } = "StreamCompanion GUI";
        public string Author { get; } = "Piotrekol";
        public string Url { get; } = "";
        public string UpdateUrl { get; } = "";
        public string SettingGroup { get; } = "General";

        private static string BaseAddress(ISettings settings) => $"http://localhost:{settings.GetRaw("httpServerPort", "20727")}/";

        private NotifyIcon CreateNotifyIcon()
        {
            if (!OperatingSystem.IsWindows())
                return null;

            var cms = new ContextMenuStrip
            {
                Items =
                {
                    new ToolStripMenuItem("Show",null,(_, __) => ShowWindow()),
                    new ToolStripSeparator(),
                    new ToolStripMenuItem("Minimize to taskbar",null,(_,__)=>
                    {
                        _settings.Add(minimizeToTaskbar.Name, !_settings.Get<bool>(minimizeToTaskbar));
                    }){Checked = _settings.Get<bool>(minimizeToTaskbar), CheckOnClick = true},
                    new ToolStripMenuItem("Start minimized",null,(_,__)=>
                    {
                        _settings.Add(_names.StartHidden.Name, !_settings.Get<bool>(_names.StartHidden));
                    }){Checked = _settings.Get<bool>(_names.StartHidden), CheckOnClick = true},
                    new ToolStripSeparator(),
                    new ToolStripMenuItem("Close",null,(_, __) => Quit(false))
                }
            };
            var notifyIcon = new NotifyIcon()
            {
                ContextMenuStrip = cms,
                Text = "StreamCompanion",
                Icon = Icon.ExtractAssociatedIcon(Assembly.GetEntryAssembly().Location),
                Visible = true
            };

            notifyIcon.MouseDoubleClick += (_, __) => ShowWindow();

            return notifyIcon;
        }

        public MainWindowPlugin(ISettings settings, IMainWindowModel mainWindowModel, List<Lazy<ISettingsSource>> settingsSources, Delegates.Exit exitAction, ILogger logger)
        {
            _settings = settings;
            _settings.SettingUpdated += SettingUpdated;
            _mainWindowModel = mainWindowModel;
            _exitAction = exitAction;
            _logger = logger;
            _settingsList = settingsSources;
            _settingsList.Add(new Lazy<ISettingsSource>(this));

            if (!_settings.Get<bool>(_names.StartHidden))
                ShowWindow();

            _notifyIcon = CreateNotifyIcon();
        }

        private void SettingUpdated(object sender, SettingUpdated e)
        {
            if (e.Name == Theme.Name)
            {
                _mainWindow?.SetTheme(_settings.Get<string>(Theme));
            }
            else if (e.Name == _names.MainOsuDirectory.Name)
            {
                UpdateStatusText();
            }
        }

        private void UpdateStatusText()
        {
            var tourneyMode = _settings.GetRaw("TournamentMode").ToLowerInvariant() == "true";
            _mainWindowModel.BeatmapsLoaded = $"{(tourneyMode ? "In tourney mode! " : "")}{_settings.GetFullOsuLocation()}";
        }

        private void ShowWindow()
        {
            if (_mainWindow == null)
            {
                UpdateStatusText();
                _mainWindow = new MainWindow(_mainWindowModel, _settings);
                _mainWindow.OnOpenSettingsClicked += (_, __) =>
                {
                    if (_settingsForm != null)
                    {
                        _settingsForm.Focus();
                        return;
                    }

                    List<ISettingsSource> settingsSources = new();
                    foreach (var settingSource in _settingsList)
                    {
                        try
                        {
                            settingsSources.Add(settingSource.Value);
                        }
                        catch (Exception)
                        {
                            _logger.Log($"Failed to load settings for one of setting tabs, this is most likely accompanied by plugin load error at startup.", LogLevel.Error);
                        }
                    }
                    
                    _settingsForm = new SettingsForm(settingsSources);
                    _settingsForm.Closed += SettingsFormOnClosed;
                    _settingsForm.Show();
                };
                _mainWindow.Closed += (_, __) => Quit(true);
                _mainWindow.OnOpenInfoClicked += (_, __) =>
                {
                    var aboutFrm = new AboutForm();
                    aboutFrm.ShowDialog();
                };
                _mainWindow.OnUpdateClicked += (sender, args) => _mainWindowModel.UpdateTextClicked(sender, args);
                _mainWindow.OnPpClicked += (_, __) => ProcessExt.OpenUrl(BaseAddress(_settings));
                _mainWindow.OnWikiClicked += (_, __) => ProcessExt.OpenUrl("https://piotrekol.github.io/StreamCompanion/");
                _mainWindow.Show();

            }
            else if (_mainWindow.WindowState == WindowState.Minimized)
            {
                _mainWindow.Show();
                _mainWindow.Dispatcher.Invoke(() =>
                {
                    _mainWindow.Focusable = true;
                    _mainWindow.Focus();
                });

            }
        }

        private void Quit(bool fromClosedEvent)
        {
            if (!fromClosedEvent && _mainWindow != null)
            {
                _mainWindow.Close();
                return;
            }

            _settingsForm?.Close();
            if (_notifyIcon != null)
                _notifyIcon.Visible = false;
            _settings.Save();
            _exitAction?.Invoke("User pressed exit");
        }

        private void SettingsFormOnClosed(object sender, EventArgs e)
        {
            _settingsForm.Closed -= SettingsFormOnClosed;
            _settingsForm?.Dispose();
            _settingsForm = null;
        }

        public Task SetNewMapAsync(IMapSearchResult map, CancellationToken cancellationToken)
        {
            if (map.BeatmapsFound.Any())
            {
                var foundMap = map.BeatmapsFound[0];
                var nowPlaying = string.Format("{0} - {1}", foundMap.ArtistRoman, foundMap.TitleRoman);
                if (map.Action == OsuStatus.Playing || map.Action == OsuStatus.Watching || map.MapSource != "Msn")
                {
                    nowPlaying += $" [{foundMap.DiffName}] {map.Mods?.ShownMods ?? ""}";
                    nowPlaying += $"{Environment.NewLine}NoMod:{foundMap.StarsNomod:##.###} ";

                    var mods = map.Mods?.Mods ?? Mods.Omod;
                    var token = Tokens.AllTokens.FirstOrDefault(t => t.Key.ToLower() == "mstars").Value;
                    if (mods != Mods.Omod && token != null)
                        nowPlaying += $"Modded: {token.Value:##.###} ";

                    nowPlaying += $"{map.Action}";
                }
                _mainWindowModel.NowPlaying = nowPlaying;
            }
            else
            {
                _mainWindowModel.NowPlaying = "Map data not found: " + map.MapSearchString;
            }

            return Task.CompletedTask;
        }

        public void Free()
        {
            _scGuiSettings?.Dispose();
        }

        private ScGuiSettings _scGuiSettings = null;
        public object GetUiSettings()
        {
            if (_scGuiSettings == null || _scGuiSettings.IsDisposed)
            {
                _scGuiSettings = new ScGuiSettings(_settings);
            }

            return _scGuiSettings;
        }
    }
}
