using CollectionManager.Enums;
using OsuMemoryDataProvider;
using PpCalculator;
using StreamCompanionTypes;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CollectionManager.DataTypes;
using StreamCompanionTypes.Interfaces.Services;

namespace OsuMemoryEventSource
{
    public class MemoryDataProcessor : IDisposable
    {
        private readonly object _lockingObject = new object();
        private OsuStatus _lastStatus = OsuStatus.Null;
        private LivePerformanceCalculator _rawData = new LivePerformanceCalculator();
        private ISettings _settings;
        private readonly IContextAwareLogger _logger;
        private readonly Dictionary<string, LiveToken> _liveTokens = new Dictionary<string, LiveToken>();
        private Tokens.TokenSetter _liveTokenSetter => OsuMemoryEventSourceBase.LiveTokenSetter;
        private Tokens.TokenSetter _tokenSetter => OsuMemoryEventSourceBase.TokenSetter;
        public Mods Mods { get; set; }
        private IToken _strainsToken;
        private IToken _skinToken;
        private IToken _skinPathToken;

        private ushort _lastMisses = 0;
        private ushort _lastCombo = 0;
        private ushort _sliderBreaks = 0;
        private enum InterpolatedValueName
        {
            PpIfMapEndsNow,
            AimPpIfMapEndsNow,
            SpeedPpIfMapEndsNow,
            AccPpIfMapEndsNow,
            StrainPpIfMapEndsNow,
            PpIfRestFced,
            NoChokePp,
            SimulatedPp,
            UnstableRate,
        }

        private readonly Dictionary<InterpolatedValueName, InterpolatedValue> InterpolatedValues = new Dictionary<InterpolatedValueName, InterpolatedValue>();
        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        public EventHandler<OsuStatus> TokensUpdated { get; set; }
        public MemoryDataProcessor(ISettings settings, IContextAwareLogger logger, bool enablePpSmoothing = true)
        {
            _settings = settings;
            _logger = logger;
            foreach (var v in (InterpolatedValueName[])Enum.GetValues(typeof(InterpolatedValueName)))
            {
                InterpolatedValues.Add(v, new InterpolatedValue(0.15));
            }

            ToggleSmoothing(enablePpSmoothing);

            _strainsToken = _tokenSetter("mapStrains", new Dictionary<int, double>(), TokenType.Normal, ",", new Dictionary<int, double>());

            _skinToken = _tokenSetter("skin", string.Empty, TokenType.Normal, null, string.Empty);
            _skinPathToken = _tokenSetter("skinPath", string.Empty, TokenType.Normal, null, string.Empty);

            InitLiveTokens();

            Task.Run(InterpolatedValueThreadWork, cancellationTokenSource.Token);
            Task.Run(TokenThreadWork, cancellationTokenSource.Token);
        }

        public Task TokenThreadWork()
        {
            try
            {
                while (true)
                {
                    if (_tokenTick == null || _tokenCallbackTick == null)
                        return Task.CompletedTask;

                    if (_tokenTick.WaitOne(1))
                    {
                        _tokenCallbackTick.Reset();
                        UpdateLiveTokens(_lastStatus);

                        _tokenTick.Reset();
                    }

                    _tokenCallbackTick.Set();
                }
            }
            catch (TaskCanceledException)
            {

            }

            return Task.CompletedTask;
        }
        public async Task InterpolatedValueThreadWork()
        {
            try
            {
                while (true)
                {
                    foreach (var interpolatedValue in InterpolatedValues)
                    {
                        interpolatedValue.Value.Tick();
                    }

                    await Task.Delay(11);
                }
            }
            catch (TaskCanceledException)
            {
            }
        }

        public void SetNewMap(MapSearchResult map)
        {
            lock (_lockingObject)
            {
                if ((map.Action & OsuStatus.ResultsScreen) != 0)
                    return;

                if (map.FoundBeatmaps &&
                    map.BeatmapsFound[0].IsValidBeatmap(_settings, out var mapLocation))
                {
                    _sliderBreaks = 0;
                    _lastMisses = 0;
                    _lastCombo = 0;
                    var workingBeatmap = new ProcessorWorkingBeatmap(mapLocation);
                    var mods = map.Mods?.WorkingMods ?? "";

                    _rawData.SetCurrentMap(map.BeatmapsFound[0], mods, mapLocation,
                        (PlayMode)PpCalculatorHelpers.GetRulesetId(workingBeatmap.RulesetID, map.PlayMode.HasValue ? (int?)map.PlayMode : null));

                }
            }
        }

