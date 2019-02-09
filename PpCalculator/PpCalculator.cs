﻿using osu.Game.Beatmaps;
using osu.Game.Rulesets;
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

        public virtual string[] Mods { get; set; }

        public virtual int Misses { get; set; }

        public virtual int? Mehs { get; set; }

        public virtual int? Goods { get; set; }

        public PpCalculator()
        { }

        public void PreProcess(string file)
        {
            WorkingBeatmap = new ProcessorWorkingBeatmap(file);
            PlayableBeatmap = WorkingBeatmap.GetPlayableBeatmap(Ruleset.RulesetInfo);
        }

        public double Calculate(double? time = null)
        {
            if (WorkingBeatmap == null)
                return -1d;

            var ruleset = Ruleset;

            var mods = getMods(ruleset).ToArray();

            WorkingBeatmap.Mods.Value = mods;

            PlayableBeatmap = WorkingBeatmap.GetPlayableBeatmap(ruleset.RulesetInfo);

            var beatmapMaxCombo = GetMaxCombo(PlayableBeatmap);
            var maxCombo = Combo ?? (int)Math.Round(PercentCombo / 100 * beatmapMaxCombo);
            var statistics = GenerateHitResults(Accuracy / 100, PlayableBeatmap, Misses, Mehs, Goods);
            var score = Score;
            var accuracy = GetAccuracy(statistics);

            var scoreInfo = new ScoreInfo
            {
                Accuracy = accuracy,
                MaxCombo = maxCombo,
                Statistics = statistics,
                Mods = mods,
                TotalScore = score
            };

            var categoryAttribs = new Dictionary<string, double>();
            double pp;

            if (time.HasValue)
                pp = ruleset.CreatePerformanceCalculator(WorkingBeatmap, scoreInfo).Calculate(time.Value, categoryAttribs);
            else
                pp = ruleset.CreatePerformanceCalculator(WorkingBeatmap, scoreInfo).Calculate(categoryAttribs);

            //Console.WriteLine(WorkingBeatmap.BeatmapInfo.ToString());

            //WritePlayInfo(scoreInfo, beatmap);

            Console.WriteLine("Mods" + (mods.Length > 0
                ? mods.Select(m => m.Acronym).Aggregate((c, n) => $"{c}, {n}")
                : "None"));

            //foreach (var kvp in categoryAttribs)
            //    WriteAttribute(kvp.Key, kvp.Value.ToString(CultureInfo.InvariantCulture));

            //WriteAttribute("pp", pp.ToString(CultureInfo.InvariantCulture));
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
