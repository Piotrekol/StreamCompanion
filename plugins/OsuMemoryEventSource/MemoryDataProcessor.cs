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
using OsuMemoryDataProvider.OsuMemoryModels;
using OsuMemoryDataProvider.OsuMemoryModels.Direct;
using StreamCompanion.Common;
using StreamCompanion.Common.Extensions;
using StreamCompanionTypes.Interfaces.Services;
using static StreamCompanion.Common.Helpers.OsuScore;
using Newtonsoft.Json;
using OsuMemoryEventSource.Extensions;
using OsuMemoryEventSource.LiveTokens;
using PpCalculatorTypes;
using Mods = CollectionManager.DataTypes.Mods;
using StreamCompanion.Common.Helpers.Tokens;

namespace OsuMemoryEventSource
{
    public class MemoryDataProcessor : IDisposable
    {
        private readonly object _lockingObject = new object();
        private OsuStatus _lastStatus = OsuStatus.Null;
        private LivePerformanceCalculator _rawData = new LivePerformanceCalculator();
        private OsuBaseAddresses OsuMemoryData;
        private StructuredOsuMemoryReader _memoryReader;

        private ISettings _settings;
        private readonly IContextAwareLogger _logger;
        private readonly IModParser _modParser;
        private readonly Dictionary<string, BaseLiveToken> _liveTokens = new();
        private readonly Dictionary<string, JsonSerializerSettings> _tokenJsonSerializers = new();

        private Tokens.TokenSetter _liveTokenSetter => OsuMemoryEventSourceBase.LiveTokenSetter;
        private Tokens.TokenSetter _tokenSetter => OsuMemoryEventSourceBase.TokenSetter;
        public static ConfigEntry MultiplayerLeaderBoardUpdateRate = new ConfigEntry("MultiplayerLeaderBoardUpdateRate", 250);
        public static ConfigEntry SongSelectionScoresUpdateRate = new ConfigEntry("SongSelectionScoresUpdateRate", 250);
        private Mods _mods;
        private PlayMode _playMode = PlayMode.Osu;
        private int _hitObjectCount = 0;
        private double _bpmMultiplier = 1;

        private IToken _firstHitObjectTimeToken;
        private IToken _MapBreaksToken;
        private IToken _MapTimingPointsToken;
        private IToken _MapKiaiPointsToken;
        private IToken _beatmapRankedStatusToken;
        private IToken _skinToken;
        private IToken _skinPathToken;
        private IToken _statusToken;
        private IToken _rawStatusToken;
        private IToken _beatmapIdToken;
        private IToken _beatmapSetIdToken;
        private IToken _totalAudioTime;

        private ushort _lastMisses = 0;
        private ushort _lastCombo = 0;
        private int _sliderBreaks = 0;
        private double _firstHitObjectTime = 0;
        private List<TimingPoint> _reversedMapTimingPoints;

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
        private ManualResetEvent _notAddingNewTokens = new ManualResetEvent(true);
        private ManualResetEvent _notUpdatingTokens = new ManualResetEvent(true);
        private ManualResetEvent _notUpdatingMemoryValues = new ManualResetEvent(true);
        private ManualResetEvent _newPlayStarted = new ManualResetEvent(true);

        private readonly Dictionary<InterpolatedValueName, InterpolatedValue> InterpolatedValues = new();
        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        public EventHandler<OsuStatus> TokensUpdated { get; set; }
        /// <summary>
        /// Whenever this <see cref="MemoryDataProcessor"/> should be setting map-specific token values (skin, strains, mapId etc...)
        /// </summary>
        public bool IsMainProcessor { get; private set; }
        /// <summary>
        /// Do we have valid osu client available that we can read from
        /// </summary>
        public bool IsMemoryReaderActive => _memoryReader != null && _memoryReader.CanRead;

