using System;
using System.Collections.Generic;
using System.IO;
using CollectionManager.Modules.FileIO.OsuDb;
using osu_StreamCompanion.Code.Core;
using osu_StreamCompanion.Code.Core.DataTypes;
using osu_StreamCompanion.Code.Interfaces;
using osu_StreamCompanion.Code.Misc;
using osu_StreamCompanion.Code.Windows;

namespace osu_StreamCompanion.Code.Modules.MapDataFinders.SqliteData
{
    public class SqliteDataFinder : IModule, IMapDataFinder, IMainWindowUpdater, ISqliteUser, ISettings, IModParserGetter
    {
        private readonly SettingNames _names = SettingNames.Instance;
        public bool Started { get; set; }
        private ILogger _logger;
        private MainWindowUpdater _mainWindowHandle;
        private OsuDatabaseLoader _osuDatabaseLoader;

        private SqliteControler _sqliteControler;
        private Settings _settingsHandle;
        private List<IModParser> _modParser;


        public OsuStatus SearchModes { get; } = OsuStatus.Listening | OsuStatus.Null | OsuStatus.Playing |
                                                OsuStatus.Watching;

        public string SearcherName { get; } = "rawString";

        class BeatmapLoaderLogger : CollectionManager.Interfaces.ILogger
        {
            private readonly MainWindowUpdater _handle;

            public BeatmapLoaderLogger(MainWindowUpdater handle)
            {
                _handle = handle;
            }
            public void Log(string logMessage, params string[] vals)
            {
                _handle.BeatmapsLoaded = string.Format(logMessage, vals);
            }
        }
        public void Start(ILogger logger)
        {
            Started = true;
            _logger = logger;
            _osuDatabaseLoader = new LOsuDatabaseLoader(new BeatmapLoaderLogger(_mainWindowHandle), _sqliteControler, new Beatmap());

            //_osuDatabaseLoader = new OsuDatabaseLoader(_logger, _modParser, _sqliteControler, _mainWindowHandle);
            new System.Threading.Thread(() =>
            {
                string osudb = Path.Combine(_settingsHandle.Get<string>(_names.MainOsuDirectory), "osu!.db");
                if (File.Exists(osudb))
                {
                    string newOsuFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                        @"Files", "osu!.db");

                    if (File.Exists(newOsuFile))
                        File.Delete(newOsuFile);

                    File.Copy(osudb, newOsuFile);
                    _osuDatabaseLoader.LoadDatabase(newOsuFile);

                    File.Delete(newOsuFile);
                }
                else
                {
                    _mainWindowHandle.BeatmapsLoaded = "Could not locate osu!.db";
                }
            }).Start();
        }


        public MapSearchResult FindBeatmap(MapSearchArgs searchArgs)
        {
            var result = new MapSearchResult();
            Beatmap beatmap = null;
            if (searchArgs.MapId > 0)
                beatmap = _sqliteControler.GetBeatmap(searchArgs.MapId);
            if (beatmap == null || (beatmap.MapId <= 0))
            {
                if (!(string.IsNullOrEmpty(searchArgs.Artist) && string.IsNullOrEmpty(searchArgs.Title)) || !string.IsNullOrEmpty(searchArgs.Raw))
                {
                    beatmap = _sqliteControler.GetBeatmap(searchArgs.Artist, searchArgs.Title, searchArgs.Diff, searchArgs.Raw);
                }
            }

            if (beatmap?.MapId > -1 && !(string.IsNullOrWhiteSpace(beatmap.ArtistRoman) || string.IsNullOrWhiteSpace(beatmap.TitleRoman)))
            {
                result.BeatmapsFound.Add(beatmap);
            }
            result.MapSearchString = searchArgs.Raw;
            return result;
        }

        public void GetMainWindowHandle(MainWindowUpdater mainWindowHandle)
        {
            _mainWindowHandle = mainWindowHandle;
        }

        public void SetSqliteControlerHandle(SqliteControler sqLiteControler)
        {
            _sqliteControler = sqLiteControler;
        }

        public void SetSettingsHandle(Settings settings)
        {
            _settingsHandle = settings;
        }

        public void SetModParserHandle(List<IModParser> modParser)
        {
            _modParser = modParser;
        }
    }
}