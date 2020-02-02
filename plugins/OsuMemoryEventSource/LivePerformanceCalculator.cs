using CollectionManager.Enums;
using OsuMemoryDataProvider;
using PpCalculator;
using System;
using System.Collections.Generic;
using StreamCompanionTypes.DataTypes;
using Beatmap = StreamCompanionTypes.DataTypes.Beatmap;

namespace OsuMemoryEventSource
{
    public class LivePerformanceCalculator
    {
        public PlayContainer Play { get; set; } = new PlayContainer();
        public int PlayTime { get; set; }
        public List<int> HitErrors { get; set; }

        private IBeatmap _currentBeatmap = null;
        private string _currentMods;
        private string _currentOsuFileLocation = null;
        private PlayMode _currentPlayMode;

        private PpCalculator.PpCalculator _ppCalculator = null;

        public void SetCurrentMap(IBeatmap beatmap, string mods, string osuFileLocation, PlayMode playMode)
        {
            if (beatmap == null)
            {
                _ppCalculator = null;
                return;
            }

            _currentBeatmap = beatmap;
            _currentMods = mods;
            _currentOsuFileLocation = osuFileLocation;
            _currentPlayMode = playMode;

            _ppCalculator = PpCalculatorHelpers.GetPpCalculator((int)playMode, _ppCalculator);

            if (_ppCalculator == null)
                return;

            _ppCalculator.Mods = mods.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            _ppCalculator.PreProcess(osuFileLocation);
        }


        public double PPIfRestFCed()
        {
            double pp = double.NaN;

            if (_ppCalculator == null || _currentPlayMode == PlayMode.OsuMania)
                return pp;

            try
            {
                if (PlayTime <= 0)
                {
                    _ppCalculator.Goods = 0;
                    _ppCalculator.Mehs = 0;
                    _ppCalculator.Combo = null;
                    _ppCalculator.PercentCombo = 100;
                    return _ppCalculator.Calculate(); //fc pp
                }

                var comboLeft = _ppCalculator.GetMaxCombo((int)PlayTime);

                var newMaxCombo = Math.Max(Play.MaxCombo, comboLeft + Play.Combo);

                _ppCalculator.Combo = newMaxCombo;

                pp = _ppCalculator.Calculate(null);

            }
            catch { }

            return pp;

        }
        private string PlayDataToString(PlayContainer p)
        {
            return $"{p.C300}/{p.C100}/{p.C50}/{p.CMiss}|" +
                   $"acc:{p.Acc},combo: {p.Combo},maxCombo {p.MaxCombo}|";
        }
        public double StrainPPIfBeatmapWouldEndNow { get; private set; } = double.NaN;
        public double AimPPIfBeatmapWouldEndNow { get; private set; } = double.NaN;
        public double SpeedPPIfBeatmapWouldEndNow { get; private set; } = double.NaN;
        public double AccPPIfBeatmapWouldEndNow { get; private set; } = double.NaN;

        Dictionary<string, double> attribs = new Dictionary<string, double>();

        public double PPIfBeatmapWouldEndNow()
        {

            if (_ppCalculator != null && PlayTime > 0)
                try
                {
                    _ppCalculator.Goods = Play.C100;
                    _ppCalculator.Mehs = Play.C50;
                    _ppCalculator.Misses = Play.CMiss;
                    _ppCalculator.Combo = Play.MaxCombo;
                    _ppCalculator.Score = Play.Score;
                    var pp = _ppCalculator.Calculate(PlayTime, attribs);
                    if (!double.IsInfinity(pp))
                    {
                        switch (_currentPlayMode)
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

                    attribs.Clear();
                }
                catch { }
            AimPPIfBeatmapWouldEndNow = double.NaN;
            SpeedPPIfBeatmapWouldEndNow = double.NaN;
            AccPPIfBeatmapWouldEndNow = double.NaN;
            StrainPPIfBeatmapWouldEndNow = double.NaN;
            return double.NaN;
        }

        public double NoChokePp()
        {
            double pp = double.NaN;

            if (_ppCalculator == null || _currentPlayMode == PlayMode.OsuMania)
                return pp;

            try
            {
                _ppCalculator.Goods = Play.C100;
                _ppCalculator.Mehs = Play.C50;
                _ppCalculator.Misses = 0;
                _ppCalculator.Combo = null;
                _ppCalculator.PercentCombo = 100;
                _ppCalculator.Score = Play.Score;
                pp = _ppCalculator.Calculate(PlayTime, null);

                if (double.IsInfinity(pp))
                {
                    pp = Double.NaN;
                }
            }
            catch { }

            return pp;
        }

    }
}