        private IOsuMemoryReader _reader;
        private AutoResetEvent _tokenTick = new AutoResetEvent(false);
        private AutoResetEvent _tokenCallbackTick = new AutoResetEvent(true);
        public void Tick(OsuStatus status, IOsuMemoryReader reader)
        {
            lock (_lockingObject)
            {
                _tokenCallbackTick.WaitOne();
                if (!ReferenceEquals(_reader, reader))
                    _reader = reader;
                if ((OsuStatus)_liveTokens["status"].Token.Value != status)
                    _liveTokens["status"].Token.Value = status;

                if (status != OsuStatus.Playing)
                {
                    _rawData.PlayTime = reader.ReadPlayTime();
                    UpdateLiveTokens(status);
                    _lastStatus = status;
                    return;
                }

                if (_lastStatus != OsuStatus.Playing)
                {
                    Thread.Sleep(500);//Initial play delay
                }

                reader.GetPlayData(_rawData.Play);
                //TODO: change this on OsuMemoryReader side (read v2 in getPlayData & return 0 as default instead of -1)
                _rawData.Play.Score = Math.Max(0, _reader.ReadScoreV2());

                _rawData.HitErrors = reader.HitErrors() ?? new List<int>();

                _rawData.PlayTime = reader.ReadPlayTime();
                _liveTokens["time"].Update();

                _tokenTick?.Set();
                _lastStatus = status;
            }
        }

        private void ResetTokens()
        {
            foreach (var tokenkv in _liveTokens)
            {
                tokenkv.Value.Token.Reset();
            }

            _tokensUpdated();
        }

        public void _tokensUpdated()
        {
            TokensUpdated?.Invoke(this, (OsuStatus)_liveTokens["status"].Token.Value);
        }

        public Tokens Tokens
        {
            get
            {
                var ret = new Tokens("Live tokens");
                foreach (var r in _liveTokens)
                {
                    ret.Add(r.Key, r.Value.Token);
                }

                return ret;
            }
        }

