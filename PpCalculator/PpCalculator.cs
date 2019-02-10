using osu.Game.Beatmaps;
using osu.Game.Rulesets;
using osu.Game.Rulesets.Difficulty;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Scoring;
using osu.Game.Scoring;
using System;
using System.Collections.Generic;
using System.Globalization;
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

        public virtual string[] Mods { get; set; } = { "--" };

        public virtual int Misses { get; set; }

        public virtual int? Mehs { get; set; }

        public virtual int? Goods { get; set; }

        protected virtual ScoreInfo ScoreInfo { get; set; } = new ScoreInfo();

        protected virtual PerformanceCalculator PerformanceCalculator { get; set; }

        public void PreProcess(string file)
        {
            WorkingBeatmap = new ProcessorWorkingBeatmap(file);
            
            ResetPerformanceCalculator = true;
        }

        protected string LastMods { get; set; } = null;
        protected bool ResetPerformanceCalculator { get; set; }

        public double Calculate(double? time = null)
        {
            if (WorkingBeatmap == null)
                return -1d;

            //huge performance gains by reusing existing performance calculator when possible
            var createPerformanceCalculator = PerformanceCalculator == null || ResetPerformanceCalculator;

            var ruleset = Ruleset;

            Mod[] mods = null;
            var newMods = Mods != null ? string.Concat(Mods) : "";
            if (LastMods != newMods || ResetPerformanceCalculator)
            {
                mods = getMods(ruleset).ToArray();
                LastMods = newMods;

                WorkingBeatmap.Mods.Value = mods;

                PlayableBeatmap = WorkingBeatmap.GetPlayableBeatmap(ruleset.RulesetInfo);

                createPerformanceCalculator = true;
            }
            
            

            var beatmapMaxCombo = GetMaxCombo(PlayableBeatmap);
            var maxCombo = Combo ?? (int)Math.Round(PercentCombo / 100 * beatmapMaxCombo);
            var statistics = GenerateHitResults(Accuracy / 100, PlayableBeatmap, Misses, Mehs, Goods);
            var score = Score;
            var accuracy = GetAccuracy(statistics);

            ScoreInfo.Accuracy = accuracy;
            ScoreInfo.MaxCombo = maxCombo;
            ScoreInfo.Statistics = statistics;
            ScoreInfo.Mods = mods;
            ScoreInfo.TotalScore = score;


            var categoryAttribs = new Dictionary<string, double>();

            if (createPerformanceCalculator)
            {
                PerformanceCalculator = ruleset.CreatePerformanceCalculator(WorkingBeatmap, ScoreInfo);
                ResetPerformanceCalculator = false;
            }

            double pp;

            if (time.HasValue)
                pp = PerformanceCalculator.Calculate(time.Value, categoryAttribs);
            else
                pp = PerformanceCalculator.Calculate(categoryAttribs);

            return pp;
        }


        private List<Mod> getMods(Ruleset ruleset)
        {
            var mods = new List<Mod>();
            if (Mods == null)
                return mods;

            var availableMods = ruleset.GetAllMods().ToList();
            foreach (var modString in Mods)
            {
                Mod newMod = availableMods.FirstOrDefault(m => string.Equals(m.Acronym, modString, StringComparison.CurrentCultureIgnoreCase));
                if (newMod == null)
                    throw new ArgumentException($"Invalid mod provided: {modString}");
                mods.Add(newMod);
            }

            return mods;
        }


        protected abstract int GetMaxCombo(IBeatmap beatmap);

        public int GetMaxCombo() => PlayableBeatmap != null ? GetMaxCombo(PlayableBeatmap) : -1;

        protected abstract Dictionary<HitResult, int> GenerateHitResults(double accuracy, IBeatmap beatmap, int countMiss, int? countMeh, int? countGood);

        protected abstract double GetAccuracy(Dictionary<HitResult, int> statistics);

    }
}
