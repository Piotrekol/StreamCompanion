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
using PpCalculatorTypes;
using DifficultyAttributes = PpCalculatorTypes.DifficultyAttributes;
using OsuDifficultyAttributes = PpCalculatorTypes.OsuDifficultyAttributes;

namespace PpCalculator
{
    public abstract class PpCalculator : IPpCalculator, ICloneable
    {
        public ProcessorWorkingBeatmap WorkingBeatmap { get; protected set; }
        protected IBeatmap PlayableBeatmap { get; set; }
        protected abstract Ruleset Ruleset { get; }

        public virtual double Accuracy { get; set; } = 100;

        public virtual int? Combo { get; set; }

        public virtual double PercentCombo { get; set; } = 100;

        public virtual int Score { get; set; }

        private string[] _Mods { get; set; }

        public virtual string[] Mods
        {
            get => _Mods;
            set => _Mods = value;
        }

        public virtual int Misses { get; set; }

        public virtual int? Mehs { get; set; }

        public virtual int? Goods { get; set; }
        public int? Katsus { get; set; }

        protected virtual ScoreInfo ScoreInfo { get; set; } = new ScoreInfo();

        protected PerformanceCalculator PerformanceCalculator { get; set; }
        protected List<TimedDifficultyAttributes> TimedDifficultyAttributes { get; set; }

        public int RulesetId => Ruleset.RulesetInfo.ID ?? 0;
        public double BeatmapLength => WorkingBeatmap?.Length ?? 0;

        public object Clone()
        {
            var ppCalculator = CreateInstance();
            ppCalculator.WorkingBeatmap = WorkingBeatmap;
            ppCalculator.PlayableBeatmap = PlayableBeatmap;
            ppCalculator.Mods = Mods;
            ppCalculator.LastMods = LastMods;
            if (PerformanceCalculator != null)
            {
                ppCalculator.PerformanceCalculator = Ruleset.CreatePerformanceCalculator(TimedDifficultyAttributes.Last().Attributes, ppCalculator.ScoreInfo);
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

        public DifficultyAttributes AttributesAt(double time)
        {
            var attributes = TimedDifficultyAttributes?.LastOrDefault(x => x.Time <= time)?.Attributes;
            if (attributes == null)
                return null;

            DifficultyAttributes difficultyAttributes = null;
            //Implement other modes when need arises
            if (attributes is osu.Game.Rulesets.Osu.Difficulty.OsuDifficultyAttributes osuDifficultyAttributes)
            {
                return new OsuDifficultyAttributes(osuDifficultyAttributes.StarRating, osuDifficultyAttributes.MaxCombo)
                {
                    AimStrain = osuDifficultyAttributes.AimStrain,
                    SpeedStrain = osuDifficultyAttributes.SpeedStrain,
                    ApproachRate = osuDifficultyAttributes.ApproachRate,
                    OverallDifficulty = osuDifficultyAttributes.OverallDifficulty,
                    HitCircleCount = osuDifficultyAttributes.HitCircleCount,
                    SliderCount = osuDifficultyAttributes.SliderCount,
                    SpinnerCount = osuDifficultyAttributes.SpinnerCount
                };
            }

            return new DifficultyAttributes(attributes.StarRating, attributes.MaxCombo);
        }

        public double Calculate(double? endTime = null, Dictionary<string, double> categoryAttribs = null)
            => Calculate(CancellationToken.None, endTime, categoryAttribs);
        public double Calculate(CancellationToken cancellationToken, double? endTime = null, Dictionary<string, double> categoryAttribs = null)
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
                mods = getMods(ruleset).ToArray();
                LastMods = newMods;

                PlayableBeatmap = WorkingBeatmap.GetPlayableBeatmap(ruleset.RulesetInfo, mods);

                ScoreInfo.Mods = mods;

                createPerformanceCalculator = true;
            }

            IReadOnlyList<HitObject> hitObjects = endTime.HasValue
                ? PlayableBeatmap.HitObjects.Where(h => h.StartTime <= endTime).ToList()
                : PlayableBeatmap.HitObjects;

            if (!hitObjects.Any())
                return 0d;

            int beatmapMaxCombo = GetMaxCombo(hitObjects);

            var maxCombo = Combo ?? (int)Math.Round(PercentCombo / 100 * beatmapMaxCombo);
            var statistics = GenerateHitResults(Accuracy / 100, hitObjects, Misses, Mehs, Goods, Katsus);

            var score = Score;
            var accuracy = GetAccuracy(statistics);


            ScoreInfo.Accuracy = accuracy;
            ScoreInfo.MaxCombo = maxCombo;
            ScoreInfo.Statistics = statistics;
            ScoreInfo.TotalScore = score;

            if (createPerformanceCalculator)
            {
                (PerformanceCalculator, TimedDifficultyAttributes) = ruleset.CreatePerformanceCalculator(WorkingBeatmap, ScoreInfo, cancellationToken);
                ResetPerformanceCalculator = false;
            }

            try
            {

                return endTime.HasValue
                    ? PerformanceCalculator.Calculate(endTime.Value,
                        TimedDifficultyAttributes.LastOrDefault(a => endTime.Value >= a.Time)?.Attributes ?? TimedDifficultyAttributes.First().Attributes,
                        categoryAttribs)
                    : PerformanceCalculator.Calculate(categoryAttribs);
            }
            catch (InvalidOperationException)
            {
                return -1;
            }
        }


        private List<Mod> getMods(Ruleset ruleset)
        {
            var mods = new List<Mod>();
            if (_Mods == null)
                return mods;

            var availableMods = ruleset.GetAllMods().ToList();
            foreach (var modString in _Mods)
            {
                Mod newMod = availableMods.FirstOrDefault(m => string.Equals(m.Acronym, modString, StringComparison.CurrentCultureIgnoreCase));
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

        protected int GetComboFromTime(IBeatmap beatmap, int fromTime) =>
            GetMaxCombo(beatmap.HitObjects.Where(h => h.StartTime > fromTime).ToList());
        protected int GetComboToTime(IBeatmap beatmap, int toTime) =>
            GetMaxCombo(beatmap.HitObjects.Where(h => h.StartTime < toTime).ToList());

        protected abstract int GetMaxCombo(IReadOnlyList<HitObject> hitObjects);

        protected abstract Dictionary<HitResult, int> GenerateHitResults(double accuracy, IReadOnlyList<HitObject> hitObjects, int countMiss, int? countMeh, int? countGood, int? countKatsu = null);

        protected abstract double GetAccuracy(Dictionary<HitResult, int> statistics);

        protected PpCalculator CreateInstance() => PpCalculatorHelpers.GetPpCalculator(RulesetId);
    }
}