        public string TokensPath { get; private set; }
        public MemoryDataProcessor(ISettings settings, IContextAwareLogger logger, IModParser modParser,
            bool isMainProcessor,
            string tokensPath)
        {
            _settings = settings;
            _logger = logger;
            _modParser = modParser;
            foreach (var v in (InterpolatedValueName[])Enum.GetValues(typeof(InterpolatedValueName)))
            {
                InterpolatedValues.Add(v, new RoundedInterpolatedValue(0.15));
            }

            ToggleSmoothing(true);
            IsMainProcessor = isMainProcessor;
            TokensPath = tokensPath;

            //TODO: Refactor these out to separate class already..(and make sure these run under isMainProcessor only)
            _skinToken = _tokenSetter("skin", string.Empty, TokenType.Normal, null, string.Empty);
            _skinPathToken = _tokenSetter("skinPath", string.Empty, TokenType.Normal, null, string.Empty);
            _firstHitObjectTimeToken = _tokenSetter("firstHitObjectTime", 0d, TokenType.Normal, null, 0d);
            _MapBreaksToken = _tokenSetter("mapBreaks", new List<BreakPeriod>(), TokenType.Normal, null, new List<BreakPeriod>());
            _MapTimingPointsToken = _tokenSetter("mapTimingPoints", new List<TimingPoint>(), TokenType.Normal, null, new List<TimingPoint>());
            _MapKiaiPointsToken = _tokenSetter("mapKiaiPoints", new List<KiaiPoint>(), TokenType.Normal, null, new List<KiaiPoint>());
            _beatmapRankedStatusToken = _tokenSetter("rankedStatus", (short)0, TokenType.Normal, null, (short)0);
            _statusToken = _tokenSetter("status", OsuStatus.Null, TokenType.Normal, "", OsuStatus.Null);
            _rawStatusToken = _tokenSetter("rawStatus", OsuMemoryStatus.NotRunning, TokenType.Normal, "", OsuMemoryStatus.NotRunning);
            _beatmapIdToken = _tokenSetter("mapid", 0, TokenType.Normal, null, 0);
            _beatmapSetIdToken = _tokenSetter("mapsetid", 0, TokenType.Normal, null, 0);
            _totalAudioTime = _tokenSetter("totalAudioTime", 0d, TokenType.Normal, "{0:0.##}", 0d);

            InitLiveTokens();

            if (isMainProcessor)
            {
                TokensExtensions.LiveTokenSetter = CreateLiveToken;
            }

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
        private DateTime _nextSongSelectionScoresUpdate = DateTime.MinValue;
        public void Tick(OsuStatus status, OsuMemoryStatus rawStatus, StructuredOsuMemoryReader reader)
        {
            _notUpdatingTokens.WaitOne();
            _notUpdatingMemoryValues.Reset();
            lock (_lockingObject)
            {
                if (!ReferenceEquals(_memoryReader, reader))
                    _memoryReader = reader;

                if (!ReferenceEquals(OsuMemoryData, reader.OsuMemoryAddresses))
                    OsuMemoryData = reader.OsuMemoryAddresses;

                if ((OsuStatus)_statusToken.Value != status)
                    _statusToken.Value = status;

                _rawStatusToken.Value = rawStatus;
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
                        reader.TryRead(OsuMemoryData.BanchoUser);
                        if (_nextSongSelectionScoresUpdate < DateTime.UtcNow)
                        {
                            reader.TryRead(OsuMemoryData.SongSelectionScores);
                            _nextSongSelectionScoresUpdate = DateTime.UtcNow.AddMilliseconds(_settings.Get<int>(SongSelectionScoresUpdateRate));
                        }

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
            TokensUpdated?.Invoke(this, (OsuStatus)_statusToken.Value);
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

        private void CreateLiveToken(string name, object value, TokenType tokenType, string format,
            object defaultValue, OsuStatus statusWhitelist, Func<object> updater)
        {
            var newToken = _liveTokenSetter($"{TokensPath}{name}", new Lazy<object>(() => value), tokenType, format, new Lazy<object>(() => defaultValue), statusWhitelist);
            CreateLiveToken(newToken, updater);
        }

        private void CreateLiveToken(IToken token, Func<object> updater)
        {
            try
            {
                _notUpdatingTokens.WaitOne();
                _notAddingNewTokens.Reset();
                if (token is LazyToken<object>)
                    _liveTokens[token.Name] = new LazyLiveToken(token, updater);
                else
                    _liveTokens[token.Name] = new LiveToken(token, updater);
            }
            finally
            {
                _notAddingNewTokens.Set();
            }
        }

        private void InitLiveTokens()
        {
            var playingOrWatching = OsuStatus.Playing | OsuStatus.Watching;
            var playingWatchingResults = playingOrWatching | OsuStatus.ResultsScreen;
            CreateLiveToken("username", _rawData.Play.Username, TokenType.Live, "", string.Empty, playingWatchingResults, () => _rawData.Play.Username);
            CreateLiveToken("acc", 0d, TokenType.Live, "{0:0.00}", 0d, playingWatchingResults, () => _rawData.Play is Player player
                ? Math.Round(player.Accuracy, 2)
                : Math.Round(CalculateAccuracy(_playMode, _rawData.Play.Hit50, _rawData.Play.Hit100, _rawData.Play.Hit300, _rawData.Play.HitMiss, _rawData.Play.HitGeki, _rawData.Play.HitKatu) * 100, 2)
                );
            CreateLiveToken("katsu", _rawData.Play.HitKatu, TokenType.Live, "{0}", (ushort)0, playingWatchingResults, () => _rawData.Play.HitKatu);
            CreateLiveToken("geki", _rawData.Play.HitGeki, TokenType.Live, "{0}", (ushort)0, playingWatchingResults, () => _rawData.Play.HitGeki);
            CreateLiveToken("c300", _rawData.Play.Hit300, TokenType.Live, "{0}", (ushort)0, playingWatchingResults, () => _rawData.Play.Hit300);
            CreateLiveToken("c100", _rawData.Play.Hit100, TokenType.Live, "{0}", (ushort)0, playingWatchingResults, () => _rawData.Play.Hit100);
            CreateLiveToken("c50", _rawData.Play.Hit50, TokenType.Live, "{0}", (ushort)0, playingWatchingResults, () => _rawData.Play.Hit50);
            CreateLiveToken("miss", _rawData.Play.HitMiss, TokenType.Live, "{0}", (ushort)0, playingWatchingResults, () => _rawData.Play.HitMiss);
            CreateLiveToken("grade", OsuGrade.Null, TokenType.Live, "", OsuGrade.Null, playingWatchingResults, () =>
            {
                var acc = _rawData.Play is Player p
                    ? p.Accuracy
                    : 100 * CalculateAccuracy(_playMode, _rawData.Play.Hit50, _rawData.Play.Hit100, _rawData.Play.Hit300, _rawData.Play.HitMiss, _rawData.Play.HitGeki, _rawData.Play.HitKatu);

                return CalculateGrade(_playMode, _mods, acc, _rawData.Play.Hit50, _rawData.Play.Hit100, _rawData.Play.Hit300, _rawData.Play.HitMiss);
            });
            CreateLiveToken("maxGrade", OsuGrade.Null, TokenType.Live, "", OsuGrade.Null, playingWatchingResults, () =>
            {
                var play = _rawData.Play;
                var h300left = _hitObjectCount - play.Hit300 - play.Hit100 - play.Hit50 - play.HitMiss;
                var newHit300 = (ushort)(h300left + play.Hit300);
                var acc = CalculateAccuracy(_playMode, play.Hit50, play.Hit100, newHit300, play.HitMiss, play.HitGeki, play.HitKatu) * 100;
                return CalculateGrade(_playMode, _mods, acc, play.Hit50, play.Hit100, newHit300, play.HitMiss);
            });
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
            CreateLiveToken("playerHp", 0d, TokenType.Live, "{0:0.00}", 0d, playingWatchingResults, () => _rawData.Play is Player p ? p.HP : 0d);
            CreateLiveToken("playerHpSmooth", 0d, TokenType.Live, "{0:0.00}", 0d, playingWatchingResults, () => _rawData.Play is Player p ? p.HPSmooth : 0d);

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
                if (_firstHitObjectTime > _rawData.PlayTime)
                    return _sliderBreaks = 0;

                var currentMisses = _rawData.Play.HitMiss;
                var currentCombo = _rawData.Play.Combo;

                if (_lastMisses == currentMisses && _lastCombo > currentCombo)
                    _sliderBreaks++;

                _lastMisses = currentMisses;
                _lastCombo = currentCombo;

                return _sliderBreaks;
            });
            CreateLiveToken("liveStarRating", InterpolatedValues[InterpolatedValueName.liveStarRating].Current, TokenType.Live, "{0:0.00}", 0d, OsuStatus.All, () =>
            {
                var attributes = _rawData.PpCalculator?.DifficultyAttributesAt(_rawData.PlayTime);
                InterpolatedValues[InterpolatedValueName.liveStarRating].Set(attributes?.StarRating ?? 0);
                return InterpolatedValues[InterpolatedValueName.liveStarRating].Current;
            });
            CreateLiveToken("isBreakTime", 0, TokenType.Live, "{0}", 0, OsuStatus.All, () => _rawData.PpCalculator?.IsBreakTime(_rawData.PlayTime) ?? false ? 1 : 0);
            CreateLiveToken("currentBpm", 0d, TokenType.Live, "{0}", 0d, OsuStatus.All, () => (_reversedMapTimingPoints?.FirstOrDefault(t => t.StartTime < _rawData.PlayTime)?.BPM ?? 0d) * _bpmMultiplier);

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
                return lastLeaderBoardData = JsonConvert.SerializeObject(_rawData.LeaderBoard.Players, createJsonSerializerSettings("leaderBoardPlayers"));
            });

