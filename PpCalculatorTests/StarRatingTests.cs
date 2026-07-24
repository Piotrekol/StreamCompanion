using NUnit.Framework;
using PpCalculator;

namespace PpCalculatorTests;

[TestFixture()]
public class StarRatingTests
{
    [Test]
    [TestCase(8.138, 4318971)]
    [TestCase(7.878, 3465034)]
    [TestCase(5.91, 2245774)]
    [TestCase(5.726, 2790477)]
    [TestCase(6.83, 3666536)]
    [TestCase(6.485, 4565942)]
    [TestCase(8.523, 1816113)]
    [TestCase(6.094, 386728)]
    [TestCase(4.9, 2850905)]
    [TestCase(4.597, 812010)]
    [TestCase(4.513, 1582863)]
    [TestCase(4.739, 703177)]
    [TestCase(4.718, 1253252)]
    [TestCase(5.803, 1056140)]
    [TestCase(4.429, 519080)]
    [TestCase(5.788, 949029)]
    [TestCase(6.644, 3453813)]
    [TestCase(6.614, 581623)]
    [TestCase(4.575, 1483372)]
    [TestCase(6.62, 1289926)]
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
