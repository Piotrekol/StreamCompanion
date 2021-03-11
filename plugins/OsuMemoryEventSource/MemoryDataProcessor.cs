using CollectionManager.Enums;
using OsuMemoryDataProvider;
using StreamCompanionTypes;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OsuMemoryDataProvider.OsuMemoryModels;
using OsuMemoryDataProvider.OsuMemoryModels.Abstract;
using PpCalculatorTypes;
using StreamCompanion.Common;
using StreamCompanion.Common.Helpers;
using StreamCompanionTypes.Interfaces.Services;
using static StreamCompanion.Common.Helpers.OsuScore;
using Mods = CollectionManager.DataTypes.Mods;

namespace OsuMemoryEventSource
{
    public class MemoryDataProcessor : IDisposable
    {
        private readonly object _lockingObject = new object();
        private OsuStatus _lastStatus = OsuStatus.Null;
        private LivePerformanceCalculator _rawData = new LivePerformanceCalculator();
        private OsuBaseAddresses OsuMemoryData;

        private ISettings _settings;
        private readonly IContextAwareLogger _logger;
        private readonly Dictionary<string, LiveToken> _liveTokens = new Dictionary<string, LiveToken>();
        private Tokens.TokenSetter _liveTokenSetter => OsuMemoryEventSourceBase.LiveTokenSetter;
        private Tokens.TokenSetter _tokenSetter => OsuMemoryEventSourceBase.TokenSetter;
        public static ConfigEntry StrainsAmount = new ConfigEntry("StrainsAmount", (int?)100);
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
        public MemoryDataProcessor(ISettings settings, IContextAwareLogger logger, bool isMainProcessor,
            string tokensPath)
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

            Task.Run(TokenThreadWork, cancellationTokenSource.Token).HandleExceptions();
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

        public async Task SetNewMap(IMapSearchResult map, CancellationToken cancellationToken)
        {
            if ((map.Action & OsuStatus.ResultsScreen) != 0)
                return;

            var ppCalculator = await map.GetPpCalculator(cancellationToken);
            lock (_lockingObject)
            {
                if (map.BeatmapsFound.Any() &&
                    map.BeatmapsFound[0].IsValidBeatmap(_settings, out var mapLocation))
                {
                    _sliderBreaks = 0;
                    _lastMisses = 0;
                    _lastCombo = 0;
                    if (map.SearchArgs.EventType == OsuEventType.MapChange)
                        _rawData.SetPpCalculator(ppCalculator, cancellationToken);

                    _newPlayStarted.Set();
                }
            }
        }


