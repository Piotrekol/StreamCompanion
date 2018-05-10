using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using osu_StreamCompanion.Code.Core;
using osu_StreamCompanion.Code.Core.DataTypes;
using osu_StreamCompanion.Code.Interfaces;
using osu_StreamCompanion.Code.Misc;

namespace osu_StreamCompanion.Code.Modules.MapDataParsers.Parser1
{
    public class MapDataParser : IModule, IMapDataParser, ISettingsProvider, IDisposable
    {
        private readonly SettingNames _names = SettingNames.Instance;

        private readonly BindingList<OutputPattern> _patterns = new BindingList<OutputPattern>();
        private Settings _settings;
        private readonly object _lockingObject = new object();
        private ParserSettings _parserSettings = null;
        private ILogger _logger;

        public bool Started { get; set; }
        public void Start(ILogger logger)
        {
            Started = true;
            _logger = logger;
            Load();
            if (_patterns.Count == 0)
            {
                _patterns.Add(new OutputPattern()
                {
                    Name = "np_all",
                    Pattern = "!MapArtistTitle! !MapDiff! CS:!cs! AR:!ar! OD:!od! HP:!hp!",
                    SaveEvent = OsuStatus.All
                });
                _patterns.Add(new OutputPattern()
                {
                    Name = "np_playing_details",
                    Pattern = "CS:!cs! AR:!ar! OD:!od! HP:!hp!",
                    SaveEvent = OsuStatus.Playing
                });
                _patterns.Add(new OutputPattern()
                {
                    Name = "np_playing_DL",
                    Pattern = "!dl!",
                    SaveEvent = OsuStatus.Playing
                });
                _patterns.Add(new OutputPattern()
                {
                    Name = "livepp_hits",
                    Pattern = "!100!x100 !50!x50 !miss!xMiss",
                    SaveEvent = OsuStatus.Playing
                });
                _patterns.Add(new OutputPattern()
                {
                    Name = "livepp_current_pp",
                    Pattern = "!PpIfMapEndsNow!",
                    SaveEvent = OsuStatus.Playing
                });
                _patterns.Add(new OutputPattern()
                {
                    Name = "current_mods",
                    Pattern = "!mods!",
                    SaveEvent = OsuStatus.Playing
                });
            }

            _patterns.ListChanged += PatternsOnListChanged;
        }

        private void PatternsOnListChanged(object sender, ListChangedEventArgs listChangedEventArgs)
        {
            Save();
        }

        public List<OutputPattern> GetFormatedPatterns(Dictionary<string, string> replacements, OsuStatus status)
        {
            List<OutputPattern> ret = null;

            if (replacements != null)
            {
                ret = new List<OutputPattern>();
                foreach (var p in _patterns)
                {
                    var newPattern = (OutputPattern)p.Clone();
                    newPattern.Replacements = replacements;
                    ret.Add(newPattern);
                }
                if (_parserSettings != null && !_parserSettings.IsDisposed)
                    _parserSettings.SetPreview(replacements);
            }
            return ret;
        }
        public string FormatMapString(string toFormat, Dictionary<string, string> replacements)
        {
            foreach (var r in replacements)
            {
                toFormat = toFormat.Replace(r.Key, r.Value);
            }
            return toFormat;
        }
        public void SetSettingsHandle(Settings settings)
        {
            _settings = settings;
        }

        public string SettingGroup { get; } = "Output patterns";
        public void Free()
        {
            _parserSettings.Dispose();
        }

