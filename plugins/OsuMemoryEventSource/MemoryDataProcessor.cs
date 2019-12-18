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
    public class MemoryDataProcessor : IDisposable
    {
        private readonly object _lockingObject = new object();
        private OsuStatus _lastStatus = OsuStatus.Null;
        LivePerformanceCalculator _rawData = new LivePerformanceCalculator();
        private List<IHighFrequencyDataHandler> _highFrequencyDataHandler;

        private BlockingCollection<OutputPattern> OutputPatterns = new BlockingCollection<OutputPattern>();
        private ISettingsHandler _settings;

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
                        PrepareLiveTokens();
                        SendData();

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
                    //_ppIfMapEndsNow.Tick();
                    //_ppIfRestFced.Tick();
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

                while (OutputPatterns.TryTake(out _)) { }

                if (map.FoundBeatmaps &&
                    map.BeatmapsFound[0].IsValidBeatmap(_settings, out var mapLocation) &&
                    (map.Action & (OsuStatus.Playing | OsuStatus.Watching)) != 0)
                {
                    var workingBeatmap = new ProcessorWorkingBeatmap(mapLocation);
                    var mods = map.Mods?.WorkingMods ?? "";

                    _rawData.SetCurrentMap(map.BeatmapsFound[0], mods, mapLocation,
                        (PlayMode)PpCalculatorHelpers.GetRulesetId(workingBeatmap.RulesetID, map.PlayMode.HasValue ? (int?)map.PlayMode : null));

                    CopyPatterns(map.FormatedStrings);
                }
                else
                    _rawData.SetCurrentMap(null, "", null, PlayMode.Osu);
            }
        }

        private void CopyPatterns(List<OutputPattern> patterns)
        {
            foreach (var f in patterns)
            {
                var newPattern = (OutputPattern)f.Clone();
                newPattern.Replacements = new Tokens();
                foreach (var r in f.Replacements)
                {
                    newPattern.Replacements.Add(r.Key, r.Value.Clone());
                }
                OutputPatterns.Add(newPattern);
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

                liveTokens["status"].Value = _lastStatus;

                if (status != OsuStatus.Playing)
                {
                    if (_clearLiveTokensAfterResultScreenExit && !_clearedLiveTokens && (status & OsuStatus.ResultsScreen) == 0)
                    {//we're not playing or we haven't just finished playing - clear
                        ResetOutput();
                        ResetTokens();
                        _lastStatus = status;
                        _clearedLiveTokens = true;
                    }

                    _rawData.PlayTime = reader.ReadPlayTime();
                    PrepareTimeToken();
                    return;
                }

                if (_lastStatus != OsuStatus.Playing)
                {
                    Thread.Sleep(500);//Initial play delay
                }

                _clearedLiveTokens = false;
                _lastStatus = status;


                reader.GetPlayData(_rawData.Play);
                _rawData.HitErrors = reader.HitErrors() ?? new List<int>();

                _rawData.PlayTime = reader.ReadPlayTime();
                PrepareTimeToken();

                _tokenTick?.Set();
            }
        }

        private void ResetOutput()
        {
            SendData(true);
        }

        private void ResetTokens()
        {
            foreach (var tokenkv in liveTokens)
            {
                tokenkv.Value.Reset();
            }
        }
        private void SendData(bool emptyPatterns = false)
        {
            if (OutputPatterns != null && OutputPatterns.Count > 0)
            {
                Dictionary<string, string> output = new Dictionary<string, string>();

                foreach (var pattern in OutputPatterns)
                {
                    if (!pattern.IsMemoryFormat) continue;

                    if (emptyPatterns)
                    {
                        SetOutput(pattern, "   ");
                        output[pattern.Name] = "   ";
                        continue;
                    }

                    foreach (var r in liveTokens)
                    {
                        if (pattern.Replacements.ContainsKey(r.Key))
                            pattern.Replacements[r.Key].Value = r.Value.Value;
                        else
                            pattern.Replacements.Add(r.Key, r.Value);
                    }
                    output[pattern.Name] = pattern.GetFormatedPattern();
                    SetOutput(pattern, output[pattern.Name]);
                }
                var json = JsonConvert.SerializeObject(output);
                _highFrequencyDataHandler.ForEach(h => h.Handle(json));
            }
        }

        private void WriteToHandlers(string name, string value)
        {
            _highFrequencyDataHandler.ForEach(h => h.Handle(name, value));
        }
        private void SetOutput(OutputPattern p, string value)
        {
            //Standard output
            WriteToHandlers(p.Name, value.Replace("\r", ""));

            //ingameOverlay part
            if (p.ShowInOsu)
            {
                var configName = "conf-" + p.Name;
                var valueName = "value-" + p.Name;
                var config = $"{p.XPosition} {p.YPosition} {p.Color.R} {p.Color.G} {p.Color.B} {p.Color.A} {p.FontName.Replace(' ', '/')} {p.FontSize} {p.Aligment}";
                WriteToHandlers(configName, config);
                WriteToHandlers(valueName, value);
            }
        }

        private readonly Dictionary<string, Token> liveTokens = new Dictionary<string, Token>();

        public Tokens Tokens
        {
            get
            {
                var ret = new Tokens("Live tokens");
                foreach (var r in liveTokens)
                {
                    ret.Add(r.Key, r.Value);
                }

                return ret;
            }
        }

        private Token HitErrors;
        private void InitLiveTokens()
        {
            liveTokens["status"] = _tokenSetter("status", OsuStatus.Null, TokenType.Live, "", OsuStatus.Null);
            liveTokens["acc"] = _tokenSetter("acc", _rawData.Play.Acc, TokenType.Live, "{0:0.00}", 0d);
            liveTokens["300"] = _tokenSetter("300", _rawData.Play.C300, TokenType.Live, "{0}", (ushort)0);
            liveTokens["100"] = _tokenSetter("100", _rawData.Play.C100, TokenType.Live, "{0}", (ushort)0);
            liveTokens["50"] = _tokenSetter("50", _rawData.Play.C50, TokenType.Live, "{0}", (ushort)0);
            liveTokens["miss"] = _tokenSetter("miss", _rawData.Play.CMiss, TokenType.Live, "{0}", (ushort)0);
            liveTokens["time"] = _tokenSetter("time", 0d, TokenType.Live, "{0:0.00}", 0d);
            liveTokens["combo"] = _tokenSetter("combo", _rawData.Play.Combo, TokenType.Live, "{0}", (ushort)0);
            liveTokens["score"] = _tokenSetter("score", _rawData.Play.Score, TokenType.Live, "{0}", 0);
            liveTokens["CurrentMaxCombo"] = _tokenSetter("CurrentMaxCombo", _rawData.Play.MaxCombo, TokenType.Live, "{0}", (ushort)0);
            liveTokens["PlayerHp"] = _tokenSetter("PlayerHp", _rawData.Play.Hp, TokenType.Live, "{0:0.00}", 0d);
            liveTokens["PpIfMapEndsNow"] = _tokenSetter("PpIfMapEndsNow", InterpolatedValues[InterpolatedValueName.PpIfMapEndsNow].Current, TokenType.Live, "{0:0.00}", 0d);
            liveTokens["AimPpIfMapEndsNow"] = _tokenSetter("AimPpIfMapEndsNow", InterpolatedValues[InterpolatedValueName.AimPpIfMapEndsNow].Current, TokenType.Live, "{0:0.00}", 0d);
            liveTokens["SpeedPpIfMapEndsNow"] = _tokenSetter("SpeedPpIfMapEndsNow", InterpolatedValues[InterpolatedValueName.SpeedPpIfMapEndsNow].Current, TokenType.Live, "{0:0.00}", 0d);
            liveTokens["AccPpIfMapEndsNow"] = _tokenSetter("AccPpIfMapEndsNow", InterpolatedValues[InterpolatedValueName.AccPpIfMapEndsNow].Current, TokenType.Live, "{0:0.00}", 0d);
            liveTokens["StrainPpIfMapEndsNow"] = _tokenSetter("StrainPpIfMapEndsNow", InterpolatedValues[InterpolatedValueName.StrainPpIfMapEndsNow].Current, TokenType.Live, "{0:0.00}", 0d);
            liveTokens["PpIfRestFced"] = _tokenSetter("PpIfRestFced", InterpolatedValues[InterpolatedValueName.PpIfRestFced].Current, TokenType.Live, "{0:0.00}", 0d);
            liveTokens["NoChokePp"] = _tokenSetter("NoChokePp", InterpolatedValues[InterpolatedValueName.NoChokePp].Current, TokenType.Live, "{0:0.00}", 0d);
            liveTokens["UnstableRate"] = _tokenSetter("UnstableRate", InterpolatedValues[InterpolatedValueName.UnstableRate].Current, TokenType.Live, "{0:0.000}", 0d);
            HitErrors = _tokenSetter("HitErrors", new List<int>(), TokenType.Live | TokenType.Hidden, defaultValue: new List<int>());
        }

        private void PrepareTimeToken()
        {
            double time = 0;
            if (_rawData.PlayTime != 0)
                time = _rawData.PlayTime / 1000d;
            liveTokens["time"].Value = time;
        }
        private void PrepareLiveTokens()
        {
            liveTokens["acc"].Value = _rawData.Play.Acc;
            liveTokens["300"].Value = _rawData.Play.C300;
            liveTokens["100"].Value = _rawData.Play.C100;
            liveTokens["50"].Value = _rawData.Play.C50;
            liveTokens["miss"].Value = _rawData.Play.CMiss;

            liveTokens["combo"].Value = _rawData.Play.Combo;
            liveTokens["score"].Value = _rawData.Play.Score;

            liveTokens["CurrentMaxCombo"].Value = _rawData.Play.MaxCombo;
            liveTokens["PlayerHp"].Value = _rawData.Play.Hp;

            InterpolatedValues[InterpolatedValueName.PpIfMapEndsNow].Set(_rawData.PPIfBeatmapWouldEndNow());
            InterpolatedValues[InterpolatedValueName.AimPpIfMapEndsNow].Set(_rawData.AimPPIfBeatmapWouldEndNow);
            InterpolatedValues[InterpolatedValueName.SpeedPpIfMapEndsNow].Set(_rawData.SpeedPPIfBeatmapWouldEndNow);
            InterpolatedValues[InterpolatedValueName.AccPpIfMapEndsNow].Set(_rawData.AccPPIfBeatmapWouldEndNow);
            InterpolatedValues[InterpolatedValueName.StrainPpIfMapEndsNow].Set(_rawData.StrainPPIfBeatmapWouldEndNow);
            InterpolatedValues[InterpolatedValueName.PpIfRestFced].Set(_rawData.PPIfRestFCed());
            InterpolatedValues[InterpolatedValueName.NoChokePp].Set(_rawData.NoChokePp());
            InterpolatedValues[InterpolatedValueName.UnstableRate].Set(UnstableRate(_rawData.HitErrors));
            HitErrors.Value = HitErrors;

            liveTokens["PpIfMapEndsNow"].Value = InterpolatedValues[InterpolatedValueName.PpIfMapEndsNow].Current;
            liveTokens["AimPpIfMapEndsNow"].Value = InterpolatedValues[InterpolatedValueName.AimPpIfMapEndsNow].Current;
            liveTokens["SpeedPpIfMapEndsNow"].Value = InterpolatedValues[InterpolatedValueName.SpeedPpIfMapEndsNow].Current;
            liveTokens["AccPpIfMapEndsNow"].Value = InterpolatedValues[InterpolatedValueName.AccPpIfMapEndsNow].Current;
            liveTokens["StrainPpIfMapEndsNow"].Value = InterpolatedValues[InterpolatedValueName.StrainPpIfMapEndsNow].Current;
            liveTokens["PpIfRestFced"].Value = InterpolatedValues[InterpolatedValueName.PpIfRestFced].Current;
            liveTokens["NoChokePp"].Value = InterpolatedValues[InterpolatedValueName.NoChokePp].Current;
            liveTokens["UnstableRate"].Value = InterpolatedValues[InterpolatedValueName.UnstableRate].Current;

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

        public void SetHighFrequencyDataHandlers(List<IHighFrequencyDataHandler> handlers)
        {
            _highFrequencyDataHandler = handlers;
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