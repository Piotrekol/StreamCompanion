using CollectionManager.DataTypes;
using CollectionManager.Enums;
using Newtonsoft.Json;
using OsuMemoryDataProvider;
using PpCalculator;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Enums;
using StreamCompanionTypes.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using StreamCompanionTypes;

namespace OsuMemoryEventSource
{
    public class MemoryDataProcessor : IHighFrequencyDataSender, ISettings, IDisposable
    {
        private readonly string _songsFolderLocation;
        private readonly object _lockingObject = new object();
        private OsuStatus _lastStatus = OsuStatus.Null;
        RawMemoryDataProcessor _rawData = new RawMemoryDataProcessor();
        private List<IHighFrequencyDataHandler> _highFrequencyDataHandler;

        private List<OutputPattern> OutputPatterns = new List<OutputPattern>();
        private ISettingsHandler _settings;

        private enum InterpolatedValueName
        {
            PpIfMapEndsNow,
            AimPpIfMapEndsNow,
            SpeedPpIfMapEndsNow,
            AccPpIfMapEndsNow,
            StrainPpIfMapEndsNow,
            PpIfRestFced,

        }
        private readonly Dictionary<InterpolatedValueName, InterpolatedValue> InterpolatedValues = new Dictionary<InterpolatedValueName, InterpolatedValue>();

        private Thread _workerThread;

        public MemoryDataProcessor(string songsFolderLocation, bool enablePpSmoothing = true)
        {
            _songsFolderLocation = songsFolderLocation;

            foreach (var v in (InterpolatedValueName[])Enum.GetValues(typeof(InterpolatedValueName)))
            {
                InterpolatedValues.Add(v, new InterpolatedValue(0.15));
            }


            ToggleSmoothing(enablePpSmoothing);

            InitLiveTokens();

            _workerThread = new Thread(ThreadWork);
            _workerThread.Start();
        }

