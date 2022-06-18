using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Grace.DependencyInjection;
using osu_StreamCompanion.Code.Core.Loggers;
using osu_StreamCompanion.Code.Core.Maps.Processing;
using osu_StreamCompanion.Code.Core.Savers;
using osu_StreamCompanion.Code.Misc;
using osu_StreamCompanion.Code.Modules.FirstRun;
using osu_StreamCompanion.Code.Modules.osuFallbackDetector;
using osu_StreamCompanion.Code.Modules.osuPathReslover;
using osu_StreamCompanion.Code.Modules.Updater;
using StreamCompanionTypes;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Interfaces;
using StreamCompanionTypes.Enums;
using StreamCompanionTypes.Interfaces.Consumers;
using StreamCompanionTypes.Interfaces.Services;

namespace osu_StreamCompanion.Code.Core
{
    internal class Initializer : ApplicationContext
    {
        private readonly ILogger _logger;
        private readonly SettingNames _names = SettingNames.Instance;
        public readonly Settings Settings;

        public Initializer(string settingsProfileName)
        {
            new FileChecker();

            var di = DiContainer.Container;

            var mainLogger = MainLogger.Instance;
            _logger = mainLogger;
            Settings = di.Locate<Settings>();

            if (!string.IsNullOrEmpty(settingsProfileName))
            {
                Settings.ConfigFileName = Settings.ConfigFileName.Replace(".ini", $".{settingsProfileName}.ini");
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
            _logger.Log("Created DI container", LogLevel.Information);
        }

        public void Start()
        {
            _logger.Log("Booting up...", LogLevel.Information);
            _logger.Log($"Stream Companion Version: {Program.ScVersion}", LogLevel.Information);
            _logger.Log($"Running as { (Environment.Is64BitProcess ? "x64" : "x86")} process on .NET {Environment.Version}", LogLevel.Information);

            DiContainer.Container.Locate<Updater>();
            DiContainer.Container.Locate<FirstRun>();

            DiContainer.Container.Locate<OsuPathResolver>();
            DiContainer.Container.Locate<OsuFallbackDetector>();

            var lazyPluginMetas = DiContainer.Container.Locate<List<Meta<Lazy<IPlugin>>>>();
            _logger.Log("Initializing {0} plugins", LogLevel.Information, lazyPluginMetas.Count.ToString());

            foreach (var lazyPluginMeta in lazyPluginMetas)
            {
                var pluginType = lazyPluginMeta.Metadata.ActivationType;
                _logger.Log($">loading \"{pluginType.FullName}\" v: {pluginType.Assembly.GetName().Version}", LogLevel.Trace);

                IPlugin plugin = null;
                try
                {
                    plugin = lazyPluginMeta.Value.Value;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Plugin \"{pluginType.FullName}\" could not get initialized. StreamCompanion will most likely continue to work, however some features might be missing." +
                                    Environment.NewLine + Environment.NewLine + "Errors:" +
                                    Environment.NewLine +
                                    ex,
                        "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    _logger.Log(ex, LogLevel.Error);
                }

                if (plugin != null)
                    _logger.Log(">loaded \"{0}\" by {1} ({2}) v:{3}", LogLevel.Debug, plugin.Name, plugin.Author,
                        plugin.GetType().FullName, plugin.GetType().Assembly.GetName().Version.ToString());
            }

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