using System;
using System.Collections.Generic;
using System.Globalization;
using StreamCompanionTypes.DataTypes;

namespace osu_StreamCompanion.Code.Helpers
{
    public static class BeatmapHelpers
    {
        public static Dictionary<string,object> GetTokens(this IBeatmap bm, bool empty = false)
        {
            Dictionary<string, object> dict;
            if (bm == null || empty)
            {
                dict = new Dictionary<string, object>
                {
                    {"titleRoman", null},
                    {"artistRoman", null},
                    {"titleUnicode", null},
                    {"artistUnicode", null},
                    {"mapArtistTitle", null},
                    {"mapArtistTitleUnicode", null},
                    {"mapDiff", null},
                    {"creator", null},
                    {"diffName", null},
                    {"mp3Name", null},
                    {"md5", null},
                    {"osuFileName", null},
                    {"maxBpm", null},
                    {"minBpm", null},
                    {"bpm", null},
                    {"mainBpm", null },
                    {"tags", null},
                    {"circles", null},
                    {"sliders", null},
                    {"spinners", null},
                    {"ar", null},
                    {"cs", null},
                    {"hp", null},
                    {"od", null},
                    {"sv", null},
                    {"starsNomod", null},
                    {"drainingtime", null},
                    {"totaltime", null},
                    {"previewtime", null},
                    {"mapid", null},
                    {"dl", null},
                    {"mapsetid", null},
                    {"threadid", null},
                    {"sl", null},
                    {"mode", null},
                    {"source", null},
                    {"dir", null},
                };
            }
            else
            {
                dict = new Dictionary<string, object>
                {
                    {"titleRoman", bm.TitleRoman},
                    {"artistRoman", bm.ArtistRoman},
                    {"titleUnicode", bm.TitleUnicode},
                    {"artistUnicode", bm.ArtistUnicode},
                    {"mapArtistTitle", string.Format("{0} - {1}", bm.ArtistRoman, bm.TitleRoman) },
                    {"mapArtistTitleUnicode", string.Format("{0} - {1}", bm.ArtistUnicode, bm.TitleUnicode) },
                    {"mapDiff", string.IsNullOrWhiteSpace(bm.DiffName)? "" : "[" + bm.DiffName + "]" },
                    {"creator", bm.Creator},
                    {"diffName", bm.DiffName},
                    {"mp3Name", bm.Mp3Name},
                    {"md5", bm.Md5},
                    {"osuFileName", bm.OsuFileName},
                    {"maxBpm", Math.Round(bm.MaxBpm, 2)},
                    {"minBpm", Math.Round(bm.MinBpm, 2)},
                    {"bpm", Math.Abs(bm.MinBpm - bm.MaxBpm) < double.Epsilon
                        ? Math.Round(bm.MinBpm, 2).ToString(CultureInfo.InvariantCulture)
                        : string.Format(CultureInfo.InvariantCulture, "{0:0.##} - {1:0.##} ({2:0.##})", bm.MinBpm, bm.MaxBpm, bm.MainBpm)
                    },
                    {"mainBpm", Math.Round(bm.MainBpm, 2) },
                    {"tags", bm.Tags},
                    {"circles", bm.Circles},
                    {"sliders", bm.Sliders},
                    {"spinners", bm.Spinners},
                    {"ar", bm.ApproachRate},
                    {"cs", bm.CircleSize},
                    {"hp", bm.HpDrainRate},
                    {"od", bm.OverallDifficulty},
                    {"sv", bm.SliderVelocity},
                    {"starsNomod", bm.StarsNomod},
                    {"drainingtime", bm.DrainingTime},
                    {"totaltime", bm.TotalTime},
                    {"previewtime", bm.PreviewTime},
                    {"dl", bm.MapLink},
                    {"threadid", bm.ThreadId},
                    {"sl", bm.StackLeniency},
                    {"mode", bm.PlayMode.GetHashCode().ToString()},
                    {"source", bm.Source},
                    {"dir", bm.Dir},
                };
            }

            dict["lb"] = Environment.NewLine;

            return dict;
        }