            CreateLiveToken("leaderBoardMainPlayer", "{}", TokenType.Live, "", "{}", playingOrWatching, () => JsonConvert.SerializeObject(_rawData.LeaderBoard.MainPlayer, createJsonSerializerSettings("leaderBoardMainPlayer")));
            CreateLiveToken("keyOverlay", "{}", TokenType.Live, "", "{}", playingOrWatching, () => JsonConvert.SerializeObject(OsuMemoryData.KeyOverlay, createJsonSerializerSettings("keyOverlay")));
            CreateLiveToken("chatIsEnabled", 0, TokenType.Live, null, 0, OsuStatus.All, () => OsuMemoryData.GeneralData.ChatIsExpanded ? 1 : 0);
            CreateLiveToken("ingameInterfaceIsEnabled", 0, TokenType.Live, null, 0, OsuStatus.All, () => OsuMemoryData.GeneralData.ShowPlayingInterface ? 1 : 0);

            CreateLiveToken("songSelectionRankingType", RankingType.Unknown, TokenType.Live, null, RankingType.Unknown, OsuStatus.Listening, () => OsuMemoryData.SongSelectionScores.RankingType);
            CreateLiveToken("songSelectionTotalScores", 0, TokenType.Live, null, 0, OsuStatus.Listening, () => OsuMemoryData.SongSelectionScores.TotalScores);
            var cachedSongSelectionScores = "[]";
            CreateLiveToken("songSelectionScores", "[]", TokenType.Live, null, 0, OsuStatus.Listening, () => _lastStatus != OsuStatus.Listening ? cachedSongSelectionScores : JsonConvert.SerializeObject(OsuMemoryData.SongSelectionScores.Scores.Convert(_modParser), createJsonSerializerSettings("songSelectionScores")));
            CreateLiveToken("songSelectionMainPlayerScore", "{}", TokenType.Live, null, 0, OsuStatus.Listening, () => JsonConvert.SerializeObject(OsuMemoryData.SongSelectionScores.MainPlayerScore?.Convert(_modParser), createJsonSerializerSettings("songSelectionMainPlayerScore")));

