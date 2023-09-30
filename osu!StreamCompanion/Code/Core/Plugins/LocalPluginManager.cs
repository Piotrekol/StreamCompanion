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
using StreamCompanionTypes.Attributes;
using System.Threading.Tasks;

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

        private void ForceEnablePlugin(LocalPluginEntry requestorPlugin, string pluginName)
        {
            var plugin = GetPluginByTypeName(pluginName);
            plugin.EnabledForcefully = plugin.Enabled = true;
            plugin.EnabledForcefullyByPlugins.Add(requestorPlugin);
        }

        private LocalPluginEntry GetPluginByTypeName(string typeName)
            => _configuration.Plugins.First(x => x.TypeName == typeName);

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
                if (CorePluginNames.Contains(pluginEntry.TypeName))
                {
                    ForceEnablePlugin(GetPluginByTypeName(Consts.SCPLUGIN_TYPENAME), pluginEntry.TypeName);
                }

                if (!GetPluginByTypeName(pluginEntry.TypeName).Enabled)
                {
                    disabledPlugins.Add(pluginEntry);
                    activePlugins[i] = null;
                    continue;
                }

                foreach (var requestedDependency in pluginEntry.Dependencies)
                {
                    var foundPlugin = activePlugins.FirstOrDefault(pType => pType?.TypeName == requestedDependency.PluginName);
                    if (foundPlugin == null)
                    {
                        if (disabledPlugins.FirstOrDefault(pType => pType.TypeName == requestedDependency.PluginName) != null)
                        {
                            ForceEnablePlugin(pluginEntry, requestedDependency.PluginName);
                            reloadPending = true;
                            continue;
                        }

                        failedPlugins.Add((pluginEntry, $"Plugin \"{pluginEntry.TypeName}\" failed to resolve required dependency \"{requestedDependency.PluginName}\" - file not found."));
                        activePlugins[i] = null;
                        break;
                    }

                    var foundPluginVersion = foundPlugin.Type.Assembly.GetName().Version;
                    if (requestedDependency.MinVersion > foundPluginVersion)
                    {
                        failedPlugins.Add((pluginEntry, $"Plugin \"{pluginEntry.TypeName}\" failed to resolve required dependency \"{requestedDependency.PluginName}\" - version {foundPluginVersion} does not satisfy requested minimum version {requestedDependency.MinVersion}; \"{requestedDependency.PluginName}\" most likely needs to be updated."));
                        activePlugins[i] = null;
                        break;
                    }

                    ForceEnablePlugin(pluginEntry, requestedDependency.PluginName);
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

        private void PopulateConfiguration(List<Type> allLoadedPlugins)
        {
            foreach (var pluginType in allLoadedPlugins)
            {
                var typeName = pluginType.Name;
                var pluginEntry = _configuration.Plugins.FirstOrDefault(p => p.TypeName == typeName);
                if (pluginEntry == null)
                {
                    _configuration.Plugins.Add(pluginEntry = new LocalPluginEntry
                    {
                        TypeName = typeName,
                    });
                }

                pluginEntry.Type = pluginType;
                pluginEntry.Dependencies = pluginType.GetCustomAttributes<SCPluginDependencyAttribute>().ToList();
                pluginEntry.EnabledForcefully = false;
                pluginEntry.EnabledForcefullyByPlugins.Clear();
                pluginEntry.Metadata = pluginType.GetPluginMetadata();
            }

            var dummySCPlugin = _configuration.Plugins.FirstOrDefault(p => p.TypeName == Consts.SCPLUGIN_TYPENAME);
            if (dummySCPlugin == null)
            {
                _configuration.Plugins.Add(dummySCPlugin = new LocalPluginEntry
                {
                    TypeName = Consts.SCPLUGIN_TYPENAME,
                    Enabled = true,
                });
            }

            dummySCPlugin.Metadata = new PluginMetadata(Consts.SCPLUGIN_NAME, "Base application", Consts.SCPLUGIN_AUTHOR, Consts.SC_BASE_REPO_URL);
            dummySCPlugin.EnabledForcefully = true;
            dummySCPlugin.EnabledForcefullyByPlugins.Add(dummySCPlugin);
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
                    //TODO: handle plugin updates after plugin repository impl.
                    _ = Task.Run(() => MessageBox.Show($"Plugin \"{pluginType.FullName}\" could not get initialized. StreamCompanion will most likely continue to work, however some features might be missing." +
                                    Environment.NewLine + Environment.NewLine + "Errors:" +
                                    Environment.NewLine +
                                    ex,
                        "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning));
                    _logger.Log(ex, LogLevel.Error);
                }

                if (plugin != null)
                {
                    var pluginConfig = GetPluginByTypeName(pluginType.Name);
                    pluginConfig.Plugin = plugin;
                    pluginConfig.Metadata ??= plugin.GetPluginMetadata();

                    if (pluginConfig.Metadata == null)
                    {
                        _logger.Log("************", LogLevel.Critical);
                        _logger.Log("plugin \"{0}\" is missing metadata! add {1} on its class.", LogLevel.Critical, plugin.GetType().FullName, nameof(SCPluginAttribute));
                        _logger.Log("************", LogLevel.Critical);
                        continue;
                    }

                    _logger.Log(">loaded \"{0}\" by {1} ({2}) v:{3}", LogLevel.Debug, pluginConfig.Metadata.Name, pluginConfig.Metadata.Authors,
                        plugin.GetType().FullName, plugin.GetType().Assembly.GetName().Version.ToString());
                }
            }
        }
    }
}