        public static Dictionary<string, string> GetDict(this IBeatmap bm, bool empty = false)
        {
            Dictionary<string, string> dict;
            if (bm == null || empty)
            {
                dict = new Dictionary<string, string>
                {
                    {"!TitleRoman!", string.Empty},
                    {"!ArtistRoman!", string.Empty},
                    {"!TitleUnicode!", string.Empty},
                    {"!ArtistUnicode!", string.Empty},
                    {"!MapArtistTitle!", string.Empty},
                    {"!MapArtistTitleUnicode!", string.Empty},
                    {"!MapDiff!", string.Empty},
                    {"!Creator!", string.Empty},
                    {"!DiffName!", string.Empty},
                    {"!Mp3Name!", string.Empty},
                    {"!Md5!", string.Empty},
                    {"!OsuFileName!", string.Empty},
                    {"!MaxBpm!", string.Empty},
                    {"!MinBpm!", string.Empty},
                    {"!Bpm!", string.Empty},
                    {"!tags!", string.Empty},
                    {"!state!", string.Empty},
                    {"!circles!", string.Empty},
                    {"!sliders!", string.Empty},
                    {"!spinners!", string.Empty},
                    {"!ar!", string.Empty},
                    {"!cs!", string.Empty},
                    {"!hp!", string.Empty},
                    {"!od!", string.Empty},
                    {"!sv!", string.Empty},
                    {"!starsNomod!", string.Empty},
                    {"!drainingtime!", string.Empty},
                    {"!totaltime!", string.Empty},
                    {"!previewtime!", string.Empty},
                    {"!mapid!", string.Empty},
                    {"!dl!", string.Empty},
                    {"!mapsetid!", string.Empty},
                    {"!threadid!", string.Empty},
                    {"!SL!", string.Empty},
                    {"!mode!", string.Empty},
                    {"!source!", string.Empty},
                    {"!dir!", string.Empty},
                    {"!lb!", string.Empty},
                };
            }
            else
            {
                var sv = bm.SliderVelocity.HasValue ? bm.SliderVelocity.Value : double.NaN;
                var sl = bm.StackLeniency.HasValue ? bm.StackLeniency.Value : double.NaN;
                dict = new Dictionary<string, string>
                {
                    {"!TitleRoman!", bm.TitleRoman},
                    {"!ArtistRoman!", bm.ArtistRoman},
                    {"!TitleUnicode!", bm.TitleUnicode},
                    {"!ArtistUnicode!", bm.ArtistUnicode},
                    {"!MapArtistTitle!", string.Format("{0} - {1}", bm.ArtistRoman, bm.TitleRoman) },
                    {"!MapArtistTitleUnicode!", string.Format("{0} - {1}", bm.ArtistUnicode, bm.TitleUnicode) },
                    {"!MapDiff!", string.IsNullOrWhiteSpace(bm.DiffName)? "" : "[" + bm.DiffName + "]" },
                    {"!Creator!", bm.Creator},
                    {"!DiffName!", bm.DiffName},
                    {"!Mp3Name!", bm.Mp3Name},
                    {"!Md5!", bm.Md5},
                    {"!OsuFileName!", bm.OsuFileName},
                    {"!MaxBpm!", Math.Round(bm.MaxBpm, 2).ToString(CultureInfo.InvariantCulture)},
                    {"!MinBpm!", Math.Round(bm.MinBpm, 2).ToString(CultureInfo.InvariantCulture)},
                    {
                        "!Bpm!", bm.MinBpm == bm.MaxBpm
                            ? Math.Round(bm.MinBpm, 2).ToString(CultureInfo.InvariantCulture)
                            : string.Format("{0} - {1}",
                                Math.Round(bm.MinBpm, 2).ToString(CultureInfo.InvariantCulture),
                                Math.Round(bm.MaxBpm, 2).ToString(CultureInfo.InvariantCulture))
                    },
                    {"!tags!", bm.Tags},
                    {"!state!", bm.StateStr},
                    {"!circles!", bm.Circles.ToString()},
                    {"!sliders!", bm.Sliders.ToString()},
                    {"!spinners!", bm.Spinners.ToString()},
                    {"!ar!", bm.ApproachRate.ToString(CultureInfo.InvariantCulture)},
                    {"!cs!", bm.CircleSize.ToString(CultureInfo.InvariantCulture)},
                    {"!hp!", bm.HpDrainRate.ToString(CultureInfo.InvariantCulture)},
                    {"!od!", bm.OverallDifficulty.ToString(CultureInfo.InvariantCulture)},
                    {"!sv!", sv.ToString(CultureInfo.InvariantCulture)},

                    {"!starsNomod!", bm.StarsNomod.ToString("##.###", CultureInfo.InvariantCulture)},
                    {"!drainingtime!", bm.DrainingTime.ToString()},
                    {"!totaltime!", bm.TotalTime.ToString()},
                    {"!previewtime!", bm.PreviewTime.ToString()},
                    {"!mapid!", bm.MapId.ToString()},
                    {"!dl!", bm.MapLink},
                    {"!mapsetid!", bm.MapSetId.ToString()},
                    {"!threadid!", bm.ThreadId.ToString()},
                    {"!SL!", sl.ToString(CultureInfo.InvariantCulture)},
                    {"!mode!", bm.PlayMode.GetHashCode().ToString()},
                    {"!source!", bm.Source},
                    {"!dir!", bm.Dir},
                    {"!lb!", Environment.NewLine}
                };
            }

            return dict;
        }



    }

}