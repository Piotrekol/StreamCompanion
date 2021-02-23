using CollectionManager.Enums;
using OsuMemoryDataProvider;
using System;
using System.Collections.Generic;
using System.Threading;
using OsuMemoryDataProvider.Models;
using PpCalculatorTypes;
using StreamCompanionTypes.DataTypes;

namespace OsuMemoryEventSource
{
    public class LivePerformanceCalculator
    {
        public Player Play { get; set; } = new Player();
        public int PlayTime { get; set; }
        public List<int> HitErrors { get; set; }
        private PlayMode _currentPlayMode;

        public IPpCalculator PpCalculator { get; private set; }

        public void SetPpCalculator(IPpCalculator ppCalculator, string mods, CancellationToken cancellationToken)
        {
            ppCalculator.Mods = mods.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            _currentPlayMode = (PlayMode)ppCalculator.RulesetId;

            try
            {
                ppCalculator.Calculate(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                PpCalculator = null;
                throw;
            }

            PpCalculator = ppCalculator;
        }

        public double PPIfRestFCed()
        {
            double pp = double.NaN;

            if (PpCalculator == null || PpCalculator.RulesetId == (int)PlayMode.OsuMania)
                return pp;

            try
            {
                if (PlayTime <= 0)
                {
                    PpCalculator.Goods = 0;
                    PpCalculator.Mehs = 0;
                    PpCalculator.Combo = null;
                    PpCalculator.PercentCombo = 100;
                    return PpCalculator.Calculate(); //fc pp
                }

                if (_currentPlayMode == PlayMode.CatchTheBeat)
                {
                    PpCalculator.Goods = Play.Hit100;
                    PpCalculator.Mehs = null;
                    PpCalculator.Misses = Play.HitMiss;
                    PpCalculator.Katsus = Play.HitKatu;
                    PpCalculator.Combo = Play.MaxCombo;
                    PpCalculator.Score = Play.Score;
                }

                var comboLeft = PpCalculator.GetMaxCombo((int)PlayTime);

                var newMaxCombo = Math.Max(Play.MaxCombo, comboLeft + Play.Combo);

                PpCalculator.Combo = newMaxCombo;

                pp = PpCalculator.Calculate(null);

            }
            catch { }

            return pp;

        }
        private string PlayDataToString(PlayContainer p)
        {
            return $"{p.C300}/{p.C100}/{p.C50}/{p.CMiss}|" +
                   $"acc:{p.Acc},combo: {p.Combo},maxCombo {p.MaxCombo}|";
        }
        public double StrainPPIfBeatmapWouldEndNow { get; private set; } = double.NaN;
        public double AimPPIfBeatmapWouldEndNow { get; private set; } = double.NaN;
        public double SpeedPPIfBeatmapWouldEndNow { get; private set; } = double.NaN;
        public double AccPPIfBeatmapWouldEndNow { get; private set; } = double.NaN;

        Dictionary<string, double> attribs = new Dictionary<string, double>();

        public double PPIfBeatmapWouldEndNow()
        {

            if (PpCalculator != null && PlayTime > 0)
                try
                {
                    PpCalculator.Goods = Play.Hit100;
                    PpCalculator.Mehs = Play.Hit50;
                    PpCalculator.Misses = Play.HitMiss;
                    PpCalculator.Combo = Play.MaxCombo;
                    PpCalculator.Score = Play.Score;
                    var pp = PpCalculator.Calculate(PlayTime, attribs);
                    if (!double.IsInfinity(pp))
                    {
                        switch (_currentPlayMode)
                        {
                            case PlayMode.Taiko:
                            case PlayMode.OsuMania:
                                StrainPPIfBeatmapWouldEndNow = attribs["Strain"];
                                AccPPIfBeatmapWouldEndNow = attribs["Accuracy"];
                                AimPPIfBeatmapWouldEndNow = double.NaN;
                                SpeedPPIfBeatmapWouldEndNow = double.NaN;
                                break;
                            case PlayMode.CatchTheBeat:
                                break;
                            default:
                                AimPPIfBeatmapWouldEndNow = attribs["Aim"];
                                SpeedPPIfBeatmapWouldEndNow = attribs["Speed"];
                                AccPPIfBeatmapWouldEndNow = attribs["Accuracy"];
                                break;
                        }

                        attribs.Clear();

                        return pp;
                    }

                    attribs.Clear();
                }
                catch { }
            AimPPIfBeatmapWouldEndNow = double.NaN;
            SpeedPPIfBeatmapWouldEndNow = double.NaN;
            AccPPIfBeatmapWouldEndNow = double.NaN;
            StrainPPIfBeatmapWouldEndNow = double.NaN;
            return double.NaN;
        }

        public double NoChokePp()
        {
            double pp = double.NaN;

            if (PpCalculator == null || _currentPlayMode == PlayMode.OsuMania)
                return pp;

            try
            {
                PpCalculator.Goods = Play.Hit100;
                PpCalculator.Mehs = Play.Hit50;
                PpCalculator.Misses = 0;
                PpCalculator.Combo = null;
                PpCalculator.PercentCombo = 100;
                PpCalculator.Score = Play.Score;
                pp = PpCalculator.Calculate(PlayTime, null);

                if (double.IsInfinity(pp))
                {
                    pp = Double.NaN;
                }
            }
            catch { }

            return pp;
        }

        public double SimulatedPp()
        {
            double pp = double.NaN;

            if (PpCalculator == null)
                return pp;

            try
            {
                PpCalculator.Goods = null;
                PpCalculator.Mehs = null;
                PpCalculator.Misses = 0;
                PpCalculator.Combo = null;
                PpCalculator.PercentCombo = 100;
                PpCalculator.Score = 1_000_000;
                pp = PpCalculator.Calculate(PlayTime, null);

                if (double.IsInfinity(pp))
                {
                    pp = Double.NaN;
                }
            }
            catch { }

            return pp;
        }

    }
}