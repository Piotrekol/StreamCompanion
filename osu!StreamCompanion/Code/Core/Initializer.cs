using System;
using System.Windows.Forms;
using osu_StreamCompanion.Code.Core.Loggers;
using osu_StreamCompanion.Code.Core.Maps.Processing;
using osu_StreamCompanion.Code.Core.Savers;
using osu_StreamCompanion.Code.Misc;
using osu_StreamCompanion.Code.Modules.osuFallbackDetector;
using osu_StreamCompanion.Code.Modules.osuPathReslover;
using osu_StreamCompanion.Code.Modules.Updater;
using StreamCompanionTypes;
using StreamCompanionTypes.Enums;
using StreamCompanionTypes.Interfaces.Services;
using osu_StreamCompanion.Code.Modules;
using osu_StreamCompanion.Code.Core.Plugins;

namespace osu_StreamCompanion.Code.Core
{
    internal class Initializer : ApplicationContext
    {
        private readonly ILogger _logger;
        private readonly SettingNames _names = SettingNames.Instance;
        public readonly Settings Settings;
        private readonly LocalPluginManager _pluginManager;
        public Initializer(string settingsProfileName)
        {
            new FileChecker();

            var di = DiContainer.Container;

            var mainLogger = MainLogger.Instance;
            _logger = mainLogger;
            Settings = di.Locate<Settings>();

            if (!string.IsNullOrEmpty(settingsProfileName))
            {
                Settings.ConfigFileName = Settings.ConfigFileName.Replace(".json", $".{settingsProfileName}.json");
            }

            Settings.Load();

            if ((Control.ModifierKeys & Keys.Shift) != 0 &&
                (Control.ModifierKeys & Keys.Control) != 0)
            {
                Settings.Add(SettingNames.Instance.Console.Name, true, true);
                Settings.Add(SettingNames.Instance.LogLevel.Name, LogLevel.Trace.GetHashCode(), true);
            }

            var saver = di.Locate<MainSaver>();

            if (Settings.Get<bool>(_names.Console) && OperatingSystem.IsWindows())
            {
                mainLogger.AddLogger(new ConsoleLogger(Settings));
            }
            else
            {
                mainLogger.AddLogger(new EmptyLogger());
            }

            mainLogger.AddLogger(new FileLogger(saver, Settings, mainLogger));
#if !DEBUG
            mainLogger.AddLogger(new SentryLogger());
#endif

            _pluginManager = new LocalPluginManager(Settings, mainLogger);
            foreach (var moduleType in _pluginManager.GetModules())
            {
                di.Configure(x => x.ExportDefault(moduleType));
            }

            foreach (var pluginType in _pluginManager.GetPlugins())
            {
                di.Configure(x => x.ExportDefault(pluginType));
            }

            _logger.Log("Created DI container", LogLevel.Information);
        }

        public void Start()
        {
            _logger.Log("Booting up...", LogLevel.Information);
            _logger.Log($"Stream Companion Version: {Program.ScVersion}", LogLevel.Information);
            _logger.Log($"Running as {(Environment.Is64BitProcess ? "x64" : "x86")} process on .NET {Environment.Version}", LogLevel.Information);

            DiContainer.Container.Locate<Updater>();
            DiContainer.Container.Locate<AdministratorChecker>();

            DiContainer.Container.Locate<OsuPathResolver>();
            DiContainer.Container.Locate<OsuFallbackDetector>();

            _pluginManager.StartPlugins(DiContainer.Container);

            Settings.Add(_names.FirstRun.Name, false);
            Settings.Add(_names.LastRunVersion.Name, Program.ScVersion);

            DiContainer.Container.Locate<OsuEventHandler>();

            _logger.Log("Started!", LogLevel.Information);
        }

        public void Exit()
        {
            DiContainer.Container.Dispose();
            Settings.Save();
        }
    }
}