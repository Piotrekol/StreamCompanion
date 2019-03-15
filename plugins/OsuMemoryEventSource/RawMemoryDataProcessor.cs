﻿using CollectionManager.DataTypes;
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
        private string _currentMods;
        private string _currentOsuFileLocation = null;
        

        private PpCalculator.PpCalculator _ppCalculator = null;

        private double _accIfRestFced = double.NaN;


        public void SetCurrentMap(Beatmap beatmap, string mods, string osuFileLocation, PlayMode playMode)
        {
            if (beatmap == null)
                return;

            _currentBeatmap = beatmap;
            _currentMods = mods;
            _currentOsuFileLocation = osuFileLocation;

            _ppCalculator = PpCalculatorHelpers.GetPpCalculator((int)playMode, _ppCalculator);

            if (_ppCalculator == null)
                return;

            _ppCalculator.Mods = mods.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            _ppCalculator.PreProcess(osuFileLocation);
        }
        

        public double PPIfRestFCed()
        {
            if (_ppCalculator == null || _currentBeatmap.PlayMode == PlayMode.OsuMania)
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

            var pp = _ppCalculator.Calculate(null);


            return pp;

        }
        private string PlayDataToString(PlayContainer p)
        {
            return $"{p.C300}/{p.C100}/{p.C50}/{p.CMiss}|" +
                   $"acc:{p.Acc},combo: {p.Combo},maxCombo {p.MaxCombo}|" +
                   $"time: {p.Time}";
        }
        public double StrainPPIfBeatmapWouldEndNow { get; private set; } = double.NaN;
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
                    _ppCalculator.Score = Play.Score;
                    var pp = _ppCalculator.Calculate(Play.Time, attribs);

                    switch (_currentBeatmap.PlayMode)
                    {
                        case PlayMode.Taiko:
                        case PlayMode.OsuMania:
                            StrainPPIfBeatmapWouldEndNow = attribs["Strain"];
                            AccPPIfBeatmapWouldEndNow = attribs["Accuracy"];
                            AimPPIfBeatmapWouldEndNow = double.NaN;
                            SpeedPPIfBeatmapWouldEndNow = double.NaN;
                            break;
                        default:
                            AimPPIfBeatmapWouldEndNow = attribs["Aim"];
                            SpeedPPIfBeatmapWouldEndNow = attribs["Speed"];
                            AccPPIfBeatmapWouldEndNow = attribs["Accuracy"];
                            break;
                    }

                    attribs.Clear();

                    return pp;
                }
                catch { }
            AimPPIfBeatmapWouldEndNow = double.NaN;
            SpeedPPIfBeatmapWouldEndNow = double.NaN;
            AccPPIfBeatmapWouldEndNow = double.NaN;
            StrainPPIfBeatmapWouldEndNow = double.NaN;
            return double.NaN;
        }

    }
}