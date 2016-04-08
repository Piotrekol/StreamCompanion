#define GetStarsCombinations
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using osu_StreamCompanion.Code.Core.DataTypes;
using osu_StreamCompanion.Code.Interfeaces;
using osu_StreamCompanion.Code.Windows;
namespace osu_StreamCompanion.Code.Modules.MapDataFinders
{
    public class OsuDatabaseLoader
    {
        private readonly List<IModParser> _modParser;
        private readonly ILogger _logger;
        private readonly IMapDataStorer _mapDataStorer;
        private readonly MainWindowUpdater _mainWindowUpdater;
        private FileStream _fileStream;
        private BinaryReader _binaryReader;
        private Exception _exception;
        private readonly Beatmap _tempBeatmap = new Beatmap();
        #if GetStarsCombinations && DEBUG
        private List<int> AllPossibleModCombinations = new List<int>();
        #endif
        public int MapsWithNoId { get; private set; }
        public string Username { get; private set; }
        public int FileDate { get; private set; }
        public int ExpectedNumberOfMapSets { get; private set; }
        public int ExpectedNumOfBeatmaps { get; private set; } = -1;
        private bool _stopProcessing;
        private int _numberOfLoadedBeatmaps;

        public OsuDatabaseLoader(ILogger logger, List<IModParser> modParser, IMapDataStorer mapDataStorer, MainWindowUpdater mainWindowHandle)
        {
            _mapDataStorer = mapDataStorer;
            _logger = logger;
            _modParser = modParser;
            _mainWindowUpdater = mainWindowHandle;
        }
        public void LoadDatabase(string fullOsuDbPath)
        {
            string loadedLog;
            if (FileExists(fullOsuDbPath))
            {
                _fileStream = new FileStream(fullOsuDbPath, FileMode.Open, FileAccess.Read);
                _binaryReader = new BinaryReader(_fileStream);
                if (DatabaseContainsData())
                {
                    ReadDatabaseEntries();
                }
                DestoryReader();
                loadedLog = $"Loaded {_numberOfLoadedBeatmaps} beatmaps";
            }
            else
            {
                loadedLog = "Could not find osu!.db file!";
            }
            _logger.Log(loadedLog, LogLevel.Advanced);
            _mainWindowUpdater.BeatmapsLoaded = loadedLog;
        }

        private void ReadDatabaseEntries()
        {
            _mapDataStorer.StartMassStoring();
            for (_numberOfLoadedBeatmaps = 0; _numberOfLoadedBeatmaps < ExpectedNumOfBeatmaps; _numberOfLoadedBeatmaps++)
            {
                if (_numberOfLoadedBeatmaps % 100 == 0)
                {
                    _mainWindowUpdater.BeatmapsLoaded = $"{_numberOfLoadedBeatmaps}/{ExpectedNumOfBeatmaps} please wait...";
                }
                if (_stopProcessing)
                {
                    _logger.Log("Something went wrong while processing beatmaps(database is corrupt or its format changed)", LogLevel.Basic);
                    _logger.Log("Try restarting your osu! first before reporting this problem.", LogLevel.Basic);
                    _logger.Log("Exception: {0},{1}", LogLevel.Basic, _exception.Message, _exception.StackTrace);
                    return;
                }
                ReadNextBeatmap();
            }
            #if GetStarsCombinations && DEBUG
            var sb = new StringBuilder();
            foreach (var modCombination in AllPossibleModCombinations)
            {
                sb.AppendFormat("{0} : {1}{2}", modCombination, _modParser[0].GetModsFromEnum(modCombination),Environment.NewLine);
            }
            File.WriteAllText(@"D:\mods.txt",sb.ToString());
            #endif
            _mapDataStorer.EndMassStoring();
        }
        private void ReadNextBeatmap()
        {
            _tempBeatmap.InitEmptyValues();
            try
            {
                ReadMapHeader(_tempBeatmap);
                ReadMapInfo(_tempBeatmap);
                ReadTimingPoints(_tempBeatmap);
                ReadMapMetaData(_tempBeatmap);
            }
            catch (Exception e)
            {
                _exception = e;
                _stopProcessing = true;
                return;
            }
            _mapDataStorer.StoreBeatmap(_tempBeatmap);
        }

