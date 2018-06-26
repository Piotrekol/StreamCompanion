using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Globalization;
using System.IO;
using CollectionManager.Enums;
using osu_StreamCompanion.Code.Core.Maps;
using Beatmap = StreamCompanionTypes.DataTypes.Beatmap;

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
            beatmap.PlayMode = (PlayMode)reader.GetByte(i); i++;
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
            beatmap.VideoDir = reader.GetString(i); i++;
            beatmap.DeSerializeStars((byte[])reader.GetValue(i));
        }

        private static void WriteAll(this MemoryStream ms, int value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            ms.Write(bytes, 0, bytes.Length);
        }
        private static void WriteAll(this MemoryStream ms, double value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            ms.Write(bytes, 0, bytes.Length);
        }
        private static BinaryReader _binaryReader;

        public static byte[] SerializeStars(this Beatmap bm)
        {
            using (MemoryStream mStream = new MemoryStream())
            {
                mStream.WriteAll(bm.ModPpStars.Count);
                foreach (var stars in bm.ModPpStars)
                {
                    var vals = bm.ModPpStars[stars.Key];
                    mStream.WriteAll((byte)stars.Key);
                    mStream.WriteAll(vals.Count);

                    foreach (var val in vals)
                    {
                        mStream.WriteAll(val.Key);
                        mStream.WriteAll(val.Value);
                    }
                }
                return mStream.ToArray();
            }
        }
        public static void DeSerializeStars(this Beatmap bm, byte[] starsData)
        {
            using (MemoryStream mStream = new MemoryStream())
            {
                mStream.Write(starsData, 0, starsData.Length);
                mStream.Position = 0;
                _binaryReader = new BinaryReader(mStream);
                int modesCount = _binaryReader.ReadInt32();
                for (int i = 0; i < modesCount; i++)
                {
                    var key = (PlayMode)_binaryReader.ReadInt32();
                    if (!bm.ModPpStars.ContainsKey(key))
                        bm.ModPpStars.Add(key, new Dictionary<int, double>());
                    var dict = bm.ModPpStars[key];
                    var starsCount = _binaryReader.ReadInt32();
                    for (int j = 0; j < starsCount; j++)
                    {
                        dict.Add(_binaryReader.ReadInt32(), _binaryReader.ReadDouble());
                    }
                }
            }
        }

        public static Dictionary<string, string> GetDict(this Beatmap bm, bool empty = false)
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
                dict = new Dictionary<string, string>
                {
                    {"!TitleRoman!", bm.TitleRoman},
                    {"!ArtistRoman!", bm.ArtistRoman},
                    {"!TitleUnicode!", bm.TitleUnicode},
                    {"!ArtistUnicode!", bm.ArtistUnicode},
                    {"!MapArtistTitle!", string.Format("{0} - {1}", bm.ArtistRoman, bm.TitleRoman) },
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
                    {"!sv!", bm.SliderVelocity.ToString(CultureInfo.InvariantCulture)},

                    {"!starsNomod!", bm.StarsNomod.ToString("##.###", CultureInfo.InvariantCulture)},
                    {"!drainingtime!", bm.DrainingTime.ToString()},
                    {"!totaltime!", bm.TotalTime.ToString()},
                    {"!previewtime!", bm.PreviewTime.ToString()},
                    {"!mapid!", bm.MapId.ToString()},
                    {"!dl!", bm.MapLink},
                    {"!mapsetid!", bm.MapSetId.ToString()},
                    {"!threadid!", bm.ThreadId.ToString()},
                    {"!SL!", bm.StackLeniency.ToString(CultureInfo.InvariantCulture)},
                    {"!mode!", bm.PlayMode.GetHashCode().ToString()},
                    {"!source!", bm.Source},
                    {"!dir!", bm.Dir},
                    {"!lb!", Environment.NewLine}
                };
            }

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