using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Osu;
using PpCalculator;

namespace PpCalculatorTests;

[TestFixture]
public class StrainValuesCalculatorTests
{
    [Test]
    public void GetStrainsCoversWholeMapTimeline()
    {
        ProcessorWorkingBeatmap workingBeatmap = new(PpCalculatorTests.GetMapPath(5494538));

        Dictionary<int, double> strains = StrainValuesCalculator.GetStrains(new OsuRuleset(), workingBeatmap, Array.Empty<Mod>(), 100);

        Assert.That(strains, Has.Count.EqualTo(100));
        Assert.That(strains.Keys, Is.Ordered.Ascending);
        Assert.That(strains.Keys.First(), Is.EqualTo(0));
        // Graph must span the whole map: from millisecond 0 up to the end of the last hit object.
        Assert.That(strains.Keys.Last(), Is.GreaterThanOrEqualTo(0.9 * workingBeatmap.Beatmap.HitObjects[^1].GetEndTime()));
        Assert.That(strains.Values.Max(), Is.GreaterThan(0));
    }
}
