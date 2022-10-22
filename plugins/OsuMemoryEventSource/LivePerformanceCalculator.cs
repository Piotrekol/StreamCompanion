using CollectionManager.Enums;
using System;
using System.Collections.Concurrent;
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
        private readonly RulesetPlayData ssPlayData = new() { Score = 1_000_000 };
        public int ComboLeft { get; private set; } = 0;
        public double StrainPPIfBeatmapWouldEndNow { get; private set; }
        public double AimPPIfBeatmapWouldEndNow { get; private set; }
        public double SpeedPPIfBeatmapWouldEndNow { get; private set; }
        public double AccPPIfBeatmapWouldEndNow { get; private set; }

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
            {
                ppCalculators[name] = ppCalculator = (IPpCalculator)PpCalculator?.Clone();
                if (ppCalculator != null)
                    ppCalculator.UseScoreMultiplier = false;
            }

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
                return ppCalculator.Calculate(CancellationToken.None).Total;
            }

            ComboLeft = ppCalculator.GetMaxCombo(PlayTime);
            var newMaxCombo = Math.Max(Play.MaxCombo, ComboLeft + Play.Combo);
            PreparePpCalculator(ppCalculator, Play);
            ppCalculator.Combo = newMaxCombo;
            return ppCalculator.Calculate(CancellationToken.None).Total;
        }

        public double PPIfBeatmapWouldEndNow()
        {
            var ppCalculator = GetPpCalculator();
            if (ppCalculator == null || PlayTime <= 0)
            {
                SetPerformanceAttributesStats(double.NaN, double.NaN, double.NaN, double.NaN);
                return double.NaN;
            }


            PreparePpCalculator(ppCalculator, Play);
            var attribs = ppCalculator.Calculate(CancellationToken.None, endTime: PlayTime);
            if (attribs.Total < 0 || double.IsInfinity(attribs.Total))
            {
                SetPerformanceAttributesStats(double.NaN, double.NaN, double.NaN, double.NaN);
                return double.NaN;
            }

            switch (attribs)
            {
                case OsuPerformanceAttributes osuAttribs:
                    SetPerformanceAttributesStats(osuAttribs.Aim, osuAttribs.Speed, osuAttribs.Accuracy, double.NaN);
                    break;
                case TaikoPerformanceAttributes taikoAttribs:
                    SetPerformanceAttributesStats(double.NaN, double.NaN, taikoAttribs.Accuracy, taikoAttribs.Difficulty);
                    break;
                case ManiaPerformanceAttributes maniaAttribs:
                    SetPerformanceAttributesStats(double.NaN, double.NaN, double.NaN, maniaAttribs.Difficulty);
                    break;
                //Nothing for Catch
                default:
                    SetPerformanceAttributesStats(double.NaN, double.NaN, double.NaN, double.NaN);
                    break;
            }

            return attribs.Total;
        }

        private void SetPerformanceAttributesStats(double aim, double speed, double accuracy, double strain)
        {
            AimPPIfBeatmapWouldEndNow = aim;
            SpeedPPIfBeatmapWouldEndNow = speed;
            AccPPIfBeatmapWouldEndNow = accuracy;
            StrainPPIfBeatmapWouldEndNow = strain;
        }

        public double NoChokePp()
        {
            var ppCalculator = GetPpCalculator();
            if (ppCalculator == null || _currentPlayMode == PlayMode.OsuMania)
                return double.NaN;

            PreparePpCalculator(ppCalculator, Play);
            ppCalculator.Misses = 0;
            ppCalculator.Combo = null;
            ppCalculator.PercentCombo = 100;
            var attribs = ppCalculator.Calculate(CancellationToken.None, endTime: PlayTime);
            return attribs.Total < 0 || double.IsInfinity(attribs.Total)
                ? double.NaN
                : attribs.Total;
        }

        public double SimulatedPp()
        {
            var ppCalculator = GetPpCalculator();
            if (ppCalculator == null)
                return double.NaN;

            PreparePpCalculator(ppCalculator, ssPlayData);
            ppCalculator.Combo = null;
            ppCalculator.PercentCombo = 100;
            var attribs = ppCalculator.Calculate(CancellationToken.None, endTime: PlayTime);
            return attribs.Total < 0 || double.IsInfinity(attribs.Total)
                ? double.NaN
                : attribs.Total;
        }

        private void PreparePpCalculator(IPpCalculator ppCalculator, RulesetPlayData play)
        {
            ppCalculator.Goods = play.Hit100;
            ppCalculator.Mehs = play.Hit50;
            ppCalculator.Misses = play.HitMiss;
            ppCalculator.Katus = Play.HitKatu;
            ppCalculator.Hit300 = Play.Hit300;
            ppCalculator.Combo = play.MaxCombo;
            ppCalculator.Score = play.Score;
        }
    }
}