        private void ReadMapMetaData(Beatmap beatmap)
        {
            beatmap.MapId = Math.Abs(_binaryReader.ReadInt32());
            if (beatmap.MapId == 0) MapsWithNoId++;

            beatmap.MapSetId = Math.Abs(_binaryReader.ReadInt32());
            beatmap.ThreadId = Math.Abs(_binaryReader.ReadInt32());
            beatmap.MapRating = _binaryReader.ReadInt32();
            beatmap.Offset = _binaryReader.ReadInt16();
            beatmap.StackLeniency = _binaryReader.ReadSingle();
            beatmap.Mode = _binaryReader.ReadByte();
            beatmap.Source = ReadString();
            beatmap.Tags = ReadString();
            beatmap.AudioOffset = _binaryReader.ReadInt16();
            beatmap.LetterBox = ReadString();
            beatmap.Played = !_binaryReader.ReadBoolean();
            beatmap.LastPlayed = GetDate();
            beatmap.IsOsz2 = _binaryReader.ReadBoolean();
            beatmap.Dir = ReadString();
            beatmap.LastSync = GetDate();
            beatmap.DisableHitsounds = _binaryReader.ReadBoolean();
            beatmap.DisableSkin = _binaryReader.ReadBoolean();
            beatmap.DisableSb = _binaryReader.ReadBoolean();
            _binaryReader.ReadBoolean();
            beatmap.BgDim = _binaryReader.ReadInt16();
            //bytes not analysed.
            _binaryReader.BaseStream.Seek(8, SeekOrigin.Current);
        }
        private void ReadTimingPoints(Beatmap beatmap)
        {
            int amountOfTimingPoints = _binaryReader.ReadInt32();
            double minBpm = double.MaxValue,
            maxBpm = double.MinValue;
            double bpmDelay, time;
            bool InheritsBPM;
            for (int i = 0; i < amountOfTimingPoints; i++)
            {
                bpmDelay = _binaryReader.ReadDouble();
                time = _binaryReader.ReadDouble();
                InheritsBPM = _binaryReader.ReadBoolean();
                if (InheritsBPM)
                {
                    if (60000 / bpmDelay < minBpm)
                        minBpm = 60000 / bpmDelay;
                    if (60000 / bpmDelay > maxBpm)
                        maxBpm = 60000 / bpmDelay;
                }
            }
            beatmap.MaxBpm = maxBpm;
            beatmap.MinBpm = minBpm;
        }
        private void ReadMapInfo(Beatmap beatmap)
        {
            beatmap.State = _binaryReader.ReadByte();
            beatmap.Circles = _binaryReader.ReadInt16();
            beatmap.Sliders = _binaryReader.ReadInt16();
            beatmap.Spinners = _binaryReader.ReadInt16();
            beatmap.EditDate = GetDate();
            beatmap.ApproachRate = _binaryReader.ReadSingle();
            beatmap.CircleSize = _binaryReader.ReadSingle();
            beatmap.HpDrainRate = _binaryReader.ReadSingle();
            beatmap.OverallDifficulty = _binaryReader.ReadSingle();
            beatmap.SliderMultiplicer = _binaryReader.ReadDouble();

            for (int j = 0; j < 4; j++)
            {
                ReadStarsData(beatmap);
            }
            beatmap.DrainingTime = _binaryReader.ReadInt32();
            beatmap.TotalTime = _binaryReader.ReadInt32();
            beatmap.PreviewTime = _binaryReader.ReadInt32();
        }

