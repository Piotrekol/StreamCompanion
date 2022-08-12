using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using osu.Framework.IO.Network;

namespace PpCalculator.Tests
{
    //TODO: revalidate values after pp changes are deployed & scores recalculated on osu side
    [TestFixture()]
    public class PpCalculatorTests
    {
        private const string base_url = "https://osu.ppy.sh";

        [Test]
        [TestCase(5, 0, 0, 595, "HD,DT", 636.205, 1185330)]
        [TestCase(9, 0, 0, 858, "HD,DT", 727.011, 2333273)]
        [TestCase(25, 0, 2, 1631, "HD,DT,HR", 1199.786, 2486881)]
        [TestCase(14, 0, 0, 1434, "HD,DT", 413.637, 812010)]
        [TestCase(8, 0, 0, 1573, "", 436.058, 1154766)]
        [TestCase(25, 6, 2, 2784, "HR", 442.889, 1228616)]
        [TestCase(60, 0, 6, 1015, "", 272.961, 3267957)]
        [TestCase(2, 0, 0, 1947, "HD,HR", 471.664, 2956396)]
        public void CalculateOsuTest(int c100, int c50, int cMiss, int combo, string mods, double expectedPp, int mapId)
            => CalculateTest(c100, c50, cMiss, combo, mods, expectedPp, mapId, new OsuCalculator());

        [Test]
        [TestCase(76, 0, 2, 1679, "HD,DT", 828.648, 1251239)]
        [TestCase(36, 0, 0, 2110, "HD,DT", 657.633, 2495119)]
        public void CalculateTaikoTest(int c100, int c50, int cMiss, int combo, string mods, double expectedPp, int mapId)
            => CalculateTest(c100, c50, cMiss, combo, mods, expectedPp, mapId, new TaikoCalculator());

        [Test]
        [TestCase(73, 79, 0, 1241, "HR", 822.357, 1972148)]
        [TestCase(25, 216, 0, 567, "HD,HR", 360.103, 2424031)]
        public void CalculateCtbTest(int c100, int c50, int cMiss, int combo, string mods, double expectedPp, int mapId)
            => CalculateTest(c100, c50, cMiss, combo, mods, expectedPp, mapId, new CtbCalculator());

        [Test]
        [TestCase(1, 0, 0, 2782, "", 654.971, 1270895, 993209)]
        [TestCase(6, 2, 4, 1830, "", 923.528, 2513195, 948258)]
        public void CalculateManiaTest(int c100, int c50, int cMiss, int combo, string mods, double expectedPp, int mapId, int score)
            => CalculateTest(c100, c50, cMiss, combo, mods, expectedPp, mapId, new ManiaCalculator(), score);

        public void CalculateTest(int c100, int c50, int cMiss, int combo, string mods, double expectedPp, int mapId, PpCalculator ppCalculator, int score = 0)
        {
            ppCalculator.PreProcess(GetMapPath(mapId));
            ppCalculator.Goods = c100;
            ppCalculator.Mehs = c50;
            ppCalculator.Misses = cMiss;
            ppCalculator.Combo = combo;
            ppCalculator.Score = score;
            ppCalculator.Mods = mods.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            var calculatedPp = ppCalculator.Calculate();

            //pp values match 1:1 values on osu! side, but osu!api values that we are comparing against are provided with 3 decimal points(rounded).
            Assert.That(calculatedPp, Is.EqualTo(expectedPp).Within(0.002));
        }

        [Test]
        [TestCase(2462439, "", 500)]
        [TestCase(2462439, "DT", 500)]
        [TestCase(2462439, "HT", 500)]
        public void HasSamePpForStaticAndTimedCalculate(int mapId, string mods, double lengthCorrection = 0)
        {
            var ppCalculator = new OsuCalculator();
            ppCalculator.PreProcess(GetMapPath(mapId));
            ppCalculator.Mods = mods.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            var ssPp = ppCalculator.Calculate();
            var endPerfectPp = ppCalculator.Calculate(ppCalculator.WorkingBeatmap.Length + lengthCorrection, new Dictionary<string, double>());

            Assert.AreEqual(ssPp, endPerfectPp);
        }

        [Test]
        [TestCase(2462439, 59_936)]
        public void HasSamePpAtSpecificMapTimeWithTimedAndCutMap(int mapId, double cutTime)
        {
            foreach (var mods in new[] { "", "DT", "HT" })
                _HasSamePpAtSpecificMapTimeWithTimedAndCutMap(mapId, mods, cutTime);
        }

        private void _HasSamePpAtSpecificMapTimeWithTimedAndCutMap(int mapId, string mods, double cutTime)
        {
            var ppCalculator1 = new OsuCalculator();
            ppCalculator1.PreProcess($@".\cache\{mapId}_cut.osu");
            ppCalculator1.Mods = mods.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var cutPp = ppCalculator1.Calculate();

            var ppCalculator2 = new OsuCalculator();
            ppCalculator2.PreProcess(GetMapPath(mapId));
            ppCalculator2.Mods = mods.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var fullPp = ppCalculator2.Calculate();
            var timedPp = ppCalculator2.Calculate(cutTime, new Dictionary<string, double>());

            Assert.That(cutPp, Is.EqualTo(timedPp).Within(0.001), () => $"Mods: {mods}");
            Assert.AreNotEqual(fullPp, timedPp, $"fullPp has same value! Mods: {mods}");
        }

        public static string GetMapPath(int mapId)
        {
            if (!Directory.Exists("cache"))
                Directory.CreateDirectory("cache");

            string cachePath = Path.Combine("cache", $"{mapId}.osu");
            if (!File.Exists(cachePath))
            {
                new FileWebRequest(cachePath, $"{base_url}/osu/{mapId}").Perform();
            }

            return cachePath;
        }
    }
}
