using osu.Game.Beatmaps;
using osu.Game.Rulesets;
using osu.Game.Rulesets.Difficulty;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Scoring;
using osu.Game.Scoring;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using osu.Game.Rulesets.Osu.Objects;
using PpCalculatorTypes;
using DifficultyAttributes = PpCalculatorTypes.DifficultyAttributes;
using osu.Game.Beatmaps.Formats;
using osu.Game.Beatmaps.ControlPoints;

namespace PpCalculator
{
    public abstract class PpCalculator : IPpCalculator, ICloneable
    {
        public ProcessorWorkingBeatmap WorkingBeatmap { get; protected set; }
        private IBeatmap _playableBeatmap;
        public IBeatmap PlayableBeatmap
        {
            get
            {
                if (_playableBeatmap == null)
                    PreparePlayableBeatmap(CancellationToken.None);

                return _playableBeatmap;
            }
            protected set => _playableBeatmap = value;
        }
        protected abstract Ruleset Ruleset { get; }
        public virtual double Accuracy { get; set; } = 100;
        public virtual int? Combo { get; set; }
        public virtual double PercentCombo { get; set; } = 100;
        public virtual int Score { get; set; }
        private string[] _Mods { get; set; }
        private static readonly string[] _NCModArray = new[] { "NC" };
        private static readonly string[] _DTModArray = new[] { "DT" };
        public virtual string[] Mods
        {
            get => _Mods;
            set
            {
                if (_Mods == value || (value != null && _Mods != null && _Mods.SequenceEqual(value)))
                    return;

                if (value != null && value.Contains("NC"))
                {
                    IEnumerable<string> tempMods = value.AsEnumerable().Except(_NCModArray);
                    if (!value.Contains("DT"))
                        tempMods = tempMods.Concat(_DTModArray);

                    _Mods = tempMods.ToArray();
                }
                else
                    _Mods = value;

                scoreMultiplier = new Lazy<double>(CalculateScoreMultiplier);
            }
        }

        public virtual int Misses { get; set; }
        public virtual int? Mehs { get; set; }
        public virtual int? Goods { get; set; }
        public int? Katus { get; set; }
        public int? Hit300 { get; set; }

        protected virtual ScoreInfo ScoreInfo { get; set; } = new ScoreInfo();
        protected PerformanceCalculator PerformanceCalculator { get; set; }
        protected List<TimedDifficultyAttributes> TimedDifficultyAttributes { get; set; }
        protected string LastMods { get; set; } = null;
        protected bool ResetPerformanceCalculator { get; set; }

        public int RulesetId => Ruleset.RulesetInfo.OnlineID;
        public double BeatmapLength => WorkingBeatmap?.Length ?? 0;
        private Lazy<double> scoreMultiplier = new Lazy<double>(() => 1d);
        public double ScoreMultiplier => scoreMultiplier.Value;
        public bool UseScoreMultiplier { get; set; } = true;
        public bool HasFullBeatmap { get; private set; } = false;

        static PpCalculator()
        {
            //Required for <=v4 maps
            LegacyDifficultyCalculatorBeatmapDecoder.Register();
        }

        public object Clone()
        {
            var ppCalculator = CreateInstance();
            ppCalculator.WorkingBeatmap = WorkingBeatmap;
            ppCalculator._playableBeatmap = _playableBeatmap;
            ppCalculator._Mods = _Mods;
            ppCalculator.LastMods = LastMods;
            ppCalculator.scoreMultiplier = scoreMultiplier;
            ppCalculator.UseScoreMultiplier = UseScoreMultiplier;
            ppCalculator.HasFullBeatmap = HasFullBeatmap;
            if (PerformanceCalculator != null)
            {
                ppCalculator.ScoreInfo.Mods = ScoreInfo.Mods.Select(m => m.DeepClone()).ToArray();

                ppCalculator.PerformanceCalculator = Ruleset.CreatePerformanceCalculator();
                ppCalculator.TimedDifficultyAttributes = TimedDifficultyAttributes;
                ppCalculator.ResetPerformanceCalculator = false;
            }

            return ppCalculator;
        }

        internal void PreProcess(ProcessorWorkingBeatmap workingBeatmap)
        {
            WorkingBeatmap = workingBeatmap;

            ResetPerformanceCalculator = true;
        }
        public void PreProcess(string file) => PreProcess(new ProcessorWorkingBeatmap(file));


        public bool IsBreakTime(double time)
        {
            return PlayableBeatmap != null && PlayableBeatmap.Breaks.Any(b => b.StartTime <= time && b.EndTime >= time);
        }

        public IEnumerable<BreakPeriod> Breaks()
        {
            return PlayableBeatmap?.Breaks.Select(b => new BreakPeriod(b.StartTime, b.EndTime, b.HasEffect)) ?? Enumerable.Empty<BreakPeriod>();
        }

