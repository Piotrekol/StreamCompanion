using CollectionManager.Enums;
using OsuMemoryDataProvider;
using System;
using System.Collections.Generic;
using System.Threading;
using OsuMemoryDataProvider.OsuMemoryModels.Abstract;
using OsuMemoryDataProvider.OsuMemoryModels.Direct;
using PpCalculatorTypes;

namespace OsuMemoryEventSource
{
    public class LivePerformanceCalculator
    {
        public RulesetPlayData Play { get; set; } = new Player();
        public int PlayTime { get; set; }
        private PlayMode _currentPlayMode;

        public IPpCalculator PpCalculator { get; private set; }

        public void SetPpCalculator(IPpCalculator ppCalculator, CancellationToken cancellationToken)
        {
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

            ComboLeft = PpCalculator.GetMaxCombo((int)PlayTime);

            var newMaxCombo = Math.Max(Play.MaxCombo, ComboLeft + Play.Combo);

            PpCalculator.Combo = newMaxCombo;

            pp = PpCalculator.Calculate(null);


            return pp;

        }
        private string PlayDataToString(PlayContainer p)
        {
            return $"{p.C300}/{p.C100}/{p.C50}/{p.CMiss}|" +
                   $"acc:{p.Acc},combo: {p.Combo},maxCombo {p.MaxCombo}|";
        }

        public int ComboLeft { get; private set; } = 0;
        public double StrainPPIfBeatmapWouldEndNow { get; private set; } = double.NaN;
        public double AimPPIfBeatmapWouldEndNow { get; private set; } = double.NaN;
        public double SpeedPPIfBeatmapWouldEndNow { get; private set; } = double.NaN;
        public double AccPPIfBeatmapWouldEndNow { get; private set; } = double.NaN;

        Dictionary<string, double> attribs = new Dictionary<string, double>();

        public double PPIfBeatmapWouldEndNow()
        {
            if (PpCalculator != null && PlayTime > 0)
            {
                PpCalculator.Goods = Play.Hit100;
                PpCalculator.Mehs = Play.Hit50;
                PpCalculator.Misses = Play.HitMiss;
                PpCalculator.Combo = Play.MaxCombo;
                PpCalculator.Score = Play.Score;
                attribs.Clear();
                var pp = PpCalculator.Calculate(PlayTime, attribs);
                if (!double.IsInfinity(pp))
                {
                    double accuracy, aim, strain, speed;
                    switch (_currentPlayMode)
                    {
                        case PlayMode.Taiko:
                        case PlayMode.OsuMania:
                            attribs.TryGetValue("Strain", out strain);
                            StrainPPIfBeatmapWouldEndNow = strain;
                            attribs.TryGetValue("Accuracy", out accuracy);
                            AccPPIfBeatmapWouldEndNow = accuracy;
                            AimPPIfBeatmapWouldEndNow = double.NaN;
                            SpeedPPIfBeatmapWouldEndNow = double.NaN;
                            break;
                        case PlayMode.CatchTheBeat:
                            ResetValues();
                            break;
                        default:
                            attribs.TryGetValue("Aim", out aim);
                            AimPPIfBeatmapWouldEndNow = aim;
                            attribs.TryGetValue("Speed", out speed);
                            SpeedPPIfBeatmapWouldEndNow = speed;
                            attribs.TryGetValue("Accuracy", out accuracy);
                            AccPPIfBeatmapWouldEndNow = accuracy;
                            StrainPPIfBeatmapWouldEndNow = double.NaN;
                            break;
                    }

                    return pp;
                }
            }
            ResetValues();
            return double.NaN;

            void ResetValues()
            {
                AimPPIfBeatmapWouldEndNow = double.NaN;
                SpeedPPIfBeatmapWouldEndNow = double.NaN;
                AccPPIfBeatmapWouldEndNow = double.NaN;
                StrainPPIfBeatmapWouldEndNow = double.NaN;
            }
        }

        public double NoChokePp()
        {
            double pp = double.NaN;

            if (PpCalculator == null || _currentPlayMode == PlayMode.OsuMania)
                return pp;


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


            return pp;
        }

        public double SimulatedPp()
        {
            double pp = double.NaN;

            if (PpCalculator == null)
                return pp;


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


            return pp;
        }

    }
}