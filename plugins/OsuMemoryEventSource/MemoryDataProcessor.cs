using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CollectionManager.DataTypes;
using Newtonsoft.Json;
using OsuMemoryDataProvider;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Interfaces;

namespace OsuMemoryEventSource
{
    public class MemoryDataProcessor : IHighFrequencyDataSender
    {
        private readonly string _songsFolderLocation;
        private readonly object _lockingObject = new object();
        private OsuStatus _lastStatus = OsuStatus.Null;
        RawMemoryDataProcessor _rawData = new RawMemoryDataProcessor();
        private List<IHighFrequencyDataHandler> _highFrequencyDataHandler;

        private List<OutputPattern> OutputPatterns = new List<OutputPattern>();

        public MemoryDataProcessor(string songsFolderLocation)
        {
            _songsFolderLocation = songsFolderLocation;
        }

        public void SetNewMap(MapSearchResult map)
        {
            lock (_lockingObject)
            {
                if ((map.Action & OsuStatus.ResultsScreen) != 0)
                    return;

                OutputPatterns.Clear();

                if (map.FoundBeatmaps)
                {
                    var mods = map.Mods?.Item1 ?? Mods.Omod;
                    _rawData.SetCurrentMap(map.BeatmapsFound[0], mods.Convert(),
                        map.BeatmapsFound[0].FullOsuFileLocation(_songsFolderLocation));

                    CopyPatterns(map.FormatedStrings);
                }
                else
                    _rawData.SetCurrentMap(null, OppaiSharp.Mods.NoMod, null);
            }
        }

        private void CopyPatterns(List<OutputPattern> patterns)
        {
            foreach (var f in patterns)
            {
                var newPattern = (OutputPattern)f.Clone();
                newPattern.Replacements = f.Replacements
                    .ToDictionary(entry => entry.Key, entry => entry.Value);
                OutputPatterns.Add(newPattern);
            }
        }

        public void Tick(OsuStatus status, IOsuMemoryReader reader)
        {
            lock (_lockingObject)
            {
                if (status != OsuStatus.Playing)
                {
                    if ((status & OsuStatus.ResultsScreen) == 0)
                    {//we're not playing or we haven't just finished playing - clear
                        ResetOutput();
                        _lastStatus = status;
                    }

                    return;
                }

                if (_lastStatus != OsuStatus.Playing)
                    Thread.Sleep(500);//Initial play delay

                _lastStatus = status;

                reader.GetPlayData(_rawData.Play);

                PrepareReplacements();

                SendData();
            }
        }

        private void ResetOutput()
        {
            SendData(true);
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

                    foreach (var r in replacements)
                    {
                        pattern.Replacements[r.Key] = r.Value;
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
            WriteToHandlers("SC-" + p.Name, value.Replace("\r", ""));

            //ingameOverlay part
            if (p.ShowInOsu)
            {
                var configName = "conf-SC-" + p.Name;
                var valueName = "value-SC-" + p.Name;
                var config = $"{p.XPosition} {p.YPosition} {p.Color.R} {p.Color.G} {p.Color.B} {p.FontName.Replace(' ', '/')} {p.FontSize}";
                WriteToHandlers(configName, config);
                WriteToHandlers(valueName, value);
            }
        }

        private readonly Dictionary<string, string> replacements = new Dictionary<string, string>();

        private void PrepareReplacements()
        {
            replacements["!acc!"] = string.Format("{0:0.00}", _rawData.Play.Acc);
            if (replacements["!acc!"] == "100.00")
                replacements["!acc!"] = "100";
            replacements["!300!"] = string.Format("{0}", _rawData.Play.C300);
            replacements["!100!"] = string.Format("{0}", _rawData.Play.C100);
            replacements["!50!"] = string.Format("{0}", _rawData.Play.C50);
            replacements["!miss!"] = string.Format("{0}", _rawData.Play.CMiss);
            double time = 0;
            if (_rawData.Play.Time != 0)
                time = _rawData.Play.Time / 1000d;
            replacements["!time!"] = string.Format("{0:0.00}", time);
            replacements["!combo!"] = string.Format("{0}", _rawData.Play.Combo);
            replacements["!CurrentMaxCombo!"] = string.Format("{0}", _rawData.Play.MaxCombo);
            replacements["!PpIfMapEndsNow!"] = Normalize(_rawData.PPIfBeatmapWouldEndNow(), "{0:0.00}");
            replacements["!PpIfRestFced!"] = Normalize(_rawData.PPIfRestFCed(), "{0:0.00}");
            replacements["!AccIfRestFced!"] = Normalize(_rawData.AccIfRestFCed(), "{0:0.00}");
            if (replacements["!AccIfRestFced!"] == "100.00")
                replacements["!AccIfRestFced!"] = "100";
        }
        private string Normalize(double pp, string pattern, bool zeroOnNaN = true)
        {
            if (double.IsNaN(pp))
            {
                if (zeroOnNaN)
                    pp = 0d;
                else
                    return "";
            }

            return string.Format(pattern, pp);
        }

        public void SetHighFrequencyDataHandlers(List<IHighFrequencyDataHandler> handlers)
        {
            _highFrequencyDataHandler = handlers;
        }
    }
}