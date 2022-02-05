using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using osu.Framework.IO.Network;

namespace PpCalculator.Tests
{
    [TestFixture()]
    public class StarRatingTests
    {
        private const string base_url = "https://osu.ppy.sh";

        [Test]
        [TestCase(5.74062d, 1185330)]
        [TestCase(5.73106d, 2333273)]
        [TestCase(6.07848d, 2486881)]
        [TestCase(2.43246d, 123)]
        [TestCase(2.06473d, 1254)]
        public void CalculateStarRating(double expectedSR, int mapId)
            => CalculateTest(expectedSR, mapId, new OsuCalculator());

        public void CalculateTest(double expectedSR, int mapId, PpCalculator ppCalculator)
        {
            ppCalculator.PreProcess(PpCalculatorTests.GetMapPath(mapId));

            //Necessary to fill map attributes
            ppCalculator.Calculate();
            var calculatedSR = ppCalculator.AttributesAt(double.MaxValue).StarRating;

            Assert.That(calculatedSR, Is.EqualTo(expectedSR).Within(0.002));
        }

    }
}
