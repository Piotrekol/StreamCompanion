using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Caching;

namespace OsuSongsFolderWatcher
{
    public static class MemoryCacheHelpers
    {
        /// <summary>
        /// By default <see cref="MemoryCache"/> implicitly removes cached items only after minimum of hardcoded 10 seconds(bucket size) <para />
        /// This modifies this behaviour to be instead <see cref="poolingFrequencyMs"/> ms using reflection <para />
        /// </summary>
        /// <returns>true whenever field value change succeeds</returns>
        public static bool IncreaseCachePoolingFrequency(int poolingFrequencyMs = 1000)
        {
            var currentAssembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x => x.FullName.StartsWith("System.Runtime.Caching,"));
            if (currentAssembly != null)
            {
                var type = currentAssembly.DefinedTypes.FirstOrDefault(x => x.Name == "CacheExpires");
                var field = type?.GetField("_tsPerBucket", BindingFlags.Static | BindingFlags.NonPublic);
                if (field != null)
                {
                    field.SetValue(null, TimeSpan.FromMilliseconds(poolingFrequencyMs));
                    return true;
                }
            }

            return false;
        }
    }
}