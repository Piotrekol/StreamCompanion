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

            var ppCalculator = (PpCalculator.PpCalculator)iPpCalculator.Clone();
            var moddedMapAttributes = ppCalculator.AttributesAt(double.MaxValue);

            if (IsDifficultyNoMod(mods.Mods))
                return (ConvertToSCBeatmap(ppCalculator.PlayableBeatmap, moddedMapAttributes, osuFilePath, mods.Mods), createPpCalculatorTask);

            ppCalculator.Mods = null;
            ppCalculator.Calculate(cancellationToken);
            var noModMapAttributes = ppCalculator.AttributesAt(double.MaxValue);
            var scBeatmap = ConvertToSCBeatmap(ppCalculator.PlayableBeatmap, noModMapAttributes, osuFilePath, Mods.Omod);
            scBeatmap.ModPpStars[(PlayMode)ppCalculator.PlayableBeatmap.BeatmapInfo.RulesetID].Add((int)(mods.Mods & Mods.MapChanging), moddedMapAttributes?.StarRating ?? 0d);

            return (scBeatmap, createPpCalculatorTask);
        }

        private static bool IsDifficultyNoMod(Mods mods) => (mods & ~(Mods.Td | Mods.Sd | Mods.Au | Mods.Ap | Mods.Pf | Mods.Cm | Mods.Tp)) == Mods.Omod;

        private static Beatmap ConvertToSCBeatmap(IBeatmap lazerBeatmap, DifficultyAttributes difficultyAttributes, string fullFilePath, Mods mods)
        {
            SanityCheck(lazerBeatmap);
            short circles, sliders, spinners;
            circles = sliders = spinners = 0;
            switch (difficultyAttributes)
            {
                case OsuDifficultyAttributes osuAttributes:
                    {
                        circles = (short)osuAttributes.HitCircleCount;
                        sliders = (short)osuAttributes.SliderCount;
                        spinners = (short)osuAttributes.SpinnerCount;
                        break;
                    }
                case TaikoDifficultyAttributes taikoAttributes:
                    {
                        circles = (short)taikoAttributes.HitCount;
                        sliders = (short)taikoAttributes.DrumRollCount;
                        spinners = (short)taikoAttributes.SwellCount;
                        break;
                    }
                case CatchDifficultyAttributes catchAttributes:
                    {
                        circles = (short)catchAttributes.FruitCount;
                        sliders = (short)catchAttributes.JuiceStreamCount;
                        spinners = (short)catchAttributes.BananaShowerCount;
                        break;
                    }
                case ManiaDifficultyAttributes maniaAttributes:
                    {
                        circles = (short)maniaAttributes.NoteCount;
                        sliders = (short)maniaAttributes.HoldNoteCount;
                        break;
                    }
            }
            lazerBeatmap.BeatmapInfo.StarRating = difficultyAttributes?.StarRating ?? 0;
            return new Beatmap
            {
                PlayMode = (PlayMode)lazerBeatmap.BeatmapInfo.RulesetID,
                ArtistRoman = lazerBeatmap.Metadata.Artist ?? string.Empty,
                ArtistUnicode = lazerBeatmap.Metadata.ArtistUnicode ?? string.Empty,
                TitleRoman = lazerBeatmap.Metadata.Title ?? string.Empty,
                TitleUnicode = lazerBeatmap.Metadata.TitleUnicode ?? string.Empty,
                DiffName = lazerBeatmap.BeatmapInfo.DifficultyName ?? string.Empty,
                Md5 = lazerBeatmap.BeatmapInfo.MD5Hash,
                MapId = lazerBeatmap.BeatmapInfo.OnlineBeatmapID ?? 0,
                ModPpStars = new PlayModeStars { { (PlayMode)lazerBeatmap.BeatmapInfo.RulesetID, new StarRating { { (int)(mods & Mods.MapChanging), lazerBeatmap.BeatmapInfo.StarRating } } } },
                MainBpm = Math.Round(60000 / lazerBeatmap.GetMostCommonBeatLength()),
                MinBpm = Math.Round(lazerBeatmap.ControlPointInfo.BPMMinimum),
                MaxBpm = Math.Round(lazerBeatmap.ControlPointInfo.BPMMaximum),
                Creator = lazerBeatmap.Metadata.AuthorString ?? string.Empty,
                ApproachRate = lazerBeatmap.BeatmapInfo.BaseDifficulty.ApproachRate,
                CircleSize = lazerBeatmap.BeatmapInfo.BaseDifficulty.CircleSize,
                SliderVelocity = lazerBeatmap.BeatmapInfo.BaseDifficulty.SliderMultiplier,
                OverallDifficulty = lazerBeatmap.BeatmapInfo.BaseDifficulty.OverallDifficulty,
                HpDrainRate = lazerBeatmap.BeatmapInfo.BaseDifficulty.DrainRate,
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
                TotalTime = Convert.ToInt32(CalculateLength(lazerBeatmap)),
                OsuFileName = string.IsNullOrEmpty(fullFilePath) ? null : Path.GetFileName(fullFilePath),
                AudioOffset = 0,
                DrainingTime = Convert.ToInt32(CalculateLength(lazerBeatmap, true)),
                ThreadId = 0,
                EditDate = DateTime.UtcNow,
                LastPlayed = DateTime.MinValue,
                LastSync = DateTime.MinValue
            };
        }

        private static void SanityCheck(IBeatmap lazerBeatmap)
        {
            if (lazerBeatmap == null)
                throw new LazerNullReferenceException("lazerBeatmap");
            if (lazerBeatmap.Metadata == null)
                throw new LazerNullReferenceException("lazerBeatmap.Metadata");
            if (lazerBeatmap.BeatmapInfo == null)
                throw new LazerNullReferenceException("lazerBeatmap.BeatmapInfo");
            if (lazerBeatmap.ControlPointInfo == null)
                throw new LazerNullReferenceException("lazerBeatmap.ControlPointInfo");
            if (lazerBeatmap.BeatmapInfo.BaseDifficulty == null)
                throw new LazerNullReferenceException("lazerBeatmap.BeatmapInfo.BaseDifficulty");
            if (lazerBeatmap.BeatmapInfo.Metadata == null)
                throw new LazerNullReferenceException("lazerBeatmap.BeatmapInfo.Metadata");
        }

        private static double CalculateLength(IBeatmap b, bool drain = false)
        {
            if (!b.HitObjects.Any())
                return 0;

            var lastObject = b.HitObjects.Last();
            //TODO: this isn't always correct (consider mania where a non-last object may last for longer than the last in the list).
            double endTime = lastObject.GetEndTime();
            if (!drain)
                return endTime;

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