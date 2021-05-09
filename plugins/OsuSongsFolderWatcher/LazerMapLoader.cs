using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CollectionManager.DataTypes;
using CollectionManager.Enums;
using osu.Game.Rulesets.Objects;
using PpCalculator;
using PpCalculatorTypes;
using StreamCompanion.Common;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Enums;
using StreamCompanionTypes.Interfaces.Services;
using Beatmap = StreamCompanionTypes.DataTypes.Beatmap;
using IBeatmap = osu.Game.Beatmaps.IBeatmap;

namespace OsuSongsFolderWatcher
{
    public static class LazerMapLoader
    {
        
        public static async Task<(Beatmap Beatmap, CancelableAsyncLazy<IPpCalculator> CreatePpCalculatorLazyTask)> LoadLazerBeatmapWithPerformanceCalculator(string osuFilePath, PlayMode? desiredPlayMode, IModsEx mods, ILogger logger, CancellationToken cancellationToken)
        {
            var createPpCalculatorTask = CreatePpCalculatorTask(osuFilePath, desiredPlayMode, mods, logger);
            var iPpCalculator = await createPpCalculatorTask.GetValueAsync(cancellationToken);
            if (iPpCalculator == null)
                return (null, null);

            var ppCalculator = (PpCalculator.PpCalculator)iPpCalculator;
            var lazerBeatmap = ppCalculator!.PlayableBeatmap;

            var mapAttributes = ppCalculator.AttributesAt(double.MaxValue);
            var scBeatmap = ConvertToSCBeatmap(lazerBeatmap, mapAttributes, osuFilePath);

            return (scBeatmap, createPpCalculatorTask);
        }

        private static Beatmap ConvertToSCBeatmap(IBeatmap lazerBeatmap, DifficultyAttributes difficultyAttributes, string fullFilePath)
        {
            short circles, sliders, spinners;
            circles = sliders = spinners = 0;
            if (difficultyAttributes is OsuDifficultyAttributes osuAttributes)
            {
                circles = (short)osuAttributes.HitCircleCount;
                spinners = (short)osuAttributes.SpinnerCount;
                sliders = (short)osuAttributes.SliderCount;
            }

            lazerBeatmap.BeatmapInfo.StarDifficulty = Math.Round(difficultyAttributes?.StarRating ?? 0, 2);
            lazerBeatmap.BeatmapInfo.Length = CalculateLength(lazerBeatmap);
            
            return new Beatmap
            {
                PlayMode = (PlayMode)lazerBeatmap.BeatmapInfo.RulesetID,
                ArtistRoman = lazerBeatmap.Metadata.Artist ?? string.Empty,
                ArtistUnicode = lazerBeatmap.Metadata.ArtistUnicode ?? string.Empty,
                TitleRoman = lazerBeatmap.Metadata.Title ?? string.Empty,
                TitleUnicode = lazerBeatmap.Metadata.TitleUnicode ?? string.Empty,
                DiffName = lazerBeatmap.BeatmapInfo.Version ?? string.Empty,
                Md5 = lazerBeatmap.BeatmapInfo.MD5Hash,
                MapId = lazerBeatmap.BeatmapInfo.OnlineBeatmapID ?? 0,
                ModPpStars = new PlayModeStars { { (PlayMode)lazerBeatmap.BeatmapInfo.RulesetID, new StarRating { { (int)Mods.Omod, lazerBeatmap.BeatmapInfo.StarDifficulty } } } },
                MainBpm = Math.Round(lazerBeatmap.ControlPointInfo.BPMMode),
                MinBpm = Math.Round(lazerBeatmap.ControlPointInfo.BPMMinimum),
                MaxBpm = Math.Round(lazerBeatmap.ControlPointInfo.BPMMaximum),
                Creator = lazerBeatmap.Metadata.AuthorString ?? string.Empty,
                ApproachRate = lazerBeatmap.BeatmapInfo.BaseDifficulty.ApproachRate,
                CircleSize = lazerBeatmap.BeatmapInfo.BaseDifficulty.CircleSize,
                SliderVelocity = lazerBeatmap.BeatmapInfo.BaseDifficulty.SliderMultiplier,
                OverallDifficulty = lazerBeatmap.BeatmapInfo.BaseDifficulty.OverallDifficulty,
                Circles = circles,
                Dir = string.IsNullOrEmpty(fullFilePath) ? null : Path.GetFileName(Path.GetDirectoryName(fullFilePath)),
                MapSetId = lazerBeatmap.BeatmapInfo.BeatmapSet?.OnlineBeatmapSetID ?? 0,
                Mp3Name = lazerBeatmap.Metadata.AudioFile,
                PreviewTime = Convert.ToInt32(lazerBeatmap.BeatmapInfo.Metadata.PreviewTime),
                Sliders = sliders,
                Source = lazerBeatmap.Metadata.Source ?? string.Empty,
                Spinners = spinners,
                StackLeniency = lazerBeatmap.BeatmapInfo.StackLeniency,
                Tags = lazerBeatmap.Metadata.Tags ?? string.Empty,
                TotalTime = Convert.ToInt32(lazerBeatmap.BeatmapInfo.Length),
                OsuFileName = string.IsNullOrEmpty(fullFilePath) ? null : Path.GetFileName(fullFilePath),
                AudioOffset = 0,
                DrainingTime = 0,
                ThreadId = 0,
                EditDate = DateTime.UtcNow,
                LastPlayed = DateTime.MinValue,
                LastSync = DateTime.MinValue
            };
        }

        private static double CalculateLength(IBeatmap b)
        {
            if (!b.HitObjects.Any())
                return 0;

            var lastObject = b.HitObjects.Last();

            //TODO: this isn't always correct (consider mania where a non-last object may last for longer than the last in the list).
            double endTime = lastObject.GetEndTime();
            double startTime = b.HitObjects.First().StartTime;

            return endTime - startTime;
        }

        private static CancelableAsyncLazy<IPpCalculator> CreatePpCalculatorTask(string osuFilePath, PlayMode? desiredPlayMode, IModsEx mods, ILogger logger) =>
            new CancelableAsyncLazy<IPpCalculator>((cancellationToken) =>
            {
                if (string.IsNullOrEmpty(osuFilePath))
                    return Task.FromResult<IPpCalculator>(null);

                var ppCalculator = PpCalculatorHelpers.GetPpCalculator((int)(desiredPlayMode ?? PlayMode.Osu), osuFilePath, null);
                ppCalculator.Mods = (mods?.WorkingMods ?? "").Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                try
                {
                    ppCalculator.Calculate(cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                //specifically for BeatmapInvalidForRulesetException (beatmap had invalid hitobject with missing position data)
                catch (Exception e)
                {
                    e.Data["PreventedCrash"] = 1;
                    logger.Log(e, LogLevel.Critical);
                    return Task.FromResult<IPpCalculator>(null);
                }

                return Task.FromResult((IPpCalculator)ppCalculator);
            });
    }
}