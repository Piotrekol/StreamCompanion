using NUnit.Framework;
using osu.Framework.IO.Network;
using PpCalculator;
using BasePpCalculator = PpCalculator.PpCalculator;

namespace PpCalculatorTests;

[TestFixture]
public class PpCalculatorTests
{
    private const string base_url = "https://osu.ppy.sh";

    [Test]
    //[TestCase({count100}, {count50}, {countMiss}, {maxCombo}, "{enabledMods}", {ppValue}d, {score}, {beatmapId})]
    [TestCase(2, 0, 0, 1729, "", 395.315d, 62890520, 5494538)]
    [TestCase(7, 0, 0, 1729, "", 382.063d, 62843300, 5494538)]
    [TestCase(8, 0, 0, 1727, "", 379.453d, 62667240, 5494538)]
    [TestCase(10, 0, 0, 1727, "", 374.534d, 62657280, 5494538)]
    [TestCase(7, 0, 0, 1727, "", 381.975d, 62567820, 5494538)]
    [TestCase(19, 1, 0, 1723, "", 350.849d, 62118090, 5494538)]
    [TestCase(4, 0, 0, 1728, "HD", 432.068d, 66609640, 5494538)]
    [TestCase(5, 0, 0, 1729, "HD", 429.413d, 66600336, 5494538)]
    [TestCase(7, 0, 0, 1728, "HD", 424.104d, 66515293, 5494538)]
    [TestCase(5, 0, 1, 1728, "HD", 390.868d, 66376448, 5494538)]
    [TestCase(4, 0, 0, 495, "HD", 311.913d, 5230126, 5413209)]
    [TestCase(13, 0, 4, 201, "HD", 198.055d, 1711424, 5413209)]
    [TestCase(32, 2, 4, 418, "HR", 175.331d, 3552043, 5413209)]
    [TestCase(41, 1, 6, 200, "HR", 143.184d, 1186603, 5413209)]
    [TestCase(12, 0, 0, 1640, "HR", 102.301d, 35682374, 5670074)]
    [TestCase(18, 1, 1, 1168, "HR", 79.585d, 20987984, 5670074)]
    [TestCase(10, 0, 0, 1639, "DT", 184.463d, 37788277, 5670074)]
    [TestCase(9, 1, 0, 1532, "DT", 167.418d, 34291115, 5670074)]
    [TestCase(37, 0, 0, 1284, "DT", 125.172d, 24174334, 5670074)]
    [TestCase(15, 1, 0, 873, "DT", 134.513d, 14354497, 5670074)]
    [TestCase(0, 0, 0, 495, "HD,HR", 390.55d, 5562683, 5413209)]
    [TestCase(5, 0, 0, 495, "HD,HR", 358.621d, 5507427, 5413209)]
    [TestCase(0, 0, 0, 1640, "HD,HR", 147.645d, 38266275, 5670074)]
    [TestCase(1, 0, 0, 1640, "HD,HR", 144.263d, 38262084, 5670074)]
    [TestCase(4, 0, 0, 1640, "HD,HR", 134.9d, 38193744, 5670074)]
    [TestCase(26, 0, 0, 1640, "HD,HR", 92.194d, 37443420, 5670074)]
    [TestCase(23, 1, 1, 1347, "HD,DT", 893.138d, 52018458, 5374538)]
    [TestCase(47, 0, 5, 851, "HD,DT", 682.345d, 31424313, 5374538)]
    [TestCase(75, 11, 11, 846, "HD,DT", 459.776d, 25666256, 5374538)]
    [TestCase(26, 0, 0, 1173, "HD,DT", 649.581d, 29281744, 5674368)]
    [TestCase(29, 0, 1, 947, "HD,DT", 554.639d, 19831001, 5674368)]
    [TestCase(0, 0, 0, 449, "HR,DT", 95.25d, 2964225, 5088141)]
    [TestCase(0, 0, 0, 449, "HR,DT", 95.25d, 2961825, 5088141)]
    [TestCase(0, 0, 0, 449, "HR,DT", 95.25d, 2954725, 5088141)]
    [TestCase(0, 0, 0, 449, "HD,HR,DT", 106.312d, 3149247, 5088141)]
    [TestCase(0, 0, 0, 449, "HD,HR,DT", 106.312d, 3144447, 5088141)]
    [TestCase(0, 0, 0, 449, "HD,HR,DT", 106.312d, 3136247, 5088141)]
    [TestCase(33, 6, 5, 536, "HD,DT,NC", 426.72d, 9671764, 5674368)]
    [TestCase(0, 0, 0, 449, "HD,HR,DT,NC", 106.312d, 3136047, 5088141)]
    [TestCase(0, 0, 0, 449, "HD,HR,DT,NC", 106.312d, 3129347, 5088141)]
    [TestCase(39, 10, 2, 142, "DT,NC", 17.57d, 546233, 5088141)]
    [TestCase(4, 0, 0, 449, "EZ", 14.738d, 1297810, 5088141)]
    [TestCase(144, 14, 38, 58, "NF", 26.607d, 179252, 5413209)]
    [TestCase(48, 2, 11, 836, "NF", 135.124d, 8713535, 5618607)]
    [TestCase(6, 0, 0, 1386, "HD,HR", 496.207d, 46785879, 5618607)]
    [TestCase(42, 0, 3, 979, "HD,HR", 316.809d, 25596007, 5618607)]
    [TestCase(46, 4, 4, 901, "HD,HR", 282.295d, 24608493, 5618607)]
    [TestCase(4, 0, 0, 1034, "HR", 432.377d, 27899305, 5618607)]
    [TestCase(3, 0, 1, 1266, "HR", 427.779d, 37088371, 5618607)]
    [TestCase(8, 0, 2, 1257, "HR", 393.285d, 36210516, 5618607)]
    public void CalculateOsuTest(int c100, int c50, int cMiss, int combo, string mods, double expectedPp, int score, int mapId)
        => CalculateTest(c100, c50, cMiss, combo, mods, expectedPp, mapId, new OsuCalculator(), score: score);

