using osu_StreamCompanion.Code.Misc;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Enums;
using StreamCompanionTypes.Interfaces;
using StreamCompanionTypes.Interfaces.Services;

namespace osu_StreamCompanion.Code.Modules.MapDataFinders.SqliteData
{
    public class SqliteDataFinder : IModule, IMapDataFinder
    {
        public bool Started { get; set; }
        private ILogger _logger;
        private IMainWindowModel _mainWindowHandle;

        private IDatabaseController _databaseController;
        private ISettings _settings;


        public OsuStatus SearchModes { get; } = OsuStatus.Listening | OsuStatus.Null | OsuStatus.Playing |
                                                OsuStatus.Watching | OsuStatus.ResultsScreen | OsuStatus.Editing;

        public string SearcherName { get; } = "rawString";
        public int Priority { get; set; } = 10;
        public SqliteDataFinder(ILogger logger, ISettings settings, IMainWindowModel mainWindowHandle, IDatabaseController databaseController)
        {
            _mainWindowHandle = mainWindowHandle;
            _settings = settings;
            _databaseController = databaseController;
            Start(logger);
        }
        public void Start(ILogger logger)
        {
            Started = true;
            _logger = logger;
            var cacheInitalizer = new CacheInitalizer(_mainWindowHandle, _databaseController, _settings, _logger);
            cacheInitalizer.Initalize();

        }


        public IMapSearchResult FindBeatmap(IMapSearchArgs searchArgs)
        {
            var result = new MapSearchResult(searchArgs);
            IBeatmap beatmap = null;

            if (!string.IsNullOrEmpty(searchArgs.MapHash))
                beatmap = _databaseController.GetBeatmap(searchArgs.MapHash);

            if (!IsValidBeatmap(beatmap) && searchArgs.MapId > 0)
                beatmap = _databaseController.GetBeatmap(searchArgs.MapId);

            if (!IsValidBeatmap(beatmap))
            {
                if (!(string.IsNullOrEmpty(searchArgs.Artist) && string.IsNullOrEmpty(searchArgs.Title)) || !string.IsNullOrEmpty(searchArgs.Raw))
                {
                    beatmap = _databaseController.GetBeatmap(searchArgs.Artist, searchArgs.Title, searchArgs.Diff, searchArgs.Raw);
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
                   && !(string.IsNullOrWhiteSpace(beatmap.ArtistRoman) && string.IsNullOrWhiteSpace(beatmap.TitleRoman));
        }

    }
}