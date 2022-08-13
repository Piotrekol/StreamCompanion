// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Audio.Track;
using osu.Framework.Graphics.Textures;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.Formats;
using osu.Game.Rulesets;
using osu.Game.Rulesets.Catch;
using osu.Game.Rulesets.Mania;
using osu.Game.Rulesets.Osu;
using osu.Game.Rulesets.Taiko;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using osu.Framework.Extensions;
using osu.Game.IO;
using osu.Game.Rulesets.Objects.Types;
using osu.Game.Skinning;

namespace PpCalculator
{
    /// <summary>
    /// A <see cref="WorkingBeatmap"/> which reads from a .osu file.
    /// </summary>
    public class ProcessorWorkingBeatmap : WorkingBeatmap
    {
        private readonly Beatmap beatmap;
        public int RulesetID => beatmap.BeatmapInfo.OnlineID;
        public double Length
        {
            get
            {
                if (!beatmap.HitObjects.Any())
                    return 0;

                var hitObject = beatmap.HitObjects.Last();
                return (hitObject as IHasDuration)?.EndTime ?? hitObject.StartTime;
            }
        }

        public string BackgroundFile => beatmap.Metadata.BackgroundFile;
        /// <summary>
        /// Constructs a new <see cref="ProcessorWorkingBeatmap"/> from a .osu file.
        /// </summary>
        /// <param name="file">The .osu file.</param>
        /// <param name="beatmapId">An optional beatmap ID (for cases where .osu file doesn't have one).</param>
        public ProcessorWorkingBeatmap(string file, int? beatmapId = null)
            : this(readFromFile(file), beatmapId)
        {
        }

        internal ProcessorWorkingBeatmap(Beatmap beatmap, int? beatmapId = null)
            : base(beatmap.BeatmapInfo, null)
        {
            this.beatmap = beatmap;

            if (beatmapId.HasValue)
                beatmap.BeatmapInfo.OnlineID = beatmapId ?? 0;
        }

        private static Beatmap readFromFile(string filename, int retryCount = 0)
        {
            try
            {
                using (var stream = File.OpenRead(filename))
                using (var streamReader = new LineBufferedReader(stream))
                {
                    var beatmap = Decoder.GetDecoder<Beatmap>(streamReader).Decode(streamReader);
                    stream.Position = 0;
                    beatmap.BeatmapInfo.MD5Hash = stream.ComputeMD5Hash();
                    return beatmap;
                }
            }
            catch (IOException)
            {
                //file is being used by another process..
                if (retryCount < 10)
                {
                    Thread.Sleep(5);
                    return readFromFile(filename, ++retryCount);
                }

                throw;
            }
        }

        protected override IBeatmap GetBeatmap() => beatmap;
        protected override Texture GetBackground() => null;
        protected override Track GetBeatmapTrack() => null;
        protected override ISkin GetSkin() => null;

        public override Stream GetStream(string storagePath) => null;

        public static Ruleset GetRulesetFromLegacyID(int id)
        {
            switch (id)
            {
                default:
                    throw new ArgumentException("Invalid ruleset ID provided.");
                case 0:
                    return new OsuRuleset();
                case 1:
                    return new TaikoRuleset();
                case 2:
                    return new CatchRuleset();
                case 3:
                    return new ManiaRuleset();
            }
        }
    }
}
