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
    [TestCase(15, 0, 0, 2469, "HR", 818.116, 4886597)]
    [TestCase(4, 0, 0, 2900, "", 783.819, 3881559)]
    [TestCase(18, 0, 0, 2100, "NF", 774.808, 3946158)]
    [TestCase(46, 0, 0, 3821, "NF", 759.175, 4181770)]
    [TestCase(19, 0, 0, 1775, "", 749.401, 3385971)]
    [TestCase(31, 0, 1, 1714, "", 720.843, 4318971)]
    [TestCase(20, 0, 0, 2745, "", 724.107, 3465034)]
    [TestCase(2, 0, 0, 192, "NC", 706.966, 2245774)]
    [TestCase(3, 0, 0, 522, "NC", 693.854, 2790477)]
    [TestCase(22, 0, 0, 2427, "HR", 680.506, 3666536)]
    [TestCase(18, 0, 2, 241, "DT", 682.051, 4565942)]
    [TestCase(15, 0, 0, 2846, "", 679.348, 3837137)]
    [TestCase(6, 0, 0, 1421, "HR", 656.645, 4064320)]
    [TestCase(9, 0, 0, 963, "", 662.744, 4720294)]
    [TestCase(11, 3, 0, 2583, "", 660.952, 3647082)]
    [TestCase(29, 3, 0, 2795, "", 669.863, 2077121)]
    [TestCase(21, 0, 2, 161, "NC", 616.203, 4834695)]
    [TestCase(4, 0, 4, 115, "NC", 627.325, 4836716)]
    [TestCase(27, 0, 5, 205, "NC", 584.004, 4853246)]
    [TestCase(4, 0, 0, 2186, "", 644.585, 2111505)]
    [TestCase(48, 5, 4, 2283, "", 637.526, 4704020)]
    [TestCase(3, 0, 1, 201, "DT,HR,HD", 622.377, 2596018)]
    [TestCase(15, 0, 0, 3809, "", 646.29, 3415931)]
    [TestCase(16, 0, 0, 2500, "", 641.134, 3648544)]
    [TestCase(9, 0, 0, 2161, "HR", 624.684, 1754777)]
    [TestCase(81, 0, 0, 3842, "", 633.646, 1860169)]
    [TestCase(32, 3, 1, 3182, "", 630.134, 1997892)]
    [TestCase(1, 0, 0, 333, "DT", 638.306, 2596015)]
    [TestCase(28, 2, 1, 2254, "", 648.946, 2244060)]
    [TestCase(8, 0, 2, 219, "DT", 641.746, 4905645)]
    [TestCase(8, 0, 2, 401, "DT", 635.268, 2469345)]
    [TestCase(7, 0, 0, 610, "NC", 636.587, 1885374)]
    [TestCase(27, 0, 0, 2136, "", 640.028, 3975714)]
    [TestCase(5, 0, 0, 1010, "HR,SO", 614.671, 1621319)]
    [TestCase(54, 0, 9, 874, "", 610.188, 1816113)]
    [TestCase(2, 0, 0, 503, "DT", 619.302, 2360153)]
    [TestCase(2, 0, 1, 145, "NC", 622.86, 4419203)]
    [TestCase(15, 0, 0, 510, "DT", 618.056, 1988911)]
    [TestCase(23, 0, 1, 1201, "", 604.227, 1985735)]
    [TestCase(8, 0, 8, 230, "NC", 563.604, 4844447)]
    [TestCase(1, 0, 0, 492, "DT", 611.944, 2444149)]
    [TestCase(24, 1, 2, 313, "DT", 596.985, 795810)]
    [TestCase(8, 0, 0, 458, "DT", 609.179, 2200001)]
    [TestCase(38, 0, 0, 1790, "", 603.353, 3644487)]
    [TestCase(13, 3, 0, 1770, "", 603.721, 3385967)]
    [TestCase(32, 0, 9, 49, "DT", 516.28, 4876550)]
    [TestCase(9, 0, 0, 971, "HR", 591.919, 4551189)]
    [TestCase(18, 0, 0, 2837, "", 596.582, 2077119)]
    [TestCase(11, 0, 7, 72, "NC", 530.869, 4905829)]
    [TestCase(10, 0, 4, 102, "DT", 520.924, 4874355)]
    [TestCase(31, 0, 7, 1282, "HD", 420.726, 3359653)]
    public void CalculateOsuTest(int c100, int c50, int cMiss, int combo, string mods, double expectedPp, int mapId)
        => CalculateTest(c100, c50, cMiss, combo, mods, expectedPp, mapId, new OsuCalculator());

    [Test]
    [TestCase(217, 0, 71, 714, "DT", 377.1038, 2528629)]
    public void CalculateTaikoTest(int c100, int c50, int cMiss, int combo, string mods, double expectedPp, int mapId)
        => CalculateTest(c100, c50, cMiss, combo, mods, expectedPp, mapId, new TaikoCalculator());

    [Test]
    [TestCase(73, 0, 79, 0, 1241, "HR", 822.3440, 1972148)]
    [TestCase(165, 4, 397, 1, 1467, "HD,HR", 472.8664, 2264827)]
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
