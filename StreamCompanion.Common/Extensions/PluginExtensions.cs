using System;
using System.Collections.Concurrent;
using System.Reflection;
using StreamCompanionTypes.Attributes;
using StreamCompanionTypes.Interfaces;

namespace StreamCompanion.Common
{
    public static class PluginExtensions
    {
        private static ConcurrentDictionary<Type, IPluginMetadata> PluginMetadataCache = new();
        public static IPluginMetadata GetPluginMetadata(this IPlugin plugin) 
            => plugin.GetType().GetPluginMetadata();

        public static IPluginMetadata GetPluginMetadata(this Type pluginType) 
            => PluginMetadataCache.GetOrAdd(pluginType, type => type.GetCustomAttribute<SCPluginAttribute>());
    }
}