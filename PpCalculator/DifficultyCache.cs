#nullable enable
using Dapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using osu.Game.Rulesets.Difficulty;
using StreamCompanionTypes.Enums;
using StreamCompanionTypes.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
namespace PpCalculator;

/// <summary>
/// Persistent on-disk cache of <see cref="TimedDifficultyAttributes"/>.
/// </summary>
internal sealed class DifficultyCache : IDisposable
{
    /// <summary>
    /// Cache version. Bump on osu submodule update or format change.
    /// </summary>
    public const string DefaultCacheVersion = "1";

    private const int FlushEntryThreshold = 10;

    internal sealed record CacheEntry(string BeatmapMd5, int RulesetId, string Mods, byte[] CompressedAttributes, long GenerationMilliseconds);
    private sealed record CacheEntryKey(string BeatmapMd5, int RulesetId, string Mods);

    public bool Enabled { get; private set; } = true;

    public long CacheHits => cacheHits;
    public long CacheStores => cacheStores;

    private long cacheHits;
    private long cacheStores;

    private readonly string cacheVersion;
    private readonly int maxCacheEntries;
    private readonly SQLiteConnection? connection;
    private readonly object databaseSyncRoot = new();
    private readonly Queue<CacheEntry> pendingWrites = new();
    private readonly Dictionary<CacheEntryKey, byte[]> unflushedEntries = [];
    private readonly object pendingWritesSyncRoot = new();
    private readonly ILogger? logger;

