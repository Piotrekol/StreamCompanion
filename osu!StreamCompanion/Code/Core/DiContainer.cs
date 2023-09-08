using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Loader;
using System.Windows.Forms;
using Grace.DependencyInjection;
using osu_StreamCompanion.Code.Core.Loggers;
using osu_StreamCompanion.Code.Core.Maps.Processing;
using osu_StreamCompanion.Code.Core.Savers;
using osu_StreamCompanion.Code.Helpers;
using osu_StreamCompanion.Code.Misc;
using osu_StreamCompanion.Code.Modules.Updater;
using osu_StreamCompanion.Code.Windows;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Interfaces;
using StreamCompanionTypes.Enums;
using StreamCompanionTypes.Interfaces.Services;
using System.Diagnostics.CodeAnalysis;
using osu_StreamCompanion.Code.Core.Plugins;

namespace osu_StreamCompanion.Code.Core
{
    internal static class DiContainer
    {
        public static DependencyInjectionContainer Container => LazyContainer.Value;
        private static Lazy<DependencyInjectionContainer> LazyContainer = new Lazy<DependencyInjectionContainer>(() =>
        {
            var di = new DependencyInjectionContainer();
            di.Configure(x => x.ExportFactory(() => MainLogger.Instance));
            di.Configure(x => x.ExportFactory((StaticInjectionContext context) =>
                {
                    var pluginName = context.TargetInfo.InjectionType?.Name;
                    if (pluginName == null)
                        return MainLogger.Instance;

                    return (IContextAwareLogger)new PluginLogger(MainLogger.Instance, pluginName);
                })
                    .As<ILogger>().As<IContextAwareLogger>());

            di.Configure(x => x.ExportDefault(typeof(Settings)));
            di.Configure(x => x.ExportDefault(typeof(MainWindowUpdater)));
            di.Configure(x => x.ExportDefault(typeof(MainSaver)));
            di.Configure(x => x.ExportDefault(typeof(OsuEventHandler)));
            di.Configure(x => x.ExportDefault(typeof(MainMapDataGetter)));
            di.Configure(c => c.ImportMembers<object>(MembersThat.HaveAttribute<ImportAttribute>()));
            di.Configure(x => x.ExportFuncWithContext<Delegates.Exit>((scope, context, arg3) =>
              {
                  var logger = scope.Locate<IContextAwareLogger>();
                  var isModule = context.TargetInfo.InjectionType.GetInterfaces().Contains(typeof(IModule));
                  if (isModule)
                  {
                      return reason =>
                      {
                          logger.SetContextData("exiting", "Yes - from module");
                          logger.Log("StreamCompanion is shutting down", LogLevel.Information);
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
                          reason = "Plugin provided invalid reason object";
                      }

                      logger.Log("Plugin {0} has requested StreamCompanion shutdown! due to: {1}", LogLevel.Information,
                          context.TargetInfo.InjectionType.FullName, reason);
                      logger.SetContextData("exiting", $"Yes - plugin:{context.TargetInfo.InjectionType.FullName}, with reason:{reason}");
                      Program.SafeQuit();
                  };
              }));
            di.Configure(x => x.ExportFuncWithContext<Delegates.Restart>((scope, context, arg3) =>
            {
                var logger = scope.Locate<IContextAwareLogger>();
                var isModule = context.TargetInfo.InjectionType == null || context.TargetInfo.InjectionType.GetInterfaces().Contains(typeof(IModule));
                if (isModule)
                {
                    return reason =>
                    {
                        logger.SetContextData("restarting", "from module");
                        logger.Log("StreamCompanion is restarting", LogLevel.Information);
                        Process.Start(Updater.UpdaterExeName, Process.GetCurrentProcess().Id.ToString());
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

                    logger.Log("Plugin {0} has requested StreamCompanion restart! due to: {1}", LogLevel.Information,
                        context.TargetInfo.InjectionType.FullName, reason);
                    logger.SetContextData("restarting", $"plugin:{context.TargetInfo.InjectionType.FullName}, with reason:{reason}");

                    Process.Start(Updater.UpdaterExeName, Process.GetCurrentProcess().Id.ToString());
                    Program.SafeQuit();
                };
            }));
            
            return di;
        });

        public static void ExportDefault(this IExportRegistrationBlock e, Type type)
        {
            e.Export(type).ByInterfaces().As(type).Lifestyle.Singleton();
        }
    }
}