    [Test]
    //[TestCase({count100}, {count50}, {countMiss}, {maxCombo}, "{enabledMods}", {ppValue}d, {beatmapId})]
    [TestCase(12, 0, 2, 1301, "", 531.438d, 5526783)]
    [TestCase(86, 0, 1, 696, "HR", 568.838d, 5664085)]
    [TestCase(223, 0, 5, 2359, "DT", 707.594d, 5526757)]
    [TestCase(90, 0, 4, 808, "DT,NC", 765.288d, 5526784)]
    [TestCase(3, 0, 0, 777, "HD", 414.132d, 5619630)]
    [TestCase(0, 0, 0, 502, "HR,DT,NC", 238.499d, 5560325)]
    [TestCase(7, 0, 0, 533, "HD,DT", 523.449d, 5619632)]
    [TestCase(4, 0, 0, 851, "HR,DT", 429.391d, 5560324)]
    [TestCase(151, 0, 72, 217, "HT", 25.706d, 5664087)]
    [TestCase(224, 0, 105, 224, "NF", 50.44d, 5645681)]
    public void CalculateTaikoTest(int c100, int c50, int cMiss, int combo, string mods, double expectedPp, int mapId)
        => CalculateTest(c100, c50, cMiss, combo, mods, expectedPp, mapId, new TaikoCalculator());

    [Test]
    //[TestCase({count100}, {countKatu}, {count50}, {countMiss}, {maxCombo}, "{enabledMods}", {ppValue}d, {beatmapId})]
    [TestCase(38, 0, 330, 0, 1448, "", 295.43d, 5646785)]
    [TestCase(42, 0, 354, 0, 1286, "HD", 227.654d, 5610738)]
    [TestCase(2, 2, 477, 0, 1133, "DT", 211.849d, 5617475)]
    [TestCase(2, 0, 479, 0, 1133, "HD,DT", 231.431d, 5617475)]
    [TestCase(38, 0, 330, 0, 1448, "HD,HR", 407.162d, 5646785)]
    [TestCase(42, 6, 348, 22, 306, "EZ", 93.922d, 5610738)]
    [TestCase(2, 0, 479, 1, 1121, "EZ,DT", 378.638d, 5617475)]
    [TestCase(42, 0, 354, 0, 1286, "SD,PF", 199.697d, 5610738)]
    [TestCase(32, 55, 275, 78, 153, "NF", 7.338d, 5646785)]
    [TestCase(38, 0, 330, 0, 1448, "HD,SD,PF", 327.928d, 5646785)]
    public void CalculateCtbTest(int c100, int cKatu, int c50, int cMiss, int combo, string mods, double expectedPp, int mapId)
        => CalculateTest(c100, c50, cMiss, combo, mods, expectedPp, mapId, new CtbCalculator(), cKatu: cKatu);

