using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Forms;
using Newtonsoft.Json;
using StreamCompanionTypes;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Interfaces;
using StreamCompanionTypes.Enums;

namespace osu_StreamCompanion.Code.Modules.MapDataParsers.Parser1
{
    public class MapDataParser : IModule, IMapDataParser, ISettingsProvider, IDisposable
    {
        private readonly SettingNames _names = SettingNames.Instance;

        private readonly BindingList<OutputPattern> _patterns = new BindingList<OutputPattern>();
        private ISettingsHandler _settings;
        private readonly object _lockingObject = new object();
        private ParserSettings _parserSettings = null;
        private ILogger _logger;
        public bool Started { get; set; }
        public string SettingGroup { get; } = "Output patterns";

        [Import]
        public Lazy<IEnumerable<IPlugin>> LoadedPlugins { get; set; }
        
        public MapDataParser(ILogger logger, ISettingsHandler settings)
        {
            _logger = logger;
            _settings = settings;
            Start(logger);
        }

        public void Start(ILogger logger)
        {
            Started = true;
            _logger = logger;
            Load();
            if (_patterns.Count == 0)
            {
                ResetPatterns();
            }

            _patterns.ListChanged += PatternsOnListChanged;
        }

        public void ResetPatterns()
        {
            _patterns.Clear();

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
        private void PatternsOnListChanged(object sender, ListChangedEventArgs listChangedEventArgs)
        {
            Save();
        }

        public List<IOutputPattern> GetFormatedPatterns(Tokens replacements, OsuStatus status)
        {
            List<IOutputPattern> ret = null;

            if (replacements != null)
            {
                ret = new List<IOutputPattern>();
                foreach (var p in _patterns)
                {
                    var newPattern = (OutputPattern)p.Clone();
                    newPattern.Replacements = replacements;
                    ret.Add(newPattern);
                }
                if (_parserSettings != null && !_parserSettings.IsDisposed)
                    _parserSettings.SetPreview(replacements, status);
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

        public void Free()
        {
            _parserSettings.Dispose();
        }

        public object GetUiSettings()
        {
            if (_parserSettings == null || _parserSettings.IsDisposed)
            {
                var inGameOverlayIsAvailable = LoadedPlugins.Value.Any(p => p.Name == "IngameOverlay");
                _parserSettings = new ParserSettings(_settings, inGameOverlayIsAvailable, ResetPatterns);
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
                var legacyConfig = new LegacyParserConfigConverter();
                var oldPatterns = legacyConfig.Convert(_settings);
                if (oldPatterns.Count > 0)
                {
                    foreach (var p in oldPatterns)
                    {
                        _patterns.Add(p);
                    }
                    MessageBox.Show("There was a big change in how patterns are stored and they needed to be converted." + Environment.NewLine +
                                    "Go to your pattern settings and make sure that everything is still correct(Especialy save events)", "osu!StreamCompanion - Update message", MessageBoxButtons.OK);
                }
                else
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
        }

        public void Dispose()
        {
            _parserSettings?.Dispose();
            Save();
        }
    }
}