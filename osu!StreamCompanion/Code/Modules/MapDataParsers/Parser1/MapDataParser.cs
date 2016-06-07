using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using osu_StreamCompanion.Code.Core;
using osu_StreamCompanion.Code.Core.DataTypes;
using osu_StreamCompanion.Code.Interfeaces;
using osu_StreamCompanion.Code.Misc;

namespace osu_StreamCompanion.Code.Modules.MapDataParsers.Parser1
{
    public class MapDataParser : IModule, IMapDataParser, ISettingsProvider
    {
        private readonly SettingNames _names = SettingNames.Instance;

        private BindingList<FileFormating> _patternDictionary = new BindingList<FileFormating>();
        private Settings _settings;
        public bool Started { get; set; }
        public void Start(ILogger logger)
        {
            Started = true;
            Load();
            _patternDictionary.ListChanged += _patternDictionary_ListChanged;
            if (_settings.Get<bool>(_names.FirstRun))
            {
                _mapDataParserSettings = new MapDataParserSettings(ref _patternDictionary);
                _mapDataParserSettings.AddDefault();
                Free();
            }
        }

        private void _patternDictionary_ListChanged(object sender, ListChangedEventArgs e)
        {
            Save();
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
                _mapDataParserSettings?.SetPreviewDict(replacementDict);
            }
            //res.Add("np", FormatMapString("!ArtistRoman! - !TitleRoman! [!DiffName!] !Mods!", replacementDict));

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
                }
            }
            return _mapDataParserSettings;
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

                _settings.Add(_names.PatternFileNames.Name, filenames);
                _settings.Add(_names.Patterns.Name, Patterns);
                _settings.Add(_names.saveEvents.Name, saveEvents);
            }
            _settings.Save();
        }

        private void Load()
        {
            List<string> filenames = _settings.Get(_names.PatternFileNames.Name);
            List<string> Patterns = _settings.Get(_names.Patterns.Name);
            List<int> saveEvents = _settings.Geti(_names.saveEvents.Name);
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