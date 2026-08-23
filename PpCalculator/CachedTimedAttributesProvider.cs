#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Threading;
using osu.Game.Beatmaps;
using osu.Game.Rulesets;
using osu.Game.Rulesets.Difficulty;
using osu.Game.Rulesets.Mods;
using StreamCompanionTypes.Enums;
using StreamCompanionTypes.Interfaces.Services;

namespace PpCalculator
{
    public sealed class CachedTimedAttributesProvider : ITimedAttributesProvider, IDisposable
    {
        public const int DefaultMinimumGenerationMilliseconds = 250;

        private readonly int minimumGenerationMilliseconds;

        private readonly DifficultyCache cache;
        private readonly ITimedAttributesProvider timedAttributesProvider;
        private readonly ILogger? logger;

        public bool Enabled => cache.Enabled;
        public long CacheHits => cache.CacheHits;
        public long CacheStores => cache.CacheStores;
        public CachedTimedAttributesProvider(string databaseFilePath, int maxCacheEntries, string cacheVersion = DifficultyCache.DefaultCacheVersion,
            ILogger? logger = null, ITimedAttributesProvider? timedAttributesProvider = null,
            int minimumGenerationMilliseconds = DefaultMinimumGenerationMilliseconds)
        {
            this.logger = logger;
            this.timedAttributesProvider = timedAttributesProvider ?? CalculatingTimedAttributesProvider.Instance;
            this.minimumGenerationMilliseconds = minimumGenerationMilliseconds;
            cache = new DifficultyCache(databaseFilePath, maxCacheEntries, cacheVersion, logger);
        }

        public List<TimedDifficultyAttributes> GetTimedAttributes(IWorkingBeatmap workingBeatmap, Ruleset ruleset, IReadOnlyList<Mod> mods, bool isFullBeatmap, CancellationToken cancellationToken)
        {
            string? beatmapMd5 = workingBeatmap.BeatmapInfo.MD5Hash;
            string modsKey = string.Join("|", mods.Select(mod => mod.Acronym));
            bool isCachableRequest = isFullBeatmap && cache.Enabled;

            if (isCachableRequest && beatmapMd5 != null)
            {
                try
                {
                    byte[]? blob = cache.TryGetRaw(beatmapMd5, ruleset.RulesetInfo.OnlineID, modsKey);
                    if (blob != null)
                    {
                        logger?.Log($"difficulty cache hit: {beatmapMd5} ruleset {ruleset.RulesetInfo.OnlineID} mods [{modsKey}]", LogLevel.Debug);
                        return DifficultyCache.Deserialize(blob);
                    }
                }
                catch (Exception exception)
                {
                    logger?.Log($"difficulty cache hit failed, computing instead (map {beatmapMd5} ruleset {ruleset.RulesetInfo.OnlineID} mods [{modsKey}])", LogLevel.Warning);
                    logger?.Log(exception, LogLevel.Warning);
                }
            }

            Stopwatch computeStopwatch = Stopwatch.StartNew();
            List<TimedDifficultyAttributes> computed =
                timedAttributesProvider.GetTimedAttributes(workingBeatmap, ruleset, mods, isFullBeatmap, cancellationToken);
            computeStopwatch.Stop();

            if (isCachableRequest && beatmapMd5 != null && computeStopwatch.ElapsedMilliseconds >= minimumGenerationMilliseconds)
            {
                try
                {
                    byte[] compressedAttributes = DifficultyCache.Serialize(computed);
                    cache.Store(new DifficultyCache.CacheEntry(
                        beatmapMd5, ruleset.RulesetInfo.OnlineID, modsKey, compressedAttributes, computeStopwatch.ElapsedMilliseconds));
                    logger?.Log($"difficulty cache store: {beatmapMd5} ruleset {ruleset.RulesetInfo.OnlineID} mods [{modsKey}] ({compressedAttributes.Length / 1024} KB)", LogLevel.Debug);
                }
                catch (Exception exception)
                {
                    logger?.Log($"difficulty cache store failed, result unaffected (map {beatmapMd5} ruleset {ruleset.RulesetInfo.OnlineID} mods [{modsKey}])", LogLevel.Warning);
                    logger?.Log(exception, LogLevel.Warning);
                }
            }

            return computed;
        }

        internal void FlushPendingEntries() => cache.FlushPendingEntries();

        public void Dispose() => cache.Dispose();
    }
}
