using NUnit.Framework;
using osu.Framework.IO.Network;
using PpCalculator;
using BasePpCalculator = PpCalculator.PpCalculator;

namespace PpCalculatorTests;

//TODO: revalidate values after pp changes are deployed & scores recalculated on osu side
[TestFixture()]
public class PpCalculatorTests
{
    private const string base_url = "https://osu.ppy.sh";

    [Test]
    [TestCase(5, 0, 0, 595, "HD,DT", 629.9953, 1185330)]
    [TestCase(9, 0, 0, 858, "HD,DT", 705.8835, 2333273)]
    [TestCase(25, 0, 2, 1631, "HD,DT,HR", 1084.1293, 2486881)]
    [TestCase(14, 0, 0, 1434, "HD,DT", 404.2468, 812010)]
    [TestCase(8, 0, 0, 1573, "", 426.9051, 1154766)]
    [TestCase(25, 6, 2, 2784, "HR", 416.4768, 1228616)]
    [TestCase(60, 0, 6, 1015, "", 456.7430, 3267957)]
    [TestCase(2, 0, 0, 1947, "HD,HR", 466.9570, 2956396)]

    public void CalculateOsuTest(int c100, int c50, int cMiss, int combo, string mods, double expectedPp, int mapId)
        => CalculateTest(c100, c50, cMiss, combo, mods, expectedPp, mapId, new OsuCalculator());

    [Test]
    [TestCase(11, 0, 0, 1233, "HD,DT", 746.2931, 3716953)]
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

        //pp values match 1:1 values on osu! side, but osu!api values that we are comparing against are provided with 3 decimal points(rounded).
        Assert.That(calculatedPp, Is.EqualTo(expectedPp).Within(0.002));
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

        return cachePath;
    }
}