        private void InitLiveTokens()
        {
            _liveTokens["status"] = new LiveToken(_tokenSetter("status", OsuStatus.Null, TokenType.Normal, "", OsuStatus.Null), null);

            _liveTokens["acc"] = new LiveToken(_liveTokenSetter("acc", _rawData.Play.Acc, TokenType.Live, "{0:0.00}", 0d, OsuStatus.Playing), () => _rawData.Play.Acc);
            _liveTokens["katsu"] = new LiveToken(_liveTokenSetter("katsu", _rawData.Play.CKatsu, TokenType.Live, "{0}", (ushort)0, OsuStatus.Playing), () => _rawData.Play.CKatsu);
            _liveTokens["geki"] = new LiveToken(_liveTokenSetter("geki", _rawData.Play.CGeki, TokenType.Live, "{0}", (ushort)0, OsuStatus.Playing), () => _rawData.Play.CGeki);
            _liveTokens["c300"] = new LiveToken(_liveTokenSetter("c300", _rawData.Play.C300, TokenType.Live, "{0}", (ushort)0, OsuStatus.Playing), () => _rawData.Play.C300);
            _liveTokens["c100"] = new LiveToken(_liveTokenSetter("c100", _rawData.Play.C100, TokenType.Live, "{0}", (ushort)0, OsuStatus.Playing), () => _rawData.Play.C100);
            _liveTokens["c50"] = new LiveToken(_liveTokenSetter("c50", _rawData.Play.C50, TokenType.Live, "{0}", (ushort)0, OsuStatus.Playing), () => _rawData.Play.C50);
            _liveTokens["miss"] = new LiveToken(_liveTokenSetter("miss", _rawData.Play.CMiss, TokenType.Live, "{0}", (ushort)0, OsuStatus.Playing), () => _rawData.Play.CMiss);
            _liveTokens["mapPosition"] = new LiveToken(_liveTokenSetter("mapPosition", TimeSpan.Zero, TokenType.Live, "{0:mm\\:ss}", TimeSpan.Zero), () =>
            {
                if (_rawData.PlayTime != 0)
                    return TimeSpan.FromMilliseconds(_rawData.PlayTime);
                return TimeSpan.Zero;
            });
            _liveTokens["time"] = new LiveToken(_liveTokenSetter("time", 0d, TokenType.Live, "{0:0.00}", 0d), () =>
            {
                if (_rawData.PlayTime != 0)
                    return _rawData.PlayTime / 1000d;
                return 0d;
            });
            _liveTokens["timeLeft"] = new LiveToken(_liveTokenSetter("timeLeft", TimeSpan.Zero, TokenType.Live, "{0:mm\\:ss}", TimeSpan.Zero), () =>
            {
                var beatmapLength = _rawData.PpCalculator?.WorkingBeatmap?.Length;
                if (beatmapLength.HasValue)
                {
                    var timeLeft = TimeSpan.FromMilliseconds(beatmapLength.Value) - (TimeSpan)_liveTokens["mapPosition"].Token.Value;
                    return timeLeft.Ticks <= 0
                        ? TimeSpan.Zero
                        : timeLeft;
                }

                return TimeSpan.Zero;
            });
            _liveTokens["combo"] = new LiveToken(_liveTokenSetter("combo", _rawData.Play.Combo, TokenType.Live, "{0}", (ushort)0, OsuStatus.Playing), () => _rawData.Play.Combo);
            _liveTokens["score"] = new LiveToken(_liveTokenSetter("score", _rawData.Play.Score, TokenType.Live, "{0}", 0, OsuStatus.Playing), () => _rawData.Play.Score);
            _liveTokens["currentMaxCombo"] = new LiveToken(_liveTokenSetter("currentMaxCombo", _rawData.Play.MaxCombo, TokenType.Live, "{0}", (ushort)0, OsuStatus.Playing), () => _rawData.Play.MaxCombo);
            _liveTokens["playerHp"] = new LiveToken(_liveTokenSetter("playerHp", _rawData.Play.Hp, TokenType.Live, "{0:0.00}", 0d, OsuStatus.Playing), () => _rawData.Play.Hp);

            _liveTokens["ppIfMapEndsNow"] = new LiveToken(_liveTokenSetter("ppIfMapEndsNow", InterpolatedValues[InterpolatedValueName.PpIfMapEndsNow].Current, TokenType.Live, "{0:0.00}", 0d, OsuStatus.Playing), () =>
            {
                InterpolatedValues[InterpolatedValueName.PpIfMapEndsNow].Set(_rawData.PPIfBeatmapWouldEndNow());
                return InterpolatedValues[InterpolatedValueName.PpIfMapEndsNow].Current;
            });
            _liveTokens["aimPpIfMapEndsNow"] = new LiveToken(_liveTokenSetter("aimPpIfMapEndsNow", InterpolatedValues[InterpolatedValueName.AimPpIfMapEndsNow].Current, TokenType.Live, "{0:0.00}", 0d, OsuStatus.Playing), () =>
            {
                InterpolatedValues[InterpolatedValueName.AimPpIfMapEndsNow].Set(_rawData.AimPPIfBeatmapWouldEndNow);
                return InterpolatedValues[InterpolatedValueName.AimPpIfMapEndsNow].Current;
            });
            _liveTokens["speedPpIfMapEndsNow"] = new LiveToken(_liveTokenSetter("speedPpIfMapEndsNow", InterpolatedValues[InterpolatedValueName.SpeedPpIfMapEndsNow].Current, TokenType.Live, "{0:0.00}", 0d, OsuStatus.Playing), () =>
            {
                InterpolatedValues[InterpolatedValueName.SpeedPpIfMapEndsNow].Set(_rawData.SpeedPPIfBeatmapWouldEndNow);
                return InterpolatedValues[InterpolatedValueName.SpeedPpIfMapEndsNow].Current;
            });
            _liveTokens["accPpIfMapEndsNow"] = new LiveToken(_liveTokenSetter("accPpIfMapEndsNow", InterpolatedValues[InterpolatedValueName.AccPpIfMapEndsNow].Current, TokenType.Live, "{0:0.00}", 0d, OsuStatus.Playing), () =>
            {
                InterpolatedValues[InterpolatedValueName.AccPpIfMapEndsNow].Set(_rawData.AccPPIfBeatmapWouldEndNow);
                return InterpolatedValues[InterpolatedValueName.AccPpIfMapEndsNow].Current;
            });
            _liveTokens["strainPpIfMapEndsNow"] = new LiveToken(_liveTokenSetter("strainPpIfMapEndsNow", InterpolatedValues[InterpolatedValueName.StrainPpIfMapEndsNow].Current, TokenType.Live, "{0:0.00}", 0d, OsuStatus.Playing), () =>
            {
                InterpolatedValues[InterpolatedValueName.StrainPpIfMapEndsNow].Set(_rawData.StrainPPIfBeatmapWouldEndNow);
                return InterpolatedValues[InterpolatedValueName.StrainPpIfMapEndsNow].Current;
            });
            _liveTokens["ppIfRestFced"] = new LiveToken(_liveTokenSetter("ppIfRestFced", InterpolatedValues[InterpolatedValueName.PpIfRestFced].Current, TokenType.Live, "{0:0.00}", 0d, OsuStatus.Playing), () =>
            {
                InterpolatedValues[InterpolatedValueName.PpIfRestFced].Set(_rawData.PPIfRestFCed());
                return InterpolatedValues[InterpolatedValueName.PpIfRestFced].Current;
            });
            _liveTokens["noChokePp"] = new LiveToken(_liveTokenSetter("noChokePp", InterpolatedValues[InterpolatedValueName.NoChokePp].Current, TokenType.Live, "{0:0.00}", 0d, OsuStatus.Playing), () =>
            {
                InterpolatedValues[InterpolatedValueName.NoChokePp].Set(_rawData.NoChokePp());
                return InterpolatedValues[InterpolatedValueName.NoChokePp].Current;
            });
            _liveTokens["simulatedPp"] = new LiveToken(_liveTokenSetter("simulatedPp", InterpolatedValues[InterpolatedValueName.SimulatedPp].Current, TokenType.Live, "{0:0.00}", 0d, OsuStatus.All), () =>
            {
                InterpolatedValues[InterpolatedValueName.SimulatedPp].Set(_rawData.SimulatedPp());
                return InterpolatedValues[InterpolatedValueName.SimulatedPp].Current;
            });
            _liveTokens["unstableRate"] = new LiveToken(_liveTokenSetter("unstableRate", InterpolatedValues[InterpolatedValueName.UnstableRate].Current, TokenType.Live, "{0:0.000}", 0d, OsuStatus.Playing), () =>
            {
                InterpolatedValues[InterpolatedValueName.UnstableRate].Set(UnstableRate(_rawData.HitErrors));
                return InterpolatedValues[InterpolatedValueName.UnstableRate].Current;
            });
            _liveTokens["convertedUnstableRate"] = new LiveToken(_liveTokenSetter("convertedUnstableRate", InterpolatedValues[InterpolatedValueName.UnstableRate].Current, TokenType.Live, "{0:0.000}", 0d, OsuStatus.Playing), () =>
            {
                var ur = (double)_liveTokens["unstableRate"].Token.Value;
                if ((Mods & Mods.Dt) != 0)
                    return ur / 1.5d;
                if ((Mods & Mods.Ht) != 0)
                    return ur / 0.75d;
                return ur;
            });
            _liveTokens["hitErrors"] = new LiveToken(_liveTokenSetter("hitErrors", new List<int>(), TokenType.Live, ",", new List<int>(), OsuStatus.Playing), () => _rawData.HitErrors);
            _liveTokens["localTimeISO"] = new LiveToken(_liveTokenSetter("localTimeISO", DateTime.UtcNow.ToString("o"), TokenType.Live, "", DateTime.UtcNow, OsuStatus.All), () => DateTime.UtcNow.ToString("o"));
            _liveTokens["localTime"] = new LiveToken(_liveTokenSetter("localTime", DateTime.Now.TimeOfDay, TokenType.Live, "{0:hh}:{0:mm}:{0:ss}", DateTime.Now.TimeOfDay, OsuStatus.All), () => DateTime.Now.TimeOfDay);
            _liveTokens["sliderBreaks"] = new LiveToken(_liveTokenSetter("sliderBreaks", 0, TokenType.Live, "{0}", 0, OsuStatus.Playing), () =>
            {
                var currentMisses = (ushort)_liveTokens["miss"].Token.Value;
                var currentCombo = (ushort)_liveTokens["combo"].Token.Value;
                if (_lastMisses == currentMisses && _lastCombo > currentCombo)
                    _sliderBreaks++;

                _lastMisses = currentMisses;
                _lastCombo = currentCombo;

                return _sliderBreaks;
            });
        }

