using System;
using System.Linq;
using System.Windows.Forms;
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

            var saver = di.Locate<MainSaver>();

            if (Settings.Get<bool>(_names.Console))
            {
                mainLogger.AddLogger(new ConsoleLogger(Settings));
            }
            else
            {
                mainLogger.AddLogger(new EmptyLogger());
            }

            mainLogger.AddLogger(new FileLogger(saver, Settings));
#if !DEBUG
            mainLogger.AddLogger(new SentryLogger());
#endif
            _logger.Log("Created DI container", LogLevel.Basic);
        }

        public void Start()
        {
            _logger.Log("Booting up...", LogLevel.Basic);

            DiContainer.Container.Locate<FirstRun>();

            DiContainer.Container.Locate<IDatabaseController>();
            DiContainer.Container.Locate<OsuPathResolver>();
            DiContainer.Container.Locate<OsuFallbackDetector>();

            var plugins = DiContainer.Container.LocateAll(typeof(IPlugin)).Cast<IPlugin>().ToList();

            //TODO: remove this check after few releases (2020?)
            var ingameOverlay = plugins.FirstOrDefault(p => p.Name == "IngameOverlay");
            if (ingameOverlay != null && ingameOverlay is IMapDataConsumer mapDataConsumer)
            {
                try
                {
                    mapDataConsumer.SetNewMap(new MapSearchResult(new MapSearchArgs("dummy")));
                }
                catch (Exception) 
                {
                    MessageBox.Show(
                        $"IngameOverlay plugin version is not valid for this version of StreamCompanion. {Environment.NewLine} Either update or remove it from plugins folder",
                        "osu!StreamCompanion Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Program.SafeQuit();
                }
            }

            _logger.Log(">Initialized {0} plugins", LogLevel.Basic, plugins.Count.ToString());

            foreach (var plugin in plugins)
            {
                _logger.Log(">>plugin \"{0}\" by {1} ({2})", LogLevel.Advanced, plugin.Name, plugin.Author,
                    plugin.GetType().FullName);
            }

            Settings.Add(_names.FirstRun.Name, false);
            Settings.Add(_names.LastRunVersion.Name, Program.ScVersion);

            DiContainer.Container.Locate<MapStringFormatter>();
            DiContainer.Container.Locate<Updater>();

            _logger.Log("Started!", LogLevel.Basic);
        }

        public void Exit()
        {
            DiContainer.Container.Dispose();
            Settings.Save();
        }
    }
}