    [Test]
    //[TestCase({countGeki}, {count300}, {countKatu}, {count100}, {count50}, {countMiss}, {maxCombo}, "{enabledMods}", {ppValue}d, {beatmapId})]
    [TestCase(3060, 84, 0, 0, 0, 0, 5987, "", 257.974d, 5500310)]
    [TestCase(2236, 298, 1, 0, 0, 0, 2535, "MR", 229.897d, 5622749)]
    [TestCase(2573, 562, 9, 0, 0, 0, 5938, "HD", 244.218d, 5500310)]
    [TestCase(1338, 428, 32, 2, 3, 7, 1373, "DT", 298.16d, 5645865)]
    [TestCase(463, 0, 0, 0, 0, 0, 834, "DT,NC", 28.352d, 5445039)]
    [TestCase(2336, 199, 0, 0, 0, 0, 2535, "SD,PF", 232.989d, 5622749)]
    [TestCase(1194, 579, 32, 4, 1, 0, 1852, "FL", 135.909d, 5645865)]
    [TestCase(151, 195, 88, 17, 2, 10, 265, "NF", 2.962d, 5445039)]
    [TestCase(449, 14, 0, 0, 0, 0, 835, "SD,DT,PF", 28.084d, 5445039)]
    [TestCase(2118, 973, 45, 3, 0, 5, 4413, "DT,NC", 468.773d, 5500310)]
    public void CalculateManiaTest(int? geki, int c300, int cKatu, int c100, int c50, int cMiss, int combo, string mods, double expectedPp, int mapId)
        => CalculateTest(c100, c50, cMiss, combo, mods, expectedPp, mapId, new ManiaCalculator(), c300, cKatu, geki);

    public void CalculateTest(int c100, int? c50, int cMiss, int combo, string mods, double expectedPp, int mapId, BasePpCalculator ppCalculator, int c300 = 0, int cKatu = 0, int? cGeki = 0, int score = 0)
    {
        ppCalculator.PreProcess(GetMapPath(mapId));
        ppCalculator.Hit300 = c300;
        ppCalculator.Katus = cKatu;
        ppCalculator.Goods = c100;
        ppCalculator.Mehs = c50;
        ppCalculator.Misses = cMiss;
        ppCalculator.Combo = combo;
        ppCalculator.Gekis = cGeki;
        ppCalculator.Score = score;
        ppCalculator.Mods = mods.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
        ppCalculator.UseScoreMultiplier = false;

        double calculatedPp = ppCalculator.Calculate(CancellationToken.None).Total;

        Assert.That(calculatedPp, Is.EqualTo(expectedPp).Within(0.05));
    }

    [Test]
    [TestCase(29, 0, 1, 947, "HD,DT", 554.639d, 19831001, 5674368)]
    [TestCase(23, 1, 1, 1347, "HD,DT", 893.138d, 52018458, 5374538)]
    [TestCase(47, 0, 5, 851, "HD,DT", 682.345d, 31424313, 5374538)]
    [TestCase(75, 11, 11, 846, "HD,DT", 459.776d, 25666256, 5374538)]
    public void CalculateOsuTimedTest(int c100, int c50, int cMiss, int combo, string mods, double expectedPp, int score, int mapId)
        => CalculateTimedTest(c100, c50, cMiss, combo, mods, expectedPp, mapId, new OsuCalculator(), score: score);

    public void CalculateTimedTest(int c100, int? c50, int cMiss, int combo, string mods, double expectedPp, int mapId, BasePpCalculator ppCalculator, int score = 0)
    {
        ppCalculator.PreProcess(GetMapPath(mapId));
        ppCalculator.Hit300 = 0;
        ppCalculator.Katus = 0;
        ppCalculator.Goods = c100;
        ppCalculator.Mehs = c50;
        ppCalculator.Misses = cMiss;
        ppCalculator.Combo = combo;
        ppCalculator.Gekis = 0;
        ppCalculator.Score = score;
        ppCalculator.Mods = mods.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
        ppCalculator.UseScoreMultiplier = false;

        double calculatedPp = ppCalculator.Calculate(CancellationToken.None, endTime: ppCalculator.WorkingBeatmap.Length).Total;

        Assert.That(calculatedPp, Is.EqualTo(expectedPp).Within(0.05));
    }