            CreateLiveToken("banchoIsConnected", 0, TokenType.Live, null, 0, OsuStatus.All, () => OsuMemoryData.BanchoUser.IsLoggedIn ? 1 : 0);
            CreateLiveToken("banchoUsername", "", TokenType.Live, null, "", OsuStatus.All, () => OsuMemoryData.BanchoUser.Username);
            CreateLiveToken("banchoId", null, TokenType.Live, null, null, OsuStatus.All, () => OsuMemoryData.BanchoUser.UserId);
            CreateLiveToken("banchoStatus", null, TokenType.Live, null, null, OsuStatus.All, () => OsuMemoryData.BanchoUser.BanchoStatus);
            CreateLiveToken("banchoCountry", "", TokenType.Live, null, "", OsuStatus.All, () => OsuMemoryData.BanchoUser.UserCountry);
        }

        private JsonSerializerSettings createJsonSerializerSettings(string tokenName)
        {
            if (_tokenJsonSerializers.TryGetValue(tokenName, out var jsonSerializer))
            {
                return jsonSerializer;
            }

            return _tokenJsonSerializers[tokenName] = new JsonSerializerSettings
            {
                Error = (sender, args) =>
                {
                    _logger.Log($"Failed to serialize {tokenName} token data.", LogLevel.Debug);
                    _logger.Log(args, LogLevel.Trace);
                }
            };
        }