        private void UpdateLiveTokens(OsuStatus status)
        {
            foreach (var liveToken in _liveTokens)
            {
                try
                {
                    if (liveToken.Value.Token.CanSave(status))
                    {
                        liveToken.Value.Update(status);
                    }
                }
                catch (Exception ex)
                {
                    ex.Data["liveTokenName"] = liveToken.Key;
                    throw;
                }
            }
            _tokensUpdated();
        }

        private double UnstableRate(List<int> hitErrors)
        {
            if (hitErrors.Count == 0 || hitErrors.Any(x => x > 10000))
                return 0;

            double sum = hitErrors.Sum();

            double average = sum / hitErrors.Count;
            double variance = 0;

            foreach (var hit in hitErrors)
            {
                variance += Math.Pow(hit - average, 2);
            }

            return Math.Sqrt(variance / hitErrors.Count) * 10;
        }

        private static StrainsResult GetStrains(string mapLocation, PlayMode? desiredPlayMode)
        {
            var workingBeatmap = new ProcessorWorkingBeatmap(mapLocation);

            var playMode = (PlayMode)PpCalculatorHelpers.GetRulesetId(workingBeatmap.RulesetID,
                desiredPlayMode.HasValue ? (int?)desiredPlayMode : null);

            var ppCalculator = PpCalculatorHelpers.GetPpCalculator((int)playMode, mapLocation, null);

            //Length refers to beatmap time, not song total time
            var mapLength = workingBeatmap.Length;
            var strainLength = 5000;
            var interval = 1500;
            var time = 0;
            var strains = new Dictionary<int, double>(300);

            if (ppCalculator == null)
            {
                while (time + strainLength / 2 < mapLength)
                {
                    strains.Add(time, 50);
                    time += interval;
                }
            }
            else if (playMode == PlayMode.Osu || playMode == PlayMode.Taiko || playMode == PlayMode.OsuMania)
            {

                var a = new Dictionary<string, double>();
                while (time + strainLength / 2 < mapLength)
                {
                    var strain = ppCalculator.Calculate(time, time + strainLength, a);

                    if (double.IsNaN(strain) || strain < 0)
                        strain = 0;
                    else if (strain > 2000)
                        strain = 2000; //lets not freeze everything with aspire/fancy 100* maps

                    strains.Add(time, strain);
                    time += interval;
                    a.Clear();
                }
            }

            return new StrainsResult
            {
                Strains = strains,
                PpCalculator = ppCalculator,
                WorkingBeatmap = workingBeatmap,
                PlayMode = playMode,
                MapLocation = mapLocation
            };
        }
        public void Dispose()
        {
            cancellationTokenSource.Cancel();
        }

