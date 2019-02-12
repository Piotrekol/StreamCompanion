using CollectionManager.DataTypes;
using CollectionManager.Enums;
using OsuMemoryDataProvider;
using PpCalculator;
using StreamCompanionTypes.DataTypes;
using System;
using System.Collections.Generic;
using System.IO;
using Beatmap = StreamCompanionTypes.DataTypes.Beatmap;
using Mods = CollectionManager.DataTypes.Mods;

namespace OsuMemoryEventSource
{
    public class RawMemoryDataProcessor
    {
        public PlayContainer Play { get; set; } = new PlayContainer();

        private Beatmap _currentBeatmap = null;
        private Mods _currentMods;
        private PpCalculator.PpCalculator _ppCalculator = null;

        private double _accIfRestFced = double.NaN;


        public void SetCurrentMap(Beatmap beatmap, Mods mods, string osuFileLocation)
        {
            _currentBeatmap = beatmap;
            _currentMods = mods;
            if (beatmap == null)
                return;

            //TODO: remove this after implementing gamemode-specific calculators
            if (_currentBeatmap.PlayMode != PlayMode.Osu || !File.Exists(osuFileLocation))
                return;


            if (_ppCalculator == null || _ppCalculator.RulesetId != (int)beatmap.PlayMode)
            {
                //TODO: change pp calculator depending on played gamemode
                _ppCalculator = new OsuCalculator();
            }

            _ppCalculator.Mods = mods == Mods.Omod ? null : mods.ToString().Split(new[] { ", " }, StringSplitOptions.None);

            _ppCalculator.PreProcess(osuFileLocation);

        }

        public double PPIfRestFCed()
        {
            if (_ppCalculator == null)
                return double.NaN;

            if (Play.Time <= 0)
            {
                _ppCalculator.Goods = 0;
                _ppCalculator.Mehs = 0;
                _ppCalculator.Combo = null;
                _ppCalculator.PercentCombo = 100;
                _accIfRestFced = 100;
                return _ppCalculator.Calculate();//fc pp

            }

            var comboLeft = _ppCalculator.GetMaxCombo(Play.Time);

            var newMaxCombo = Math.Max(Play.MaxCombo, comboLeft + Play.Combo);

            _ppCalculator.Combo = newMaxCombo;

            var pp = _ppCalculator.Calculate(null, attribs);

            _accIfRestFced = attribs["PlayAccuracy"];
            attribs.Clear();

            return pp;

        }
        private string PlayDataToString(PlayContainer p)
        {
            return $"{p.C300}/{p.C100}/{p.C50}/{p.CMiss}|" +
                   $"acc:{p.Acc},combo: {p.Combo},maxCombo {p.MaxCombo}|" +
                   $"time: {p.Time}";
        }
        public double AimPPIfBeatmapWouldEndNow { get; private set; } = double.NaN;
        public double SpeedPPIfBeatmapWouldEndNow { get; private set; } = double.NaN;
        public double AccPPIfBeatmapWouldEndNow { get; private set; } = double.NaN;

        Dictionary<string, double> attribs = new Dictionary<string, double>();
        public double PPIfBeatmapWouldEndNow()
        {

            if (_ppCalculator != null && Play.Time > 0)
                try
                {
                    _ppCalculator.Goods = Play.C100;
                    _ppCalculator.Mehs = Play.C50;
                    _ppCalculator.Misses = Play.CMiss;
                    _ppCalculator.Combo = Play.MaxCombo;

                    var pp = _ppCalculator.Calculate(Play.Time, attribs);

                    AimPPIfBeatmapWouldEndNow = attribs["Aim"];
                    SpeedPPIfBeatmapWouldEndNow = attribs["Speed"];
                    AccPPIfBeatmapWouldEndNow = attribs["Accuracy"];

                    attribs.Clear();

                    return pp;
                }
                catch { }
            AimPPIfBeatmapWouldEndNow = double.NaN;
            SpeedPPIfBeatmapWouldEndNow = double.NaN;
            AccPPIfBeatmapWouldEndNow = double.NaN;
            return double.NaN;
        }

    }
}