        public double FirstHitObjectTime()
            => PlayableBeatmap?.HitObjects.FirstOrDefault()?.StartTime ?? 0d;

        public IEnumerable<TimingPoint> TimingPoints()
        {
            return PlayableBeatmap?.ControlPointInfo?.TimingPoints.Select(tp => new TimingPoint(tp.Time, Math.Round(tp.BPM, 5), Math.Round(tp.BeatLength, 5))) ?? Enumerable.Empty<TimingPoint>();
        }
        public IEnumerable<KiaiPoint> KiaiPoints()
        {
            return PlayableBeatmap?.ControlPointInfo?.EffectPoints.Zip(
                    PlayableBeatmap.ControlPointInfo.EffectPoints.Skip(1), (first, second) => first.KiaiMode ? new KiaiPoint(first.Time, second.Time - first.Time) : null
                ).Where(e => e is not null) 
                ?? Enumerable.Empty<KiaiPoint>();
        }

        public DifficultyAttributes DifficultyAttributesAt(double time)
        {
            var attributes = TimedDifficultyAttributes?.LastOrDefault(x => x.Time <= time)?.Attributes;
            if (attributes == null)
                return null;

            return attributes.ConvertToSCAttributes(PlayableBeatmap.HitObjects);
        }

        public PpCalculatorTypes.PerformanceAttributes Calculate(CancellationToken cancellationToken, double? startTime = null, double? endTime = null)
        {
            try
            {
                return InternalCalculate(cancellationToken, startTime, endTime)
                    ?? new PpCalculatorTypes.PerformanceAttributes { Total = -1 };
            }
            catch (TimeoutException)
            {
                return new PpCalculatorTypes.PerformanceAttributes { Total = -2 };
            }
        }


