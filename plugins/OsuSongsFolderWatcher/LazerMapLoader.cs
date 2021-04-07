using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CollectionManager.DataTypes;
using CollectionManager.Enums;
using osu.Framework.Audio.Track;
using osu.Framework.Extensions;
using osu.Framework.Graphics.Textures;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.Formats;
using osu.Game.IO;
using osu.Game.Rulesets;
using osu.Game.Rulesets.Catch;
using osu.Game.Rulesets.Difficulty;
using osu.Game.Rulesets.Mania;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Osu;
using osu.Game.Rulesets.Osu.Difficulty;
using osu.Game.Rulesets.Taiko;
using Beatmap = StreamCompanionTypes.DataTypes.Beatmap;

namespace OsuSongsFolderWatcher
{
    public class LazerMapLoader
    {
        public static LazerMapLoader Instance { get; } = new LazerMapLoader();

        private LazerMapLoader()
        { }

        private Dictionary<int, Ruleset> Rulesets = new Dictionary<int, Ruleset>
        {
            { 0 , new OsuRuleset() },
            { 1 , new TaikoRuleset() },
            { 2 , new CatchRuleset() },
            { 3 , new ManiaRuleset() }
        };

        /// <summary>
        /// Prepares bare minimum information about beatmap
        /// </summary>
        /// <param name="file">beatmap path</param>
        /// <returns></returns>
        public Beatmap LoadBeatmap(string file)
        {
            var (lazerBeatmap, difficultyAttributes) = LoadLazerBeatmap(file);
            if (lazerBeatmap == null)
                return null;

            short circles, sliders, spinners;
            circles = sliders = spinners = 0;
            if (difficultyAttributes is OsuDifficultyAttributes osuAttributes)
            {
                circles = (short)osuAttributes.HitCircleCount;
                spinners = (short)osuAttributes.SpinnerCount;
                sliders = (short)osuAttributes.SliderCount;
            }

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
                Dir = Path.GetFileName(Path.GetDirectoryName(file)),
                MapSetId = lazerBeatmap.BeatmapInfo.BeatmapSet?.OnlineBeatmapSetID ?? 0,
                Mp3Name = lazerBeatmap.Metadata.AudioFile,
                PreviewTime = Convert.ToInt32(lazerBeatmap.BeatmapInfo.Metadata.PreviewTime),
                Sliders = sliders,
                Source = lazerBeatmap.Metadata.Source ?? string.Empty,
                Spinners = spinners,
                StackLeniency = lazerBeatmap.BeatmapInfo.StackLeniency,
                Tags = lazerBeatmap.Metadata.Tags ?? string.Empty,
                TotalTime = Convert.ToInt32(lazerBeatmap.BeatmapInfo.Length),
                OsuFileName = Path.GetFileName(file),
                AudioOffset = 0,
                DrainingTime = 0,
                ThreadId = 0,
                EditDate = DateTime.UtcNow,
                LastPlayed = DateTime.MinValue,
                LastSync = DateTime.MinValue
            };
        }

        private (IBeatmap lazerBeatmap, DifficultyAttributes difficultyAttributes) LoadLazerBeatmap(string file)
        {
            IBeatmap lazerBeatmap;
            DifficultyAttributes difficultyAttributes;
            using (var raw = File.OpenRead(file))
            using (var ms = new MemoryStream())
            using (var sr = new LineBufferedReader(ms))
            {
                raw.CopyTo(ms);
                ms.Position = 0;

                var decoder = Decoder.GetDecoder<osu.Game.Beatmaps.Beatmap>(sr);
                lazerBeatmap = decoder.Decode(sr);

                lazerBeatmap.BeatmapInfo.Path = Path.GetFileName(file);
                lazerBeatmap.BeatmapInfo.MD5Hash = ms.ComputeMD5Hash();

                var ruleset = Rulesets.GetOrDefault(lazerBeatmap.BeatmapInfo.RulesetID);
                if (ruleset == null)
                    return (null, null);

                lazerBeatmap.BeatmapInfo.Ruleset = ruleset.RulesetInfo;
                difficultyAttributes = ruleset.CreateDifficultyCalculator(new DummyConversionBeatmap(lazerBeatmap)).Calculate();

                lazerBeatmap.BeatmapInfo.StarDifficulty = Math.Round(difficultyAttributes?.StarRating ?? 0, 2);
                lazerBeatmap.BeatmapInfo.Length = CalculateLength(lazerBeatmap);
            }

            return (lazerBeatmap, difficultyAttributes);
        }

        private double CalculateLength(IBeatmap b)
        {
            if (!b.HitObjects.Any())
                return 0;

            var lastObject = b.HitObjects.Last();

            //TODO: this isn't always correct (consider mania where a non-last object may last for longer than the last in the list).
            double endTime = lastObject.GetEndTime();
            double startTime = b.HitObjects.First().StartTime;

            return endTime - startTime;
        }

        private class DummyConversionBeatmap : WorkingBeatmap
        {
            private readonly IBeatmap beatmap;

            public DummyConversionBeatmap(IBeatmap beatmap)
                : base(beatmap.BeatmapInfo, null)
            {
                this.beatmap = beatmap;
            }

            protected override IBeatmap GetBeatmap() => beatmap;
            protected override Texture GetBackground() => null;
            protected override Track GetBeatmapTrack() => null;
        }
    }
}