    [Test]
    [TestCase(812010)]
    public void HasSamePpForDTAndDTNCScore(int mapId)
    {
        OsuCalculator dtPpCalculator = new();
        dtPpCalculator.PreProcess(GetMapPath(mapId));
        dtPpCalculator.Mods = new[] { "DT" };

        OsuCalculator ncdtPpCalculator = new();
        ncdtPpCalculator.PreProcess(GetMapPath(mapId));
        ncdtPpCalculator.Mods = new[] { "DT", "NC" };

        double dtPp = dtPpCalculator.Calculate(CancellationToken.None).Total;
        double ncdtPp = ncdtPpCalculator.Calculate(CancellationToken.None).Total;

        Assert.That(ncdtPp, Is.EqualTo(dtPp));
    }

    [Test]
    [TestCase(2462439, "", 500)]
    [TestCase(2462439, "DT", 500)]
    [TestCase(2462439, "HT", 500)]
    public void HasSamePpForStaticAndTimedCalculate(int mapId, string mods, double lengthCorrection = 0)
    {
        OsuCalculator ppCalculator = new();
        ppCalculator.PreProcess(GetMapPath(mapId));
        ppCalculator.Mods = mods.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);

        double ssPp = ppCalculator.Calculate(CancellationToken.None).Total;
        double endPerfectPp = ppCalculator.Calculate(CancellationToken.None, endTime: ppCalculator.WorkingBeatmap.Length + lengthCorrection).Total;

        Assert.That(ssPp, Is.EqualTo(endPerfectPp));
    }

    [Test]
    [TestCase(2462439, 59_936)]
    public void HasSamePpAtSpecificMapTimeWithTimedAndCutMap(int mapId, double cutTime, string cutOsuFileName = "", int rulesetId = -1)
    {
        if (string.IsNullOrEmpty(cutOsuFileName))
        {
            cutOsuFileName = $"{mapId}_cut.osu";
        }

        BasePpCalculator[] ppCalculators = rulesetId < 0
            ? [new OsuCalculator(), new TaikoCalculator(), new CtbCalculator(), new ManiaCalculator()]
            : [PpCalculatorHelpers.GetPpCalculator(rulesetId)];

        foreach (BasePpCalculator ppCalculator in ppCalculators)
        {
            foreach (string? mods in new[] { "", "DT", "HT" })
            {
                BasePpCalculator ppCalculator1 = (BasePpCalculator)ppCalculator.Clone();
                ppCalculator1.PreProcess($@".\cache\{cutOsuFileName}");
                ppCalculator1.Mods = mods.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                double cutPp = ppCalculator1.Calculate(CancellationToken.None).Total;

                BasePpCalculator ppCalculator2 = (BasePpCalculator)ppCalculator.Clone();
                ppCalculator2.PreProcess(GetMapPath(mapId));
                ppCalculator2.Mods = mods.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                double fullPp = ppCalculator2.Calculate(CancellationToken.None).Total;
                double timedPp = ppCalculator2.Calculate(CancellationToken.None, endTime: cutTime).Total;

                // 0.01: lazer CalculateTimed mid-attributes differ slightly from a fresh Calculates even after ppy/osu#38276.
                Assert.That(cutPp, Is.EqualTo(timedPp).Within(0.01), () => $"ModeId: {ppCalculator.RulesetId} Mods: {mods}");
                Assert.That(fullPp, Is.Not.EqualTo(timedPp), $"fullPp has same value! ModeId: {ppCalculator.RulesetId} Mods: {mods}");
            }
        }
    }

    public static string GetMapPath(int mapId)
    {
        if (!Directory.Exists("cache"))
        {
            _ = Directory.CreateDirectory("cache");
        }

        string cachePath = Path.Combine("cache", $"{mapId}.osu");
        if (!File.Exists(cachePath))
        {
            new FileWebRequest(cachePath, $"{base_url}/osu/{mapId}").Perform();
        }

        FileInfo cacheFileInfo = new(cachePath);

        if (cacheFileInfo.Length == 0)
        {
            cacheFileInfo.Delete();
            Assert.Inconclusive("Difficulty file download failed");
        }

        return cachePath;
    }
}
