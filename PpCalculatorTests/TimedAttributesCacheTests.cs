using NUnit.Framework;
using osu.Game.Beatmaps;
using osu.Game.Rulesets;
using osu.Game.Rulesets.Difficulty;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Osu;
using osu.Game.Rulesets.Osu.Difficulty;
using osu.Game.Rulesets.Taiko;
using PpCalculator;
using System.Collections.Concurrent;
using BasePpCalculator = PpCalculator.PpCalculator;

namespace PpCalculatorTests;

[TestFixture]
public class TimedAttributesCacheTests
{
    private static string tempDatabasePath() => Path.Combine(Path.GetTempPath(), $"pp_cache_test_{Guid.NewGuid():N}.sqlite");

    private static readonly OsuRuleset sharedRuleset = new();
    private static readonly ProcessorWorkingBeatmap sharedWorkingBeatmap = new(PpCalculatorTests.GetMapPath(5494538));

    private static BasePpCalculator CreateCalculator(int rulesetId, ITimedAttributesProvider? timedAttributesProvider) => rulesetId switch
    {
        0 => new OsuCalculator(timedAttributesProvider),
        1 => new TaikoCalculator(timedAttributesProvider),
        2 => new CtbCalculator(timedAttributesProvider),
        3 => new ManiaCalculator(timedAttributesProvider),
        _ => throw new ArgumentException($"Invalid ruleset ID {rulesetId}"),
    };

    private static Ruleset CreateRuleset(int rulesetId) => rulesetId switch
    {
        0 => new OsuRuleset(),
        1 => new TaikoRuleset(),
        _ => throw new ArgumentException($"Invalid ruleset ID {rulesetId}"),
    };

    private static double CalculatePp(BasePpCalculator ppCalculator, int mapId, string mods)
    {
        ppCalculator.PreProcess(PpCalculatorTests.GetMapPath(mapId));
        ppCalculator.Mods = mods.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
        ppCalculator.UseScoreMultiplier = false;
        return ppCalculator.Calculate(CancellationToken.None).Total;
    }

    private sealed class FakeTimedAttributesProvider : ITimedAttributesProvider
    {
        public int ComputeCount { get; private set; }

        public List<TimedDifficultyAttributes> GetTimedAttributes(IWorkingBeatmap workingBeatmap, Ruleset ruleset,
            IReadOnlyList<Mod> mods, bool isFullBeatmap, CancellationToken cancellationToken)
        {
            ComputeCount++;
            return [new(0, new OsuDifficultyAttributes { StarRating = expectedStarRating(mods) })];
        }

        public static double expectedStarRating(IReadOnlyList<Mod> mods)
            => mods.Sum(mod => mod.Acronym.Sum(character => character));
    }

    private static Mod[] ModsFor(params string[] acronyms)
        => acronyms
            .Select(acronym => sharedRuleset.AllMods.First(mod => string.Equals(mod.Acronym, acronym, StringComparison.OrdinalIgnoreCase)).CreateInstance())
            .ToArray();

    private static List<TimedDifficultyAttributes> GetTimedAttributes(CachedTimedAttributesProvider provider, params string[] mods)
        => provider.GetTimedAttributes(
            sharedWorkingBeatmap,
            sharedRuleset,
            ModsFor(mods.SelectMany(combination => combination.Split(',', StringSplitOptions.RemoveEmptyEntries)).ToArray()),
            true,
            CancellationToken.None);

    [Test]
    [TestCase(5494538, "", 0)]
    [TestCase(5494538, "HD", 0)]
    public void CachedPpEqualsUncachedPp(int mapId, string mods, int rulesetId)
    {
        double uncachedPp = CalculatePp(CreateCalculator(rulesetId, null), mapId, mods);
        Assert.That(uncachedPp, Is.GreaterThan(0), "reference calculation failed");

        using CachedTimedAttributesProvider provider = new(tempDatabasePath(), 2000);

        double firstCachedPp = CalculatePp(CreateCalculator(rulesetId, provider), mapId, mods);
        Assert.That(provider.CacheStores, Is.EqualTo(1), "first calculation must store");
        Assert.That(provider.CacheHits, Is.EqualTo(0));

        provider.FlushPendingEntries();

        double secondCachedPp = CalculatePp(CreateCalculator(rulesetId, provider), mapId, mods);
        Assert.That(provider.CacheHits, Is.EqualTo(1), "second calculation must hit the cache");
        Assert.That(provider.CacheStores, Is.EqualTo(1), "cache hit must not store again");

        Assert.That(firstCachedPp, Is.EqualTo(uncachedPp).Within(0.000000001), "cached (miss) pp must equal uncached pp");
        Assert.That(secondCachedPp, Is.EqualTo(uncachedPp).Within(0.000000001), "cached (hit) pp must equal uncached pp");
    }

