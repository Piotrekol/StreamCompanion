using NUnit.Framework;
using osu.Framework.IO.Network;
using PpCalculator;
using PpCalculatorTypes;
using BasePpCalculator = PpCalculator.PpCalculator;

namespace PpCalculatorTests;

[TestFixture]
public class PpCalculatorTests
{
    private const string base_url = "https://osu.ppy.sh";

    [Test]
    //[TestCase({count100}, {count50}, {countMiss}, {maxCombo}, "{enabledMods}", {ppValue}d, {score}, {beatmapId})]
    [TestCase(11, 0, 0, 543, "DT", 600.66d, 7337969, 5277324)]
    [TestCase(33, 1, 0, 2705, "HD,HR", 651.96d, 200661520, 3281757)]
    [TestCase(41, 1, 5, 1920, "", 547.69d, 97392950, 5143698)]
    [TestCase(9, 0, 0, 1363, "HD,DT", 624.4d, 45705016, 2079130)]
    [TestCase(5, 0, 0, 643, "HR", 582.83d, 9966935, 5211120)]
    [TestCase(25, 0, 3, 459, "DT,NC", 536.21d, 3753841, 4978117)]
    [TestCase(6, 0, 0, 493, "HD,DT", 554.61d, 4561464, 4933095)]
    [TestCase(35, 0, 0, 2942, "", 759.84d, 206184570, 2235465)]
    [TestCase(57, 0, 0, 3841, "", 663.35d, 318164760, 1860169)]
    [TestCase(0, 0, 0, 266, "HD,DT", 600.35d, 1579137, 5287832)]
    [TestCase(24, 2, 1, 1367, "", 623.03d, 84780530, 5020005)]
    [TestCase(1, 0, 0, 633, "", 594.72d, 8053454, 5206808)]
    [TestCase(40, 6, 2, 3109, "", 550.38d, 221069770, 5016427)]
    [TestCase(1, 0, 0, 267, "HD,DT", 678.83d, 1597198, 5279580)]
    [TestCase(4, 0, 0, 333, "", 525.55d, 2210430, 5091768)]
    [TestCase(5, 0, 0, 435, "HD,DT", 569.62d, 3915728, 5328092)]
    [TestCase(16, 2, 1, 2450, "", 548.86d, 116067726, 4861693)]
    [TestCase(11, 0, 0, 1070, "HR", 585.45d, 30477495, 4266525)]
    [TestCase(43, 0, 0, 832, "", 536.14d, 13195874, 4883674)]
    [TestCase(39, 0, 1, 1074, "HR", 594.92d, 25163123, 4767844)]
    [TestCase(56, 0, 0, 605, "HD,DT", 1065.53d, 7326847, 5186856)]
    [TestCase(0, 0, 3, 402, "HD,DT", 529.29d, 3416865, 5315940)]
    [TestCase(8, 0, 0, 1675, "HR", 574.1d, 52319804, 5119288)]
    [TestCase(1, 0, 0, 283, "HD,HR,DT,NC", 552.29d, 1693276, 4650478)]
    [TestCase(57, 0, 4, 1364, "", 712.41d, 43205300, 4155614)]
    [TestCase(4, 0, 0, 332, "HD,DT", 704.66d, 3112508, 4867588)]
    [TestCase(0, 0, 0, 263, "HD,DT", 514.19d, 1472613, 5331280)]
    [TestCase(1, 0, 0, 741, "HD,DT", 556.94d, 11049748, 1462836)]
    [TestCase(0, 0, 0, 385, "HD,DT", 618.98d, 2817055, 5242138)]
    [TestCase(3, 0, 0, 417, "HD,DT", 547.39d, 3694568, 5329484)]
    [TestCase(0, 0, 0, 185, "HD,DT", 524.14d, 779521, 4798302)]
    [TestCase(5, 0, 1, 407, "HD,DT", 509.35d, 3525994, 5328092)]
    [TestCase(3, 0, 0, 633, "", 583.77d, 8045598, 5206808)]
    [TestCase(10, 0, 0, 425, "HD,DT", 529.64d, 3744424, 5328092)]
    [TestCase(4, 0, 0, 1341, "HD,DT", 544.21d, 38587959, 3863682)]
    [TestCase(41, 0, 0, 1098, "", 583.66d, 19945870, 3760479)]
    [TestCase(3, 0, 0, 416, "HD,DT", 528.07d, 3372734, 5329379)]
    [TestCase(35, 0, 4, 943, "HD,HR", 584.94d, 26433364, 2417422)]
    [TestCase(21, 0, 0, 903, "HD,DT", 541.77d, 16436428, 140821)]
    [TestCase(3, 0, 5, 1020, "", 525.93d, 22877870, 4952940)]
    [TestCase(7, 0, 2, 359, "HD,DT", 504.78d, 2640698, 5327983)]
    [TestCase(1, 0, 0, 629, "HR", 601.84d, 8395633, 5341644)]
    [TestCase(7, 0, 1, 370, "HD,DT", 615.18d, 2917136, 5370003)]
    [TestCase(12, 0, 3, 360, "HD,DT", 517.44d, 2644047, 5370003)]
    [TestCase(13, 0, 0, 3852, "", 895.44d, 344167630, 5272081)]
    [TestCase(2, 0, 0, 621, "HD", 502.66d, 8813544, 4883675)]
    [TestCase(8, 1, 1, 1608, "", 567.96d, 62784690, 4775621)]
    [TestCase(15, 2, 0, 955, "HD,DT", 643.61d, 15921967, 1828961)]
    [TestCase(4, 0, 0, 434, "HD,DT", 579d, 3911093, 5328092)]
    [TestCase(12, 0, 1, 678, "DT", 816.48d, 9168624, 5271748)]
    public void CalculateOsuTest(int c100, int c50, int cMiss, int combo, string mods, double expectedPp, int score, int mapId)
        => CalculateTest(c100, c50, cMiss, combo, mods, expectedPp, mapId, new OsuCalculator(), score: score);

