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
using static StreamCompanion.Common.Helpers.OsuScore;

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

        private Mods _mods;
        private PlayMode _playMode = PlayMode.Osu;

        private IToken _strainsToken;
        private IToken _skinToken;
        private IToken _skinPathToken;

        private ushort _lastMisses = 0;
        private ushort _lastCombo = 0;
        private int _sliderBreaks = 0;

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
            liveStarRating,
        }
        private ManualResetEvent _notUpdatingTokens = new ManualResetEvent(true);
        private ManualResetEvent _notUpdatingMemoryValues = new ManualResetEvent(true);
        private ManualResetEvent _newPlayStarted = new ManualResetEvent(true);

        private readonly Dictionary<InterpolatedValueName, InterpolatedValue> InterpolatedValues = new Dictionary<InterpolatedValueName, InterpolatedValue>();
        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        public EventHandler<OsuStatus> TokensUpdated { get; set; }
        public bool IsMainProcessor { get; private set; }
        public string TokensPath { get; private set; }
        public MemoryDataProcessor(ISettings settings, IContextAwareLogger logger, bool isMainProcessor, string tokensPath)
        {
            _settings = settings;
            _logger = logger;
            foreach (var v in (InterpolatedValueName[])Enum.GetValues(typeof(InterpolatedValueName)))
            {
                InterpolatedValues.Add(v, new InterpolatedValue(0.15));
            }

            ToggleSmoothing(true);
            IsMainProcessor = isMainProcessor;
            TokensPath = tokensPath;

            _strainsToken = _tokenSetter("mapStrains", new Dictionary<int, double>(), TokenType.Normal, ",", new Dictionary<int, double>());

            _skinToken = _tokenSetter("skin", string.Empty, TokenType.Normal, null, string.Empty);
            _skinPathToken = _tokenSetter("skinPath", string.Empty, TokenType.Normal, null, string.Empty);

            InitLiveTokens();

            Task.Run(TokenThreadWork, cancellationTokenSource.Token);
        }

        public async Task TokenThreadWork()
        {
            while (true)
            {
                if (cancellationTokenSource.IsCancellationRequested)
                    return;

                if (_notUpdatingMemoryValues.WaitOne(0))
                {
                    _notUpdatingTokens.Reset();
                    UpdateLiveTokens(_lastStatus);
                    _notUpdatingTokens.Set();
                }

                if (_newPlayStarted.WaitOne(0))
                {
                    _sliderBreaks = 0;
                    _newPlayStarted.Reset();
                }

                foreach (var interpolatedValue in InterpolatedValues)
                {
                    interpolatedValue.Value.Tick();
                }

                await Task.Delay(1);
            }
        }

        public void SetNewMap(IMapSearchResult map)
        {
            lock (_lockingObject)
            {
                if ((map.Action & OsuStatus.ResultsScreen) != 0)
                    return;

                if (map.BeatmapsFound.Any() &&
                    map.BeatmapsFound[0].IsValidBeatmap(_settings, out var mapLocation))
                {
                    _sliderBreaks = 0;
                    _lastMisses = 0;
                    _lastCombo = 0;
                    if (map.SearchArgs.EventType == OsuEventType.MapChange)
                    {
                        var workingBeatmap = new ProcessorWorkingBeatmap(mapLocation);
                        var mods = map.Mods?.WorkingMods ?? "";
                        _rawData.SetCurrentMap(map.BeatmapsFound[0], mods, mapLocation,
                            (PlayMode)PpCalculatorHelpers.GetRulesetId(workingBeatmap.RulesetID, map.PlayMode.HasValue ? (int?)map.PlayMode : null));
                    }

                    _newPlayStarted.Set();
                }
            }
        }

        private IOsuMemoryReader _reader;
        public void Tick(OsuStatus status, OsuMemoryStatus rawStatus, IOsuMemoryReader reader)
        {
            _notUpdatingTokens.WaitOne();
            _notUpdatingMemoryValues.Reset();
            lock (_lockingObject)
            {
                if (!ReferenceEquals(_reader, reader))
                    _reader = reader;
                if ((OsuStatus)_liveTokens["status"].Token.Value != status)
                    _liveTokens["status"].Token.Value = status;

                _liveTokens["rawStatus"].Token.Value = rawStatus;

                if (status != OsuStatus.Playing && status != OsuStatus.Watching)
                {
                    _rawData.PlayTime = reader.ReadPlayTime();
                    UpdateLiveTokens(status);
                    _lastStatus = status;
                    _notUpdatingMemoryValues.Set();
                    return;
                }

                if (_lastStatus != OsuStatus.Playing && _lastStatus != OsuStatus.Watching)
                {
                    _newPlayStarted.Set();
                    Thread.Sleep(500);//Initial play delay
                }

                reader.GetPlayData(_rawData.Play);
                //TODO: change this on OsuMemoryReader side (read v2 in getPlayData & return 0 as default instead of -1)
                _rawData.Play.Score = Math.Max(0, _reader.ReadScoreV2());

                _rawData.HitErrors = reader.HitErrors() ?? new List<int>();

                _rawData.PlayTime = reader.ReadPlayTime();
                _liveTokens["time"].Update();

                _lastStatus = status;
            }
            _notUpdatingMemoryValues.Set();
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

        private void CreateToken(string name, object value, TokenType tokenType, string format,
            object defaultValue, OsuStatus statusWhitelist, Func<object> updater)
        {
            _liveTokens[name] = new LiveToken(_tokenSetter($"{TokensPath}{name}", value, tokenType, format, defaultValue, statusWhitelist), updater);
        }
        private void CreateLiveToken(string name, object value, TokenType tokenType, string format,
            object defaultValue, OsuStatus statusWhitelist, Func<object> updater)
        {
            _liveTokens[name] = new LiveToken(_liveTokenSetter($"{TokensPath}{name}", value, tokenType, format, defaultValue, statusWhitelist), updater) { IsLazy = false };
            //_liveTokens[name] = new LiveToken(_liveTokenSetter(name, new Lazy<object>(() => value), tokenType, format, defaultValue, statusWhitelist), updater) { IsLazy = false };
        }

        private void InitLiveTokens()
        {
            CreateToken("status", OsuStatus.Null, TokenType.Normal, "", OsuStatus.Null, OsuStatus.All, null);
            CreateToken("rawStatus", OsuMemoryStatus.NotRunning, TokenType.Normal, "", OsuMemoryStatus.NotRunning, OsuStatus.All, null);
            var playingOrWatching = OsuStatus.Playing | OsuStatus.Watching;
            var playingWatchingResults = playingOrWatching | OsuStatus.ResultsScreen;
            CreateLiveToken("acc", _rawData.Play.Acc, TokenType.Live, "{0:0.00}", 0d, playingWatchingResults, () => _rawData.Play.Acc);
            CreateLiveToken("katsu", _rawData.Play.CKatsu, TokenType.Live, "{0}", (ushort)0, playingWatchingResults, () => _rawData.Play.CKatsu);
            CreateLiveToken("geki", _rawData.Play.CGeki, TokenType.Live, "{0}", (ushort)0, playingWatchingResults, () => _rawData.Play.CGeki);
            CreateLiveToken("c300", _rawData.Play.C300, TokenType.Live, "{0}", (ushort)0, playingWatchingResults, () => _rawData.Play.C300);
            CreateLiveToken("c100", _rawData.Play.C100, TokenType.Live, "{0}", (ushort)0, playingWatchingResults, () => _rawData.Play.C100);
            CreateLiveToken("c50", _rawData.Play.C50, TokenType.Live, "{0}", (ushort)0, playingWatchingResults, () => _rawData.Play.C50);
            CreateLiveToken("miss", _rawData.Play.CMiss, TokenType.Live, "{0}", (ushort)0, playingWatchingResults, () => _rawData.Play.CMiss);
            CreateLiveToken("grade", OsuGrade.Null, TokenType.Live, "", OsuGrade.Null, playingWatchingResults,
                () => CalculateGrade(_playMode, _mods, _rawData.Play.Acc, _rawData.Play.C50, _rawData.Play.C100, _rawData.Play.C300, _rawData.Play.CMiss));
            CreateLiveToken("mapPosition", TimeSpan.Zero, TokenType.Live, "{0:mm\\:ss}", TimeSpan.Zero, OsuStatus.All, () =>
            {
                if (_rawData.PlayTime != 0)
                    return TimeSpan.FromMilliseconds(_rawData.PlayTime);
                return TimeSpan.Zero;
            });
            CreateLiveToken("time", 0d, TokenType.Live, "{0:0.00}", 0d, OsuStatus.All, () =>
             {
                 if (_rawData.PlayTime != 0)
                     return _rawData.PlayTime / 1000d;
                 return 0d;
             });
            CreateLiveToken("timeLeft", TimeSpan.Zero, TokenType.Live, "{0:mm\\:ss}", TimeSpan.Zero, OsuStatus.All, () =>
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
            CreateLiveToken("combo", _rawData.Play.Combo, TokenType.Live, "{0}", (ushort)0, playingWatchingResults, () => _rawData.Play.Combo);
            CreateLiveToken("score", _rawData.Play.Score, TokenType.Live, "{0}", 0, playingWatchingResults, () => _rawData.Play.Score);
            CreateLiveToken("currentMaxCombo", _rawData.Play.MaxCombo, TokenType.Live, "{0}", (ushort)0, playingWatchingResults, () => _rawData.Play.MaxCombo);
            CreateLiveToken("playerHp", _rawData.Play.Hp, TokenType.Live, "{0:0.00}", 0d, playingWatchingResults, () => _rawData.Play.Hp);

            CreateLiveToken("ppIfMapEndsNow", InterpolatedValues[InterpolatedValueName.PpIfMapEndsNow].Current, TokenType.Live, "{0:0.00}", 0d, playingWatchingResults, () =>
            {
                InterpolatedValues[InterpolatedValueName.PpIfMapEndsNow].Set(_rawData.PPIfBeatmapWouldEndNow());
                return InterpolatedValues[InterpolatedValueName.PpIfMapEndsNow].Current;
            });
            CreateLiveToken("aimPpIfMapEndsNow", InterpolatedValues[InterpolatedValueName.AimPpIfMapEndsNow].Current, TokenType.Live, "{0:0.00}", 0d, playingWatchingResults, () =>
            {
                InterpolatedValues[InterpolatedValueName.AimPpIfMapEndsNow].Set(_rawData.AimPPIfBeatmapWouldEndNow);
                return InterpolatedValues[InterpolatedValueName.AimPpIfMapEndsNow].Current;
            });
            CreateLiveToken("speedPpIfMapEndsNow", InterpolatedValues[InterpolatedValueName.SpeedPpIfMapEndsNow].Current, TokenType.Live, "{0:0.00}", 0d, playingWatchingResults, () =>
            {
                InterpolatedValues[InterpolatedValueName.SpeedPpIfMapEndsNow].Set(_rawData.SpeedPPIfBeatmapWouldEndNow);
                return InterpolatedValues[InterpolatedValueName.SpeedPpIfMapEndsNow].Current;
            });
            CreateLiveToken("accPpIfMapEndsNow", InterpolatedValues[InterpolatedValueName.AccPpIfMapEndsNow].Current, TokenType.Live, "{0:0.00}", 0d, playingWatchingResults, () =>
            {
                InterpolatedValues[InterpolatedValueName.AccPpIfMapEndsNow].Set(_rawData.AccPPIfBeatmapWouldEndNow);
                return InterpolatedValues[InterpolatedValueName.AccPpIfMapEndsNow].Current;
            });
            CreateLiveToken("strainPpIfMapEndsNow", InterpolatedValues[InterpolatedValueName.StrainPpIfMapEndsNow].Current, TokenType.Live, "{0:0.00}", 0d, playingWatchingResults, () =>
            {
                InterpolatedValues[InterpolatedValueName.StrainPpIfMapEndsNow].Set(_rawData.StrainPPIfBeatmapWouldEndNow);
                return InterpolatedValues[InterpolatedValueName.StrainPpIfMapEndsNow].Current;
            });
            CreateLiveToken("ppIfRestFced", InterpolatedValues[InterpolatedValueName.PpIfRestFced].Current, TokenType.Live, "{0:0.00}", 0d, playingWatchingResults, () =>
            {
                InterpolatedValues[InterpolatedValueName.PpIfRestFced].Set(_rawData.PPIfRestFCed());
                return InterpolatedValues[InterpolatedValueName.PpIfRestFced].Current;
            });
            CreateLiveToken("noChokePp", InterpolatedValues[InterpolatedValueName.NoChokePp].Current, TokenType.Live, "{0:0.00}", 0d, playingWatchingResults, () =>
            {
                InterpolatedValues[InterpolatedValueName.NoChokePp].Set(_rawData.NoChokePp());
                return InterpolatedValues[InterpolatedValueName.NoChokePp].Current;
            });
            CreateLiveToken("simulatedPp", InterpolatedValues[InterpolatedValueName.SimulatedPp].Current, TokenType.Live, "{0:0.00}", 0d, OsuStatus.All, () =>
            {
                InterpolatedValues[InterpolatedValueName.SimulatedPp].Set(_rawData.SimulatedPp());
                return InterpolatedValues[InterpolatedValueName.SimulatedPp].Current;
            });
            CreateLiveToken("unstableRate", InterpolatedValues[InterpolatedValueName.UnstableRate].Current, TokenType.Live, "{0:0.000}", 0d, playingWatchingResults, () =>
            {
                InterpolatedValues[InterpolatedValueName.UnstableRate].Set(UnstableRate(_rawData.HitErrors));
                return InterpolatedValues[InterpolatedValueName.UnstableRate].Current;
            });
            CreateLiveToken("convertedUnstableRate", InterpolatedValues[InterpolatedValueName.UnstableRate].Current, TokenType.Live, "{0:0.000}", 0d, playingWatchingResults,
                () => ConvertedUnstableRate((double)_liveTokens["unstableRate"].Token.Value, _mods));
            CreateLiveToken("hitErrors", new List<int>(), TokenType.Live, ",", new List<int>(), playingWatchingResults, () => _rawData.HitErrors);
            CreateLiveToken("localTimeISO", DateTime.UtcNow.ToString("o"), TokenType.Live, "", DateTime.UtcNow, OsuStatus.All, () => DateTime.UtcNow.ToString("o"));
            CreateLiveToken("localTime", DateTime.Now.TimeOfDay, TokenType.Live, "{0:hh}:{0:mm}:{0:ss}", DateTime.Now.TimeOfDay, OsuStatus.All, () => DateTime.Now.TimeOfDay);
            CreateLiveToken("sliderBreaks", 0, TokenType.Live, "{0}", 0, playingWatchingResults, () =>
            {
                var currentMisses = (ushort)_liveTokens["miss"].Token.Value;
                var currentCombo = (ushort)_liveTokens["combo"].Token.Value;

                if (_lastMisses == currentMisses && _lastCombo > currentCombo)
                    _sliderBreaks++;

                _lastMisses = currentMisses;
                _lastCombo = currentCombo;

                return _sliderBreaks;
            });
            CreateLiveToken("liveStarRating", InterpolatedValues[InterpolatedValueName.liveStarRating].Current, TokenType.Live, "{0:0.00}", 0d, OsuStatus.All, () =>
            {
                var attributes = _rawData.PpCalculator?.AttributesAt(_rawData.PlayTime);
                InterpolatedValues[InterpolatedValueName.liveStarRating].Set(attributes?.StarRating ?? 0);
                return InterpolatedValues[InterpolatedValueName.liveStarRating].Current;
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

        //TODO: this should end up in StreamCompanion.Common project along with pp calc (remove all references to PpCalculator/osu.Game from all plugins except common one)
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
            else
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
            var speed = enable ? 0.07d : 1d;

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

        public Task CreateTokensAsync(IMapSearchResult mapSearchResult, CancellationToken cancellationToken)
        {
            if (IsMainProcessor)
                SetSkinTokens();

            if (mapSearchResult.SearchArgs.EventType != OsuEventType.MapChange)
                return Task.CompletedTask;

            _mods = mapSearchResult.Mods?.Mods ?? Mods.Omod;
            _playMode = mapSearchResult.PlayMode ?? PlayMode.Osu;

            if (IsMainProcessor && mapSearchResult.BeatmapsFound.Any() && mapSearchResult.BeatmapsFound[0].IsValidBeatmap(_settings, out var mapLocation))
                _strainsToken.Value = GetStrains(mapLocation, mapSearchResult.PlayMode).Strains;

            return Task.CompletedTask;
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