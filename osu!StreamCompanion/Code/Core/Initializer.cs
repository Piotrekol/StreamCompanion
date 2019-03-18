using osu_StreamCompanion.Code.Core.Loggers;
using osu_StreamCompanion.Code.Core.Maps;
using osu_StreamCompanion.Code.Core.Maps.Processing;
using osu_StreamCompanion.Code.Core.Savers;
using osu_StreamCompanion.Code.Helpers;
using osu_StreamCompanion.Code.Misc;
using osu_StreamCompanion.Code.Modules.CommandsPreview;
using osu_StreamCompanion.Code.Modules.Donation;
using osu_StreamCompanion.Code.Modules.FileSaveLocation;
using osu_StreamCompanion.Code.Modules.FirstRun;
using osu_StreamCompanion.Code.Modules.MapDataFinders.NoData;
using osu_StreamCompanion.Code.Modules.MapDataFinders.osuMemoryID;
using osu_StreamCompanion.Code.Modules.MapDataFinders.SqliteData;
using osu_StreamCompanion.Code.Modules.MapDataParsers.Parser1;
using osu_StreamCompanion.Code.Modules.MapDataReplacements.Map;
using osu_StreamCompanion.Code.Modules.osuFallbackDetector;
using osu_StreamCompanion.Code.Modules.osuPathReslover;
using osu_StreamCompanion.Code.Modules.Updater;
using osu_StreamCompanion.Code.Windows;
using StreamCompanionTypes;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace osu_StreamCompanion.Code.Core
{
    internal class Initializer : ApplicationContext
    {
        private readonly SettingNames _names = SettingNames.Instance;

        public readonly MainWindowUpdater MainWindowUpdater = new MainWindowUpdater();
        readonly MainLogger _logger = new MainLogger();
        readonly MainSaver _saver;
        SqliteControler _sqliteControler;
        public List<IMsnGetter> MsnGetters = new List<IMsnGetter>();
        public readonly Settings Settings;
        private bool _started;
        public readonly string ConfigSaveLocation = AppDomain.CurrentDomain.BaseDirectory;

        private List<IModParser> _modParser = new List<IModParser>(1);
        private List<IModule> _modules = new List<IModule>();
        public List<ISettingsProvider> SettingsList = new List<ISettingsProvider>();
        private readonly List<IMapDataFinder> _mapDataFinders = new List<IMapDataFinder>();
        private readonly List<IMapDataParser> _mapDataParsers = new List<IMapDataParser>();
        private readonly List<IMapDataGetter> _mapDataGetters = new List<IMapDataGetter>();
        private readonly List<IMapDataReplacements> _mapDataReplacementSetters = new List<IMapDataReplacements>();

        private readonly List<IOsuEventSource> _osuEventSources = new List<IOsuEventSource>();
        private readonly List<IHighFrequencyDataHandler> _highFrequencyDataHandlers = new List<IHighFrequencyDataHandler>(
            );
        public Initializer()
        {
            new FileChecker();
            _saver = new MainSaver(_logger);
            this.Settings = new Settings(_logger);
            Settings.SetSavePath(ConfigSaveLocation);
            Settings.Load();

            if (Settings.Get<bool>(_names.Console))
                _logger.ChangeLogger(new ConsoleLogger(Settings));
            else
                _logger.ChangeLogger(new EmptyLogger());
            _logger.AddLogger(new FileLogger(_saver, Settings));
#if !DEBUG
            _logger.AddLogger(new SentryLogger());
#endif
            _logger.Log("booting up...", LogLevel.Basic);
        }

        public void Start()
        {
            if (_started)
                return;

            #region First run

            bool shouldForceFirstRun = false;
            var lastVersionStr = Settings.Get<string>(_names.LastRunVersion);
            if (lastVersionStr == _names.LastRunVersion.Default<string>())
            {
                shouldForceFirstRun = true;
            }
            else
            {
                try
                {
                    var lastVersion = Helpers.Helpers.GetDateFromVersionString(lastVersionStr);
                    var versionToResetOn = Helpers.Helpers.GetDateFromVersionString("v180209.13");
                    shouldForceFirstRun = lastVersion < versionToResetOn;
                }
                catch (Exception e)
                {
                    if (e is FormatException || e is ArgumentNullException)
                        shouldForceFirstRun = true;
                    else
                        throw;
                }
            }

            _logger.Log("Preloading plugins", LogLevel.Advanced);

            var plugins = GetPlugins();


            if (Settings.Get<bool>(_names.FirstRun) || shouldForceFirstRun)
            {
                var firstRunControls = new List<FirstRunUserControl>();

                var firstRunControlProviders =
                    plugins.Where(p => p.GetType().GetInterfaces().Contains(typeof(IFirstRunControlProvider))).ToList();


                foreach (var plugin in firstRunControlProviders)
                {
                    LoadPlugin(plugin);
                    firstRunControls.AddRange(((IFirstRunControlProvider)plugin).GetFirstRunUserControls());
                }
                _logger.Log(">Early loaded {0} plugins for firstRun setup", LogLevel.Advanced, firstRunControlProviders.Count.ToString());

                var firstRunModule = new FirstRun(firstRunControls);

                AddModule(firstRunModule);
                StartModule(firstRunModule);
                if (!firstRunModule.CompletedSuccesfully)
                    Program.SafeQuit();
            }
            MsnGetters.Clear();
            #endregion



            _logger.Log("Starting...", LogLevel.Advanced);
            _logger.Log(">Main classes...", LogLevel.Advanced);
            _sqliteControler = new SqliteControler(new SqliteConnector(_logger));

            _logger.Log(">Modules...", LogLevel.Advanced);
            StartModules();
            _logger.Log(">loaded {0} modules, where {1} are providing settings", LogLevel.Basic, _modules.Count.ToString(), SettingsList.Count.ToString());

            #region plugins
            _logger.Log("Initalizing {0} plugins", LogLevel.Advanced, plugins.Count.ToString());

            _logger.Log("==========", LogLevel.Advanced);

            var sortedPlugins = SortPlugins(plugins);

            LoadPlugins(sortedPlugins[0]);
            LoadPlugins(sortedPlugins[1]);
            
            _logger.Log("==========", LogLevel.Advanced);

            ProritizePluginsUsage();
            #endregion plugins

            Settings.Add(_names.FirstRun.Name, false);
            Settings.Add(_names.LastRunVersion.Name, Program.ScVersion);

            var mapStringFormatter = new MapStringFormatter(
                new MainMapDataGetter(_mapDataFinders, _mapDataGetters, _mapDataParsers, _mapDataReplacementSetters, _saver, _logger, Settings),
                _osuEventSources);

            AddModule(mapStringFormatter);
            StartModule(mapStringFormatter);

            _started = true;
            _logger.Log("Started!", LogLevel.Basic);
        }

        private void ProritizePluginsUsage()
        {
            var mapDataFindersCopy = new List<IMapDataFinder>(_mapDataFinders);
            _mapDataFinders.Clear();
            foreach (var finder in mapDataFindersCopy)
            {
                if (finder is IPlugin)
                    _mapDataFinders.Add(finder);
            }
            foreach (var finder in mapDataFindersCopy)
            {
                if (finder is IModule && !(finder is IPlugin))
                    _mapDataFinders.Add(finder);
            }
        }

        /// <summary>
        /// Creates plugin dictionary with defines plugin loading order
        /// </summary>
        /// <param name="plugins"></param>
        /// <returns></returns>
        public Dictionary<int, List<IPlugin>> SortPlugins(List<IPlugin> plugins)
        {
            var dict = new Dictionary<int, List<IPlugin>>
            {
                { 0, new List<IPlugin>() },
                { 1, new List<IPlugin>() }
            };

            foreach (var plugin in plugins)
            {
                if (plugin is IOsuEventSource)
                    dict[1].Add(plugin);
                else
                    dict[0].Add(plugin);
            }

            return dict;
        }

        private void LoadPlugins(List<IPlugin> plugins)
        {
            foreach (var plugin in plugins)
            {
                LoadPlugin(plugin);
            }
        }
        private void LoadPlugin(IPlugin plugin)
        {
            _logger.Log("Loading: {0} by {1}", LogLevel.Advanced, plugin.Name, plugin.Author);

            if (AddModule(plugin))
            {
                StartModule(plugin);
                _logger.Log(">Started: {0}", LogLevel.Advanced, plugin.Name);
            }
            else
            {
                _logger.Log(">FAILED: {0}", LogLevel.Advanced, plugin.Name);
            }
        }
        public string PluginsLocation => Path.Combine(ConfigSaveLocation, "Plugins");
        private List<IPlugin> GetPlugins()
        {
            var files = Directory.GetFiles(PluginsLocation, "*.dll");
            List<Assembly> assemblies = new List<Assembly>();
            List<IPlugin> plugins = new List<IPlugin>();

            foreach (var file in files)
            {
                try
                {
                    var asm = Assembly.LoadFile(Path.Combine(PluginsLocation, file));

                    foreach (var type in asm.GetTypes())
                    {
                        if (type.GetInterfaces().Contains(typeof(IPlugin)))
                        {
                            if (!type.IsAbstract)
                            {
                                assemblies.Add(asm);

                                var p = Activator.CreateInstance(type) as IPlugin;
                                plugins.Add(p);
                            }
                        }
                    }
                }
                catch (BadImageFormatException e)
                {
                    e.Data.Add("PluginsLocation", PluginsLocation);
                    e.Data.Add("file", file);
                    e.Data.Add("netFramework", GetDotNetVersion.Get45PlusFromRegistry());
                    _logger.Log(e, LogLevel.Error);
                }
            }

            return plugins;
        }

        public void StartModules()
        {
            AddModule(new OsuPathResolver());
            AddModule(new OsuFallbackDetector());
            //AddModule(new MapDataParser());
            AddModule(new MapDataParser());
            //AddModule(new WindowDataGetter()); //refactor
            //AddModule(new PlaysReplacements()); //refactor
            AddModule(new MapReplacement());
            //AddModule(new PpReplacements()); //refactor
#if !DEBUG
            //AddModule(new ClickCounter());
#endif
            //AddModule(new OsuSongsFolderWatcher()); //refactor

            AddModule(new FileSaveLocation());
            AddModule(new CommandsPreview());
            //AddModule(new OsuPost());
            AddModule(new Updater());

            //AddModule(new MemoryDataFinder()); //refactor
            AddModule(new SqliteDataFinder());
            AddModule(new NoDataFinder());

            //AddModule(new ModsHandler()); //refactor
            //AddModule(new ModImageGenerator()); //refactor
            //AddModule(new MainWindow()); //refactor
            //AddModule(new FileMapDataGetter()); //refactor
            //AddModule(new TcpSocketDataGetter()); //refactor
            AddModule(new Donation());




            for (int i = 0; i < _modules.Count; i++)
            {
                StartModule(_modules[i]);
            }
        }

        public void Exit()
        {
            for (int i = 0; i < _modules.Count; i++)
            {
                if (_modules[i] is IDisposable)
                {
                    ((IDisposable)_modules[i]).Dispose();
                }
            }
            Settings.Save();
        }
        /// <summary>
        /// Adds module to _module array while ensuring that only 1 instance of specific module exists at the time.
        /// </summary>
        /// <param name="newModule"></param>
        /// <returns>true if module has been added to list</returns>
        public bool AddModule(IModule newModule)
        {
            var ss = newModule.GetType();
            foreach (var module in _modules)
            {
                var ss2 = module.GetType();
                if (ss == ss2)
                    return false;
            }
            _modules.Add(newModule);
            return true;
        }
        public void StartModule(IModule module)
        {
            if (module.Started) return;

            var settings = module as ISettingsProvider;
            if (settings != null)
            {
                settings.SetSettingsHandle(Settings);
                SettingsList.Add(settings);
            }

            var requester = module as ISaveRequester;
            if (requester != null)
            {
                requester.SetSaveHandle(_saver);
            }

            var mapDataFinder = module as IMapDataFinder;
            if (mapDataFinder != null)
            {
                _mapDataFinders.Add(mapDataFinder);
            }

            var mapDataParser = module as IMapDataParser;
            if (mapDataParser != null)
            {
                _mapDataParsers.Add(mapDataParser);
            }

            var mapDataGetter = module as IMapDataGetter;
            if (mapDataGetter != null)
            {
                _mapDataGetters.Add(mapDataGetter);
            }
            var mainWindowUpdater = module as IMainWindowUpdater;
            if (mainWindowUpdater != null)
            {
                mainWindowUpdater.GetMainWindowHandle(MainWindowUpdater);
            }

            var SqliteUser = module as ISqliteUser;
            if (SqliteUser != null)
            {
                SqliteUser.SetSqliteControlerHandle(_sqliteControler);
            }

            var SettingsProvider = module as ISettingsGetter;
            if (SettingsProvider != null)
            {
                SettingsProvider.SetSettingsListHandle(SettingsList);
            }

            var mapDataReplacements = module as IMapDataReplacements;
            if (mapDataReplacements != null)
            {
                _mapDataReplacementSetters.Add(mapDataReplacements);
            }

            var SettingsGetter = module as ISettings;
            if (SettingsGetter != null)
                SettingsGetter.SetSettingsHandle(Settings);

            var modParser = module as IModParser;
            if (modParser != null)
                this._modParser.Add(modParser);

            var modParserGetter = module as IModParserGetter;
            if (modParserGetter != null)
                modParserGetter.SetModParserHandle(this._modParser);

            var msnGetter = module as IMsnGetter;
            if ((msnGetter) != null)
            {
                MsnGetters.Add(msnGetter);
            }

            if (module is IOsuEventSource osuEventSource)
            {
                _osuEventSources.Add(osuEventSource);
            }

            if (module is IHighFrequencyDataHandler handler)
            {
                _highFrequencyDataHandlers.Add(handler);
            }

            if (module is IHighFrequencyDataSender sender)
            {
                sender.SetHighFrequencyDataHandlers(_highFrequencyDataHandlers);
            }


            if (module is IExiter exiter)
            {
                if (module is IPlugin plugin)
                {
                    exiter.SetExitHandle((o) =>
                    {
                        string reason = "unknown";
                        try
                        {
                            reason = o.ToString();
                        }
                        catch { }
                        _logger.Log("Plugin {0} has requested StreamCompanion shutdown! due to: {1}", LogLevel.Basic, plugin.Name, reason);
                        Program.SafeQuit();
                    });
                }
                else
                {
                    exiter.SetExitHandle((o) =>
                    {
                        _logger.Log("StreamCompanion is shutting down", LogLevel.Basic);
                        Program.SafeQuit();
                    });
                }

            }

            module.Start(_logger);

        }

        public void RemoveModule(IModule module)
        {
            //TODO: finish removeModule
            var mapDataFinder = module as IMapDataFinder;
            if (mapDataFinder != null)
            {
                _mapDataFinders.Remove(mapDataFinder);
            }
            var settings = module as ISettingsProvider;
            if (settings != null)
            {
                settings.SetSettingsHandle(Settings);
                SettingsList.Remove(settings);
            }

            //ISettings-nothing to do

            var msnGetter = module as IMsnGetter;
            if ((msnGetter) != null)
            {
                MsnGetters.Remove(msnGetter);
            }
            //var memoryGetter = module as IMemoryGetter;
            //if (memoryGetter != null)
            //{
            //    MemoryGetters.Remove(memoryGetter);
            //}

            _modules.Remove(module);
            if (module is IDisposable)
                ((IDisposable)module).Dispose();

        }


    }


}
