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
    [TestCase(5, 0, 0, 595, "HD,DT", 636.042, 1185330)]
    [TestCase(9, 0, 0, 858, "HD,DT", 726.989, 2333273)]
    [TestCase(25, 0, 2, 1631, "HD,DT,HR", 1199.535, 2486881)]
    [TestCase(14, 0, 0, 1434, "HD,DT", 413.584, 812010)]
    [TestCase(8, 0, 0, 1573, "", 435.998, 1154766)]
    [TestCase(25, 6, 2, 2784, "HR", 442.877, 1228616)]
    [TestCase(60, 0, 6, 1015, "", 272.848, 3267957)]
    [TestCase(2, 0, 0, 1947, "HD,HR", 471.647, 2956396)]

    public void CalculateOsuTest(int c100, int c50, int cMiss, int combo, string mods, double expectedPp, int mapId)
        => CalculateTest(c100, c50, cMiss, combo, mods, expectedPp, mapId, new OsuCalculator());

    [Test]
    [TestCase(11, 0, 0, 1233, "HD,DT", 743.635, 3716953)]
    public void CalculateTaikoTest(int c100, int c50, int cMiss, int combo, string mods, double expectedPp, int mapId)
        => CalculateTest(c100, c50, cMiss, combo, mods, expectedPp, mapId, new TaikoCalculator());

    [Test]
    [TestCase(73, 0, 79, 0, 1241, "HR", 822.357, 1972148)]
    [TestCase(165, 4, 397, 1, 1467, "HD,HR", 417.887, 2264827, Ignore = "looks like osu lazer side bug - DifficultyCalculator.CalculateTimed is unable to calculate correct max combo for ctb attributes (including final max combo)")]
    public void CalculateCtbTest(int c100, int cKatu, int c50, int cMiss, int combo, string mods, double expectedPp, int mapId)
        => CalculateTest(c100, c50, cMiss, combo, mods, expectedPp, mapId, new CtbCalculator(), cKatu: cKatu);

    [Test]
    //mania score consists of: Geki(c300P, auto calculated),c300,Katu(c200),c100,c50,cMiss
    [TestCase(673, 20, 0, 0, 0, 3835, "", 901.925, 3563179, 990307)]
    [TestCase(1486, 131, 13, 11, 28, 1256, "", 795.277, 3449261, 913494)]
    public void CalculateManiaTest(int c300, int cKatu, int c100, int c50, int cMiss, int combo, string mods, double expectedPp, int mapId, int score)
        => CalculateTest(c100, c50, cMiss, combo, mods, expectedPp, mapId, new ManiaCalculator(), score, c300, cKatu);

    public void CalculateTest(int c100, int? c50, int cMiss, int combo, string mods, double expectedPp, int mapId, BasePpCalculator ppCalculator, int score = 0, int c300 = 0, int cKatu = 0)
    {
        ppCalculator.PreProcess(GetMapPath(mapId));
        ppCalculator.Hit300 = c300;
        ppCalculator.Katus = cKatu;
        ppCalculator.Goods = c100;
        ppCalculator.Mehs = c50;
        ppCalculator.Misses = cMiss;
        ppCalculator.Combo = combo;
        ppCalculator.Score = score;
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