        public void Tick(OsuStatus status, OsuMemoryStatus rawStatus, StructuredOsuMemoryReader reader)
        {
            _notUpdatingTokens.WaitOne();
            _notUpdatingMemoryValues.Reset();
            lock (_lockingObject)
            {
                if (!ReferenceEquals(OsuMemoryData, reader.OsuMemoryAddresses))
                {
                    OsuMemoryData = reader.OsuMemoryAddresses;
                    _rawData.Play = OsuMemoryData.Player;
                }
                if ((OsuStatus)_liveTokens["status"].Token.Value != status)
                    _liveTokens["status"].Token.Value = status;

                _liveTokens["rawStatus"].Token.Value = rawStatus;

                if (status != OsuStatus.Playing && status != OsuStatus.Watching)
                {
                    var rawAudioTime = Retry.RetryMe(() =>
                        reader.ReadProperty(OsuMemoryData.GeneralData, nameof(GeneralData.AudioTime)), (val => val != null), 5);
                    if (int.TryParse(rawAudioTime?.ToString(), out var audioTime))
                        _rawData.PlayTime = audioTime;

                    reader.Read(OsuMemoryData.Skin);
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

                reader.Read(OsuMemoryData.Player);
                _rawData.PlayTime = OsuMemoryData.GeneralData.AudioTime;
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
            CreateLiveToken("acc", _rawData.Play.Accuracy, TokenType.Live, "{0:0.00}", 0d, playingWatchingResults, () => _rawData.Play.Accuracy);
            CreateLiveToken("katsu", _rawData.Play.HitKatu, TokenType.Live, "{0}", (ushort)0, playingWatchingResults, () => _rawData.Play.HitKatu);
            CreateLiveToken("geki", _rawData.Play.HitGeki, TokenType.Live, "{0}", (ushort)0, playingWatchingResults, () => _rawData.Play.HitGeki);
            CreateLiveToken("c300", _rawData.Play.Hit300, TokenType.Live, "{0}", (ushort)0, playingWatchingResults, () => _rawData.Play.Hit300);
            CreateLiveToken("c100", _rawData.Play.Hit100, TokenType.Live, "{0}", (ushort)0, playingWatchingResults, () => _rawData.Play.Hit100);
            CreateLiveToken("c50", _rawData.Play.Hit50, TokenType.Live, "{0}", (ushort)0, playingWatchingResults, () => _rawData.Play.Hit50);
            CreateLiveToken("miss", _rawData.Play.HitMiss, TokenType.Live, "{0}", (ushort)0, playingWatchingResults, () => _rawData.Play.HitMiss);
            CreateLiveToken("grade", OsuGrade.Null, TokenType.Live, "", OsuGrade.Null, playingWatchingResults,
                () => CalculateGrade(_playMode, _mods, _rawData.Play.Accuracy, _rawData.Play.Hit50, _rawData.Play.Hit100, _rawData.Play.Hit300, _rawData.Play.HitMiss));
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
                 var beatmapLength = _rawData.PpCalculator?.BeatmapLength;
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
            CreateLiveToken("comboLeft", _rawData.ComboLeft, TokenType.Live, "{0}", 0, playingWatchingResults, () => _rawData.ComboLeft);
            CreateLiveToken("score", _rawData.Play.Score, TokenType.Live, "{0}", 0, playingWatchingResults, () => _rawData.Play.Score);
            CreateLiveToken("currentMaxCombo", _rawData.Play.MaxCombo, TokenType.Live, "{0}", (ushort)0, playingWatchingResults, () => _rawData.Play.MaxCombo);
            CreateLiveToken("playerHp", _rawData.Play.HP, TokenType.Live, "{0:0.00}", 0d, playingWatchingResults, () => _rawData.Play.HP);

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
                InterpolatedValues[InterpolatedValueName.UnstableRate].Set(UnstableRate(_rawData.Play.HitErrors));
                return InterpolatedValues[InterpolatedValueName.UnstableRate].Current;
            });
            CreateLiveToken("convertedUnstableRate", InterpolatedValues[InterpolatedValueName.UnstableRate].Current, TokenType.Live, "{0:0.000}", 0d, playingWatchingResults,
                () => ConvertedUnstableRate((double)_liveTokens["unstableRate"].Token.Value, _mods));
            CreateLiveToken("hitErrors", new List<int>(), TokenType.Live, ",", new List<int>(), playingWatchingResults, () => _rawData.Play.HitErrors);
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
            CreateLiveToken("isBreakTime", 0, TokenType.Live, "{0}", 0, OsuStatus.All, () => _rawData.PpCalculator?.IsBreakTime(_rawData.PlayTime) ?? false ? 1 : 0);
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
                catch (TaskCanceledException) { }
                catch (OperationCanceledException) { }
                catch (Exception ex)
                {
                    ex.Data["liveTokenName"] = liveToken.Key;
                    throw;
                }
            }
            _tokensUpdated();
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

        public async Task CreateTokensAsync(IMapSearchResult mapSearchResult, CancellationToken cancellationToken)
        {
            if (IsMainProcessor)
                SetSkinTokens();

            if (mapSearchResult.SearchArgs.EventType != OsuEventType.MapChange)
                return;

            _mods = mapSearchResult.Mods?.Mods ?? Mods.Omod;
            _playMode = mapSearchResult.PlayMode ?? PlayMode.Osu;
            if (!IsMainProcessor)
                return;

            _strainsToken.Value = (await mapSearchResult.GetPpCalculator(cancellationToken))?.CalculateStrains(cancellationToken, _settings.Get<int?>(StrainsAmount));
        }

        private void SetSkinTokens()
        {
            var osuSkinsDirectory = Path.Combine(_settings.Get<string>(SettingNames.Instance.MainOsuDirectory), "Skins");
            var skinName = OsuMemoryData.Skin.Folder ?? string.Empty;
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