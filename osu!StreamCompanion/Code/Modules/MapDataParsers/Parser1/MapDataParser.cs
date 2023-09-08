using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using osu_StreamCompanion.Code.Misc;
using StreamCompanion.Common;
using StreamCompanionTypes;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Interfaces;
using StreamCompanionTypes.Enums;
using StreamCompanionTypes.Interfaces.Services;
using StreamCompanionTypes.Interfaces.Sources;
using osu_StreamCompanion.Code.Helpers;

namespace osu_StreamCompanion.Code.Modules.MapDataParsers.Parser1
{
    public sealed class MapDataParser : IModule, IOutputPatternSource, ISettingsSource, IDisposable
    {
        private readonly SettingNames _names = SettingNames.Instance;

        private readonly BindingList<OutputPattern> _patterns = new BindingList<OutputPattern>();
        private ISettings _settings;
        private readonly object _lockingObject = new object();
        private ParserSettings _parserSettings = null;
        private ILogger _logger;
        public bool Started { get; set; }
        public string SettingGroup { get; } = "Output patterns";

        [Import]
        public Lazy<IEnumerable<IPlugin>> LoadedPlugins { get; set; }

        public MapDataParser(ILogger logger, ISettings settings)
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
                Pattern = "!mapArtistTitle! !mapDiff! CS:!cs! AR:!ar! OD:!od! HP:!hp!",
                SaveEvent = OsuStatus.All
            });
            _patterns.Add(new OutputPattern()
            {
                Name = "np_playing_details",
                Pattern = "CS:!cs! AR:!ar! OD:!od! HP:!hp!",
                SaveEvent = OsuStatus.Playing | OsuStatus.Watching
            });
            _patterns.Add(new OutputPattern()
            {
                Name = "np_playing_DL",
                Pattern = "!dl!",
                SaveEvent = OsuStatus.Playing | OsuStatus.Watching
            });
            _patterns.Add(new OutputPattern()
            {
                Name = "livepp_hits",
                Pattern = "!c100!x100 !c50!x50 !miss!xMiss",
                SaveEvent = OsuStatus.Playing | OsuStatus.Watching
            });
            _patterns.Add(new OutputPattern()
            {
                Name = "livepp_current_pp",
                Pattern = "!ppIfMapEndsNow!",
                SaveEvent = OsuStatus.Playing | OsuStatus.Watching
            });
            _patterns.Add(new OutputPattern()
            {
                Name = "current_mods",
                Pattern = "!mods!",
                SaveEvent = OsuStatus.Playing | OsuStatus.Watching
            });
            _patterns.Add(new OutputPattern()
            {
                Name = "ingameOverlay_testPattern",
                Pattern = "This is test pattern for ingameOverlay, \r\nYou should be able to see it ingame after installing and enabling overlay plugin",
                ShowInOsu = true,
                SaveEvent = OsuStatus.All
            });
            _patterns.Add(new OutputPattern()
            {
                Name = "ExamplePatternCalc",
                //https://github.com/pieterderycke/Jace/wiki
                //all numerical tokens
                Pattern = "map completion: {min((time*1000/totalTime)*100,100) :0.0}%",
                SaveEvent = OsuStatus.All
            });

        }
        private void PatternsOnListChanged(object sender, ListChangedEventArgs listChangedEventArgs)
        {
            Save();
        }

        public Task<List<IOutputPattern>> GetOutputPatterns(IMapSearchResult map, Tokens tokens, OsuStatus status)
        {
            List<IOutputPattern> ret = null;

            if (tokens != null)
            {
                ret = new List<IOutputPattern>();
                foreach (var p in _patterns)
                {
                    var newPattern = (OutputPattern)p.Clone();
                    newPattern.Replacements = tokens;
                    ret.Add(newPattern);
                }
                if (_parserSettings != null && !_parserSettings.IsDisposed)
                    _parserSettings.SetPreview(tokens, status);
            }

            return Task.FromResult(ret);
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
                //TODO: Refactor ingame text overlay settings to separate settings tab, instead of hacking it in here.
                var inGameOverlayIsAvailable = LoadedPlugins.Value.Any(p => p.GetType().Name == "TextOverlay");
                _parserSettings = new ParserSettings(_settings, inGameOverlayIsAvailable, ResetPatterns);
                _parserSettings.SetPatterns(_patterns);
            }

            return _parserSettings;
        }
        private void Save()
        {
            lock (_lockingObject)
            {
                _settings.Add(_names.ActualPatterns, _patterns);

            }
            _settings.Save();
        }
        private void Load()
        {
            lock (_lockingObject)
            {
                var outputPatterns = _settings.GetConfiguration<List<OutputPattern>>(_names.ActualPatterns);
                if (outputPatterns != null)
                    foreach (var p in outputPatterns)
                    {
                        p.Name = p.Name.RemoveInvalidFileNameChars().RemoveWhitespace();
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