using System;
using System.Collections.Generic;
using System.IO;
using osu_StreamCompanion.Code.Core;
using osu_StreamCompanion.Code.Core.DataTypes;
using osu_StreamCompanion.Code.Interfeaces;
using osu_StreamCompanion.Code.Windows;

namespace osu_StreamCompanion.Code.Modules.MapDataFinders.SqliteData
{
    public class SqliteDataFinder : IModule, IMapDataFinder, IMainWindowUpdater, ISqliteUser, ISettings, IModParserGetter
    {
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

        public void Start(ILogger logger)
        {
            Started = true;
            _logger = logger;
            _osuDatabaseLoader = new OsuDatabaseLoader(_logger, _modParser, _sqliteControler, _mainWindowHandle);
            new System.Threading.Thread(() =>
            {
                string osudb = Path.Combine(_settingsHandle.Get("MainOsuDirectory", ""), "osu!.db");
                string newOsuFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                    @"Files", "osu!.db");

                if (File.Exists(newOsuFile))
                    File.Delete(newOsuFile);

                File.Copy(osudb, newOsuFile);
                _osuDatabaseLoader.LoadDatabase(newOsuFile);
                File.Delete(newOsuFile);
            }).Start();
        }


        public MapSearchResult FindBeatmap(Dictionary<string, string> mapDictionary)
        {
            var result = new MapSearchResult();
            var b = _sqliteControler.GetBeatmap(mapDictionary["artist"], mapDictionary["title"], mapDictionary["diff"], mapDictionary["raw"]);
            if (b?.MapId > -1 && !(string.IsNullOrWhiteSpace(b.ArtistRoman) || string.IsNullOrWhiteSpace(b.TitleRoman)))
            {
                result.BeatmapsFound.Add(b);
            }
            result.MapSearchString = mapDictionary["raw"];
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