using System;
using System.IO;
using CollectionManager.DataTypes;
using CollectionManager.Enums;
using OppaiSharp;
using OsuMemoryDataProvider;
using Beatmap = StreamCompanionTypes.DataTypes.Beatmap;
using Mods = OppaiSharp.Mods;

namespace OsuMemoryEventSource
{
    public class RawMemoryDataProcessor
    {
        public PlayContainer Play { get; set; } = new PlayContainer();

        private Beatmap _currentBeatmap = null;
        private OppaiSharp.Beatmap _preprocessedBeatmap = null;
        private Mods _currentMods;

        private double _accIfRestFced = double.NaN;

        public RawMemoryDataProcessor()
        {
            OppaiSharpBeatmapHelper.GetCurrentBeatmap = () => _currentBeatmap;
        }
        public void SetCurrentMap(Beatmap beatmap, OppaiSharp.Mods mods, string osuFileLocation)
        {
            _currentBeatmap = beatmap;
            _currentMods = mods;
            if (beatmap == null)
                return;
            try
            {
                if (_currentBeatmap.PlayMode != PlayMode.Osu || !File.Exists(osuFileLocation))
                {
                    _preprocessedBeatmap = null;
                    return;
                }
                _preprocessedBeatmap = Helpers.GetOppaiSharpBeatmap(osuFileLocation);
            }
            catch (FileNotFoundException) { }
        }

        public double PPIfRestFCed()
        {
            if (_preprocessedBeatmap == null)
                return double.NaN;
            Accuracy accCalc;

            PPv2 ppCalculator;
            if (Play.Time <= 0)
            {
                accCalc = new Accuracy(100d, _preprocessedBeatmap.Objects.Count, 0);
                ppCalculator = new PPv2(new PPv2Parameters(_preprocessedBeatmap, accCalc.Count100,
                    accCalc.Count50, accCalc.CountMiss, -1, accCalc.Count300, _currentMods));
                return ppCalculator.Total;
            }

            _preprocessedBeatmap.ResetCut();

            //Calculate how much objects we can get starting from current Time
            _preprocessedBeatmap.Cut(Play.Time, 100000000);
            var c300Left = _preprocessedBeatmap.GetMaxComboSafe(true);
            var comboLeft = _preprocessedBeatmap.GetMaxComboSafe();

            var newMaxCombo = Math.Max(Play.MaxCombo, comboLeft + Play.Combo);
            var newC300Count = Play.C300 + c300Left;

            _preprocessedBeatmap.ResetCut();
            accCalc = new Accuracy(newC300Count, Play.C100, Play.C50, Play.CMiss);
            _accIfRestFced = accCalc.Value() * 100;

            ppCalculator = new PPv2(new PPv2Parameters(_preprocessedBeatmap, Play.C100,
                    Play.C50, Play.CMiss, newMaxCombo, newC300Count, _currentMods));
            return ppCalculator.Total;
        }

        public double PPIfBeatmapWouldEndNow()
        {
            if (_preprocessedBeatmap == null || Play.Time <= 0)
                return double.NaN;
            try
            {
                _preprocessedBeatmap.Cut(0, Play.Time);
                var ppCalculator =
                    new PPv2(new PPv2Parameters(_preprocessedBeatmap, Play.C100,
                        Play.C50, Play.CMiss, Play.MaxCombo, Play.C300, _currentMods));
                return ppCalculator.Total;
            }
            catch
            {
                return double.NaN;
            }
        }

        public double AccIfRestFCed() => _accIfRestFced;
    }
}