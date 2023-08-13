using osu_StreamCompanion.Code.Misc;
using StreamCompanionTypes.Enums;
using StreamCompanionTypes.Interfaces;
using StreamCompanionTypes.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Loader;
using System.Windows.Forms;

namespace osu_StreamCompanion.Code.Core.Plugins
{
    internal class AssemblyLoader
    {
        private static string PluginsLocation = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins");
        private static string AdditionalDllsLocation = Path.Combine(PluginsLocation, "Dlls");
        private static HashSet<string> customProbingPaths = new() { AppDomain.CurrentDomain.BaseDirectory, PluginsLocation, AdditionalDllsLocation };
        private readonly ILogger _logger;
        private List<Type> _allPluginTypes;

        public AssemblyLoader(ILogger logger)
        {
            _logger = logger;
        }

        static AssemblyLoader()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            PopulateProbingPaths();
        }

        /// <summary>
        /// Recusively preload all dll assemblies located in specified folder its subfolders.
        /// </summary>
        /// <param name="startPath"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        private List<Assembly> GetAssemblies(string startPath)
        {
            var assemblies = new List<Assembly>();
            var fileList = Directory.GetFiles(startPath, "*.dll");
            foreach (var file in fileList)
            {
                var fileName = Path.GetFileNameWithoutExtension(file);
                try
                {
                    var assemblyName = new AssemblyName(fileName);
                    assemblies.Add(AssemblyLoadContext.Default.LoadFromAssemblyName(assemblyName));
                }
                catch (BadImageFormatException e)
                {
                    e.Data.Add("PluginsLocation", PluginsLocation);
                    e.Data.Add("file", file);
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
                catch (FileLoadException e)
                {
                    MessageBox.Show($"Plugin \"{fileName}\" could not get loaded. StreamCompanion will continue to work, however, some features might be missing." +
                                Environment.NewLine + Environment.NewLine + "Error:" +
                                Environment.NewLine + e,
                    "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

            foreach (var directoryName in Directory.GetDirectories(startPath))
            {
                if (directoryName.EndsWith("\\Dlls"))
                    continue;

                assemblies.AddRange(GetAssemblies(Path.Combine(startPath, directoryName)));
            }

            return assemblies;
        }

        public List<Type> GetModules()
            => GetTypes<IModule>(Assembly.GetExecutingAssembly());

        public List<Type> GetPluginTypes()
        {
            if (_allPluginTypes != null)
                return _allPluginTypes;

            var pluginAssemblies = GetAssemblies(PluginsLocation);
            var allPlugins = new List<Type>();
            foreach (var asm in pluginAssemblies)
            {
                var plugins = GetPluginsInAssembly(asm);
                allPlugins.AddRange(plugins);
            }

            allPlugins.AddRange(GetPluginsInAssembly(Assembly.GetExecutingAssembly()));
            return _allPluginTypes = allPlugins;
        }

        public List<Type> GetPluginsInAssembly(Assembly assembly)
            => GetTypes<IPlugin>(assembly);

        public List<Type> GetTypes<T>(Assembly asm)
        {
            List<Type> plugins = new List<Type>();
            List<Type> types = new List<Type>();

            try
            {
                types = asm.GetTypes().ToList();
            }
            catch (ReflectionTypeLoadException e)
            {
                var dllName = asm.ManifestModule.Name;
                MessageBox.Show($"Plugin \"{dllName}\" could not get loaded. StreamCompanion will continue to work, however, some features might be missing." +
                                Environment.NewLine + Environment.NewLine + "Errors:" +
                                Environment.NewLine +
                                string.Join(Environment.NewLine, e.LoaderExceptions.Select(x => $"{x.GetType()}: {x.Message}")),
                    "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            foreach (var type in types)
            {
                if (type.GetInterfaces().Contains(typeof(T)))
                {
                    if (!type.IsAbstract)
                    {
                        plugins.Add(type);
                    }
                }
            }

            return plugins;
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var name = args.Name.Split(",", StringSplitOptions.TrimEntries)[0];

            foreach (var probingPath in customProbingPaths)
            {
                var filePath = Path.Combine(probingPath, $"{name}.dll");
                if (File.Exists(filePath))
                {
                    return Assembly.LoadFrom(filePath);
                }
            }

            return null;
        }

        private static void PopulateProbingPaths()
        {
            foreach (var path in GetDirectoriesRecursive(PluginsLocation))
            {
                customProbingPaths.Add(path);
            }
        }

        private static List<string> GetDirectoriesRecursive(string directory)
        {
            if (!Directory.Exists(directory))
                return new List<string>();

            var dirs = new List<string>();
            foreach (var dir in Directory.GetDirectories(directory))
            {
                dirs.Add(dir);
                dirs.AddRange(GetDirectoriesRecursive(dir));
            }

            return dirs;
        }
    }
}