        public void ToggleSmoothing(bool enable)
        {
            var speed = enable ? 0.15d : 1d;

            foreach (var v in InterpolatedValues)
            {
                v.Value.ChangeSpeed(speed);
            }
        }

        private class StrainsResult
        {
            public Dictionary<int, double> Strains;
            public PpCalculator.PpCalculator PpCalculator;
            public ProcessorWorkingBeatmap WorkingBeatmap;
            public PlayMode PlayMode;
            public string MapLocation;
        }

        public void CreateTokens(MapSearchResult mapSearchResult)
        {
            Mods = mapSearchResult.Mods?.Mods ?? Mods.Omod;
            if (mapSearchResult.FoundBeatmaps && mapSearchResult.BeatmapsFound[0].IsValidBeatmap(_settings, out var mapLocation))
                _strainsToken.Value = GetStrains(mapLocation, mapSearchResult.PlayMode).Strains;

            SetSkinTokens();
        }

        private void SetSkinTokens()
        {
            var osuSkinsDirectory = Path.Combine(_settings.Get<string>(SettingNames.Instance.MainOsuDirectory), "Skins");

            var skinName = _reader?.GetSkinFolderName() ?? string.Empty;
            _skinToken.Value = skinName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0
                ? string.Empty
                : skinName;

            string skinPath;
            try
            {
                skinPath = Path.Combine(osuSkinsDirectory, (string)_skinToken.Value);
            }
            catch (ArgumentException)
            {
                skinPath = string.Empty;
            }

            _skinPathToken.Value = skinPath;
        }
    }
}