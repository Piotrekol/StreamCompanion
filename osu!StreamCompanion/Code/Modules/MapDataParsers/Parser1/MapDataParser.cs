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

        public bool Started { get; set; }
        public void Start(ILogger logger)
        {
            Started = true;
            Load();
            if (_settings.Get<bool>(_names.FirstRun))
            {
                if (_patterns.Count == 0)
                {
                    _patterns.Add(new OutputPattern()
                    {
                        Name="np_listening",
                        Pattern = "Listening: !ArtistRoman! - !TitleRoman!",
                        SaveEvent = OsuStatus.Listening
                    });
                    _patterns.Add(new OutputPattern()
                    {
                        Name = "np_playing",
                        Pattern = "Playing: !ArtistRoman! - !TitleRoman! [!DiffName!] CS:!cs! AR:!ar! OD:!od! HP:!hp!",
                        SaveEvent = OsuStatus.Playing
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
                    //TODO: add default patterns
                }
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
                    var newPattern = (OutputPattern) p.Clone();
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
                List<int> IsPatternMemory = new List<int>();
                foreach (var f in _patterns)
                {
                    filenames.Add(f.Name);
                    Patterns.Add(f.Pattern);
                    saveEvents.Add((int)f.SaveEvent);
                    IsPatternMemory.Add(f.IsMemoryFormat ? 1 : 0);
                }

                _settings.Add(_names.PatternFileNames.Name, filenames);
                _settings.Add(_names.Patterns.Name, Patterns);
                _settings.Add(_names.saveEvents.Name, saveEvents);
                _settings.Add(_names.PatternIsMemory.Name, IsPatternMemory);
            }
            _settings.Save();
        }
        private void Load()
        {
            List<string> filenames = _settings.Get(_names.PatternFileNames.Name);
            List<string> Patterns = _settings.Get(_names.Patterns.Name);
            List<int> saveEvents = _settings.Geti(_names.saveEvents.Name);
            List<int> IsPatternMemory = _settings.Geti(_names.PatternIsMemory.Name);
            lock (_lockingObject)
            {
                _patterns.Clear();
                for (int i = 0; i < filenames.Count; i++)
                {
                    //Converting from ealier versions
                    bool isMemory = false;
                    if (IsPatternMemory.Count > i)
                    {
                        isMemory = IsPatternMemory[i] == 1;
                    }
                    if (saveEvents[i] == 27)
                        saveEvents[i] = (int)OsuStatus.All;
                    if (filenames[i].EndsWith(".txt"))
                        filenames[i] = filenames[i].Substring(0, filenames[i].LastIndexOf(".txt", StringComparison.Ordinal));



                    _patterns.Add(new OutputPattern()
                    {
                        Name = filenames[i],
                        Pattern = Patterns[i],
                        SaveEvent = (OsuStatus)saveEvents[i],
                        IsMemoryFormat = isMemory
                    });
                }
            }

        }
    }
}