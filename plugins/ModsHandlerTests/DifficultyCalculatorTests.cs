using NUnit.Framework;
using ModsHandler;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks;
using CollectionManager.DataTypes;
using Beatmap = StreamCompanionTypes.DataTypes.Beatmap;

namespace ModsHandler.Tests
{
    [TestFixture()]
    public class DifficultyCalculatorTests
    {
        private Dictionary<Mods, List<float>> OD_VALUES = new Dictionary<Mods, List<float>>
        {
            {Mods.Omod,  new List<float>{10f, 9f, 8f, 7f, 6f, 5f, 4f, 3f, 2f, 1f, 0} },
            {Mods.Dt,  new List<float>{11.08f, 10.42f, 9.75f, 9.08f, 8.42f, 7.75f, 7.08f, 6.42f, 5.75f, 5.08f, 4.42f} },
            {Mods.Ht,  new List<float>{8.92f, 7.54f, 6.25f, 4.92f, 3.54f, 2.25f, 0.92f, -0.42f, -1.75f, -3.08f, -4.42f} },
            {Mods.Hr,  new List<float>{10f, 10f, 10f, 9.8f, 8.4f, 7f, 5.6f, 4.2f, 2.8f, 1.4f, 0} },
            {Mods.Hr | Mods.Dt,  new List<float>{11.08f, 11.08f, 11.08f, 10.97f, 10.08f, 9.08f, 8.19f, 7.31f, 6.31f, 5.42f, 4.42f} },
            {Mods.Hr | Mods.Ht,  new List<float>{8.92f, 8.92f, 8.92f, 8.69f, 6.92f, 4.92f, 3.14f, 1.36f, -0.64f, -2.42f, -4.42f} },
            {Mods.Ez,  new List<float>{5f, 4.5f, 4f, 3.5f, 3f, 2.5f, 2f, 1.5f, 1f, 0.5f, 0} },
            {Mods.Ez | Mods.Dt,  new List<float>{7.75f, 7.42f, 7.08f, 6.75f, 6.42f, 6.08f, 5.75f, 5.42f, 5.08f, 4.75f, 4.42f} },
            {Mods.Ez | Mods.Ht,  new List<float>{2.25f, 1.54f, 0.92f, 0.25f, 0.42f, -1.08f, -1.75f, -2.42f, -3.08f, -3.75f, -4.42f} },
        };

        [Test]
        [TestCase(Mods.Omod)]
        [TestCase(Mods.Ht)]
        [TestCase(Mods.Hr)]
        [TestCase(Mods.Hr | Mods.Dt)]
        [TestCase(Mods.Hr | Mods.Ht)]
        [TestCase(Mods.Ez)]
        [TestCase(Mods.Ez | Mods.Dt)]
        [TestCase(Mods.Ez | Mods.Ht)]
        public void TestOD(Mods mods)
        {
            var diffCalc = new DifficultyCalculator();
            var beatmap = new Beatmap();

            var resultValues = new List<float>();
            for (var i = 10; i >= 0; i--)
            {
                beatmap.OverallDifficulty = i;
                var odResult = diffCalc.ApplyMods(beatmap, mods)["OD"];
                resultValues.Add((float)Math.Round(odResult, 2));
            }
            CollectionAssert.AreEqual(OD_VALUES[mods], resultValues);
        }
    }
}