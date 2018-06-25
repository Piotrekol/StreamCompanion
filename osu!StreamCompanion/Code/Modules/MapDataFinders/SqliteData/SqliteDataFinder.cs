using System;
using System.Collections.Generic;
using System.IO;
using CollectionManager.Modules.FileIO.OsuDb;
using osu_StreamCompanion.Code.Core;
using osu_StreamCompanion.Code.Misc;
using osu_StreamCompanion.Code.Windows;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Interfaces;

namespace osu_StreamCompanion.Code.Modules.MapDataFinders.SqliteData
{
    public class SqliteDataFinder : IModule, IMapDataFinder, IMainWindowUpdater, ISqliteUser, ISettings
    {
        public bool Started { get; set; }
        private ILogger _logger;
        private IMainWindowModel _mainWindowHandle;

        private ISqliteControler _sqliteControler;
        private ISettingsHandler _settingsHandle;


        public OsuStatus SearchModes { get; } = OsuStatus.Listening | OsuStatus.Null | OsuStatus.Playing |
                                                OsuStatus.Watching;

        public string SearcherName { get; } = "rawString";

        
        public void Start(ILogger logger)
        {
            Started = true;
            _logger = logger;
            var cacheInitalizer = new CacheInitalizer(_mainWindowHandle, _sqliteControler, _settingsHandle,_logger);
            cacheInitalizer.Initalize();
            
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

        public void GetMainWindowHandle(IMainWindowModel mainWindowHandle)
        {
            _mainWindowHandle = mainWindowHandle;
        }

        public void SetSqliteControlerHandle(ISqliteControler sqLiteControler)
        {
            _sqliteControler = sqLiteControler;
        }

        public void SetSettingsHandle(ISettingsHandler settings)
        {
            _settingsHandle = settings;
        }
    }
}