        private void UpdateLiveTokens(OsuStatus status)
        {
            _notAddingNewTokens.WaitOne();
            BulkTokenUpdateContext bulkTokenUpdateContext = null;
            if (IsMainProcessor)
                bulkTokenUpdateContext = TokensBulkUpdate.StartBulkUpdate(BulkTokenUpdateType.LiveTokens);

            foreach (var liveToken in _liveTokens)
            {
                try
                {
                    liveToken.Value.Update(status);
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
            bulkTokenUpdateContext?.Dispose();
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
            if (IsMemoryReaderActive && IsMainProcessor)
                SetSkinTokens();

            if (mapSearchResult.SearchArgs.EventType != OsuEventType.MapChange)
                return;

            _mods = mapSearchResult.Mods?.Mods ?? Mods.Omod;
            _playMode = mapSearchResult.PlayMode ?? PlayMode.Osu;
            var map = mapSearchResult.BeatmapsFound[0];
            _hitObjectCount = map.Circles + map.Sliders + map.Spinners;
            _bpmMultiplier = (_mods & Mods.Dt) != 0
                ? 1.5
                : (_mods & Mods.Ht) != 0
                    ? 0.75
                    : 1;
            if (!IsMainProcessor)
                return;

            _beatmapIdToken.Value = OsuMemoryData?.Beatmap.Id ?? map.MapId;
            _beatmapSetIdToken.Value = OsuMemoryData?.Beatmap.SetId ?? map.MapSetId;
            _beatmapRankedStatusToken.Value = OsuMemoryData?.Beatmap.Status ?? (short)-2; // Unknown
            _totalAudioTime.Value = OsuMemoryData?.GeneralData.TotalAudioTime ?? Convert.ToDouble(map.TotalTime);
            var ppCalculator = await mapSearchResult.GetPpCalculator(cancellationToken);
            _firstHitObjectTimeToken.Value = _firstHitObjectTime = ppCalculator?.FirstHitObjectTime() ?? 0d;
            _MapBreaksToken.Value = ppCalculator?.Breaks().ToList();
            _MapKiaiPointsToken.Value = ppCalculator?.KiaiPoints().ToList();
            var mapTimingPoints = ppCalculator?.TimingPoints().ToList();
            _MapTimingPointsToken.Value = mapTimingPoints;
            if (mapTimingPoints == null)
                _reversedMapTimingPoints = new List<TimingPoint>();
            else
            {
                mapTimingPoints = mapTimingPoints.ToList();
                mapTimingPoints.Reverse();
                _reversedMapTimingPoints = mapTimingPoints;
            }
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