    [Test]
    [TestCase(5494538, 0, "HD")]
    [TestCase(5494538, 0, "FL")]
    [TestCase(5526783, 1, "")]
    public void SerializationRoundtripIsByteStable(int mapId, int rulesetId, string mods)
    {
        Ruleset ruleset = CreateRuleset(rulesetId);
        ProcessorWorkingBeatmap workingBeatmap = new(PpCalculatorTests.GetMapPath(mapId));

        Mod[] enabledMods = mods.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
            .Select(modAcronym => ruleset.AllMods.First(mod => string.Equals(mod.Acronym, modAcronym, StringComparison.OrdinalIgnoreCase)).CreateInstance())
            .ToArray();

        List<TimedDifficultyAttributes> computed = CalculatingTimedAttributesProvider.Instance
            .GetTimedAttributes(workingBeatmap, ruleset, enabledMods, true, CancellationToken.None);

        byte[] firstBlob = DifficultyCache.Serialize(computed);
        List<TimedDifficultyAttributes> roundtripped = DifficultyCache.Deserialize(firstBlob);
        byte[] secondBlob = DifficultyCache.Serialize(roundtripped);

        Assert.That(secondBlob, Is.EqualTo(firstBlob), "serialize→deserialize→serialize must be byte-identical");
        Assert.That(roundtripped.Last().Attributes.GetType(), Is.EqualTo(computed.Last().Attributes.GetType()), "concrete attributes type must survive the roundtrip");
    }

    [Test]
    public void DifferentAlgorithmVersionMissesAndRecomputes()
    {
        string databaseFilePath = tempDatabasePath();
        FakeTimedAttributesProvider versionOneFake = new();
        FakeTimedAttributesProvider versionTwoFake = new();

        using CachedTimedAttributesProvider versionOneProvider = new(databaseFilePath, 2000, "test-v1", timedAttributesProvider: versionOneFake, minimumGenerationMilliseconds: 0);
        using CachedTimedAttributesProvider versionTwoProvider = new(databaseFilePath, 2000, "test-v2", timedAttributesProvider: versionTwoFake, minimumGenerationMilliseconds: 0);

        _ = GetTimedAttributes(versionOneProvider, "HD");
        versionOneProvider.FlushPendingEntries();

        _ = GetTimedAttributes(versionTwoProvider, "HD");
        versionTwoProvider.FlushPendingEntries();

        Assert.That(versionOneProvider.CacheStores, Is.EqualTo(1));
        Assert.That(versionOneProvider.CacheHits, Is.EqualTo(0));
        Assert.That(versionTwoProvider.CacheStores, Is.EqualTo(1), "new algorithm version must recompute, not reuse v1 attributes");
        Assert.That(versionTwoProvider.CacheHits, Is.EqualTo(0));
        Assert.That(versionOneFake.ComputeCount, Is.EqualTo(1));
        Assert.That(versionTwoFake.ComputeCount, Is.EqualTo(1));

        _ = GetTimedAttributes(versionOneProvider, "HD");
        Assert.That(versionOneProvider.CacheHits, Is.EqualTo(1));
        Assert.That(versionOneFake.ComputeCount, Is.EqualTo(1), "hit must not recompute");
    }

