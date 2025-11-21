using NUnit.Framework;
using PpCalculator;

namespace PpCalculatorTests;

[TestFixture()]
public class StarRatingTests
{
    [Test]
    [TestCase(7.984, 4318971)]
    [TestCase(7.639, 3465034)]
    [TestCase(6.043, 2245774)]
    [TestCase(5.747, 2790477)]
    [TestCase(6.757, 3666536)]
    [TestCase(6.511, 4565942)]
    [TestCase(8.304, 1816113)]
    [TestCase(5.843, 386728)]
    [TestCase(4.842, 2850905)]
    [TestCase(4.504, 812010)]
    [TestCase(4.572, 1582863)]
    [TestCase(4.576, 703177)]
    [TestCase(4.738, 1253252)]
    [TestCase(5.85, 1056140)]
    [TestCase(4.45, 519080)]
    [TestCase(5.811, 949029)]
    [TestCase(6.228, 3453813)]
    [TestCase(6.527, 581623)]
    [TestCase(4.482, 1483372)]
    [TestCase(6.622, 1289926)]
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
