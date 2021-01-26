using System;
using System.Collections.Generic;
using System.IO;
using CollectionManager.DataTypes;
using NUnit.Framework;
using osu.Framework.IO.Network;
using Beatmap = StreamCompanionTypes.DataTypes.Beatmap;


namespace PpCalculator.Tests
{
    [TestFixture()]
    public class PpCalculatorTests
    {
        private const string base_url = "https://osu.ppy.sh";

        [Test]
        [TestCase(5, 0, 0, 595, "HD,DT", 628.534, 1185330)]
        [TestCase(9, 0, 0, 858, "HD,DT", 698.977, 2333273)]
        public void CalculateOsuTest(int c100, int c50, int cMiss, int combo, string mods, double expectedPp, int mapId)
            => CalculateTest(c100, c50, cMiss, combo, mods, expectedPp, mapId, new OsuCalculator());

        [Test]
        [TestCase(76, 0, 2, 1679, "HD,DT", 594.078, 1251239)]
        [TestCase(36, 0, 0, 2110, "HD,DT", 521.732, 2495119)]
        public void CalculateTaikoTest(int c100, int c50, int cMiss, int combo, string mods, double expectedPp, int mapId)
            => CalculateTest(c100, c50, cMiss, combo, mods, expectedPp, mapId, new TaikoCalculator());

        [Test]
        [TestCase(73, 79, 0, 1241, "HR", 822.357, 1972148)]
        [TestCase(25, 216, 0, 567, "HD,HR", 378.109, 2424031)]
        public void CalculateCtbTest(int c100, int c50, int cMiss, int combo, string mods, double expectedPp, int mapId)
            => CalculateTest(c100, c50, cMiss, combo, mods, expectedPp, mapId, new CtbCalculator());

        [Ignore("Star rating is diferent in osu and on site itself, resulting in slightly changed pp values(calculator SR has same values as one in osu! itself)")]
        [Test]
        [TestCase(1, 0, 0, 2782, "", 641.782, 1270895, 993209)]
        [TestCase(6, 2, 4, 1830, "", 768.33, 2513195, 948258)]
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

            Assert.That(calculatedPp, Is.EqualTo(expectedPp).Within(0.01));
        }

        private string GetMapPath(int mapId)
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
