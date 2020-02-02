using CollectionManager.Enums;
using Newtonsoft.Json;
using OsuMemoryDataProvider;
using PpCalculator;
using StreamCompanionTypes;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Enums;
using StreamCompanionTypes.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OsuMemoryEventSource
{
    public class TokensUpdateEventArgs
    {
        public OsuStatus status { get; set; }
    }
    public class MemoryDataProcessor : IDisposable
    {
        private readonly object _lockingObject = new object();
        private OsuStatus _lastStatus = OsuStatus.Null;
        private LivePerformanceCalculator _rawData = new LivePerformanceCalculator();
        private ISettingsHandler _settings;
        private readonly Dictionary<string, LiveToken> _liveTokens = new Dictionary<string, LiveToken>();
        private Tokens.TokenSetter _tokenSetter => OsuMemoryEventSourceBase.TokenSetter;
        private enum InterpolatedValueName
        {
            PpIfMapEndsNow,
            AimPpIfMapEndsNow,
            SpeedPpIfMapEndsNow,
            AccPpIfMapEndsNow,
            StrainPpIfMapEndsNow,
            PpIfRestFced,
            NoChokePp,
            UnstableRate,

        }
        private readonly Dictionary<InterpolatedValueName, InterpolatedValue> InterpolatedValues = new Dictionary<InterpolatedValueName, InterpolatedValue>();
        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        public EventHandler<OsuStatus> TokensUpdated { get; set; }
        public MemoryDataProcessor(bool enablePpSmoothing = true)
        {
            foreach (var v in (InterpolatedValueName[])Enum.GetValues(typeof(InterpolatedValueName)))
            {
                InterpolatedValues.Add(v, new InterpolatedValue(0.15));
            }

            ToggleSmoothing(enablePpSmoothing);

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
                        UpdateLiveTokens();

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
                    map.BeatmapsFound[0].IsValidBeatmap(_settings, out var mapLocation) &&
                    (map.Action & (OsuStatus.Playing | OsuStatus.Watching)) != 0)
                {
                    var workingBeatmap = new ProcessorWorkingBeatmap(mapLocation);
                    var mods = map.Mods?.WorkingMods ?? "";

                    _rawData.SetCurrentMap(map.BeatmapsFound[0], mods, mapLocation,
                        (PlayMode)PpCalculatorHelpers.GetRulesetId(workingBeatmap.RulesetID, map.PlayMode.HasValue ? (int?)map.PlayMode : null));
                }
                else
                    _rawData.SetCurrentMap(null, "", null, PlayMode.Osu);
            }
        }

        

        private bool _clearedLiveTokens = false;
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
                    if (_clearLiveTokensAfterResultScreenExit && !_clearedLiveTokens && (status & OsuStatus.ResultsScreen) == 0)
                    {//we're not playing or we haven't just finished playing - clear
                        ResetTokens();
                        _lastStatus = status;
                        _clearedLiveTokens = true;
                    }

                    _rawData.PlayTime = reader.ReadPlayTime();
                    _liveTokens["time"].Update();
                    _lastStatus = status;
                    _tokensUpdated();
                    return;
                }

                if (_lastStatus != OsuStatus.Playing)
                {
                    Thread.Sleep(500);//Initial play delay
                }

                _clearedLiveTokens = false;

                reader.GetPlayData(_rawData.Play);
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

        private IToken HitErrors;
        private void InitLiveTokens()
        {
            _liveTokens["status"] = new LiveToken(_tokenSetter("status", OsuStatus.Null, TokenType.Live, "", OsuStatus.Null), null);
            _liveTokens["acc"] = new LiveToken(_tokenSetter("acc", _rawData.Play.Acc, TokenType.Live, "{0:0.00}", 0d, OsuStatus.Playing), () => _rawData.Play.Acc);
            _liveTokens["300"] = new LiveToken(_tokenSetter("300", _rawData.Play.C300, TokenType.Live, "{0}", (ushort)0, OsuStatus.Playing), () => _rawData.Play.C300);
            _liveTokens["100"] = new LiveToken(_tokenSetter("100", _rawData.Play.C100, TokenType.Live, "{0}", (ushort)0, OsuStatus.Playing), () => _rawData.Play.C100);
            _liveTokens["50"] = new LiveToken(_tokenSetter("50", _rawData.Play.C50, TokenType.Live, "{0}", (ushort)0, OsuStatus.Playing), () => _rawData.Play.C50);
            _liveTokens["miss"] = new LiveToken(_tokenSetter("miss", _rawData.Play.CMiss, TokenType.Live, "{0}", (ushort)0, OsuStatus.Playing), () => _rawData.Play.CMiss);
            _liveTokens["time"] = new LiveToken(_tokenSetter("time", 0d, TokenType.Live, "{0:0.00}", 0d), () =>
            {
                if (_rawData.PlayTime != 0)
                    return _rawData.PlayTime / 1000d;
                return 0d;
            });
            _liveTokens["combo"] = new LiveToken(_tokenSetter("combo", _rawData.Play.Combo, TokenType.Live, "{0}", (ushort)0, OsuStatus.Playing), () => _rawData.Play.Combo);
            _liveTokens["score"] = new LiveToken(_tokenSetter("score", _rawData.Play.Score, TokenType.Live, "{0}", 0, OsuStatus.Playing), () => _rawData.Play.Score);
            _liveTokens["CurrentMaxCombo"] = new LiveToken(_tokenSetter("CurrentMaxCombo", _rawData.Play.MaxCombo, TokenType.Live, "{0}", (ushort)0, OsuStatus.Playing), () => _rawData.Play.MaxCombo);
            _liveTokens["PlayerHp"] = new LiveToken(_tokenSetter("PlayerHp", _rawData.Play.Hp, TokenType.Live, "{0:0.00}", 0d, OsuStatus.Playing), () => _rawData.Play.Hp);

            _liveTokens["PpIfMapEndsNow"] = new LiveToken(_tokenSetter("PpIfMapEndsNow", InterpolatedValues[InterpolatedValueName.PpIfMapEndsNow].Current, TokenType.Live, "{0:0.00}", 0d, OsuStatus.Playing), () =>
            {
                InterpolatedValues[InterpolatedValueName.PpIfMapEndsNow].Set(_rawData.PPIfBeatmapWouldEndNow());
                return InterpolatedValues[InterpolatedValueName.PpIfMapEndsNow].Current;
            });
            _liveTokens["AimPpIfMapEndsNow"] = new LiveToken(_tokenSetter("AimPpIfMapEndsNow", InterpolatedValues[InterpolatedValueName.AimPpIfMapEndsNow].Current, TokenType.Live, "{0:0.00}", 0d, OsuStatus.Playing), () =>
            {
                InterpolatedValues[InterpolatedValueName.AimPpIfMapEndsNow].Set(_rawData.AimPPIfBeatmapWouldEndNow);
                return InterpolatedValues[InterpolatedValueName.AimPpIfMapEndsNow].Current;
            });
            _liveTokens["SpeedPpIfMapEndsNow"] = new LiveToken(_tokenSetter("SpeedPpIfMapEndsNow", InterpolatedValues[InterpolatedValueName.SpeedPpIfMapEndsNow].Current, TokenType.Live, "{0:0.00}", 0d, OsuStatus.Playing), () =>
            {
                InterpolatedValues[InterpolatedValueName.SpeedPpIfMapEndsNow].Set(_rawData.SpeedPPIfBeatmapWouldEndNow);
                return InterpolatedValues[InterpolatedValueName.SpeedPpIfMapEndsNow].Current;
            });
            _liveTokens["AccPpIfMapEndsNow"] = new LiveToken(_tokenSetter("AccPpIfMapEndsNow", InterpolatedValues[InterpolatedValueName.AccPpIfMapEndsNow].Current, TokenType.Live, "{0:0.00}", 0d, OsuStatus.Playing), () =>
            {
                InterpolatedValues[InterpolatedValueName.AccPpIfMapEndsNow].Set(_rawData.AccPPIfBeatmapWouldEndNow);
                return InterpolatedValues[InterpolatedValueName.AccPpIfMapEndsNow].Current;
            });
            _liveTokens["StrainPpIfMapEndsNow"] = new LiveToken(_tokenSetter("StrainPpIfMapEndsNow", InterpolatedValues[InterpolatedValueName.StrainPpIfMapEndsNow].Current, TokenType.Live, "{0:0.00}", 0d, OsuStatus.Playing), () =>
            {
                InterpolatedValues[InterpolatedValueName.StrainPpIfMapEndsNow].Set(_rawData.StrainPPIfBeatmapWouldEndNow);
                return InterpolatedValues[InterpolatedValueName.StrainPpIfMapEndsNow].Current;
            });
            _liveTokens["PpIfRestFced"] = new LiveToken(_tokenSetter("PpIfRestFced", InterpolatedValues[InterpolatedValueName.PpIfRestFced].Current, TokenType.Live, "{0:0.00}", 0d, OsuStatus.Playing), () =>
            {
                InterpolatedValues[InterpolatedValueName.PpIfRestFced].Set(_rawData.PPIfRestFCed());
                return InterpolatedValues[InterpolatedValueName.PpIfRestFced].Current;
            });
            _liveTokens["NoChokePp"] = new LiveToken(_tokenSetter("NoChokePp", InterpolatedValues[InterpolatedValueName.NoChokePp].Current, TokenType.Live, "{0:0.00}", 0d, OsuStatus.Playing), () =>
            {
                InterpolatedValues[InterpolatedValueName.NoChokePp].Set(_rawData.NoChokePp());
                return InterpolatedValues[InterpolatedValueName.NoChokePp].Current;
            });
            _liveTokens["UnstableRate"] = new LiveToken(_tokenSetter("UnstableRate", InterpolatedValues[InterpolatedValueName.UnstableRate].Current, TokenType.Live, "{0:0.000}", 0d, OsuStatus.Playing), () =>
            {
                InterpolatedValues[InterpolatedValueName.UnstableRate].Set(UnstableRate(_rawData.HitErrors));
                return InterpolatedValues[InterpolatedValueName.UnstableRate].Current;
            });

            HitErrors = _tokenSetter("HitErrors", new List<int>(), TokenType.Live | TokenType.Hidden, defaultValue: new List<int>(), whitelist: OsuStatus.Playing);
        }

        private void UpdateLiveTokens()
        {
            foreach (var liveToken in _liveTokens)
            {
                liveToken.Value.Update();
            }
            _tokensUpdated();
        }

        private double UnstableRate(List<int> hitErrors)
        {
            if (hitErrors.Count == 0 || hitErrors.Any(x => x > 10000))
                return 0;

            double sum = hitErrors.Sum();

            double avarage = sum / hitErrors.Count;
            double variance = 0;

            foreach (var hit in hitErrors)
            {
                variance += Math.Pow(hit - avarage, 2);
            }

            return Math.Sqrt(variance / hitErrors.Count) * 10;
        }


        public void Dispose()
        {
            cancellationTokenSource.Cancel();
            _settings.SettingUpdated -= SettingUpdated;
        }

        public void ToggleSmoothing(bool enable)
        {
            var speed = enable ? 0.15d : 1d;

            foreach (var v in InterpolatedValues)
            {
                v.Value.ChangeSpeed(speed);
            }
        }

        public void SetSettingsHandle(ISettingsHandler settings)
        {
            _settings = settings;
            _settings.SettingUpdated += SettingUpdated;
            _clearLiveTokensAfterResultScreenExit = _settings.Get<bool>(Helpers.ClearLiveTokensAfterResultScreenExit);
        }

        private bool _clearLiveTokensAfterResultScreenExit;
        private void SettingUpdated(object sender, SettingUpdated e)
        {
            if (e.Name == Helpers.ClearLiveTokensAfterResultScreenExit.Name)
            {
                _clearLiveTokensAfterResultScreenExit = _settings.Get<bool>(Helpers.ClearLiveTokensAfterResultScreenExit);
            }
        }
    }
}