        public void ThreadWork()
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
                    Thread.Sleep(11);
                }
            }
            catch (ThreadAbortException)
            {
            }
        }
        public void SetNewMap(MapSearchResult map)
        {
            lock (_lockingObject)
            {
                if ((map.Action & OsuStatus.ResultsScreen) != 0)
                    return;

                OutputPatterns.Clear();

                if (map.FoundBeatmaps && 
                    map.BeatmapsFound[0].IsValidBeatmap(_settings, out var mapLocation))
                {
                    var workingBeatmap = new ProcessorWorkingBeatmap(mapLocation);
                    var mods = map.Mods?.Item1 ?? Mods.Omod;

                    _rawData.SetCurrentMap(map.BeatmapsFound[0], mods, mapLocation,
                        (PlayMode)PpCalculatorHelpers.GetRulesetId(workingBeatmap.RulesetID, map.PlayMode.HasValue ? (int?)map.PlayMode : null));

                    CopyPatterns(map.FormatedStrings);
                }
                else
                    _rawData.SetCurrentMap(null, Mods.Omod, null, PlayMode.Osu);
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
        public void Tick(OsuStatus status, IOsuMemoryReader reader)
        {
            lock (_lockingObject)
            {

                PrepareTimeToken(reader.ReadPlayTime());

                if (status != OsuStatus.Playing)
                {
                    if (_clearLiveTokensAfterResultScreenExit && !_clearedLiveTokens && (status & OsuStatus.ResultsScreen) == 0)
                    {//we're not playing or we haven't just finished playing - clear
                        ResetOutput();
                        ResetTokens();
                        _lastStatus = status;
                        _clearedLiveTokens = true;
                    }


                    return;
                }

                if (_lastStatus != OsuStatus.Playing)
                {
                    Thread.Sleep(500);//Initial play delay
                    //var readGamemode = reader.ReadPlayedGameMode();
                    //var playMode = (PlayMode) (Enum.IsDefined(typeof(PlayMode), readGamemode) ? readGamemode : 0);
                    //_rawData.SetPlayMode(playMode);
                }

                _clearedLiveTokens = false;

                _lastStatus = status;

                reader.GetPlayData(_rawData.Play);

                PrepareLiveTokens();

                SendData();
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
                            ((TokenWithFormat)pattern.Replacements[r.Key]).Value = r.Value.Value;
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
                var config = $"{p.XPosition} {p.YPosition} {p.Color.R} {p.Color.G} {p.Color.B} {p.FontName.Replace(' ', '/')} {p.FontSize}";
                WriteToHandlers(configName, config);
                WriteToHandlers(valueName, value);
            }
        }

        private readonly Dictionary<string, TokenWithFormat> liveTokens = new Dictionary<string, TokenWithFormat>();

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

        private void InitLiveTokens()
        {
            liveTokens["acc"] = new TokenWithFormat(_rawData.Play.Acc, TokenType.Live, "{0:0.00}", 0d);
            liveTokens["300"] = new TokenWithFormat(_rawData.Play.C300, TokenType.Live, "{0}", (ushort)0);
            liveTokens["100"] = new TokenWithFormat(_rawData.Play.C100, TokenType.Live, "{0}", (ushort)0);
            liveTokens["50"] = new TokenWithFormat(_rawData.Play.C50, TokenType.Live, "{0}", (ushort)0);
            liveTokens["miss"] = new TokenWithFormat(_rawData.Play.CMiss, TokenType.Live, "{0}", (ushort)0);
            liveTokens["time"] = new TokenWithFormat(0d, TokenType.Live, "{0:0.00}", 0d);
            liveTokens["combo"] = new TokenWithFormat(_rawData.Play.Combo, TokenType.Live, "{0}", (ushort)0);
            liveTokens["CurrentMaxCombo"] = new TokenWithFormat(_rawData.Play.MaxCombo, TokenType.Live, "{0}", (ushort)0);
            liveTokens["PlayerHp"] = new TokenWithFormat(_rawData.Play.Hp, TokenType.Live, "{0:0.00}", 0d);

            liveTokens["PpIfMapEndsNow"] = new TokenWithFormat(InterpolatedValues[InterpolatedValueName.PpIfMapEndsNow].Current, TokenType.Live, "{0:0.00}", 0d);
            liveTokens["AimPpIfMapEndsNow"] = new TokenWithFormat(InterpolatedValues[InterpolatedValueName.AimPpIfMapEndsNow].Current, TokenType.Live, "{0:0.00}", 0d);
            liveTokens["SpeedPpIfMapEndsNow"] = new TokenWithFormat(InterpolatedValues[InterpolatedValueName.SpeedPpIfMapEndsNow].Current, TokenType.Live, "{0:0.00}", 0d);
            liveTokens["AccPpIfMapEndsNow"] = new TokenWithFormat(InterpolatedValues[InterpolatedValueName.AccPpIfMapEndsNow].Current, TokenType.Live, "{0:0.00}", 0d);
            liveTokens["StrainPpIfMapEndsNow"] = new TokenWithFormat(InterpolatedValues[InterpolatedValueName.StrainPpIfMapEndsNow].Current, TokenType.Live, "{0:0.00}", 0d);

            liveTokens["PpIfRestFced"] = new TokenWithFormat(InterpolatedValues[InterpolatedValueName.PpIfRestFced].Current, TokenType.Live, "{0:0.00}", 0d);
        }

        private void PrepareTimeToken(int readPlayTime)
        {
            double time = 0;
            if (readPlayTime != 0)
                time = readPlayTime / 1000d;
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
            liveTokens["CurrentMaxCombo"].Value = _rawData.Play.MaxCombo;
            liveTokens["PlayerHp"].Value = _rawData.Play.Hp;


            InterpolatedValues[InterpolatedValueName.PpIfMapEndsNow].Set(_rawData.PPIfBeatmapWouldEndNow());
            InterpolatedValues[InterpolatedValueName.AimPpIfMapEndsNow].Set(_rawData.AimPPIfBeatmapWouldEndNow);
            InterpolatedValues[InterpolatedValueName.SpeedPpIfMapEndsNow].Set(_rawData.SpeedPPIfBeatmapWouldEndNow);
            InterpolatedValues[InterpolatedValueName.AccPpIfMapEndsNow].Set(_rawData.AccPPIfBeatmapWouldEndNow);
            InterpolatedValues[InterpolatedValueName.StrainPpIfMapEndsNow].Set(_rawData.StrainPPIfBeatmapWouldEndNow);
            InterpolatedValues[InterpolatedValueName.PpIfRestFced].Set(_rawData.PPIfRestFCed());

            liveTokens["PpIfMapEndsNow"].Value = InterpolatedValues[InterpolatedValueName.PpIfMapEndsNow].Current;
            liveTokens["AimPpIfMapEndsNow"].Value = InterpolatedValues[InterpolatedValueName.AimPpIfMapEndsNow].Current;
            liveTokens["SpeedPpIfMapEndsNow"].Value = InterpolatedValues[InterpolatedValueName.SpeedPpIfMapEndsNow].Current;
            liveTokens["AccPpIfMapEndsNow"].Value = InterpolatedValues[InterpolatedValueName.AccPpIfMapEndsNow].Current;
            liveTokens["StrainPpIfMapEndsNow"].Value = InterpolatedValues[InterpolatedValueName.StrainPpIfMapEndsNow].Current;
            liveTokens["PpIfRestFced"].Value = InterpolatedValues[InterpolatedValueName.PpIfRestFced].Current;
        }

        public void SetHighFrequencyDataHandlers(List<IHighFrequencyDataHandler> handlers)
        {
            _highFrequencyDataHandler = handlers;
        }

        public void Dispose()
        {
            _workerThread?.Abort();
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