// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.IO;
using osu.Framework.Audio.Track;
using osu.Framework.Graphics.Textures;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.Formats;
using osu.Game.Rulesets;
using osu.Game.Rulesets.Catch;
using osu.Game.Rulesets.Mania;
using osu.Game.Rulesets.Osu;
using osu.Game.Rulesets.Taiko;

namespace PpCalculator
{
    /// <summary>
    /// A <see cref="WorkingBeatmap"/> which reads from a .osu file.
    /// </summary>
    public class ProcessorWorkingBeatmap : WorkingBeatmap
    {
        private readonly Beatmap beatmap;
        public int RulesetID => beatmap.BeatmapInfo.RulesetID;
        /// <summary>
        /// Constructs a new <see cref="ProcessorWorkingBeatmap"/> from a .osu file.
        /// </summary>
        /// <param name="file">The .osu file.</param>
        /// <param name="beatmapId">An optional beatmap ID (for cases where .osu file doesn't have one).</param>
        public ProcessorWorkingBeatmap(string file, int? beatmapId = null)
            : this(readFromFile(file), beatmapId)
        {
        }

        private ProcessorWorkingBeatmap(Beatmap beatmap, int? beatmapId = null)
            : base(beatmap.BeatmapInfo)
        {
            this.beatmap = beatmap;

            beatmap.BeatmapInfo.Ruleset = GetRulesetFromLegacyID(beatmap.BeatmapInfo.RulesetID).RulesetInfo;

            if (beatmapId.HasValue)
                beatmap.BeatmapInfo.OnlineBeatmapID = beatmapId;
        }

        private static Beatmap readFromFile(string filename)
        {
            using (var stream = File.OpenRead(filename))
            using (var streamReader = new StreamReader(stream))
                return Decoder.GetDecoder<Beatmap>(streamReader).Decode(streamReader);
        }

        protected override IBeatmap GetBeatmap() => beatmap;
        protected override Texture GetBackground() => null;
        protected override Track GetTrack() => null;

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
