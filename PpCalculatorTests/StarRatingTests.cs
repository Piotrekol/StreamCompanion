using NUnit.Framework;
using PpCalculator;

namespace PpCalculatorTests;

[TestFixture()]
public class StarRatingTests
{
    [Test]
    [TestCase(8.06d, 4318971)]
    [TestCase(7.64d, 3465034)]
    [TestCase(6.1d, 2245774)]
    [TestCase(5.79d, 2790477)]
    [TestCase(6.79d, 3666536)]
    [TestCase(6.57d, 4565942)]
    [TestCase(8.3d, 1816113)]
    [TestCase(5.88d, 386728)]
    [TestCase(4.73d, 2850905)]
    [TestCase(4.53d, 812010)]
    [TestCase(4.61d, 1582863)]
    [TestCase(4.6d, 703177)]
    [TestCase(4.74d, 1253252)]
    [TestCase(5.87d, 1056140)]
    [TestCase(4.47d, 519080)]
    [TestCase(5.85d, 949029)]
    [TestCase(6.24d, 3453813)]
    [TestCase(6.55d, 581623)]
    [TestCase(4.44d, 1483372)]
    [TestCase(6.64d, 1289926)]
    public void CalculateStarRating(double expectedSR, int mapId)
        => CalculateTest(expectedSR, mapId, new OsuCalculator());

    public void CalculateTest(double expectedSR, int mapId, PpCalculator.PpCalculator ppCalculator)
    {
        ppCalculator.PreProcess(PpCalculatorTests.GetMapPath(mapId));

        //Necessary to fill map attributes
        _ = ppCalculator.Calculate(CancellationToken.None);
        double calculatedSR = ppCalculator.DifficultyAttributesAt(double.MaxValue).StarRating;

        Assert.That(calculatedSR, Is.EqualTo(expectedSR).Within(0.005));
    }
}
