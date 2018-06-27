using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using CollectionManager.Enums;
using StreamCompanionTypes.DataTypes;

namespace OsuSongsFolderWatcher
{
    public sealed class OsuFileParser
    {
        public static OsuFileParser Instance { get; } = new OsuFileParser();


        //TODO: support older osu file versions (Query in-game api using md5 hash??) 
        private readonly Dictionary<string, Action<List<string>, Beatmap>> _sections = new Dictionary<string, Action<List<string>, Beatmap>>() { { "General", ParseGeneral }, { "Editor", ParseEditor }, { "Metadata", ParseMetadata }, { "Difficulty", ParseDifficulty }, { "Events", IgnoreSection }, { "TimingPoints", IgnoreSection }, { "Colours", IgnoreSection }, { "HitObjects", ParseHitObjects } };

        private static void ParseHitObjects(List<string> lines, Beatmap map)
        {
            short circles = 0;
            short spinners = 0;
            short sliders = 0;

            foreach (var line in lines)
            {
                if (line.StartsWith("[") || string.IsNullOrWhiteSpace(line))
                    continue;

                var hitobject = line;
                if (!string.IsNullOrWhiteSpace(hitobject))
                {
                    var hitObjectType = Convert.ToInt32(hitobject.Split(',')[3]);
                    if ((hitObjectType & 1) != 0)
                    {
                        circles++;
                    }
                    else if ((hitObjectType & 2) != 0)
                    {
                        sliders++;
                    }
                    else if ((hitObjectType & 8) != 0)
                    {
                        spinners++;
                    }
                }
            }

            map.Circles = circles;
            map.Spinners = spinners;
            map.Sliders = sliders;
        }

        private static void ParseMetadata(List<string> lines, Beatmap map)
        {
            foreach (var line in lines)
            {
                if (line.StartsWith("[") || string.IsNullOrWhiteSpace(line))
                    continue;

                var val = SplitParam(line);
                switch (val.Key)
                {
                    case "Title":
                        map.TitleRoman = val.Value;
                        break;
                    case "TitleUnicode":
                        map.TitleUnicode = val.Value;
                        break;
                    case "Artist":
                        map.ArtistRoman = val.Value;
                        break;
                    case "ArtistUnicode":
                        map.ArtistUnicode = val.Value;
                        break;
                    case "Creator":
                        map.Creator = val.Value;
                        break;
                    case "Version":
                        //map. = val.Value;
                        break;
                    case "Source":
                        map.Source = val.Value;
                        break;
                    case "Tags":
                        map.Tags = val.Value;
                        break;
                    case "BeatmapID":
                        map.MapId = Convert.ToInt32(val.Value, CultureInfo.InvariantCulture);
                        break;
                    case "BeatmapSetID":
                        map.MapSetId = Convert.ToInt32(val.Value, CultureInfo.InvariantCulture);
                        break;
                }
            }


        }
        private static void ParseDifficulty(List<string> lines, Beatmap map)
        {
            foreach (var line in lines)
            {
                if (line.StartsWith("[") || string.IsNullOrWhiteSpace(line))
                    continue;

                var val = SplitParam(line);
                switch (val.Key)
                {
                    case "HPDrainRate":
                        map.HpDrainRate = Convert.ToSingle(val.Value, CultureInfo.InvariantCulture);
                        break;
                    case "CircleSize":
                        map.CircleSize = Convert.ToSingle(val.Value, CultureInfo.InvariantCulture);
                        break;
                    case "OverallDifficulty":
                        map.OverallDifficulty = Convert.ToSingle(val.Value, CultureInfo.InvariantCulture);
                        break;
                    case "ApproachRate":
                        map.ApproachRate = Convert.ToSingle(val.Value, CultureInfo.InvariantCulture);
                        break;
                    case "SliderMultiplier":
                        map.SliderVelocity = Convert.ToDouble(val.Value, CultureInfo.InvariantCulture);
                        break;
                    case "SliderTickRate":
                        //map. = val.Value;
                        break;
                }
            }
        }

        private static void IgnoreSection(List<string> lines, Beatmap map)
        {
            //ignored
        }
        private static void ParseEditor(List<string> lines, Beatmap map)
        {
            //ignored
        }

        private static KeyValuePair<string, string> SplitParam(string line)
        {
            var splited = line.Split(new[] { ':' }, 2);
            if (splited.Length == 2)
                return new KeyValuePair<string, string>(splited[0], splited[1].Trim());
            return new KeyValuePair<string, string>(splited[0], "");
        }
        private static void ParseGeneral(List<string> lines, Beatmap map)
        {
            foreach (var line in lines)
            {
                if (line.StartsWith("[") || string.IsNullOrWhiteSpace(line))
                    continue;

                var val = SplitParam(line);

                switch (val.Key)
                {
                    case "AudioFilename":
                        map.Mp3Name = val.Value;
                        break;
                    case "AudioLeadIn":
                        map.AudioOffset = Convert.ToInt16(val.Value, CultureInfo.InvariantCulture);
                        break;
                    case "PreviewTime":
                        map.PreviewTime = Convert.ToInt32(val.Value, CultureInfo.InvariantCulture);
                        break;
                    case "Countdown":
                        //map. = value;
                        break;
                    case "SampleSet":
                        //map. = value;
                        break;
                    case "StackLeniency":
                        map.StackLeniency = Convert.ToSingle(val.Value, CultureInfo.InvariantCulture);
                        break;
                    case "Mode":
                        map.PlayMode = (PlayMode)Convert.ToByte(val.Value, CultureInfo.InvariantCulture);
                        break;
                    case "LetterboxInBreaks":
                        //map. = value;
                        break;
                    case "WidescreenStoryboard":
                        //map. = value;
                        break;
                }
            }


        }

