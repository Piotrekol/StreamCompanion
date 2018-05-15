using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        private void Save()
        {
            lock (_lockingObject)
            {
                var serializedPatterns = JsonConvert.SerializeObject(_patterns);

                _settings.Add(_names.ActualPatterns.Name, serializedPatterns);

            }
            _settings.Save();
        }
        private void Load()
        {
            lock (_lockingObject)
            {
                string rawPatterns = _settings.Get<string>(_names.ActualPatterns);
                var deserializedPatterns = JsonConvert.DeserializeObject<List<OutputPattern>>(rawPatterns);
                if (deserializedPatterns != null)
                    foreach (var p in deserializedPatterns)
                    {
                        _patterns.Add(p);
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