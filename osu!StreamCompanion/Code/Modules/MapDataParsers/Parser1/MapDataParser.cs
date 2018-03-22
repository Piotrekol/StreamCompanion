using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using osu_StreamCompanion.Code.Core;
using osu_StreamCompanion.Code.Core.DataTypes;
using osu_StreamCompanion.Code.Interfaces;
using osu_StreamCompanion.Code.Misc;

namespace osu_StreamCompanion.Code.Modules.MapDataParsers.Parser1
{
    public class MapDataParser : IModule, IMapDataParser, ISettingsProvider
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

        public string SettingGroup { get; } = "Map formating";
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


        private void Save()
        {
            lock (_lockingObject)
            {//do saving...
                List<string> filenames = new List<string>();
                List<string> Patterns = new List<string>();
                List<int> saveEvents = new List<int>();
                foreach (var f in _patterns)
                {
                    filenames.Add(f.Name);
                    Patterns.Add(f.Pattern);
                    saveEvents.Add((int)f.SaveEvent);
                }

                _settings.Add(_names.PatternFileNames.Name, filenames);
                _settings.Add(_names.Patterns.Name, Patterns);
                _settings.Add(_names.saveEvents.Name, saveEvents);
            }
            _settings.Save();
        }
        private void Load()
        {
            List<string> filenames = _settings.Get(_names.PatternFileNames.Name);
            List<string> patterns = _settings.Get(_names.Patterns.Name);
            List<int> saveEvents = _settings.Geti(_names.saveEvents.Name);
            if (filenames.Count != patterns.Count || filenames.Count != saveEvents.Count)
            {
                _logger?.Log("Your patterns seem to be broken, reseting. {0} {1} {2}", LogLevel.Error,
                    filenames.Count.ToString(), patterns.Count.ToString(), saveEvents.Count.ToString());
                return;
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



                    _patterns.Add(new OutputPattern()
                    {
                        Name = filenames[i],
                        Pattern = patterns[i],
                        SaveEvent = (OsuStatus)saveEvents[i],
                    });
                }
            }

        }
    }
}