        public Beatmap ReadBeatmapData(string fullFileDir)
        {
            var map = new Beatmap();
            var directoryInfo = new DirectoryInfo(fullFileDir).Parent;
            map.Dir = directoryInfo?.Name ?? "";
            map.OsuFileName = Path.GetFileName(fullFileDir);

            var filename = Path.GetFileNameWithoutExtension(map.OsuFileName);
            if (!filename.EndsWith("]"))
            {
                var idx = filename.LastIndexOf("]", StringComparison.InvariantCulture);
                try
                {
                    filename = filename.Remove(idx + 1);
                }
                catch (ArgumentOutOfRangeException e)
                {
                    //TODO: create unit test for this function to figure out why does this still break from time to time
                }
            }
            map.DiffName = BeatmapHelpers.GetDiffFromString(filename);

            List<string> lines = new List<string>();
            Thread.Sleep(50);
            FileInfo fileInfo = new FileInfo(fullFileDir);
            while (BeatmapHelpers.FileIsLocked(fileInfo))
            {
                Thread.Sleep(1);
            }
            int tryCount = 0;
            do
            {
                try
                {
                    tryCount++;
                    using (var stream = File.OpenRead(fullFileDir))
                    {
                        using (var fileHandle = new StreamReader(stream, true))
                        {
                            while (!fileHandle.EndOfStream)
                            {
                                lines.Add(fileHandle.ReadLine());
                            }
                        }
                    }
                    using (var md5 = System.Security.Cryptography.MD5.Create())
                    {
                        using (var stream = File.OpenRead(fullFileDir))
                        {
                            map.Md5 = BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLower();
                        }
                    }
                    tryCount = 5;
                }
                catch (IOException e)
                {
                    if (tryCount < 2)
                        Thread.Sleep(1000);
                }
            } while (tryCount < 2);
            if (tryCount != 5)
                return map;



            if (string.IsNullOrEmpty(map.TitleUnicode))
            {
                map.TitleUnicode = map.TitleRoman;
            }
            if (string.IsNullOrEmpty(map.ArtistUnicode))
            {
                map.ArtistUnicode = map.ArtistRoman;
            }
            map.EditDate = DateTime.Now;
            map.LastPlayed = DateTime.MinValue;
            map.LastSync = DateTime.Now;

            ParseLines(lines, map);


            return map;
        }

        private void ParseLines(List<string> lines, Beatmap map)
        {
            var FileSections = GetSections(lines);

            for (int i = 0; i < FileSections.Count; i++)
            {

                if (!_sections.ContainsKey(FileSections[i].SectionName))
                    throw new NotImplementedException(string.Format("This osu file format is not yet supported (unknown section \"{0}\")", FileSections[i].SectionName));

                var linesToProcess = lines.GetRange(FileSections[i].Start, FileSections[i].Count);

                _sections[FileSections[i].SectionName](linesToProcess, map);
            }
        }

        private class Section
        {
            public string SectionName { get; set; }
            public int Start { get; set; }
            public int End { get; set; }

            public int Count
            {
                get { return 1 + this.End - this.Start; }
                set
                {
                }
            }
        }
        private List<Section> GetSections(List<string> lines)
        {
            var retSections = new List<Section>();


            //Get all start indexes of all sections in file
            var sectionStarts = new List<int>();
            int LastSectionPostion = 0;
            //TODO: proper fix for this error
            try
            {
                do
                {
                    LastSectionPostion = lines.FindIndex(LastSectionPostion + 1, startsWithOpeningBracket);
                    sectionStarts.Add(LastSectionPostion);
                } while (LastSectionPostion != -1);
            }
            catch (IndexOutOfRangeException e)
            {
                Console.WriteLine("There was a problem with processing new beatmap. | {0}", string.Join("+", lines.ToArray()));
            }
            sectionStarts.Remove(-1);

            //Construct our section(s) classes
            for (int i = 0; i < sectionStarts.Count; i++)
            {
                var section = new Section();
                section.SectionName = lines[sectionStarts[i]].Trim('[', ']');
                section.Start = sectionStarts[i];
                if (sectionStarts.Count > i + 1)
                {
                    section.End = sectionStarts[i + 1] - 1;
                }
                else
                {
                    section.End = lines.Count - 1;
                }
                retSections.Add(section);
            }


            return retSections;
        }

        private bool startsWithOpeningBracket(string line)
        {
            return line.StartsWith("[");
        }

        #region osu v14 file format
        //[General]
        //AudioFilename: jealous.mp3
        //AudioLeadIn: 0
        //PreviewTime: 177098
        //Countdown: 0
        //SampleSet: Normal
        //StackLeniency: 0.7
        //Mode: 1
        //LetterboxInBreaks: 0
        //WidescreenStoryboard: 0

        //[Editor]
        //Bookmarks: 140080
        //DistanceSpacing: 1
        //BeatDivisor: 4
        //GridSize: 4
        //TimelineZoom: 1.6

        //[Metadata]
        //Title:Ryokugan no Jealousy
        //TitleUnicode:緑眼のジェラシー
        //Artist:Dark PHOENiX
        //ArtistUnicode:Dark PHOENiX
        //Creator:EvilElvis
        //Version:31's Oni
        //Source:東方Project
        //Tags:Mizuhashi Parsee Green-Eyed Mapper 31 Arrow Realize
        //BeatmapID:849236
        //BeatmapSetID:382455

        //[Difficulty]
        //HPDrainRate:5
        //CircleSize:5
        //OverallDifficulty:6
        //ApproachRate:5
        //SliderMultiplier:1.4
        //SliderTickRate:1
        #endregion

    }
}