    public DifficultyCache(string databaseFilePath, int maxCacheEntries, string cacheVersion = DefaultCacheVersion, ILogger? logger = null)
    {
        this.cacheVersion = cacheVersion;
        this.maxCacheEntries = maxCacheEntries;
        this.logger = logger;

        if (maxCacheEntries <= 0)
        {
            Enabled = false;
            return;
        }

        try
        {
            string? databaseDirectory = Path.GetDirectoryName(Path.GetFullPath(databaseFilePath));
            if (databaseDirectory != null)
            {
                _ = Directory.CreateDirectory(databaseDirectory);
            }

            connection = new SQLiteConnection(new SQLiteConnectionStringBuilder
            {
                DataSource = databaseFilePath,
                Version = 3,
                JournalMode = SQLiteJournalModeEnum.Wal,
                SyncMode = SynchronizationModes.Normal,
                BusyTimeout = 5000,
            }.ConnectionString);

            connection.Open();
            _ = connection.Execute(@"
CREATE TABLE IF NOT EXISTS difficulty_cache (
    beatmap_md5      TEXT NOT NULL,
    ruleset_id       INTEGER NOT NULL,
    mods             TEXT NOT NULL,
    algorithm_version TEXT NOT NULL,
    timed_attributes BLOB NOT NULL,
    generation_milliseconds INTEGER NOT NULL,
    calculated_at    INTEGER NOT NULL,
    last_accessed_at INTEGER NOT NULL,
    PRIMARY KEY (beatmap_md5, ruleset_id, mods, algorithm_version)
);
CREATE INDEX IF NOT EXISTS idx_difficulty_cache_last_accessed_at ON difficulty_cache(last_accessed_at);");
        }
        catch (Exception exception)
        {
            Enabled = false;
            logger?.Log($"difficulty cache disabled: could not open \"{databaseFilePath}\"", LogLevel.Warning);
            logger?.Log(exception, LogLevel.Warning);
        }
    }
    public byte[]? TryGetRaw(string beatmapMd5, int rulesetId, string mods)
    {
        if (!Enabled)
        {
            return null;
        }

        lock (pendingWritesSyncRoot)
        {
            if (unflushedEntries.TryGetValue(new CacheEntryKey(beatmapMd5, rulesetId, mods), out byte[]? unflushedBlob))
            {
                _ = Interlocked.Increment(ref cacheHits);
                return unflushedBlob;
            }
        }

        try
        {
            byte[]? blob = connection?.ExecuteScalar<byte[]?>(
                "SELECT timed_attributes FROM difficulty_cache WHERE beatmap_md5 = @beatmapMd5 AND ruleset_id = @rulesetId AND mods = @mods AND algorithm_version = @algorithmVersion",
                new { beatmapMd5, rulesetId, mods, algorithmVersion = cacheVersion });

            if (blob != null)
            {
                _ = Interlocked.Increment(ref cacheHits);
                _ = (connection?.Execute(
                    "UPDATE difficulty_cache SET last_accessed_at = @LastAccessedAt WHERE beatmap_md5 = @beatmapMd5 AND ruleset_id = @rulesetId AND mods = @mods AND algorithm_version = @algorithmVersion",
                    new { LastAccessedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds(), beatmapMd5, rulesetId, mods, algorithmVersion = cacheVersion }));
            }

            return blob;
        }
        catch (Exception exception)
        {
            logger?.Log($"difficulty cache read failed for map {beatmapMd5} ruleset {rulesetId} mods [{mods}]", LogLevel.Warning);
            logger?.Log(exception, LogLevel.Warning);
            return null;
        }
    }

    public void Store(CacheEntry entry)
    {
        if (!Enabled)
        {
            return;
        }

        _ = Interlocked.Increment(ref cacheStores);
        bool reachedFlushThreshold;
        lock (pendingWritesSyncRoot)
        {
            pendingWrites.Enqueue(entry);
            unflushedEntries[new CacheEntryKey(entry.BeatmapMd5, entry.RulesetId, entry.Mods)] = entry.CompressedAttributes;
            reachedFlushThreshold = pendingWrites.Count >= FlushEntryThreshold;
        }

        if (reachedFlushThreshold)
        {
            FlushPendingEntries();
        }
    }

    internal void FlushPendingEntries()
    {
        List<CacheEntry> pendingEntries = [];
        lock (pendingWritesSyncRoot)
        {
            while (pendingWrites.Count > 0)
            {
                pendingEntries.Add(pendingWrites.Dequeue());
            }
        }

        if (pendingEntries.Count == 0 || connection == null)
        {
            return;
        }

        try
        {
            lock (databaseSyncRoot)
            {
                using SQLiteTransaction transaction = connection.BeginTransaction();

                foreach (CacheEntry entry in pendingEntries)
                {
                    _ = connection.Execute(
                        "INSERT OR REPLACE INTO difficulty_cache (beatmap_md5, ruleset_id, mods, algorithm_version, timed_attributes, generation_milliseconds, calculated_at, last_accessed_at) VALUES (@BeatmapMd5, @RulesetId, @Mods, @AlgorithmVersion, @CompressedAttributes, @GenerationMilliseconds, @CalculatedAt, @CalculatedAt)",
                        new { entry.BeatmapMd5, entry.RulesetId, entry.Mods, AlgorithmVersion = cacheVersion, entry.CompressedAttributes, entry.GenerationMilliseconds, CalculatedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds() },
                        transaction);
                }

                _ = connection.Execute(
                    "DELETE FROM difficulty_cache WHERE rowid IN (SELECT rowid FROM difficulty_cache ORDER BY last_accessed_at DESC, rowid DESC LIMIT -1 OFFSET @MaxCacheEntries)",
                    new { MaxCacheEntries = maxCacheEntries },
                    transaction);

                transaction.Commit();
            }

            lock (pendingWritesSyncRoot)
            {
                foreach (CacheEntry entry in pendingEntries)
                {
                    _ = unflushedEntries.Remove(new CacheEntryKey(entry.BeatmapMd5, entry.RulesetId, entry.Mods));
                }
            }
        }
        catch (Exception exception)
        {
            logger?.Log($"difficulty cache flush failed ({pendingEntries.Count} entries lost)", LogLevel.Warning);
            logger?.Log(exception, LogLevel.Warning);
        }
    }

    public void Dispose()
    {
        FlushPendingEntries();

        lock (databaseSyncRoot)
        {
            connection?.Dispose();
        }
    }

    public static byte[] Serialize(List<TimedDifficultyAttributes> timedAttributes)
    {
        JsonSerializer serializer = JsonSerializer.Create(Settings);

        JObject firstAttributes = JObject.FromObject(timedAttributes[0].Attributes, serializer);
        Dictionary<string, List<JToken>> columns = firstAttributes.Properties()
            .ToDictionary(property => property.Name, property => new List<JToken>(timedAttributes.Count));

        foreach (TimedDifficultyAttributes timedAttribute in timedAttributes)
        {
            foreach (JProperty property in JObject.FromObject(timedAttribute.Attributes, serializer).Properties())
            {
                columns[property.Name].Add(property.Value);
            }
        }

        StoredHeader header = new(
            TypeName(timedAttributes[0].Attributes.GetType()),
            timedAttributes.Count,
            columns.Keys.ToList(),
            columns.Where(column => column.Value[0].Type == JTokenType.Integer).Select(column => column.Key).ToList());
        byte[] headerBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(header));

        using MemoryStream compressed = new();
        using (BrotliStream brotliStream = new(compressed, CompressionLevel.Optimal))
        {
            brotliStream.Write(BitConverter.GetBytes(headerBytes.Length));
            brotliStream.Write(headerBytes);

            WriteDoublePlanes(brotliStream, timedAttributes.Select(timedAttribute => timedAttribute.Time).ToArray());
            foreach (KeyValuePair<string, List<JToken>> column in columns)
            {
                if (header.IntegerKeys.Contains(column.Key))
                {
                    WriteIntegers(brotliStream, column.Value);
                }
                else
                {
                    WriteDoublePlanes(brotliStream, column.Value);
                }
            }
        }

        return compressed.ToArray();
    }