        private PpCalculatorTypes.PerformanceAttributes InternalCalculate(CancellationToken cancellationToken, double? startTime = null, double? endTime = null)
        {
            if (WorkingBeatmap == null)
                return null;

            PreparePlayableBeatmap(cancellationToken);
            //huge performance gains by reusing existing performance calculator when possible
            var createPerformanceCalculator = PerformanceCalculator == null || ResetPerformanceCalculator;

            IReadOnlyList<HitObject> hitObjects = startTime.HasValue && endTime.HasValue
                ? PlayableBeatmap.HitObjects.Where(h => h.StartTime >= startTime && h.StartTime <= endTime).ToList()
                : endTime.HasValue
                    ? PlayableBeatmap.HitObjects.Where(h => h.StartTime <= endTime).ToList()
                    : startTime.HasValue
                        ? PlayableBeatmap.HitObjects.Where(h => h.StartTime >= startTime).ToList()
                        : PlayableBeatmap.HitObjects;

            if (!hitObjects.Any())
                return null;

            HasFullBeatmap = !(startTime.HasValue || endTime.HasValue);
            var workingBeatmap = WorkingBeatmap;
            if (startTime.HasValue)
            {
                createPerformanceCalculator = true;
                var tempMap = new Beatmap();
                tempMap.HitObjects.AddRange(hitObjects);
                tempMap.ControlPointInfo = workingBeatmap.Beatmap.ControlPointInfo;
                tempMap.BeatmapInfo = workingBeatmap.Beatmap.BeatmapInfo;
                workingBeatmap = new ProcessorWorkingBeatmap(tempMap);
            }

            ScoreInfo.Statistics = GenerateHitResults(Accuracy / 100, hitObjects, Misses, Mehs, Goods, Katus, Hit300);
            ScoreInfo.Accuracy = GetAccuracy(ScoreInfo.Statistics);
            ScoreInfo.MaxCombo = Combo ?? (int)Math.Round(PercentCombo / 100 * GetMaxCombo(hitObjects));
            ScoreInfo.TotalScore = UseScoreMultiplier ?
                (int)Math.Round(Score * ScoreMultiplier)
                : Score;

            if (createPerformanceCalculator)
            {
                var difficultyCalculator = Ruleset.CreateDifficultyCalculator(workingBeatmap);
                TimedDifficultyAttributes = difficultyCalculator.CalculateTimed(ScoreInfo.Mods, cancellationToken).ToList();
                PerformanceCalculator = Ruleset.CreatePerformanceCalculator();
                ResetPerformanceCalculator = false;
            }

            try
            {
                if (endTime.HasValue)
                {
                    var difficultyAttributes = TimedDifficultyAttributes.LastOrDefault(a => endTime.Value >= a.Time)?.Attributes ?? TimedDifficultyAttributes.First().Attributes;
                    return PerformanceCalculator.Calculate(ScoreInfo, difficultyAttributes).ConvertToSCAttributes();
                }

                var performanceAttributes = PerformanceCalculator.Calculate(ScoreInfo, TimedDifficultyAttributes.Last().Attributes);
                return performanceAttributes.ConvertToSCAttributes();
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }

        public int[] CalculateProgressGraphValues(CancellationToken cancellationToken, int amount = 100)
        {
            if (!HasFullBeatmap)
                Calculate(cancellationToken);

            var hitObjects = PlayableBeatmap.HitObjects;
            var values = new int[amount];
            if (!hitObjects.Any())
                return Array.Empty<int>();

            var firstObjectTime = hitObjects.First().StartTime;
            var lastObjectTime = hitObjects.Max(o => o.GetEndTime());

            if (lastObjectTime == 0)
                lastObjectTime = hitObjects.Last().StartTime;

            double interval = (lastObjectTime - firstObjectTime + 1) / amount;
            foreach (var hitObject in hitObjects)
            {
                double endTime = hitObject.GetEndTime();
                int startRange = (int)((hitObject.StartTime - firstObjectTime) / interval);
                int endRange = (int)((endTime - firstObjectTime) / interval);
                for (int i = startRange; i <= endRange; i++)
                    values[i]++;
            }

            return values;
        }

        private List<IMod> GetOsuMods(Ruleset ruleset)
        {
            var mods = new List<IMod>();
            if (_Mods == null)
                return mods;

            var availableMods = ruleset.AllMods;
            foreach (var modString in _Mods)
            {
                IMod newMod = availableMods.FirstOrDefault(m => string.Equals(m.Acronym, modString, StringComparison.CurrentCultureIgnoreCase));
                if (newMod == null)
                {
                    continue;
                }

                mods.Add(newMod);
            }

            return mods;
        }

        public int GetMaxCombo(int? fromTime = null)
        {
            if (PlayableBeatmap == null)
                return -1;

            if (fromTime.HasValue)
                return GetComboFromTime(PlayableBeatmap, fromTime.Value);

            return GetMaxCombo(PlayableBeatmap);
        }

        protected int GetMaxCombo(IBeatmap beatmap) => GetMaxCombo(beatmap.HitObjects);

        protected int GetComboFromTime(IBeatmap beatmap, int fromTime)
        {
            var hitObjects = new List<HitObject>();
            foreach (var hitObject in beatmap.HitObjects)
            {
                if (hitObject.StartTime > fromTime)
                    hitObjects.Add(hitObject);
                else if (hitObject is Slider slider)
                    hitObjects.AddRange(slider.NestedHitObjects.Where(h => h.StartTime > fromTime));
            }

            return GetMaxCombo(hitObjects);
        }

        private void PreparePlayableBeatmap(CancellationToken cancellationToken)
        {
            var newMods = _Mods != null ? string.Concat(_Mods) : "";
            if (LastMods != newMods || ResetPerformanceCalculator || _playableBeatmap == null)
            {
                var mods = GetOsuMods(Ruleset).Select(m => m.CreateInstance()).Append(Ruleset.AllMods.First(m => m.Acronym == "CL").CreateInstance()).ToArray();

                using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                cts.CancelAfter(60_000);
                PlayableBeatmap = WorkingBeatmap.GetPlayableBeatmap(Ruleset.RulesetInfo, mods, cts.Token);
                LastMods = newMods;
                ScoreInfo.Mods = mods;
                ResetPerformanceCalculator = true;
            }
        }

        private double CalculateScoreMultiplier()
        {
            var mods = GetOsuMods(Ruleset).ToArray();
            double scoreMultiplier = 1.0;
            IEnumerable<Mod> scoreIncreaseMods = Ruleset.GetModsFor(ModType.DifficultyIncrease);
            foreach (var m in mods.Where(m => !scoreIncreaseMods.Contains(m)))
                scoreMultiplier *= m.CreateInstance().ScoreMultiplier;

            IEnumerable<Mod> scoreDecreaseMods = Ruleset.GetModsFor(ModType.DifficultyReduction);
            foreach (var m in mods.Where(m => !scoreDecreaseMods.Contains(m)))
                scoreMultiplier *= m.CreateInstance().ScoreMultiplier;

            return scoreMultiplier;
        }

        protected int GetComboToTime(IBeatmap beatmap, int toTime) =>
            GetMaxCombo(beatmap.HitObjects.Where(h => h.StartTime < toTime).ToList());

        protected abstract int GetMaxCombo(IReadOnlyList<HitObject> hitObjects);

        protected abstract Dictionary<HitResult, int> GenerateHitResults(double accuracy, IReadOnlyList<HitObject> hitObjects, int countMiss, int? countMeh, int? countGood, int? countKatu = null, int? hit300 = null);

        protected abstract double GetAccuracy(Dictionary<HitResult, int> statistics);

        protected PpCalculator CreateInstance() => PpCalculatorHelpers.GetPpCalculator(RulesetId);
    }
}
