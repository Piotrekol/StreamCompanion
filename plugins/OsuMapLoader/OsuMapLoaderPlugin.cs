using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using PpCalculatorTypes;
using StreamCompanion.Common;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Enums;
using StreamCompanionTypes.Interfaces;
using StreamCompanionTypes.Interfaces.Services;

namespace OsuSongsFolderWatcher
{
    [SCPlugin("Osu map loader", "Reads and processes local .osu difficulty files", Consts.SCPLUGIN_AUTHOR, Consts.SCPLUGIN_BASEURL)]
    public class OsuMapLoaderPlugin : IPlugin, IMapDataFinder
    {
        private readonly IContextAwareLogger _logger;
        private readonly IModParser _modParser;

        public OsuMapLoaderPlugin(IContextAwareLogger logger, IModParser modParser)
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
                _logger.Log($"Osu file supplied in search args was not found on disk! ({args.OsuFilePath})", LogLevel.Error);
                return null;
            }

            (Beatmap beatmap, CancelableAsyncLazy<IPpCalculator> ppTask) result;
            try
            {
                result = await LazerMapLoader.LoadLazerBeatmapWithPerformanceCalculator(args.OsuFilePath, args.PlayMode,
                    _modParser.GetModsFromEnum((int)args.Mods), _logger, cancellationToken);
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
                if (ex is MissingMethodException)
                    throw;

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