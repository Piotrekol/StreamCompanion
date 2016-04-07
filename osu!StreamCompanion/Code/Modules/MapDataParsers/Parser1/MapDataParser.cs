using System.Collections.Generic;
using System.Windows.Forms;
using osu_StreamCompanion.Code.Core;
using osu_StreamCompanion.Code.Core.DataTypes;
using osu_StreamCompanion.Code.Interfeaces;

namespace osu_StreamCompanion.Code.Modules.MapDataParsers.Parser1
{
    public class MapDataParser : IModule, IMapDataParser, ISettingsProvider
    {
        private List<FileFormating> _patternDictionary = new List<FileFormating>();
        private Settings _settings;
        public bool Started { get; set; }
        public void Start(ILogger logger)
        {
            Started = true;
            Load();
            if (_settings.Get("firstRun", true))
            {
                _mapDataParserSettings = new MapDataParserSettings(ref _patternDictionary);
                _mapDataParserSettings.dictionaryUpdated += _mapDataParserSettings_dictionaryUpdated;
                _mapDataParserSettings.AddDefault();
                Free();
            }
        }


        public Dictionary<string, string> GetFormatedMapStrings(Dictionary<string, string> replacements, OsuStatus status)
        {
            var replacementDict = replacements;
            var res = new Dictionary<string, string>();
            if (replacementDict != null)
            {
                lock (_patternDictionary)
                {
                    foreach (var pattern in _patternDictionary)
                    {
                        if ((pattern.SaveEvent & (int)status) == 0)
                            res.Add(pattern.Filename, "");
                        else
                            res.Add(pattern.Filename, FormatMapString(pattern.Pattern, replacementDict));
                    }
                }
                _mapDataParserSettings?.SetTestDict(replacementDict);
            }
            res.Add("np", FormatMapString("!ArtistRoman! - !TitleRoman! [!DiffName!] !Mods!", replacementDict));

            return res;
        }

        public string FormatMapString(string toFormat, Dictionary<string, string> replacements)
        {
            foreach (var r in replacements)
            {
                toFormat = toFormat.Replace(r.Key, r.Value);
            }
            return toFormat;
        }

        public string SettingGroup { get; } = "Map formating";
        public void SetSettingsHandle(Settings settings)
        {
            _settings = settings;
        }

        public void Free()
        {
            _mapDataParserSettings.dictionaryUpdated -= _mapDataParserSettings_dictionaryUpdated;
            _mapDataParserSettings.Dispose();
        }

        private MapDataParserSettings _mapDataParserSettings;
        public UserControl GetUiSettings()
        {
            if (_mapDataParserSettings == null || _mapDataParserSettings.IsDisposed)
            {
                lock (_patternDictionary)
                {
                    _mapDataParserSettings = new MapDataParserSettings(ref _patternDictionary);
                    _mapDataParserSettings.dictionaryUpdated += _mapDataParserSettings_dictionaryUpdated;
                }
            }
            return _mapDataParserSettings;
        }

        private void _mapDataParserSettings_dictionaryUpdated(object sender, System.EventArgs e)
        {
            Save();
        }

        private void Save()
        {
            lock (_patternDictionary)
            {//do saving...
                List<string> filenames = new List<string>();
                List<string> Patterns = new List<string>();
                List<int> saveEvents = new List<int>();
                foreach (var f in _patternDictionary)
                {
                    filenames.Add(f.Filename);
                    Patterns.Add(f.Pattern);
                    saveEvents.Add(f.SaveEvent);
                }

                _settings.Add("PatternFileNames", filenames);
                _settings.Add("Patterns", Patterns);
                _settings.Add("saveEvents", saveEvents);
            }
            _settings.Save();
        }

        private void Load()
        {
            List<string> filenames = _settings.Get("PatternFileNames");
            List<string> Patterns = _settings.Get("Patterns");
            List<int> saveEvents = _settings.Geti("saveEvents");
            lock (_patternDictionary)
            {
                for (int i = 0; i < filenames.Count; i++)
                {
                    _patternDictionary.Add(new FileFormating()
                    {
                        Filename = filenames[i],
                        Pattern = Patterns[i],
                        SaveEvent = saveEvents[i]
                    });
                }
            }

        }
    }
}