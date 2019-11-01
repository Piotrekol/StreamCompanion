using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Grace.DependencyInjection;
using osu_StreamCompanion.Code.Core.Loggers;
using osu_StreamCompanion.Code.Core.Maps.Processing;
using osu_StreamCompanion.Code.Core.Savers;
using osu_StreamCompanion.Code.Helpers;
using osu_StreamCompanion.Code.Windows;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Interfaces;
using StreamCompanionTypes.Enums;

namespace osu_StreamCompanion.Code.Core
{
    internal static class DiContainer
    {
        private static string PluginsLocation = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins");

        public static DependencyInjectionContainer Container => LazyContainer.Value;
        private static Lazy<DependencyInjectionContainer> LazyContainer = new Lazy<DependencyInjectionContainer>(() =>
        {
            var di = new DependencyInjectionContainer();

            di.Configure(x => x.ExportDefault(typeof(MainLogger)));
            di.Configure(x => x.ExportDefault(typeof(Settings)));
            di.Configure(x => x.ExportDefault(typeof(MainWindowUpdater)));
            di.Configure(x => x.ExportDefault(typeof(MainSaver)));
            di.Configure(x => x.ExportFactory((ILogger logger) => new SqliteControler(new SqliteConnector(logger)))
                .As<ISqliteControler>().Lifestyle.Singleton());
            di.Configure(x => x.ExportDefault(typeof(MapStringFormatter)));
            di.Configure(x => x.ExportDefault(typeof(MainMapDataGetter)));
            di.Configure(c => c.ImportMembers<object>(MembersThat.HaveAttribute<ImportAttribute>()));
            di.Configure(x => x.ExportFuncWithContext<Delegates.Exit>((scope, context, arg3) =>
              {
                  var isModule = context.TargetInfo.InjectionType.GetInterfaces().Contains(typeof(IModule));
                  if (isModule)
                  {
                      return reason =>
                      {
                          var logger = scope.Locate<ILogger>();
                          logger.Log("StreamCompanion is shutting down", LogLevel.Basic);
                          Program.SafeQuit();
                      };
                  }

                  return o =>
                  {
                      string reason = string.Empty;
                      try
                      {
                          reason = o.ToString();
                      }
                      catch
                      {
                      }

                      var logger = scope.Locate<ILogger>();

                      logger.Log("Plugin {0} has requested StreamCompanion shutdown! due to: {1}", LogLevel.Basic,
                          context.TargetInfo.InjectionType.FullName, reason);
                      Program.SafeQuit();
                  };


              }));

            //Register all IModules from current assembly
            var modules = GetTypes<IModule>(Assembly.GetExecutingAssembly());
            foreach (var module in modules)
            {
                di.Configure(x => x.ExportDefault(module));
            }

            RegisterPlugins(di,di.Locate<ILogger>());

            return di;
        });
        public static void ExportDefault(this IExportRegistrationBlock e, Type type)
        {
            e.Export(type).ByInterfaces().As(type).Lifestyle.Singleton();
        }

        private static List<Assembly> GetAssemblies(IEnumerable<string> fileList, ILogger logger)
        {
            var assemblies = new List<Assembly>();
            foreach (var file in fileList)
            {
                try
                {
                    assemblies.Add(Assembly.LoadFile(file));
                }
                catch (BadImageFormatException e)
                {
                    e.Data.Add("PluginsLocation", PluginsLocation);
                    e.Data.Add("file", file);
                    e.Data.Add("netFramework", GetDotNetVersion.Get45PlusFromRegistry());
                    logger?.Log(e, LogLevel.Error);
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

            return assemblies;
        }

        private static List<Type> GetTypes<T>(Assembly asm)
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
        private static void RegisterPlugins(DependencyInjectionContainer di, ILogger logger)
        {
            var pluginAssemblies = GetAssemblies(Directory.GetFiles(PluginsLocation, "*.dll"), logger);
            foreach (var asm in pluginAssemblies)
            {
                var plugins = GetTypes<IPlugin>(asm);
                foreach (var plugin in plugins)
                {
                    di.Configure(x => x.ExportDefault(plugin));
                }
            }
        }
    }
}