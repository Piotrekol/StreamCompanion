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
using OsuDifficultyAttributes = PpCalculatorTypes.OsuDifficultyAttributes;
using osu.Game.Rulesets.Mania.Objects;
using osu.Game.Rulesets.Taiko.Objects;
using osu.Game.Rulesets.Catch.Objects;
using osu.Game.Beatmaps.Formats;

namespace PpCalculator
{
    public abstract class PpCalculator : IPpCalculator, ICloneable
    {
        public ProcessorWorkingBeatmap WorkingBeatmap { get; protected set; }
        public IBeatmap PlayableBeatmap { get; set; }
        protected abstract Ruleset Ruleset { get; }

        public virtual double Accuracy { get; set; } = 100;

        public virtual int? Combo { get; set; }

        public virtual double PercentCombo { get; set; } = 100;

        public virtual int Score { get; set; }

        private string[] _Mods { get; set; }
        public virtual string[] Mods
        {
            get => _Mods;
            set
            {
                if (_Mods == value || (value != null && _Mods != null && _Mods.SequenceEqual(value)))
                    return;

                _Mods = value;
                scoreMultiplier = new Lazy<double>(CalculateScoreMultiplier);
            }
        }

        public virtual int Misses { get; set; }

        public virtual int? Mehs { get; set; }

        public virtual int? Goods { get; set; }
        public int? Katsus { get; set; }

        protected virtual ScoreInfo ScoreInfo { get; set; }

        protected PerformanceCalculator PerformanceCalculator { get; set; }
        protected List<TimedDifficultyAttributes> TimedDifficultyAttributes { get; set; }
        protected OsuAccuracyHeatmap OsuAccuracyHeatmap { get; set; }
        public int RulesetId => Ruleset.RulesetInfo.ID ?? 0;
        public double BeatmapLength => WorkingBeatmap?.Length ?? 0;

        public double ScoreMultiplier => scoreMultiplier.Value;
        private Lazy<double> scoreMultiplier = new Lazy<double>(() => 1d);

        public PpCalculator()
        {
            ScoreInfo = new ScoreInfo()
            {
                HitEvents = new List<HitEvent>()
            };
        }

        static PpCalculator()
        {
            //Required for <=v4 maps
            LegacyDifficultyCalculatorBeatmapDecoder.Register();
        }

