using NUnit.Framework;

namespace PpCalculator.Tests
{
    [TestFixture()]
    public class StarRatingTests
    {
        [Test]
        [TestCase(5.7844d, 1185330)]
        [TestCase(5.7661d, 2333273)]
        [TestCase(6.1415d, 2486881)]
        [TestCase(2.4288d, 123)]
        [TestCase(2.068d, 1254)]
        [TestCase(6.02512d, 1228616)]
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
