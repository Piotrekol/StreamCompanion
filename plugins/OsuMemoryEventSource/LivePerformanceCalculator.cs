using CollectionManager.Enums;
using OsuMemoryDataProvider;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using OsuMemoryDataProvider.OsuMemoryModels.Abstract;
using OsuMemoryDataProvider.OsuMemoryModels.Direct;
using PpCalculatorTypes;

namespace OsuMemoryEventSource
{
    public class LivePerformanceCalculator
    {
        public RulesetPlayData Play { get; set; } = new Player();
        public LeaderBoard LeaderBoard { get; set; } = new();
        public int PlayTime { get; set; }
        private PlayMode _currentPlayMode;
        public IPpCalculator PpCalculator { get; private set; }
        private ConcurrentDictionary<string, IPpCalculator> ppCalculators = new();
        private Dictionary<string, double> attribs = new();
        private readonly RulesetPlayData ssPlayData = new() { Score = 1_000_000 };

        public int ComboLeft { get; private set; } = 0;
        public double StrainPPIfBeatmapWouldEndNow { get; private set; } = double.NaN;
        public double AimPPIfBeatmapWouldEndNow { get; private set; } = double.NaN;
        public double SpeedPPIfBeatmapWouldEndNow { get; private set; } = double.NaN;
        public double AccPPIfBeatmapWouldEndNow { get; private set; } = double.NaN;

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
            ppCalculators.Clear();
        }

        private IPpCalculator GetPpCalculator([CallerMemberName] string name = "")
        {
            if (!ppCalculators.TryGetValue(name, out var ppCalculator))
                ppCalculators[name] = ppCalculator = (IPpCalculator)PpCalculator?.Clone();

            return ppCalculator;
        }

        public double PPIfRestFCed()
        {
            double pp = double.NaN;
            var ppCalculator = GetPpCalculator();
            if (ppCalculator == null || ppCalculator.RulesetId == (int)PlayMode.OsuMania)
                return pp;

            if (PlayTime <= 0)
            {
                PreparePpCalculator(ppCalculator, ssPlayData);
                ppCalculator.Combo = null;
                ppCalculator.PercentCombo = 100;
                ComboLeft = ppCalculator.GetMaxCombo(PlayTime);
                return ppCalculator.Calculate();
            }

            ComboLeft = ppCalculator.GetMaxCombo(PlayTime);
            var newMaxCombo = Math.Max(Play.MaxCombo, ComboLeft + Play.Combo);
            PreparePpCalculator(ppCalculator, Play);
            ppCalculator.Combo = newMaxCombo;
            return ppCalculator.Calculate();
        }

        public double PPIfBeatmapWouldEndNow()
        {
            var ppCalculator = GetPpCalculator();
            if (ppCalculator != null && PlayTime > 0)
            {
                PreparePpCalculator(ppCalculator, Play);
                attribs.Clear();
                var pp = ppCalculator.Calculate(PlayTime, attribs);
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
            var ppCalculator = GetPpCalculator();
            if (ppCalculator == null || _currentPlayMode == PlayMode.OsuMania)
                return pp;

            PreparePpCalculator(ppCalculator, Play);
            ppCalculator.Misses = 0;
            ppCalculator.Combo = null;
            ppCalculator.PercentCombo= 100;
            pp = ppCalculator.Calculate(PlayTime, null);
            return double.IsInfinity(pp)
                ? double.NaN
                : pp;
        }

        public double SimulatedPp()
        {
            double pp = double.NaN;
            var ppCalculator = GetPpCalculator();
            if (ppCalculator == null)
                return pp;

            PreparePpCalculator(ppCalculator, ssPlayData);
            ppCalculator.Combo = null;
            ppCalculator.PercentCombo= 100;
            pp = ppCalculator.Calculate(PlayTime, null);
            return double.IsInfinity(pp)
                ? double.NaN
                : pp;
        }

        private void PreparePpCalculator(IPpCalculator ppCalculator, RulesetPlayData play)
        {
            ppCalculator.Goods = play.Hit100;
            ppCalculator.Mehs = play.Hit50;
            ppCalculator.Misses = play.HitMiss;
            ppCalculator.Katsus = Play.HitKatu;
            ppCalculator.Combo = play.MaxCombo;
            ppCalculator.Score = play.Score;
        }
    }
}