    public static List<TimedDifficultyAttributes> Deserialize(byte[] compressedAttributes)
    {
        using MemoryStream compressed = new(compressedAttributes);
        using BrotliStream brotliStream = new(compressed, CompressionMode.Decompress);
        using MemoryStream decompressed = new();
        brotliStream.CopyTo(decompressed);
        byte[] payload = decompressed.ToArray();

        StoredHeader? header = JsonConvert.DeserializeObject<StoredHeader>(
            Encoding.UTF8.GetString(payload, sizeof(int), BitConverter.ToInt32(payload, 0)))
            ?? throw new InvalidDataException("difficulty cache blob is empty");

        Type concreteType = Type.GetType(header.Type) ?? throw new InvalidDataException($"difficulty cache blob references unknown type \"{header.Type}\"");
        JsonSerializer serializer = JsonSerializer.Create(Settings);

        int offset = sizeof(int) + BitConverter.ToInt32(payload, 0);
        double[] times = readDoublePlanes(payload, ref offset, header.EntryCount);
        Dictionary<string, JToken[]> columns = new(header.Keys.Count);
        foreach (string key in header.Keys)
        {
            JToken[] column = new JToken[times.Length];
            if (header.IntegerKeys.Contains(key))
            {
                for (int entryIndex = 0; entryIndex < times.Length; entryIndex++)
                {
                    column[entryIndex] = new JValue(BitConverter.ToInt32(payload, offset + (entryIndex * sizeof(int))));
                }

                offset += times.Length * sizeof(int);
            }
            else
            {
                double[] values = readDoublePlanes(payload, ref offset, times.Length);
                for (int entryIndex = 0; entryIndex < times.Length; entryIndex++)
                {
                    column[entryIndex] = new JValue(values[entryIndex]);
                }
            }

            columns[key] = column;
        }

        List<TimedDifficultyAttributes> timedAttributes = new(times.Length);
        for (int entryIndex = 0; entryIndex < times.Length; entryIndex++)
        {
            JObject attributes = [];
            foreach (KeyValuePair<string, JToken[]> column in columns)
            {
                attributes[column.Key] = column.Value[entryIndex];
            }

            timedAttributes.Add(new TimedDifficultyAttributes(
                times[entryIndex],
                (DifficultyAttributes)serializer.Deserialize(attributes.CreateReader(), concreteType)!));
        }

        return timedAttributes;
    }

