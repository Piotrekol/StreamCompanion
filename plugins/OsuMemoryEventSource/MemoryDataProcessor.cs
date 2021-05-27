using CollectionManager.Enums;
using OsuMemoryDataProvider;
using StreamCompanionTypes;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using OsuMemoryDataProvider.OsuMemoryModels;
using OsuMemoryDataProvider.OsuMemoryModels.Direct;
using StreamCompanion.Common;
using StreamCompanion.Common.Extensions;
using StreamCompanionTypes.Interfaces.Services;
using static StreamCompanion.Common.Helpers.OsuScore;
using CollectionManager.DataTypes;
using Newtonsoft.Json;
using ErrorEventArgs = Newtonsoft.Json.Serialization.ErrorEventArgs;

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
        public static ConfigEntry MultiplayerLeaderBoardUpdateRate = new ConfigEntry("MultiplayerLeaderBoardUpdateRate", 250);
        private Mods _mods;
        private PlayMode _playMode = PlayMode.Osu;

        private IToken _strainsToken;
        private IToken _firstHitObjectTimeToken;
        private IToken _beatmapRankedStatusToken;
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
            _firstHitObjectTimeToken = _tokenSetter("firstHitObjectTime", 0d, TokenType.Normal, null, 0d);
            _beatmapRankedStatusToken = _tokenSetter("rankedStatus", (short)0, TokenType.Normal, null, (short)0);

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
            var ppCalculator = await map.GetPpCalculator(cancellationToken);
            lock (_lockingObject)
            {
                if (ppCalculator != null)
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

        private bool ReadLeaderboard = false;
        private DateTime _nextLeaderBoardUpdate = DateTime.MinValue;
        public void Tick(OsuStatus status, OsuMemoryStatus rawStatus, StructuredOsuMemoryReader reader)
        {
            _notUpdatingTokens.WaitOne();
            _notUpdatingMemoryValues.Reset();
            lock (_lockingObject)
            {
                if (!ReferenceEquals(OsuMemoryData, reader.OsuMemoryAddresses))
                {
                    OsuMemoryData = reader.OsuMemoryAddresses;
                }
                if ((OsuStatus)_liveTokens["status"].Token.Value != status)
                    _liveTokens["status"].Token.Value = status;

                _liveTokens["rawStatus"].Token.Value = rawStatus;
                int playTime = OsuMemoryData.GeneralData.AudioTime;
                switch (status)
                {
                    case OsuStatus.Playing:
                    case OsuStatus.Watching:
                        if (_lastStatus != OsuStatus.Playing && _lastStatus != OsuStatus.Watching)
                        {
                            _newPlayStarted.Set();
                            Thread.Sleep(500);//Initial play delay
                        }
                        
                        reader.TryRead(OsuMemoryData.KeyOverlay);
                        reader.TryRead(OsuMemoryData.Player);
                        if (!ReferenceEquals(_rawData.Play, OsuMemoryData.Player))
                            _rawData.Play = OsuMemoryData.Player;

                        //TODO: support for live multiplayer leaderboard
                        if (!ReadLeaderboard)
                        {
                            //Read whole leaderboard once
                            if (reader.TryRead(OsuMemoryData.LeaderBoard))
                            {
                                ReadLeaderboard = true;
                                if (!ReferenceEquals(_rawData.LeaderBoard, OsuMemoryData.LeaderBoard))
                                    _rawData.LeaderBoard = OsuMemoryData.LeaderBoard;
                            }
                        }
                        else
                        {
                            //Throttle whole leaderboard reads - Temporary solution until multiplayer detection is implemented, this should be only done in multiplayer
                            if (_nextLeaderBoardUpdate < DateTime.UtcNow)
                            {
                                reader.TryRead(OsuMemoryData.LeaderBoard);
                                _nextLeaderBoardUpdate = DateTime.UtcNow.AddMilliseconds(_settings.Get<int>(MultiplayerLeaderBoardUpdateRate));
                            }
                            else
                            {
                                //...then update main player data 
                                if (OsuMemoryData.LeaderBoard.HasLeaderBoard)
                                    reader.TryRead(OsuMemoryData.LeaderBoard.MainPlayer);
                            }
                        }

                        break;
                    case OsuStatus.ResultsScreen:
                        ReadLeaderboard = false;
                        reader.TryRead(OsuMemoryData.ResultsScreen);
                        if (!ReferenceEquals(_rawData.Play, OsuMemoryData.ResultsScreen))
                            _rawData.Play = OsuMemoryData.ResultsScreen;

                        playTime = Convert.ToInt32(_rawData.PpCalculator?.BeatmapLength ?? 0);
                        break;
                    default:
                        ReadLeaderboard = false;
                        _rawData.LeaderBoard = new LeaderBoard();
                        reader.TryRead(OsuMemoryData.Skin);
                        _lastStatus = status;
                        break;

                }

                _rawData.PlayTime = playTime;
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
            CreateLiveToken("username", _rawData.Play.Username, TokenType.Live, "", string.Empty, playingWatchingResults, () => _rawData.Play.Username);
            //TODO: manual acc calc from hit results
            CreateLiveToken("acc", 0d, TokenType.Live, "{0:0.00}", 0d, playingWatchingResults, () => _rawData.Play is Player p ? p.Accuracy : 0d);
            CreateLiveToken("katsu", _rawData.Play.HitKatu, TokenType.Live, "{0}", (ushort)0, playingWatchingResults, () => _rawData.Play.HitKatu);
            CreateLiveToken("geki", _rawData.Play.HitGeki, TokenType.Live, "{0}", (ushort)0, playingWatchingResults, () => _rawData.Play.HitGeki);
            CreateLiveToken("c300", _rawData.Play.Hit300, TokenType.Live, "{0}", (ushort)0, playingWatchingResults, () => _rawData.Play.Hit300);
            CreateLiveToken("c100", _rawData.Play.Hit100, TokenType.Live, "{0}", (ushort)0, playingWatchingResults, () => _rawData.Play.Hit100);
            CreateLiveToken("c50", _rawData.Play.Hit50, TokenType.Live, "{0}", (ushort)0, playingWatchingResults, () => _rawData.Play.Hit50);
            CreateLiveToken("miss", _rawData.Play.HitMiss, TokenType.Live, "{0}", (ushort)0, playingWatchingResults, () => _rawData.Play.HitMiss);
            CreateLiveToken("grade", OsuGrade.Null, TokenType.Live, "", OsuGrade.Null, playingWatchingResults,
                () => CalculateGrade(_playMode, _mods, _rawData.Play is Player p ? p.Accuracy : 0d, _rawData.Play.Hit50, _rawData.Play.Hit100, _rawData.Play.Hit300, _rawData.Play.HitMiss));
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
            CreateLiveToken("scoreV2", _rawData.Play.ScoreV2 ?? 0, TokenType.Live, "{0}", 0, playingWatchingResults, () => _rawData.Play.ScoreV2 ?? 0);
            CreateLiveToken("currentMaxCombo", _rawData.Play.MaxCombo, TokenType.Live, "{0}", (ushort)0, playingWatchingResults, () => _rawData.Play.MaxCombo);
            CreateLiveToken("playerHp", 0d, TokenType.Live, "{0:0.00}", 0d, playingWatchingResults, () => _rawData.Play is Player p ? p.HP : 0d);

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
                double unstableRate = 0d;
                if (_rawData.Play is Player p)
                    unstableRate = UnstableRate(p.HitErrors);

                InterpolatedValues[InterpolatedValueName.UnstableRate].Set(unstableRate);
                return InterpolatedValues[InterpolatedValueName.UnstableRate].Current;
            });
            CreateLiveToken("convertedUnstableRate", InterpolatedValues[InterpolatedValueName.UnstableRate].Current, TokenType.Live, "{0:0.000}", 0d, playingWatchingResults,
                () => ConvertedUnstableRate((double)_liveTokens["unstableRate"].Token.Value, _mods));
            CreateLiveToken("hitErrors", new List<int>(), TokenType.Live, ",", new List<int>(), playingWatchingResults, () => _rawData.Play is Player p ? p.HitErrors : null);
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
            
            JsonSerializerSettings createJsonSerializerSettings(string serializationErrorMessage)
                => new JsonSerializerSettings
                {
                    Error = (sender, args) =>
                    {
                        _logger.Log(serializationErrorMessage, LogLevel.Debug);
                        _logger.Log(args, LogLevel.Trace);
                    }
                };
            // object lastLeaderBoardObject = null;
            string lastLeaderBoardData = "{}";
            DateTime lastLeaderBoardUpdate = DateTime.MinValue.AddMilliseconds(1);
            CreateLiveToken("leaderBoardPlayers", "[]", TokenType.Live, "", "[]", playingOrWatching, () =>
            {
                ////TODO: assumes singleplayer(other player data never updates)
                //if (ReferenceEquals(_rawData.LeaderBoard, lastLeaderBoardObject))
                //    return lastLeaderBoardData;

                //lastLeaderBoardObject = _rawData.LeaderBoard;
                var nextUpdateAt = _nextLeaderBoardUpdate;
                if (nextUpdateAt == lastLeaderBoardUpdate)
                    return lastLeaderBoardData;

                lastLeaderBoardUpdate = nextUpdateAt;
                return lastLeaderBoardData = JsonConvert.SerializeObject(_rawData.LeaderBoard.Players, createJsonSerializerSettings("Failed to serialize leaderBoardPlayers token data."));
            });

            CreateLiveToken("leaderBoardMainPlayer", "{}", TokenType.Live, "", "{}", playingOrWatching, () => JsonConvert.SerializeObject(_rawData.LeaderBoard.MainPlayer, createJsonSerializerSettings("Failed to serialize leaderBoardMainPlayer token data.")));
            CreateLiveToken("keyOverlay", "{}", TokenType.Live, "", "{}", playingOrWatching, () => JsonConvert.SerializeObject(OsuMemoryData.KeyOverlay, createJsonSerializerSettings("Failed to serialize keyOverlay token data.")));
            CreateLiveToken("chatIsEnabled", 0, TokenType.Live, null, 0, OsuStatus.All, () => OsuMemoryData.GeneralData.ChatIsExpanded ? 1 : 0);
            CreateLiveToken("ingameInterfaceIsEnabled", 0, TokenType.Live, null, 0, OsuStatus.All, () => OsuMemoryData.GeneralData.ShowPlayingInterface ? 1 : 0);
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
            cancellationTokenSource.TryCancel();
            cancellationTokenSource.Dispose();
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

            _beatmapRankedStatusToken.Value = OsuMemoryData.Beatmap.Status;
            var ppCalculator = await mapSearchResult.GetPpCalculator(cancellationToken);
            _firstHitObjectTimeToken.Value = ppCalculator?.FirstHitObjectTime();
            _strainsToken.Value = ppCalculator?.CalculateStrains(cancellationToken, _settings.Get<int?>(StrainsAmount));
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