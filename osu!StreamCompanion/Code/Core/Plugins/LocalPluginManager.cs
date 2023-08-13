using StreamCompanion.Common;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Enums;
using StreamCompanionTypes.Interfaces;
using StreamCompanionTypes.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Exceptions;
using osu_StreamCompanion.Code.Core.Loggers;

namespace osu_StreamCompanion.Code.Core.Plugins
{
    internal class LocalPluginManager
    {
        public static readonly ConfigEntry PluginManagerConfigEntry = new("Plugins", null);
        public static readonly List<string> CorePluginNames = new()
        {
            "PluginManagerSettings",
            "MainWindowPlugin",
            "WebSocketDataGetter",
            "OsuMemoryEventSource",
            "ModsHandler",
            "OsuMapLoaderPlugin",
        };

        private readonly ISettings _settings;
        private readonly ILogger _logger;
        private readonly PluginManagerConfiguration _configuration;
        private readonly AssemblyLoader _assemblyLoader;
        internal LocalPluginManager(ISettings settings, ILogger logger, AssemblyLoader assemblyLoader = null)
        {
            _settings = settings;
            _logger = logger;
            _configuration = _settings.GetConfiguration<PluginManagerConfiguration>(PluginManagerConfigEntry);
            _assemblyLoader = assemblyLoader ?? new AssemblyLoader(logger);
        }

        private void ForceEnablePlugin(string requestorPluginName, string pluginName)
        {
            var plugin = GetPluginByName(pluginName);
            plugin.EnabledForcefully = plugin.Enabled = true;
            plugin.EnabledForcefullyByPlugins.Add(requestorPluginName);
        }

        private LocalPluginEntry GetPluginByName(string name)
            => _configuration.Plugins.First(x => x.Name == name);

        public List<Type> GetModules()
            => _assemblyLoader.GetModules();

        public List<Type> GetPlugins()
        {
            PopulateConfiguration(_assemblyLoader.GetPluginTypes());
            var failedPlugins = new List<(LocalPluginEntry plugin, string reason)>();
            var reloadPending = false;
            var disabledPlugins = new List<LocalPluginEntry>();
            var activePlugins = _configuration.Plugins.Where(x => x.Type is not null).ToList();
            for (int i = 0; i < activePlugins.Count; i++)
            {
                var pluginEntry = activePlugins[i];
                if (CorePluginNames.Contains(pluginEntry.Name))
                {
                    ForceEnablePlugin("StreamCompanion", pluginEntry.Name);
                }

                if (!GetPluginByName(pluginEntry.Name).Enabled)
                {
                    disabledPlugins.Add(pluginEntry);
                    activePlugins[i] = null;
                    continue;
                }

                foreach (var requestedDependency in pluginEntry.Dependencies)
                {
                    var foundPlugin = activePlugins.FirstOrDefault(pType => pType?.Name == requestedDependency.PluginName);
                    if (foundPlugin == null)
                    {
                        if (disabledPlugins.FirstOrDefault(pType => pType.Name == requestedDependency.PluginName) != null)
                        {
                            ForceEnablePlugin(pluginEntry.Name, requestedDependency.PluginName);
                            reloadPending = true;
                            continue;
                        }

                        failedPlugins.Add((pluginEntry, $"Plugin \"{pluginEntry.Name}\" failed to resolve required dependency \"{requestedDependency.PluginName}\" - file not found."));
                        activePlugins[i] = null;
                        break;
                    }

                    var foundPluginVersion = foundPlugin.Type.Assembly.GetName().Version;
                    if (requestedDependency.MinVersion > foundPluginVersion)
                    {
                        failedPlugins.Add((pluginEntry, $"Plugin \"{pluginEntry.Name}\" failed to resolve required dependency \"{requestedDependency.PluginName}\" - version {foundPluginVersion} does not satisfy requested minimum version {requestedDependency.MinVersion}; \"{requestedDependency.PluginName}\" most likely needs to be updated."));
                        activePlugins[i] = null;
                        break;
                    }

                    ForceEnablePlugin(pluginEntry.Name, requestedDependency.PluginName);
                }
            }

            if (reloadPending)
            {
                return GetPlugins();
            }

            if (failedPlugins.Count > 0)
            {
                foreach (var pluginLog in failedPlugins)
                {
                    _logger.Log(pluginLog.reason, LogLevel.Warning);
                }

                var message = "One or more StreamCompanion plugins failed to load:" + Environment.NewLine + Environment.NewLine;
                MessageBox.Show(message + string.Join(Environment.NewLine, failedPlugins.Select(x => x.reason)), "StreamCompanion - plugin load error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            return activePlugins.Select(p => p?.Type).Where(p => p is not null).ToList();
        }

        private void PopulateConfiguration(List<Type> allPlugins)
        {
            foreach (var pluginType in allPlugins)
            {
                var name = pluginType.Name;
                var pluginEntry = _configuration.Plugins.FirstOrDefault(p => p.Name == name);
                if (pluginEntry != null)
                {
                    pluginEntry.Type = pluginType;
                    pluginEntry.Dependencies = pluginType.GetCustomAttributes<PluginDependencyAttribute>().ToList();
                    continue;
                }


                _configuration.Plugins.Add(new LocalPluginEntry
                {
                    Name = name,
                    Type = pluginType,
                    Dependencies = pluginType.GetCustomAttributes<PluginDependencyAttribute>().ToList()
                });
            }
        }

        internal void StartPlugins(DependencyInjectionContainer di)
        {
            List<Meta<Lazy<IPlugin>>> lazyPluginMetas = new(0);
            try
            {
                lazyPluginMetas = di.Locate<List<Meta<Lazy<IPlugin>>>>();
            }
            catch (LocateException)
            {
                if (MainLogger.Instance.Loggers.All(x => x is not ConsoleLogger) && OperatingSystem.IsWindows())
                {
                    MainLogger.Instance.AddLogger(new ConsoleLogger(_settings));
                }

                _logger.Log("************", LogLevel.Critical);
                _logger.Log("************", LogLevel.Critical);
                _logger.Log(_configuration.Plugins.All(p => p.Type == null)
                    ? "Uh, oh.. There are no plugins in plugins folder!"
                    : "Uh, oh.. All plugins are disabled!", LogLevel.Critical);
                _logger.Log("************", LogLevel.Critical);
                _logger.Log("************", LogLevel.Critical);
            }

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
                {
                    _logger.Log(">loaded \"{0}\" by {1} ({2}) v:{3}", LogLevel.Debug, plugin.Name, plugin.Author,
                        plugin.GetType().FullName, plugin.GetType().Assembly.GetName().Version.ToString());
                    GetPluginByName(pluginType.Name).Plugin = plugin;
                }
            }
        }
    }
}
