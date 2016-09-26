using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Globalization;
using osu_StreamCompanion.Code.Core.DataTypes;
using osu_StreamCompanion.Code.Core.Maps;

namespace osu_StreamCompanion.Code.Helpers
{
    public static class BeatmapHelpers
    {

        private static OsuFileParser _osuFileParser = new OsuFileParser();
        public static void Read(this Beatmap beatmap, SQLiteDataReader reader)
        {
            int i = 1;
            beatmap.TitleRoman = reader.GetString(i); i++;
            beatmap.ArtistRoman = reader.GetString(i); i++;
            beatmap.TitleUnicode = reader.GetString(i); i++;
            beatmap.ArtistUnicode = reader.GetString(i); i++;
            beatmap.Creator = reader.GetString(i); i++;
            beatmap.DiffName = reader.GetString(i); i++;
            beatmap.Mp3Name = reader.GetString(i); i++;
            beatmap.Md5 = reader.GetString(i); i++;
            beatmap.OsuFileName = reader.GetString(i); i++;
            beatmap.MaxBpm = reader.GetDouble(i); i++;
            beatmap.MinBpm = reader.GetDouble(i); i++;
            beatmap.Tags = reader.GetString(i); i++;
            beatmap.State = reader.GetByte(i); i++;
            beatmap.Circles = (short)reader.GetInt32(i); i++;
            beatmap.Sliders = (short)reader.GetInt32(i); i++;
            beatmap.Spinners = (short)reader.GetInt32(i); i++;
            beatmap.EditDate = reader.GetDateTime(i); i++;
            beatmap.ApproachRate = (float)reader.GetDouble(i); i++;
            beatmap.CircleSize = (float)reader.GetDouble(i); i++;
            beatmap.HpDrainRate = (float)reader.GetDouble(i); i++;
            beatmap.OverallDifficulty = (float)reader.GetDouble(i); i++;
            beatmap.SliderVelocity = reader.GetDouble(i); i++;
            beatmap.DrainingTime = reader.GetInt32(i); i++;
            beatmap.TotalTime = reader.GetInt32(i); i++;
            beatmap.PreviewTime = reader.GetInt32(i); i++;
            beatmap.MapId = reader.GetInt32(i); i++;
            beatmap.MapSetId = reader.GetInt32(i); i++;
            beatmap.ThreadId = reader.GetInt32(i); i++;
            beatmap.MapRating = reader.GetInt32(i); i++;
            beatmap.Offset = (short)reader.GetInt32(i); i++;
            beatmap.StackLeniency = (float)reader.GetDouble(i); i++;
            beatmap.Mode = reader.GetByte(i); i++;
            beatmap.Source = reader.GetString(i); i++;
            beatmap.AudioOffset = (short)reader.GetInt32(i); i++;
            beatmap.LetterBox = reader.GetString(i); i++;
            beatmap.Played = reader.GetBoolean(i); i++;
            beatmap.LastPlayed = reader.GetDateTime(i); i++;
            beatmap.IsOsz2 = reader.GetBoolean(i); i++;
            beatmap.Dir = reader.GetString(i); i++;
            beatmap.LastSync = reader.GetDateTime(i); i++;
            beatmap.DisableHitsounds = reader.GetBoolean(i); i++;
            beatmap.DisableSkin = reader.GetBoolean(i); i++;
            beatmap.DisableSb = reader.GetBoolean(i); i++;
            beatmap.BgDim = reader.GetInt16(i); i++;
            beatmap.Somestuff = reader.GetInt16(i); i++;
            beatmap.VideoDir = reader.GetString(i);
        }

        public static Dictionary<string, string> GetDict(this Beatmap bm, string mods)
        {
            var dict = bm.GetDict();
            dict.Add("!mods!", mods);
            return dict;
        }
        public static Dictionary<string, string> GetDict(this Beatmap bm)
        {
            var dict = new Dictionary<string, string>
            {
                {"!TitleRoman!", bm.TitleRoman},
                {"!ArtistRoman!", bm.ArtistRoman},
                {"!TitleUnicode!", bm.TitleUnicode},
                {"!ArtistUnicode!", bm.ArtistUnicode},
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
                        : string.Format("{0} - {1}", Math.Round(bm.MinBpm, 2).ToString(CultureInfo.InvariantCulture),
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
                {"!sv!", bm.SliderVelocity.ToString(CultureInfo.InvariantCulture)},

                {"!drainingtime!", bm.DrainingTime.ToString()},
                {"!totaltime!", bm.TotalTime.ToString()},
                {"!previewtime!", bm.PreviewTime.ToString()},
                {"!mapid!", bm.MapId.ToString()},
                {"!dl!", bm.DlLink},
                {"!mapsetid!", bm.MapSetId.ToString()},
                {"!threadid!", bm.ThreadId.ToString()},
                {"!SL!", bm.StackLeniency.ToString(CultureInfo.InvariantCulture)},
                {"!mode!", bm.Mode.ToString()},
                {"!source!", bm.Source},
                {"!dir!", bm.Dir},
                {"!lb!", Environment.NewLine}
            };


            return dict;
        }

        public static Beatmap ReadBeatmap(string fullPath)
        {
            Console.WriteLine("reading beatmap located at {0}", fullPath);

            var beatmap = _osuFileParser.ReadBeatmapData(fullPath);
            return beatmap;
        }
        public static string GetDiffFromString(string msnString)
        {
            if (msnString.Contains("]") && msnString.Contains("["))
            {
                var openBr = 0;
                var closedBr = 0;
                var strPos = msnString.Length - 1;
                do
                {
                    var character = msnString[strPos];
                    switch (character)
                    {
                        case ']':
                            closedBr++;
                            break;
                        case '[':
                            openBr++;
                            break;
                    }
                    strPos--;
                } while (closedBr != openBr);

                return msnString.Substring(strPos + 2, msnString.Length - strPos - 3);
            }
            return string.Empty;
        }
        
    }

}