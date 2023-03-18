using System;
using System.Collections.Generic;
using System.Threading;
using CollectionManager.Enums;
using PpCalculatorTypes;

namespace StreamCompanion.Common
{
    public static class PpCalculatorExtensions
    {
        public static Dictionary<int, double> CalculateStrains(this IPpCalculator ppCalculator,
            CancellationToken cancellationToken, int? targetAmount = null)
        {
            if (ppCalculator.RulesetId == (int)PlayMode.OsuMania)
                return CalculateHitObjectDensity(ppCalculator, cancellationToken, targetAmount);

            var strains = new Dictionary<int, double>(targetAmount ?? 200);
            if (ppCalculator == null)
                return strains;

            ppCalculator.Score = 1_000_000;
            var mapLength = ppCalculator.BeatmapLength;
            var (interval, strainLength) = CalculateStrainParameters(ppCalculator, targetAmount);
            var time = 0;

            while (time + strainLength / 2 < mapLength)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var ppValue = ppCalculator.Calculate(cancellationToken, time, time + strainLength).Total;
                if (double.IsNaN(ppValue) || ppValue < 0)
                    ppValue = 0;
                else if (ppValue > 2000)
                    ppValue = 2000; //lets not freeze everything with aspire/fancy 100* maps

                strains.Add(time, ppValue);
                time += interval;
            }

            return strains;
        }

        public static Dictionary<int, double> CalculateHitObjectDensity(this IPpCalculator ppCalculator,
            CancellationToken cancellationToken, int? targetAmount = null)
        {
            if (ppCalculator == null)
                return null;

            var strains = new Dictionary<int, double>(targetAmount ?? 200);
            var (interval, _) = CalculateStrainParameters(ppCalculator, targetAmount);
            var graphValues = ppCalculator.CalculateProgressGraphValues(cancellationToken, targetAmount ?? 200);
            var time = 0;
            foreach (var graphValue in graphValues)
            {
                strains[time] = graphValue;
                time += interval;
            }

            return strains;
        }

        private static (int Interval, int StrainLength) CalculateStrainParameters(IPpCalculator ppCalculator, int? targetAmount = null)
        {
            var mapLength = ppCalculator.BeatmapLength;
            //data for 2min map
            var strainLength = 5000;
            var interval = 1500;

            if (targetAmount.HasValue)
            {
                strainLength = Convert.ToInt32(Math.Floor(strainLength * (mapLength / 120_000d)));
                interval = Convert.ToInt32(Math.Ceiling(mapLength - (strainLength / 2d)) / targetAmount) + 1;
            }

            return (interval, strainLength);
        }
    }
}