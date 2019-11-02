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

namespace PpCalculator
{
    public abstract class PpCalculator
    {
        public WorkingBeatmap WorkingBeatmap { get; private set; }
        public IBeatmap PlayableBeatmap { get; private set; }
        public abstract Ruleset Ruleset { get; }

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

        protected virtual ScoreInfo ScoreInfo { get; set; } = new ScoreInfo();

        protected virtual PerformanceCalculator PerformanceCalculator { get; set; }

        public int? RulesetId => Ruleset.LegacyID;


        public void PreProcess(ProcessorWorkingBeatmap workingBeatmap)
        {
            WorkingBeatmap = workingBeatmap;

            ResetPerformanceCalculator = true;
        }
        public void PreProcess(string file) => PreProcess(new ProcessorWorkingBeatmap(file));

        protected string LastMods { get; set; } = null;
        protected bool ResetPerformanceCalculator { get; set; }

        public double Calculate(double startTime, double endTime = double.NaN, Dictionary<string, double> categoryAttribs = null)
        {

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

        public double Calculate(double? time = null, Dictionary<string, double> categoryAttribs = null)
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

            IReadOnlyList<HitObject> hitObjects = time.HasValue
                ? PlayableBeatmap.HitObjects.Where(h => h.StartTime <= time).ToList()
                : PlayableBeatmap.HitObjects;

            int beatmapMaxCombo = GetMaxCombo(hitObjects);

            var maxCombo = Combo ?? (int)Math.Round(PercentCombo / 100 * beatmapMaxCombo);
            var statistics = GenerateHitResults(Accuracy / 100, hitObjects, Misses, Mehs, Goods);

            var score = Score;
            var accuracy = GetAccuracy(statistics);


            ScoreInfo.Accuracy = accuracy;
            ScoreInfo.MaxCombo = maxCombo;
            ScoreInfo.Statistics = statistics;
            ScoreInfo.TotalScore = score;

            if (createPerformanceCalculator)
            {
                PerformanceCalculator = ruleset.CreatePerformanceCalculator(WorkingBeatmap, ScoreInfo);
                ResetPerformanceCalculator = false;
            }

            double pp;

            if (time.HasValue)
                pp = PerformanceCalculator.Calculate(time.Value, categoryAttribs);
            else
            {
                try
                {
                    pp = PerformanceCalculator.Calculate(categoryAttribs);
                }
                catch (InvalidOperationException)
                {
                    pp = -1;
                }
            }

            return pp;
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

        protected abstract Dictionary<HitResult, int> GenerateHitResults(double accuracy, IReadOnlyList<HitObject> hitObjects, int countMiss, int? countMeh, int? countGood);

        protected abstract double GetAccuracy(Dictionary<HitResult, int> statistics);

    }
}
