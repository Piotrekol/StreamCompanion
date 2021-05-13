using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CollectionManager.Enums;
using PpCalculatorTypes;
using StreamCompanion.Common;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Enums;
using StreamCompanionTypes.Interfaces;
using StreamCompanionTypes.Interfaces.Services;

namespace OsuSongsFolderWatcher
{
    public class OsuMapLoaderPlugin : IPlugin, IMapDataFinder
    {
        private readonly ILogger _logger;
        private readonly IModParser _modParser;
        public string Description { get; } = "Provides beatmap data by parsing .osu files using osu!lazer";
        public string Name { get; } = nameof(OsuMapLoaderPlugin);
        public string Author { get; } = "Piotrekol";
        public string Url { get; } = string.Empty;
        public string UpdateUrl { get; } = string.Empty;

        public OsuMapLoaderPlugin(ILogger logger, IModParser modParser)
        {
            _logger = logger;
            _modParser = modParser;
        }
        public async Task<IMapSearchResult> FindBeatmap(IMapSearchArgs args, CancellationToken cancellationToken)
        {
            if (args == null || string.IsNullOrEmpty(args.OsuFilePath))
                return null;

            if (!File.Exists(args.OsuFilePath))
            {
                _logger.Log("Osu file supplied in search args was not found on disk!", LogLevel.Error);
                return null;
            }

            (Beatmap beatmap, CancelableAsyncLazy<IPpCalculator> ppTask) result;
            try
            {
                result = await LazerMapLoader.LoadLazerBeatmapWithPerformanceCalculator(args.OsuFilePath, args.PlayMode,
                    _modParser.GetModsFromEnum((int) args.Mods), _logger, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                ex.Data["PreventedCrash"] = 1;
                ex.Data["location"] = args.OsuFilePath;
                _logger.Log(ex, LogLevel.Critical);
                return null;
            }

            if (result.beatmap == null)
            {
                var ex = new BeatmapLoadFailedException();
                ex.Data["location"] = args.OsuFilePath;
                _logger.Log(ex, LogLevel.Critical);
                _logger.Log($"Failed to load beatmap located at {args.OsuFilePath}", LogLevel.Warning);
                return null;
            }

            return new MapSearchResult(args)
            {
                BeatmapsFound = { result.beatmap },
                SharedObjects = { result.ppTask }
            };
        }

        public OsuStatus SearchModes { get; } = OsuStatus.All;
        public string SearcherName { get; } = "osu!lazer";
        public int Priority { get; set; } = 90;
    }
}