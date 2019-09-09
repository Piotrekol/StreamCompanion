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

namespace osu_StreamCompanion.Code.Core
{
    internal class Initializer : ApplicationContext
    {
        private readonly MainLogger _logger;
        private readonly SettingNames _names = SettingNames.Instance;
        public readonly Settings Settings;

        public Initializer()
        {
            new FileChecker();

            var di = DiContainer.Container;

            _logger = di.Locate<MainLogger>();
            var saver = di.Locate<MainSaver>();
            Settings = di.Locate<Settings>();

            Settings.Load();

            if (Settings.Get<bool>(_names.Console))
            {
                _logger.ChangeLogger(new ConsoleLogger(Settings));
            }
            else
            {
                _logger.ChangeLogger(new EmptyLogger());
            }

            _logger.AddLogger(new FileLogger(saver, Settings));
#if !DEBUG
            _logger.AddLogger(new SentryLogger());
#endif
            _logger.Log("Created DI container", LogLevel.Basic);
        }

        public void Start()
        {
            _logger.Log("Booting up...", LogLevel.Basic);

            DiContainer.Container.Locate<FirstRun>();

            DiContainer.Container.Locate<ISqliteControler>();
            DiContainer.Container.Locate<OsuPathResolver>();
            DiContainer.Container.Locate<OsuFallbackDetector>();

            var plugins = DiContainer.Container.LocateAll(typeof(IPlugin)).Cast<IPlugin>().ToList();

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
            Settings.Save();
            DiContainer.Container.Dispose();
        }
    }
}