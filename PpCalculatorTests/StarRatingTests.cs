using NUnit.Framework;
using PpCalculator;

namespace PpCalculatorTests;

[TestFixture()]
public class StarRatingTests
{
    [Test]
    [TestCase(5.76113d, 1185330)]
    [TestCase(5.7077d, 2333273)]
    [TestCase(6.10426d, 2486881)]
    [TestCase(2.43049d, 123)]
    [TestCase(2.06353d, 1254)]
    [TestCase(5.93998d, 1228616)]
    public void CalculateStarRating(double expectedSR, int mapId)
        => CalculateTest(expectedSR, mapId, new OsuCalculator());

    public void CalculateTest(double expectedSR, int mapId, PpCalculator.PpCalculator ppCalculator)
    {
        ppCalculator.PreProcess(PpCalculatorTests.GetMapPath(mapId));

        //Necessary to fill map attributes
        _ = ppCalculator.Calculate(CancellationToken.None);
        double calculatedSR = ppCalculator.DifficultyAttributesAt(double.MaxValue).StarRating;

        Assert.That(calculatedSR, Is.EqualTo(expectedSR).Within(0.002));
    }
}
