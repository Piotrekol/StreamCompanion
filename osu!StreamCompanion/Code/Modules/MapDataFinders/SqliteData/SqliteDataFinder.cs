using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Enums;
using StreamCompanionTypes.Interfaces;

namespace osu_StreamCompanion.Code.Modules.MapDataFinders.SqliteData
{
    public class SqliteDataFinder : IModule, IMapDataFinder
    {
        public bool Started { get; set; }
        private ILogger _logger;
        private IMainWindowModel _mainWindowHandle;

        private ISqliteControler _sqliteControler;
        private ISettingsHandler _settingsHandle;


        public OsuStatus SearchModes { get; } = OsuStatus.Listening | OsuStatus.Null | OsuStatus.Playing |
                                                OsuStatus.Watching | OsuStatus.ResultsScreen | OsuStatus.Editing;

        public string SearcherName { get; } = "rawString";
        public int Priority { get; set; } = 10;
        public SqliteDataFinder(ILogger logger, ISettingsHandler settings, IMainWindowModel mainWindowHandle, ISqliteControler sqLiteControler)
        {
            _mainWindowHandle = mainWindowHandle;
            _settingsHandle = settings;
            _sqliteControler = sqLiteControler;
            Start(logger);
        }
        public void Start(ILogger logger)
        {
            Started = true;
            _logger = logger;
            var cacheInitalizer = new CacheInitalizer(_mainWindowHandle, _sqliteControler, _settingsHandle, _logger);
            cacheInitalizer.Initalize();

        }


        public MapSearchResult FindBeatmap(MapSearchArgs searchArgs)
        {
            var result = new MapSearchResult(searchArgs);
            IBeatmap beatmap = null;

            if (!string.IsNullOrEmpty(searchArgs.MapHash))
                beatmap = _sqliteControler.GetBeatmap(searchArgs.MapHash);

            if (!IsValidBeatmap(beatmap) && searchArgs.MapId > 0)
                beatmap = _sqliteControler.GetBeatmap(searchArgs.MapId);

            if (!IsValidBeatmap(beatmap))
            {
                if (!(string.IsNullOrEmpty(searchArgs.Artist) && string.IsNullOrEmpty(searchArgs.Title)) || !string.IsNullOrEmpty(searchArgs.Raw))
                {
                    beatmap = _sqliteControler.GetBeatmap(searchArgs.Artist, searchArgs.Title, searchArgs.Diff, searchArgs.Raw);
                }
            }

            if (IsValidBeatmap(beatmap))
            {
                result.BeatmapsFound.Add(beatmap);
            }
            return result;
        }

        private bool IsValidBeatmap(IBeatmap beatmap)
        {
            return beatmap != null
                   && !string.IsNullOrEmpty(beatmap.Md5)
                   && !(string.IsNullOrWhiteSpace(beatmap.ArtistRoman) || string.IsNullOrWhiteSpace(beatmap.TitleRoman));
        }

    }
}