        public object Clone()
        {
            var ppCalculator = CreateInstance();
            ppCalculator.WorkingBeatmap = WorkingBeatmap;
            ppCalculator.PlayableBeatmap = PlayableBeatmap;
            ppCalculator._Mods = _Mods;
            ppCalculator.LastMods = LastMods;
            ppCalculator.scoreMultiplier = scoreMultiplier;
            if (PerformanceCalculator != null)
            {
                ppCalculator.ScoreInfo.Mods = ScoreInfo.Mods.Select(m => m.DeepClone()).ToArray();
                ppCalculator.PerformanceCalculator = Ruleset.CreatePerformanceCalculator(TimedDifficultyAttributes.Last().Attributes, ppCalculator.ScoreInfo);
                ppCalculator.TimedDifficultyAttributes = TimedDifficultyAttributes;
                ppCalculator.OsuAccuracyHeatmap = OsuAccuracyHeatmap;
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

        protected string LastMods { get; set; } = null;
        protected bool ResetPerformanceCalculator { get; set; }

        public double Calculate(double startTime, double endTime = double.NaN, Dictionary<string, double> categoryAttribs = null)
        {
            if (double.IsNaN(startTime) || startTime <= 0d)
            {
                return Calculate(endTime, categoryAttribs);
            }

            var orginalWorkingBeatmap = WorkingBeatmap;
            var tempMap = new Beatmap();
            tempMap.HitObjects.AddRange(WorkingBeatmap.Beatmap.HitObjects.Where(h => h.StartTime >= startTime && h.StartTime <= endTime));
            if (tempMap.HitObjects.Count <= 1)
                return -1;
            tempMap.ControlPointInfo = WorkingBeatmap.Beatmap.ControlPointInfo;
            tempMap.BeatmapInfo = WorkingBeatmap.BeatmapInfo;

            WorkingBeatmap = new ProcessorWorkingBeatmap(tempMap);

            ResetPerformanceCalculator = true;
            var result = Calculate(null, categoryAttribs);

            WorkingBeatmap = orginalWorkingBeatmap;

            ResetPerformanceCalculator = true;

            return result;
        }

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

        public DifficultyAttributes AttributesAt(double time)
        {
            var attributes = TimedDifficultyAttributes?.LastOrDefault(x => x.Time <= time)?.Attributes;
            if (attributes == null)
                return null;

            switch (attributes)
            {
                case osu.Game.Rulesets.Osu.Difficulty.OsuDifficultyAttributes osuAttributes:
                    {
                        return new OsuDifficultyAttributes(osuAttributes.StarRating, osuAttributes.MaxCombo)
                        {
                            AimStrain = osuAttributes.AimStrain,
                            SpeedStrain = osuAttributes.SpeedStrain,
                            ApproachRate = osuAttributes.ApproachRate,
                            OverallDifficulty = osuAttributes.OverallDifficulty,
                            HitCircleCount = osuAttributes.HitCircleCount,
                            SliderCount = osuAttributes.SliderCount,
                            SpinnerCount = osuAttributes.SpinnerCount
                        };
                    }
                case osu.Game.Rulesets.Mania.Difficulty.ManiaDifficultyAttributes maniaAttributes:
                    return new ManiaDifficultyAttributes(maniaAttributes.StarRating, maniaAttributes.MaxCombo)
                    {
                        NoteCount = PlayableBeatmap.HitObjects.Count(h => h is Note),
                        HoldNoteCount = PlayableBeatmap.HitObjects.Count(h => h is HoldNote),
                        GreatHitWindow = maniaAttributes.GreatHitWindow
                    };
                case osu.Game.Rulesets.Taiko.Difficulty.TaikoDifficultyAttributes taikoAttributes:
                    return new TaikoDifficultyAttributes(taikoAttributes.StarRating, taikoAttributes.MaxCombo)
                    {
                        HitCount = PlayableBeatmap.HitObjects.Count(h => h is Hit),
                        DrumRollCount = PlayableBeatmap.HitObjects.Count(h => h is DrumRoll),
                        SwellCount = PlayableBeatmap.HitObjects.Count(h => h is Swell),
                        GreatHitWindow = taikoAttributes.GreatHitWindow
                    };
                case osu.Game.Rulesets.Catch.Difficulty.CatchDifficultyAttributes ctbAttributes:
                    return new CatchDifficultyAttributes(ctbAttributes.StarRating, ctbAttributes.MaxCombo)
                    {
                        FruitCount = PlayableBeatmap.HitObjects.Count(h => h is Fruit),
                        JuiceStreamCount = PlayableBeatmap.HitObjects.Count(h => h is JuiceStream),
                        BananaShowerCount = PlayableBeatmap.HitObjects.Count(h => h is BananaShower),
                    };
                default:
                    return new DifficultyAttributes(attributes.StarRating, attributes.MaxCombo);
            }
        }

        public double Calculate(double? endTime = null, Dictionary<string, double> categoryAttribs = null)
            => Calculate(CancellationToken.None, endTime, categoryAttribs);

        public double Calculate(CancellationToken cancellationToken, double? endTime = null,
            Dictionary<string, double> categoryAttribs = null)
        {
            try
            {
                return InternalCalculate(cancellationToken, endTime, categoryAttribs);
            }
            catch (TimeoutException)
            {
                return -2;
            }
        }

        private double InternalCalculate(CancellationToken cancellationToken, double? endTime = null, Dictionary<string, double> categoryAttribs = null)
        {
            if (WorkingBeatmap == null)
                return -1d;

            //huge performance gains by reusing existing performance calculator when possible
            var createPerformanceCalculator = PerformanceCalculator == null || ResetPerformanceCalculator;

            var ruleset = Ruleset;

            Mod[] mods = null;
            var newMods = _Mods != null ? string.Concat(_Mods) : "";
            if (LastMods != newMods || ResetPerformanceCalculator)
            {
                mods = GetOsuMods(ruleset).Select(m => m.CreateInstance()).ToArray();
                //TODO: cancellation token support
                PlayableBeatmap = WorkingBeatmap.GetPlayableBeatmap(ruleset.RulesetInfo, mods, TimeSpan.FromSeconds(20));


                LastMods = newMods;
                ScoreInfo.Mods = mods;

                createPerformanceCalculator = true;
            }

            IReadOnlyList<HitObject> hitObjects = endTime.HasValue
                ? PlayableBeatmap.HitObjects.Where(h => h.StartTime <= endTime).ToList()
                : PlayableBeatmap.HitObjects;

            if (!hitObjects.Any())
                return 0d;

            var maxCombo = Combo ?? (int)Math.Round(PercentCombo / 100 * GetMaxCombo(hitObjects));
            var statistics = GenerateHitResults(Accuracy / 100, hitObjects, Misses, Mehs, Goods, Katsus);
            var score = Score;
            var accuracy = GetAccuracy(statistics);

            ScoreInfo.Accuracy = accuracy;
            ScoreInfo.MaxCombo = maxCombo;
            ScoreInfo.Statistics = statistics;
            ScoreInfo.TotalScore = score;

            if (createPerformanceCalculator)
            {
                ClearHitEvents();
                (PerformanceCalculator, TimedDifficultyAttributes) = ruleset.CreatePerformanceCalculator(WorkingBeatmap, PlayableBeatmap, ScoreInfo, cancellationToken);
                ResetPerformanceCalculator = false;
            }

            try
            {
                if (endTime.HasValue)
                    return PerformanceCalculator.Calculate(endTime.Value,
                        TimedDifficultyAttributes.LastOrDefault(a => endTime.Value >= a.Time)?.Attributes ??
                        TimedDifficultyAttributes.First().Attributes, categoryAttribs);

                return PerformanceCalculator.Calculate(categoryAttribs);
            }
            catch (InvalidOperationException)
            {
                return -1;
            }
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

        private double CalculateScoreMultiplier()
        {
            var mods = GetOsuMods(Ruleset).ToArray();
            double scoreMultiplier = 1.0;
            IEnumerable<Mod> scoreIncreaseMods = Ruleset.GetModsFor(ModType.DifficultyIncrease);
            foreach (var m in mods.Where(m => !scoreIncreaseMods.Contains(m)))
                scoreMultiplier *= m.CreateInstance().ScoreMultiplier;

            return scoreMultiplier;
        }

        private List<HitObject> hitHitObjects = new List<HitObject>();
        public HitPoint[][] CalculateAccuracyHeatmap(int msToKeep = 0)
        {
            if (OsuAccuracyHeatmap == null)
                OsuAccuracyHeatmap = new OsuAccuracyHeatmap(ScoreInfo, PlayableBeatmap);

            OsuAccuracyHeatmap.CalculateSlow(msToKeep);
            return OsuAccuracyHeatmap.Points;
        }

        private OsuHitObject HitObjectAt(double time)
        {
            return (OsuHitObject)PlayableBeatmap.HitObjects.Except(hitHitObjects).LastOrDefault(x => x.StartTime <= time && (x.HitWindows.CanBeHit(time - x.StartTime)));
        }

        public HitObject PreviousHitObject(OsuHitObject hitObject)
        {
            var idx = ((List<OsuHitObject>)PlayableBeatmap.HitObjects).IndexOf(hitObject);
            if (idx <= 0)
                return null;

            return PlayableBeatmap.HitObjects[idx - 1];
        }

        public void ClearHitEvents()
        {
            ScoreInfo.HitEvents.Clear();
            hitHitObjects.Clear();
            OsuAccuracyHeatmap = null;
        }

        public void PushHitEvent(float x, float y, double time)
        {
            //TODO: lots of fake misses(wrongly mapped hit events). easily visible with HR. fairly sure that some of the "hits" are also mapped incorrectly.
            var hitObject = HitObjectAt(time);
            if (hitObject == null)
                return;
            
            //var hitResult = Math.Pow(x - hitObject.X, 2) + Math.Pow(y - hitObject.Y, 2) >= Math.Pow(hitObject.Radius, 2)
            //    ? HitResult.Miss
            //    : HitResult.None;

            hitHitObjects.Add(hitObject);
            var previousHitObject = PreviousHitObject(hitObject);
            ScoreInfo.HitEvents.Add(new HitEvent(time, HitResult.None, hitObject, previousHitObject, new osuTK.Vector2(x, y)));
        }

        protected int GetComboToTime(IBeatmap beatmap, int toTime) =>
            GetMaxCombo(beatmap.HitObjects.Where(h => h.StartTime < toTime).ToList());

        protected abstract int GetMaxCombo(IReadOnlyList<HitObject> hitObjects);

        protected abstract Dictionary<HitResult, int> GenerateHitResults(double accuracy, IReadOnlyList<HitObject> hitObjects, int countMiss, int? countMeh, int? countGood, int? countKatsu = null);

        protected abstract double GetAccuracy(Dictionary<HitResult, int> statistics);

        protected PpCalculator CreateInstance() => PpCalculatorHelpers.GetPpCalculator(RulesetId);
    }
}