    /// <remarks>
    /// Double columns are stored as 8 byte-planes: all first bytes, all second bytes.
    /// High bytes compress nicely, while low bytes are pretty much incompressible.
    /// </remarks>
    private static void WriteDoublePlanes(Stream stream, IReadOnlyList<JToken> values)
    {
        double[] doubles = new double[values.Count];
        for (int valueIndex = 0; valueIndex < values.Count; valueIndex++)
        {
            if (values[valueIndex].Type is not JTokenType.Float and not JTokenType.Integer)
            {
                throw new InvalidOperationException($"difficulty cache cannot store non-numeric attribute value ({values[valueIndex].Type})");
            }

            doubles[valueIndex] = values[valueIndex].Value<double>();
        }

        WriteDoublePlanes(stream, doubles);
    }

    private static void WriteDoublePlanes(Stream stream, double[] doubles)
    {
        byte[][] planes = new byte[8][];
        for (int planeIndex = 0; planeIndex < 8; planeIndex++)
        {
            planes[planeIndex] = new byte[doubles.Length];
        }

        for (int valueIndex = 0; valueIndex < doubles.Length; valueIndex++)
        {
            byte[] valueBytes = BitConverter.GetBytes(doubles[valueIndex]);
            for (int planeIndex = 0; planeIndex < 8; planeIndex++)
            {
                planes[planeIndex][valueIndex] = valueBytes[planeIndex];
            }
        }

        for (int planeIndex = 0; planeIndex < 8; planeIndex++)
        {
            stream.Write(planes[planeIndex], 0, planes[planeIndex].Length);
        }
    }

    private static void WriteIntegers(Stream stream, IReadOnlyList<JToken> values)
    {
        foreach (JToken value in values)
        {
            if (value.Type != JTokenType.Integer)
            {
                throw new InvalidOperationException($"difficulty cache expected integer attribute value but got {value.Type}");
            }

            stream.Write(BitConverter.GetBytes(value.Value<int>()));
        }
    }

    private static double[] readDoublePlanes(byte[] payload, ref int offset, int count)
    {
        double[] doubles = new double[count];
        byte[] valueBytes = new byte[8];
        for (int valueIndex = 0; valueIndex < count; valueIndex++)
        {
            for (int planeIndex = 0; planeIndex < 8; planeIndex++)
            {
                valueBytes[planeIndex] = payload[offset + (planeIndex * count) + valueIndex];
            }

            doubles[valueIndex] = BitConverter.ToDouble(valueBytes, 0);
        }

        offset += count * 8;
        return doubles;
    }

    private static string TypeName(Type type) => $"{type.FullName}, {type.Assembly.GetName().Name}";

    private sealed record StoredHeader(string Type, int EntryCount, List<string> Keys, List<string> IntegerKeys);

    private static readonly JsonSerializerSettings Settings = new()
    {
        ContractResolver = new DifficultyAttributesContractResolver(),
    };

    /// <summary>
    /// Forces <see cref="MemberSerialization.OptOut"/> on all osu-side <see cref="DifficultyAttributes"/>-derived types
    /// Also ignores <see cref="DifficultyAttributes.Mods"/> (performance calculators read score.Mods)
    /// and clears ShouldSerialize* predicates (e.g. ShouldSerializeFlashlightDifficulty).
    /// </summary>
    private sealed class DifficultyAttributesContractResolver : DefaultContractResolver
    {
        protected override JsonObjectContract CreateObjectContract(Type objectType)
        {
            JsonObjectContract contract = base.CreateObjectContract(objectType);

            if (typeof(DifficultyAttributes).IsAssignableFrom(objectType) && contract.MemberSerialization != MemberSerialization.OptOut)
            {
                contract.MemberSerialization = MemberSerialization.OptOut;
                contract.Properties.Clear();
                foreach (JsonProperty property in CreateProperties(objectType, MemberSerialization.OptOut))
                {
                    contract.Properties.Add(property);
                }
            }

            if (typeof(DifficultyAttributes).IsAssignableFrom(objectType))
            {
                foreach (JsonProperty property in contract.Properties)
                {
                    property.ShouldSerialize = null;
                }

                JsonProperty? modsProperty = contract.Properties["Mods"];
                modsProperty?.Ignored = true;
            }

            return contract;
        }
    }
}