    [Test]
    //[TestCase({count100}, {count50}, {countMiss}, {maxCombo}, "{enabledMods}", {ppValue}d, {beatmapId})]
    [TestCase(31, 0, 10, 1616, "", 668.59d, 2438332)]
    [TestCase(117, 0, 9, 2376, "", 574.15d, 4907360)]
    [TestCase(0, 0, 0, 769, "SD,PF", 527.98d, 4667676)]
    [TestCase(69, 0, 2, 1896, "HD,DT,NC", 960.07d, 5023230)]
    [TestCase(98, 0, 0, 1100, "HR", 528.66d, 3391381)]
    [TestCase(40, 0, 0, 1444, "DT", 905.09d, 4839683)]
    [TestCase(144, 0, 10, 2705, "HD,DT,NC", 945.35d, 4433473)]
    [TestCase(172, 0, 59, 1544, "DT", 639.85d, 5315977)]
    [TestCase(98, 0, 2, 817, "DT,NC", 880.47d, 2456553)]
    [TestCase(158, 0, 6, 2852, "DT", 977.39d, 5315977)]
    public void CalculateTaikoTest(int c100, int c50, int cMiss, int combo, string mods, double expectedPp, int mapId)
        => CalculateTest(c100, c50, cMiss, combo, mods, expectedPp, mapId, new TaikoCalculator());

    [Test]
    //[TestCase({count100}, {countKatu}, {count50}, {countMiss}, {maxCombo}, "{enabledMods}", {ppValue}d, {beatmapId})]
    [TestCase(5, 0, 35, 0, 385, "HR", 529.84d, 5064281)]
    [TestCase(211, 8, 671, 0, 1773, "", 517.91d, 5100805)]
    [TestCase(95, 0, 99, 13, 1164, "NF", 605.94d, 3504487)]
    [TestCase(71, 10, 216, 0, 612, "DT", 668.44d, 3128300)]
    [TestCase(91, 0, 112, 0, 2606, "", 840.47d, 5299790)]
    [TestCase(8, 2, 52, 6, 430, "HR", 515.5d, 4675222)]
    [TestCase(6, 2, 94, 0, 808, "HD,HR", 598.95d, 5061628)]
    [TestCase(188, 0, 238, 0, 1841, "HD", 727.45d, 4877768)]
    [TestCase(231, 4, 322, 1, 2229, "HD", 774.75d, 5235089)]
    [TestCase(140, 3, 180, 1, 1351, "", 615.01d, 3578471)]
    public void CalculateCtbTest(int c100, int cKatu, int c50, int cMiss, int combo, string mods, double expectedPp, int mapId)
        => CalculateTest(c100, c50, cMiss, combo, mods, expectedPp, mapId, new CtbCalculator(), cKatu: cKatu);

    [Test]
    //[TestCase({countGeki}, {count300}, {countKatu}, {count100}, {count50}, {countMiss}, {maxCombo}, "{enabledMods}", {ppValue}d, {beatmapId})]
    [TestCase(2007, 152, 4, 0, 0, 0, 2315, "SD", 587.25d, 5339691)]
    [TestCase(null, 152, 4, 0, 0, 0, 2315, "SD", 587.25d, 5339691)]
    [TestCase(null, 2660, 533, 73, 19, 96, 970, "", 610.22d, 4574013)]
    [TestCase(null, 3699, 440, 55, 30, 42, 1231, "HT", 852.83d, 5074941)]
    [TestCase(null, 2353, 187, 16, 6, 28, 1232, "", 753.94d, 5153884)]
    [TestCase(null, 2604, 329, 67, 45, 93, 2127, "", 1040.06d, 5139638)]
    [TestCase(null, 930, 59, 10, 0, 3, 4697, "", 514.92d, 2769975)]
    [TestCase(null, 2261, 205, 23, 2, 45, 2107, "", 600.05d, 5048955)]
    [TestCase(null, 63, 0, 0, 0, 0, 3226, "DT", 717.54d, 5170433)]
    [TestCase(null, 263, 16, 4, 0, 2, 2249, "DT", 875.86d, 5207902)]
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

                Assert.That(cutPp, Is.EqualTo(timedPp).Within(0.001), () => $"ModeId: {ppCalculator.RulesetId} Mods: {mods}");
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
