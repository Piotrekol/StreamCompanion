using osu_StreamCompanion.Code.Core.Loggers;
using osu_StreamCompanion.Code.Core.Maps.Processing;
using osu_StreamCompanion.Code.Core.Savers;
using osu_StreamCompanion.Code.Helpers;
using osu_StreamCompanion.Code.Misc;
using osu_StreamCompanion.Code.Modules.FirstRun;
using osu_StreamCompanion.Code.Modules.osuFallbackDetector;
using osu_StreamCompanion.Code.Modules.osuPathReslover;
using StreamCompanionTypes;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace osu_StreamCompanion.Code.Core
{
    internal class Initializer : ApplicationContext
    {
        private readonly SettingNames _names = SettingNames.Instance;

        private readonly MainLogger _logger;
        readonly MainSaver _saver;
        public readonly Settings Settings;
        private bool _started;
        public string PluginsLocation => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins");

        public Initializer()
        {

            new FileChecker();

            var di = DiContainer.Container;

            var a = di.Locate<ISettingsHandler>();
            _logger = di.Locate<MainLogger>();
            _saver = di.Locate<MainSaver>();
            Settings = di.Locate<Settings>();
            var c = di.Locate<ISettingsHandler>();

            var refeq = ReferenceEquals(a, c);

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

            _logger.Log("Starting...", LogLevel.Advanced);
            DiContainer.Container.Locate<FirstRun>();


            DiContainer.Container.Locate<ISqliteControler>();
            DiContainer.Container.Locate<OsuPathResolver>();
            DiContainer.Container.Locate<OsuFallbackDetector>();

            DiContainer.Container.LocateAll(typeof(IPlugin)).Cast<IPlugin>().ToList();
            
            Settings.Add(_names.FirstRun.Name, false);
            Settings.Add(_names.LastRunVersion.Name, Program.ScVersion);

            DiContainer.Container.Locate<MapStringFormatter>();
            
            _started = true;
            _logger.Log("Started!", LogLevel.Basic);
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
        
        private List<IPlugin> GetPlugins()
        {
            var files = Directory.GetFiles(PluginsLocation, "*.dll");
            List<Assembly> assemblies = new List<Assembly>();
            List<IPlugin> plugins = new List<IPlugin>();
            List<Type> pluginTypes = new List<Type>();
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
                                //DiContainer.Container.Configure(x => x.ExportDefault(type));
                                pluginTypes.Add(type);
                                try
                                {
                                    var p = Activator.CreateInstance(type) as IPlugin;
                                    plugins.Add(p);
                                }
                                catch (MissingMethodException)
                                {
                                    //tempPlugins.Add(DiContainer.Container.Locate(type));

                                }
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
                catch (COMException)
                {
                    if (file.Contains("osuOverlayPlugin"))
                    {
                        var nl = Environment.NewLine;
                        MessageBox.Show("Since SC version 190426.18, osu! overlay plugin started being falsely detected as virus" + nl + nl
                                        + "If you don't use it it is advised to just remove it from SC plugins folder (search for osuOverlayPlugin.dll and osuOverlay.dll inside SC folder)." + nl + nl
                                        + "However if you do use it, add these to your antivirus exceptions." + nl + nl + nl

                                        + "osu! overlay will NOT be loaded until you resolve this manually.", "StreamCompanion - WARNING", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    MessageBox.Show(
                        "StreamCompanion could not load any of the plugins because of not enough permissions." + Environment.NewLine + Environment.NewLine
                            + "Please reinstall StreamCompanion.", "StreamCompanion - ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Program.SafeQuit();
                }
            }

            return plugins;
        }

        public void Exit()
        {
            Settings.Save();
            DiContainer.Container.Dispose();
        }
    }
}