        public UserControl GetUiSettings()
        {
            if (_parserSettings == null || _parserSettings.IsDisposed)
            {
                _parserSettings = new ParserSettings(_settings);
                _parserSettings.SetPatterns(_patterns);
            }
            return _parserSettings;
        }
        //TODO: this is getting stupid. Consider changing how config values are stored/retrieved/updated.
        private void Save()
        {
            lock (_lockingObject)
            {
                List<string> filenames = new List<string>();
                List<string> patterns = new List<string>();
                List<int> saveEvents = new List<int>();
                List<int> patternShowInOsu = new List<int>();
                List<int> patternX = new List<int>();
                List<int> patternY = new List<int>();
                List<string> patternColor = new List<string>();
                List<string> patternFontName = new List<string>();
                List<int> patternFontSize = new List<int>();

                foreach (var f in _patterns)
                {
                    filenames.Add(f.Name);
                    patterns.Add(f.Pattern);
                    saveEvents.Add((int)f.SaveEvent);
                    patternShowInOsu.Add(f.ShowInOsu ? 1 : 0);
                    patternX.Add(f.XPosition);
                    patternY.Add(f.YPosition);
                    patternColor.Add($"{f.Color.R};{f.Color.G};{f.Color.B}");
                    patternFontName.Add(f.FontName);
                    patternFontSize.Add(f.FontSize);
                }

                _settings.Add(_names.PatternFileNames.Name, filenames);
                _settings.Add(_names.Patterns.Name, patterns);
                _settings.Add(_names.saveEvents.Name, saveEvents);
                _settings.Add(_names.PatternShowInOsu.Name, patternShowInOsu);
                _settings.Add(_names.PatternX.Name, patternX);
                _settings.Add(_names.PatternY.Name, patternY);
                _settings.Add(_names.PatternColor.Name, patternColor);
                _settings.Add(_names.PatternFontName.Name, patternFontName);
                _settings.Add(_names.PatternFontSize.Name, patternFontSize);

            }
            _settings.Save();
        }
        private void Load()
        {
            List<string> filenames = _settings.Get(_names.PatternFileNames.Name);
            List<string> patterns = _settings.Get(_names.Patterns.Name);
            List<int> saveEvents = _settings.Geti(_names.saveEvents.Name);

            List<int> patternShowInOsu = _settings.Geti(_names.PatternShowInOsu.Name);
            List<int> patternX = _settings.Geti(_names.PatternX.Name);
            List<int> patternY = _settings.Geti(_names.PatternY.Name);
            List<string> patternColor = _settings.Get(_names.PatternColor.Name);
            List<string> patternFontName = _settings.Get(_names.PatternFontName.Name);
            List<int> patternFontSize = _settings.Geti(_names.PatternFontSize.Name);

            var requiredCount = Math.Max(filenames.Count, Math.Max(patterns.Count, Math.Max(patternShowInOsu.Count, saveEvents.Count)));
            requiredCount = Math.Max(requiredCount, Math.Max(patternX.Count, Math.Max(patternY.Count, Math.Max(patternColor.Count, patternFontName.Count))));
            requiredCount = Math.Max(requiredCount, patternFontSize.Count);
            var lastRanVersion = _settings.Get<string>(_names.LastRunVersion);
            if (Helpers.Helpers.GetDateFromVersionString(lastRanVersion) <
                Helpers.Helpers.GetDateFromVersionString("v180501.16") && patternShowInOsu.Count == 0)
            {//New setting entrys added - fill with defaults.
                while (patternShowInOsu.Count < requiredCount)
                {
                    patternShowInOsu.Add(0);
                    patternX.Add(200);
                    patternY.Add(200);
                    patternColor.Add("255;0;0");
                    patternFontName.Add("Arial");
                    patternFontSize.Add(12);
                }
            }


            if (filenames.Count != requiredCount || filenames.Count != requiredCount ||
                patternShowInOsu.Count != requiredCount || patternX.Count != requiredCount ||
                patternY.Count != requiredCount || patternColor.Count != requiredCount ||
                patternFontName.Count != requiredCount || patternFontSize.Count != requiredCount)
            {
                string _filenames = _settings.GetRaw(_names.PatternFileNames.Name);
                string _patterns = _settings.GetRaw(_names.Patterns.Name);
                string _saveEvents = _settings.GetRaw(_names.saveEvents.Name);
                _logger?.Log("User patterns are broken: {0} {1} {2} \n{3}\n{4}\n{5}", LogLevel.Error,
                    filenames.Count.ToString(), patterns.Count.ToString(), saveEvents.Count.ToString(),
                    _filenames, _patterns, _saveEvents);
                var userResponse = MessageBox.Show("Your output patterns are broken and could not be loaded successfully" + Environment.NewLine +
                    "I can load them with missing data or just reset to default patterns." + Environment.NewLine +
                    "Do you want to try to load them?"
                    , "osu!StreamCompanion - User action required!", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk);
                if (userResponse == DialogResult.No)
                    return;

                var toFixCount = requiredCount * 9 - (filenames.Count + patterns.Count + saveEvents.Count + patternShowInOsu.Count + patternX.Count + patternY.Count + patternColor.Count + patternFontName.Count + patternFontSize.Count);
                while (filenames.Count < requiredCount)
                    filenames.Add(ParserSettings.GenerateFileName(filenames, "Recovered_"));
                while (patterns.Count < requiredCount)
                    patterns.Add("Recovered");
                while (saveEvents.Count < requiredCount)
                    saveEvents.Add((int)OsuStatus.All);
                while (patternShowInOsu.Count < requiredCount)
                    patternShowInOsu.Add(0);
                while (patternX.Count < requiredCount)
                    patternX.Add(200);
                while (patternY.Count < requiredCount)
                    patternY.Add(200);
                while (patternColor.Count < requiredCount)
                    patternColor.Add("255;0;0");
                while (patternFontName.Count < requiredCount)
                    patternFontName.Add("Arial");
                while (patternFontSize.Count < requiredCount)
                    patternFontSize.Add(12);

                MessageBox.Show("Finished recovering patterns" + Environment.NewLine +
                                toFixCount + " entrys were missing" + Environment.NewLine +
                                "Go to settings and check your patterns!!!", "osu!StreamCompanion - Done", MessageBoxButtons.OK);
            }
            lock (_lockingObject)
            {
                _patterns.Clear();
                for (int i = 0; i < filenames.Count; i++)
                {
                    //Converting from ealier versions                    
                    if (saveEvents[i] == 27)
                        saveEvents[i] = (int)OsuStatus.All;
                    if (filenames[i].EndsWith(".txt"))
                        filenames[i] = filenames[i].Substring(0, filenames[i].LastIndexOf(".txt", StringComparison.Ordinal));

                    var c = patternColor[i].Split(';').Select(Int32.Parse).ToList();
                    _patterns.Add(new OutputPattern()
                    {
                        Name = filenames[i],
                        Pattern = patterns[i],
                        SaveEvent = (OsuStatus)saveEvents[i],
                        ShowInOsu = patternShowInOsu[i] == 1,
                        XPosition = patternX[i],
                        YPosition = patternY[i],
                        Color = Color.FromArgb(c[0], c[1], c[2]),
                        FontName = patternFontName[i],
                        FontSize = patternFontSize[i]
                    });
                }
            }

        }

        public void Dispose()
        {
            _parserSettings?.Dispose();
            Save();
        }
    }
}