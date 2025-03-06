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
    [TestCase(15, 0, 0, 2469, "HR", 835.521, 4886597)]
    [TestCase(4, 0, 0, 2900, "", 788.634, 3881559)]
    [TestCase(18, 0, 0, 2100, "NF", 786.788, 3946158)]
    [TestCase(46, 0, 0, 3821, "NF", 760.562, 4181770)]
    [TestCase(19, 0, 0, 1775, "", 757.548, 3385971)]
    [TestCase(31, 0, 1, 1714, "", 730.521, 4318971)]
    [TestCase(20, 0, 0, 2745, "", 714.517, 3465034)]
    [TestCase(2, 0, 0, 192, "NC", 703.108, 2245774)]
    [TestCase(3, 0, 0, 522, "NC", 688.091, 2790477)]
    [TestCase(22, 0, 0, 2427, "HR", 687.466, 3666536)]
    [TestCase(18, 0, 2, 241, "DT", 685.45, 4565942)]
    [TestCase(15, 0, 0, 2846, "", 674.184, 3837137)]
    [TestCase(6, 0, 0, 1421, "HR", 673.033, 4064320)]
    [TestCase(9, 0, 0, 963, "", 668.366, 4720294)]
    [TestCase(11, 3, 0, 2583, "", 667.7, 3647082)]
    [TestCase(29, 3, 0, 2795, "", 662.597, 2077121)]
    [TestCase(21, 0, 2, 161, "NC", 655.985, 4834695)]
    [TestCase(4, 0, 4, 115, "NC", 651.322, 4836716)]
    [TestCase(27, 0, 5, 205, "NC", 650.604, 4853246)]
    [TestCase(4, 0, 0, 2186, "", 649.793, 2111505)]
    [TestCase(48, 5, 4, 2283, "", 648.362, 4704020)]
    [TestCase(3, 0, 1, 201, "DT,HR,HD", 646.328, 2596018)]
    [TestCase(15, 0, 0, 3809, "", 643.578, 3415931)]
    [TestCase(16, 0, 0, 2500, "", 643.308, 3648544)]
    [TestCase(9, 0, 0, 2161, "HR", 641.725, 1754777)]
    [TestCase(81, 0, 0, 3842, "", 640.108, 1860169)]
    [TestCase(32, 3, 1, 3182, "", 636.953, 1997892)]
    [TestCase(1, 0, 0, 333, "DT", 634.794, 2596015)]
    [TestCase(28, 2, 1, 2254, "", 632.101, 2244060)]
    [TestCase(8, 0, 2, 219, "DT", 629.201, 4905645)]
    [TestCase(8, 0, 2, 401, "DT", 629.061, 2469345)]
    [TestCase(7, 0, 0, 610, "NC", 624.509, 1885374)]
    [TestCase(27, 0, 0, 2136, "", 624.056, 3975714)]
    [TestCase(5, 0, 0, 1010, "HR,SO", 623.5, 1621319)]
    [TestCase(54, 0, 9, 874, "", 620.356, 1816113)]
    [TestCase(2, 0, 0, 503, "DT", 614.831, 2360153)]
    [TestCase(2, 0, 1, 145, "NC", 613.961, 4419203)]
    [TestCase(15, 0, 0, 510, "DT", 611.123, 1988911)]
    [TestCase(23, 0, 1, 1201, "", 610.296, 1985735)]
    [TestCase(8, 0, 8, 230, "NC", 605.55, 4844447)]
    [TestCase(1, 0, 0, 492, "DT", 605.264, 2444149)]
    [TestCase(24, 1, 2, 313, "DT", 602.797, 795810)]
    [TestCase(8, 0, 0, 458, "DT", 602.06, 2200001)]
    [TestCase(38, 0, 0, 1790, "", 599.985, 3644487)]
    [TestCase(13, 3, 0, 1770, "", 599.95, 3385967)]
    [TestCase(32, 0, 9, 49, "DT", 599.866, 4876550)]
    [TestCase(9, 0, 0, 971, "HR", 595.234, 4551189)]
    [TestCase(18, 0, 0, 2837, "", 593.239, 2077119)]
    [TestCase(11, 0, 7, 72, "NC", 587.612, 4905829)]
    [TestCase(10, 0, 4, 102, "DT", 570.448, 4874355)]
    public void CalculateOsuTest(int c100, int c50, int cMiss, int combo, string mods, double expectedPp, int mapId) 
        => CalculateTest(c100, c50, cMiss, combo, mods, expectedPp, mapId, new OsuCalculator());

    [Test]
    [TestCase(217, 0, 71, 714, "DT", 422.79, 2528629)]
    public void CalculateTaikoTest(int c100, int c50, int cMiss, int combo, string mods, double expectedPp, int mapId)
        => CalculateTest(c100, c50, cMiss, combo, mods, expectedPp, mapId, new TaikoCalculator());

    [Test]
    [TestCase(73, 0, 79, 0, 1241, "HR", 822.3440, 1972148)]
    [TestCase(165, 4, 397, 1, 1467, "HD,HR", 416.8892, 2264827)]
    public void CalculateCtbTest(int c100, int cKatu, int c50, int cMiss, int combo, string mods, double expectedPp, int mapId)
        => CalculateTest(c100, c50, cMiss, combo, mods, expectedPp, mapId, new CtbCalculator(), cKatu: cKatu);

    [Test]
    //mania score consists of: Geki(c300P),c300,Katu(c200),c100,c50,cMiss
    [TestCase(null, 1152, 133, 10, 10, 15, 788, "", 709.0555, 3563179)]
    [TestCase(2713, 504, 16, 3, 0, 0, 3797, "", 870.3794, 3563179)] // https://osu.ppy.sh/scores/mania/569388665
    [TestCase(null, 504, 16, 3, 0, 0, 3797, "", 870.3794, 3563179)] // same score as above - tests geki inference
    [TestCase(null, 1439, 30, 1, 0, 2, 4776, "", 787.2022, 3449261)] // https://osu.ppy.sh/scores/mania/565258177
    public void CalculateManiaTest(int? geki, int c300, int cKatu, int c100, int c50, int cMiss, int combo, string mods, double expectedPp, int mapId)
        => CalculateTest(c100, c50, cMiss, combo, mods, expectedPp, mapId, new ManiaCalculator(), c300, cKatu, geki);

    public void CalculateTest(int c100, int? c50, int cMiss, int combo, string mods, double expectedPp, int mapId, BasePpCalculator ppCalculator, int c300 = 0, int cKatu = 0, int? cGeki = 0)
    {
        ppCalculator.PreProcess(GetMapPath(mapId));
        ppCalculator.Hit300 = c300;
        ppCalculator.Katus = cKatu;
        ppCalculator.Goods = c100;
        ppCalculator.Mehs = c50;
        ppCalculator.Misses = cMiss;
        ppCalculator.Combo = combo;
        ppCalculator.Gekis = cGeki;
        ppCalculator.Mods = mods.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);

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

        var cacheFileInfo = new FileInfo(cachePath);

        if (cacheFileInfo.Length == 0)
        {
            cacheFileInfo.Delete();
            Assert.Inconclusive("Difficulty file download failed");
        }

        return cachePath;
    }
}