        private void ReadStarsData(Beatmap beatmap)
        {
            int num = _binaryReader.ReadInt32();
            if (num < 0)
            {
                return;
            }

            for (int j = 0; j < num; j++)
            {
                int modEnum = (int)ConditionalRead();
                Double stars = (Double) ConditionalRead();
                #if GetStarsCombinations && DEBUG
                if (!AllPossibleModCombinations.Contains(modEnum))
                    AllPossibleModCombinations.Add(modEnum);
                #endif
                if (!beatmap.ModPpStars.ContainsKey(modEnum))
                {
                    beatmap.ModPpStars.Add(modEnum, Math.Round(stars, 2));
                }
                else
                {
                    beatmap.ModPpStars[modEnum] = Math.Round(stars, 2);
                }

            }

        }
        private object ConditionalRead()
        {
            switch (_binaryReader.ReadByte())
            {
                case 1:
                    {
                        return _binaryReader.ReadBoolean();
                    }
                case 2:
                    {
                        return _binaryReader.ReadByte();
                    }
                case 3:
                    {
                        return _binaryReader.ReadUInt16();
                    }
                case 4:
                    {
                        return _binaryReader.ReadUInt32();
                    }
                case 5:
                    {
                        return _binaryReader.ReadUInt64();
                    }
                case 6:
                    {
                        return _binaryReader.ReadSByte();
                    }
                case 7:
                    {
                        return _binaryReader.ReadInt16();
                    }
                case 8:
                    {
                        return _binaryReader.ReadInt32();
                    }
                case 9:
                    {
                        return _binaryReader.ReadInt64();
                    }
                case 10:
                    {
                        return _binaryReader.ReadChar();
                    }
                case 11:
                    {
                        return _binaryReader.ReadString();
                    }
                case 12:
                    {
                        return _binaryReader.ReadSingle();
                    }
                case 13:
                    {
                        return _binaryReader.ReadDouble();
                    }
                case 14:
                    {
                        return _binaryReader.ReadDecimal();
                    }
                case 15:
                    {
                        return GetDate();
                    }
                case 16:
                    {
                        int num = _binaryReader.ReadInt32();
                        if (num > 0)
                        {
                            return _binaryReader.ReadBytes(num);
                        }
                        if (num < 0)
                        {
                            return null;
                        }
                        return new byte[0];

                    }
                case 17:
                    {
                        int num = _binaryReader.ReadInt32();
                        if (num > 0)
                        {
                            return _binaryReader.ReadChars(num);
                        }
                        if (num < 0)
                        {
                            return null;
                        }
                        return new char[0];
                    }
                case 18:
                    {
                        throw new NotImplementedException();
                    }
                default:
                    {
                        return null;
                    }
            }
        }
        private DateTime GetDate()
        {
            long ticks = _binaryReader.ReadInt64();
            if (ticks < 0L)
            {
                return new DateTime();
            }
            try
            {
                return new DateTime(ticks, DateTimeKind.Utc);
            }
            catch (Exception e)
            {
                _exception = e;
                _stopProcessing = true;
                return new DateTime();
            }
        }
        private void ReadMapHeader(Beatmap beatmap)
        {
            beatmap.ArtistRoman = ReadString().Trim();
            beatmap.ArtistUnicode = ReadString().Trim();
            beatmap.TitleRoman = ReadString().Trim();
            beatmap.TitleUnicode = ReadString().Trim();
            beatmap.Creator = ReadString().Trim();
            beatmap.DiffName = ReadString().Trim();
            beatmap.Mp3Name = ReadString().Trim();
            beatmap.Md5 = ReadString().Trim();
            beatmap.OsuFileName = ReadString().Trim();
        }
        private string ReadString()
        {
            try
            {
                if (_binaryReader.ReadByte() == 11)
                {
                    return _binaryReader.ReadString();
                }
                return "";
            }
            catch { _stopProcessing = true; return ""; }
        }
        private bool DatabaseContainsData()
        {
            FileDate = _binaryReader.ReadInt32();
            ExpectedNumberOfMapSets = _binaryReader.ReadInt32();
            _logger.Log(string.Format("Expected number of mapSets: {0}", ExpectedNumberOfMapSets), LogLevel.Debug);
            try
            {
                bool something = _binaryReader.ReadBoolean();
                DateTime a = GetDate().ToLocalTime();
                _binaryReader.BaseStream.Seek(1, SeekOrigin.Current);
                Username = _binaryReader.ReadString();
                ExpectedNumOfBeatmaps = _binaryReader.ReadInt32();
                _binaryReader.BaseStream.Seek(4, SeekOrigin.Current);
                _logger.Log(string.Format("Expected number of beatmaps: {0}", ExpectedNumOfBeatmaps), LogLevel.Debug);

                if (ExpectedNumOfBeatmaps < 0)
                {
                    return false;
                }
            }
            catch { return false; }
            return true;
        }
        private void DestoryReader()
        {
            _fileStream.Close();
            _binaryReader.Close();
            _fileStream.Dispose();
            _binaryReader.Dispose();
        }
        private bool FileExists(string fullPath)
        {
            return !string.IsNullOrEmpty(fullPath) && File.Exists(fullPath);
        }
    }
}
