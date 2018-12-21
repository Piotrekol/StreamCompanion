using System;
using System.IO;
using CollectionManager.DataTypes;
using CollectionManager.Enums;
using OppaiSharp;
using OsuMemoryDataProvider;
using StreamCompanionTypes.DataTypes;
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
                var preprocessedBeatmap = Helpers.GetOppaiSharpBeatmap(osuFileLocation);

                if (preprocessedBeatmap != null && preprocessedBeatmap.Mode != GameMode.Standard)
                    _preprocessedBeatmap = null;
                else
                    _preprocessedBeatmap = preprocessedBeatmap;
            }
            catch (FileNotFoundException) { }
        }

        private bool PPIfRestFCedExceptionLogged = false;
        public double PPIfRestFCed()
        {
            if (_preprocessedBeatmap == null)
                return double.NaN;
            try
            {
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
                if (newC300Count < 0)
                    newC300Count = 0;

                accCalc = new Accuracy(newC300Count, Play.C100, Play.C50, Play.CMiss);
                _accIfRestFced = accCalc.Value() * 100;

                ppCalculator = new PPv2(new PPv2Parameters(_preprocessedBeatmap, Play.C100,
                    Play.C50, Play.CMiss, newMaxCombo, newC300Count, _currentMods));
                return ppCalculator.Total;
            }
            catch (ArgumentException e)
            {
                if (!PPIfRestFCedExceptionLogged)
                {
                    PPIfRestFCedExceptionLogged = true;
                    e.Data.Add("playData", PlayDataToString(Play));
                    e.Data.Add("mapID", _currentBeatmap?.MapId);
                    e.Data.Add("mapHash", _currentBeatmap?.Md5);
                    OsuMemoryEventSourceBase.Logger?.Log(e, LogLevel.Error);
                }
            }

            return Double.NaN;
        }
        private string PlayDataToString(PlayContainer p)
        {
            return $"{p.C300}/{p.C100}/{p.C50}/{p.CMiss}|"+
                   $"acc:{p.Acc},combo: {p.Combo},maxCombo {p.MaxCombo}|" +
                   $"time: {p.Time}";
        }
        public double AimPPIfBeatmapWouldEndNow { get; private set; } = double.NaN;
        public double SpeedPPIfBeatmapWouldEndNow { get; private set; } = double.NaN;
        public double AccPPIfBeatmapWouldEndNow { get; private set; } = double.NaN;

        public double PPIfBeatmapWouldEndNow()
        {
            if (_preprocessedBeatmap != null && Play.Time > 0)
                try
                {
                    _preprocessedBeatmap.Cut(0, Play.Time);
                    var ppCalculator =
                        new PPv2(new PPv2Parameters(_preprocessedBeatmap, Play.C100,
                            Play.C50, Play.CMiss, Play.MaxCombo, Play.C300, _currentMods));
                    AimPPIfBeatmapWouldEndNow = ppCalculator.Aim;
                    SpeedPPIfBeatmapWouldEndNow = ppCalculator.Speed;
                    AccPPIfBeatmapWouldEndNow = ppCalculator.Acc;
                    return ppCalculator.Total;

                }
                catch { }
            AimPPIfBeatmapWouldEndNow = double.NaN;
            SpeedPPIfBeatmapWouldEndNow = double.NaN;
            AccPPIfBeatmapWouldEndNow = double.NaN;
            return double.NaN;
        }

        public double AccIfRestFCed() => _accIfRestFced;
    }
}