    [Test]
    public void DifferentModsProduceDistinctCacheEntries()
    {
        FakeTimedAttributesProvider fake = new();
        using CachedTimedAttributesProvider provider = new(tempDatabasePath(), 2000, timedAttributesProvider: fake, minimumGenerationMilliseconds: 0);

        double hiddenStarRating = GetTimedAttributes(provider, "HD").Last().Attributes.StarRating;
        provider.FlushPendingEntries();
        double hardRockStarRating = GetTimedAttributes(provider, "HR").Last().Attributes.StarRating;
        provider.FlushPendingEntries();
        double hiddenAgainStarRating = GetTimedAttributes(provider, "HD").Last().Attributes.StarRating;

        Assert.That(provider.CacheStores, Is.EqualTo(2), "two mod combinations must produce two entries");
        Assert.That(provider.CacheHits, Is.EqualTo(1), "repeated mod combination must hit");
        Assert.That(fake.ComputeCount, Is.EqualTo(2));

        Assert.That(hiddenStarRating, Is.EqualTo(FakeTimedAttributesProvider.expectedStarRating(ModsFor("HD"))));
        Assert.That(hiddenAgainStarRating, Is.EqualTo(hiddenStarRating));
        Assert.That(hardRockStarRating, Is.EqualTo(FakeTimedAttributesProvider.expectedStarRating(ModsFor("HR"))));
        Assert.That(hardRockStarRating, Is.Not.EqualTo(hiddenStarRating));
    }

    [Test]
    public void ConcurrentCalculationsOnSharedProviderSucceed()
    {
        string[] modCombinations = ["HD", "HR", "DT", "HT", "EZ", "NF"];
        FakeTimedAttributesProvider fake = new();
        using CachedTimedAttributesProvider provider = new(tempDatabasePath(), 2000, timedAttributesProvider: fake, minimumGenerationMilliseconds: 0);

        ConcurrentDictionary<string, double> starRatingByMods = new();
        ConcurrentDictionary<string, Exception> exceptionsByMods = new();
        Task.WaitAll(modCombinations.Select(combination => Task.Run(() =>
        {
            try
            {
                starRatingByMods[combination] = GetTimedAttributes(provider, combination).Last().Attributes.StarRating;
            }
            catch (Exception exception)
            {
                exceptionsByMods[combination] = exception;
            }
        })).ToArray());

        Assert.That(exceptionsByMods, Is.Empty, "concurrent calculations must not throw");
        foreach (string combination in modCombinations)
        {
            Assert.That(starRatingByMods[combination], Is.EqualTo(FakeTimedAttributesProvider.expectedStarRating(ModsFor(combination))), $"cached star rating for mods '{combination}' must equal computed");
        }

        Assert.That(provider.CacheStores, Is.EqualTo(modCombinations.Length));
    }

    [Test]
    public void StoredEntriesAreReadableWithoutExplicitFlush()
    {
        string databaseFilePath = tempDatabasePath();
        FakeTimedAttributesProvider fake = new();
        using CachedTimedAttributesProvider storingProvider = new(databaseFilePath, 2000, timedAttributesProvider: fake, minimumGenerationMilliseconds: 0);

        _ = GetTimedAttributes(storingProvider, "HD");
        _ = GetTimedAttributes(storingProvider, "HD");
        Assert.That(storingProvider.CacheHits, Is.EqualTo(1), "stored entry must be readable before any flush");
        string[] modCombinations = { "HR", "DT", "HT", "EZ", "NF", "FL", "SD", "PF", "HD,HR" };
        foreach (string combination in modCombinations)
        {
            _ = GetTimedAttributes(storingProvider, combination);
        }

        FakeTimedAttributesProvider freshFake = new();
        using CachedTimedAttributesProvider freshProvider = new(databaseFilePath, 2000, timedAttributesProvider: freshFake, minimumGenerationMilliseconds: 0);
        double persistedStarRating = GetTimedAttributes(freshProvider, "HD").Last().Attributes.StarRating;
        Assert.That(freshProvider.CacheHits, Is.EqualTo(1), "threshold flush must persist entries without an explicit flush call");
        Assert.That(freshFake.ComputeCount, Is.EqualTo(0));
        Assert.That(persistedStarRating, Is.EqualTo(FakeTimedAttributesProvider.expectedStarRating(ModsFor("HD"))));
    }
}
