using osu_StreamCompanion.Code.Helpers;
using StreamCompanionTypes.DataTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using StreamCompanionTypes.Enums;
using StreamCompanionTypes.Interfaces.Services;

namespace osu_StreamCompanion.Code.Core
{
    public class SqliteConnector : IDisposable, IMapDataSaver
    {
        private readonly ILogger _logger;
        private SQLiteConnection _mDbConnection;
        private string DbFilename = "StreamCompanionCacheV2.db";
        private int _schemaVersion = 8;
        /*
         * v1 - caching improvments, cfg table struct change
         * v2 - forcing db reload due to possibility of malformed data saved from v1(duplicated maps across tables)
         * v3 - StackLeniency beatmap field is now nullable because of 2018 aspire maps entries.
         * v4 - Added beatmapChecksum column
         * v5 - removed Md5 Unique constrain, Added (Dir,osuFileName) unique index, removed VideoDir column
         * v6 - checksum is now a string due to massive problems with GetHashCode collisions with huge amounts of beatmaps
         * v7 - Added MainBpm column
         * v8 - make SliderVelocity nullable
             */
        private SQLiteCommand _insertSql;
        private SQLiteTransaction _transation;
        public bool MassInsertIsActive => _transation != null;
        private readonly BeatmapTbStruct _tableStruct = new BeatmapTbStruct();
        private class BeatmapTbStruct
        {
            public List<string> Fieldnames;
            public List<string> Type;
            public List<string> TypeModifiers;

            public string GetFieldnames(string PreededEachWith = "")
            {
                var ret = new StringBuilder();
                foreach (var fieldname in Fieldnames)
                {
                    ret.AppendFormat("{0}{1},", PreededEachWith, fieldname);
                }
                ret.Remove(ret.Length - 1, 1);

                return ret.ToString();
            }
            public string GetTableDef()
            {
                var ret = new StringBuilder();
                ret.Append('(');
                for (int i = 0; i < Fieldnames.Count; i++)
                {
                    ret.AppendFormat("{0} {1} {2},", Fieldnames[i], Type[i], TypeModifiers[i]);
                }
                ret.Remove(ret.Length - 1, 1);
                ret.Append(')');
                return ret.ToString();
            }
        }
        public SqliteConnector(ILogger logger)
        {
            _logger = logger;

            _tableStruct.Fieldnames = new List<string>(new[] { "Raw", "TitleRoman", "ArtistRoman", "TitleUnicode", "ArtistUnicode", "Creator", "DiffName", "Mp3Name", "Md5", "OsuFileName", "MaxBpm", "MinBpm", "Tags", "State", "Circles", "Sliders", "Spinners", "EditDate", "ApproachRate", "CircleSize", "HpDrainRate", "OverallDifficulty", "SliderVelocity", "DrainingTime", "TotalTime", "PreviewTime", "MapId", "MapSetId", "ThreadId", "MapRating", "Offset", "StackLeniency", "Mode", "Source", "AudioOffset", "LetterBox", "Played", "LastPlayed", "IsOsz2", "Dir", "LastSync", "DisableHitsounds", "DisableSkin", "DisableSb", "BgDim", "Somestuff", "StarsOsu", "BeatmapChecksum", "MainBpm" });
            _tableStruct.Type = new List<string>(new[] { "VARCHAR", "VARCHAR", "VARCHAR", "VARCHAR", "VARCHAR", "VARCHAR", "VARCHAR", "VARCHAR", "VARCHAR", "VARCHAR", "DOUBLE", "DOUBLE", "VARCHAR", "INTEGER", "INTEGER", "INTEGER", "INTEGER", "DATETIME", "DOUBLE", "DOUBLE", "DOUBLE", "DOUBLE", "DOUBLE", "INTEGER", "INTEGER", "INTEGER", "INTEGER", "INTEGER", "INTEGER", "INTEGER", "INTEGER", "DOUBLE", "INTEGER", "VARCHAR", "INTEGER", "VARCHAR", "BOOL", "DATETIME", "BOOL", "VARCHAR", "DATETIME", "BOOL", "BOOL", "BOOL", "INTEGER", "INTEGER", "BLOB", "VARCHAR", "DOUBLE" });
            _tableStruct.TypeModifiers = new List<string>(new[] { "NOT NULL", "NOT NULL", "NOT NULL ", "NOT NULL ", "NOT NULL ", "NOT NULL ", "NOT NULL ", "NOT NULL ", "NOT NULL", "NOT NULL ", "NOT NULL ", "NOT NULL ", "NOT NULL ", "NOT NULL ", "NOT NULL ", "NOT NULL ", "NOT NULL ", "NOT NULL ", "NOT NULL ", "NOT NULL ", "NOT NULL ", "NOT NULL ", "", "NOT NULL", "NOT NULL", "NOT NULL", "NOT NULL ", "NOT NULL", "NOT NULL", "NOT NULL", "NOT NULL", "", "NOT NULL", "NOT NULL", "NOT NULL", "NOT NULL", "", "NOT NULL", "", "NOT NULL", "NOT NULL", "", "", "", "NOT NULL", "NOT NULL", "NOT NULL", "NOT NULL", "NOT NULL" });

            try
            {
                Init();
            }
            catch (SQLiteException)
            {
                Init(true);
            }
        }

        public void ResetDatabase()
        {
            CloseConnection();
            Init(true);
        }
        private void Init(bool reset = false)
        {
            CreateFile(DbFilename, reset);
            _mDbConnection =
                new SQLiteConnection("Data Source=" + DbFilename + ";Version=3;New=False;Compress=True;");
            OpenConnection();
            CreateTables();
        }

        private void CreateTables()
        {
            bool recteate = false;
            var result = Query("SELECT name FROM sqlite_master WHERE type='table' AND name='cfg';");
            if (result.HasRows)
            {
                result.Dispose();
                //Check for old table struct with never got used(No data inside)
                result = Query("SELECT * from cfg");
                if (!result.HasRows)
                {
                    //Old struct - drop everything 
                    recteate = true;
                }
                else
                {
                    //new struct - check schema version
                    result.Dispose();
                    result = Query("SELECT * from `cfg` where `Key` = 'SchemaVersion' ");
                    if (!result.HasRows || !result.Read())
                        recteate = true;
                    else
                    {
                        var version = Int32.Parse(result.GetString(1));
                        if (version != _schemaVersion)
                            recteate = true;
                    }
                    result.Dispose();


                }
            }
            else
                recteate = true;

            if (recteate)
            {
                NonQuery("DROP TABLE IF EXISTS `cfg`;");
                NonQuery("DROP TABLE IF EXISTS `withoutID`;");
                NonQuery("DROP TABLE IF EXISTS `withID`;");


                string sql = "CREATE  TABLE IF NOT EXISTS withoutID " + _tableStruct.GetTableDef();
                NonQuery(sql);

                sql = "CREATE  TABLE IF NOT EXISTS withID " + _tableStruct.GetTableDef();
                NonQuery(sql);

                sql = "CREATE TABLE IF NOT EXISTS `cfg` (`Key` TEXT, `Value` TEXT)";
                NonQuery(sql);
                NonQuery($"insert into `cfg` values ('SchemaVersion','{_schemaVersion}')");

                NonQuery("CREATE UNIQUE INDEX \"MapLocationIndex1\" ON \"withID\" (\r\n\t\"Dir\",\r\n\t\"OsuFileName\"\r\n)");
                NonQuery("CREATE UNIQUE INDEX \"MapLocationIndex2\" ON \"withoutID\" (\r\n\t\"Dir\",\r\n\t\"OsuFileName\"\r\n)");
                NonQuery("CREATE UNIQUE INDEX \"BeatmapChecksumIndex1\" ON \"withID\" (\r\n\t\"BeatmapChecksum\")");
                NonQuery("CREATE UNIQUE INDEX \"BeatmapChecksumIndex2\" ON \"withoutID\" (\r\n\t\"BeatmapChecksum\")");
                NonQuery("CREATE INDEX \"MapIdIndex1\" ON \"WithId\"(\r\n\"MapId\"\r\n)");
            }

            NonQuery("DROP TABLE IF EXISTS `Temp`;");
            NonQuery("CREATE TABLE IF NOT EXISTS Temp " + _tableStruct.GetTableDef());
            NonQuery("CREATE UNIQUE INDEX \"MapLocationIndex3\" ON \"Temp\" (\r\n\t\"Dir\",\r\n\t\"OsuFileName\"\r\n)");

        }

        public void NonQuery(string query)
        {
            _insertSql = new SQLiteCommand(query, _mDbConnection);
            _insertSql.ExecuteNonQuery();

        }
        private string lastQuery = string.Empty;
        private string lastQueryParameters = string.Empty;
        private SQLiteDataReader Query(string query, IDictionary<string, string> replacements)
        {
            SQLiteCommand cmd = new SQLiteCommand(query, _mDbConnection);
            foreach (var replacement in replacements)
            {
                cmd.Parameters.Add(replacement.Key, DbType.String).Value = replacement.Value;
            }

            lastQuery = cmd.CommandText;
            lastQueryParameters = string.Join(" |~| ", replacements.Select((k, v) => $"{k} = {v}").ToList());

            return cmd.ExecuteReader();
        }
        public SQLiteDataReader Query(string query)
        {
            SQLiteCommand cmd = new SQLiteCommand(query, _mDbConnection);
            return cmd.ExecuteReader();
        }
        private void CreateFile(string filename, bool reset = false)
        {
            try
            {
                if (File.Exists("Test.db"))
                {
                    File.Delete("Test.db");
                }
                if (File.Exists("StreamCompanionCache.db"))
                {
                    File.Delete("StreamCompanionCache.db");
                }
                if (reset && File.Exists(filename))
                {
                    File.Delete(filename);
                }
                if (!File.Exists(filename))
                {
                    SQLiteConnection.CreateFile(filename);
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new NonLoggableException(ex, "Could not save beatmap cache file due to insuffisient premissions" +
                    Environment.NewLine + "Please move this exectuable into a non-system folder");
            }
        }
        private void OpenConnection()
        {
            _mDbConnection.Open();
        }
        private void CloseConnection()
        {
            _mDbConnection.Close();
            _mDbConnection.Dispose();
            //SQLiteConnection depends on GC for closing db file handles...
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
        public void Dispose()
        {
            CloseConnection();
        }

        public void StartMassStoring()
        {
            _transation = _mDbConnection.BeginTransaction();
        }

        public void EndMassStoring()
        {
            _transation.Commit();
            _transation.Dispose();
            _transation = null;
        }

        public void RemoveBeatmap(string hash)
        {
            var sql = string.Format("DELETE FROM withID WHERE Md5 = '{0}'", hash);
            NonQuery(sql);
            sql = string.Format("DELETE FROM withoutID WHERE Md5 = '{0}'", hash);
            NonQuery(sql);
        }

        public void RemoveBeatmaps(IList<string> checksums)
        {
            var strHashes = new StringBuilder();
            foreach (var hash in checksums)
            {
                strHashes.AppendFormat("'{0}',", hash);
            }

            var sql = string.Format("DELETE FROM withID WHERE BeatmapChecksum IN ({0})", strHashes.ToString().TrimEnd(','));
            NonQuery(sql);

            sql = string.Format("DELETE FROM withoutID WHERE BeatmapChecksum IN ({0})", strHashes.ToString().TrimEnd(','));
            NonQuery(sql);

        }
        public void StoreBeatmap(IBeatmap beatmap)
        {
            if (float.IsNaN(beatmap.ApproachRate))
                beatmap.ApproachRate = 0;
            if (float.IsNaN(beatmap.CircleSize))
                beatmap.CircleSize = 0;
            if (float.IsNaN(beatmap.OverallDifficulty))
                beatmap.OverallDifficulty = 0;

            string sql;
            if (beatmap.MapId != 0)
                sql =
                    string.Format("INSERT OR REPLACE INTO withID ({0}) VALUES ({1})", _tableStruct.GetFieldnames(), _tableStruct.GetFieldnames("@"));
            else
                sql =
                    string.Format("INSERT OR REPLACE INTO withoutID ({0}) VALUES ({1})", _tableStruct.GetFieldnames(), _tableStruct.GetFieldnames("@"));

            _insertSql = new SQLiteCommand(sql, _mDbConnection);
            FillBeatmapParameters(beatmap);
            _insertSql.ExecuteNonQuery();
        }

        public void StoreTempBeatmap(IBeatmap beatmap)
        {
            string sql =
                    string.Format("INSERT OR REPLACE INTO Temp ({0}) VALUES ({1})", _tableStruct.GetFieldnames(), _tableStruct.GetFieldnames("@"));
            _insertSql = new SQLiteCommand(sql, _mDbConnection);
            FillBeatmapParameters(beatmap);
            _insertSql.ExecuteNonQuery();
        }

        private void FillBeatmapParameters(IBeatmap beatmap)
        {
            _insertSql.Parameters.Add("@Raw", DbType.String).Value = $"{beatmap.TitleRoman} - {beatmap.ArtistRoman}";
            _insertSql.Parameters.Add("@TitleRoman", DbType.String).Value = beatmap.TitleRoman;
            _insertSql.Parameters.Add("@ArtistRoman", DbType.String).Value = beatmap.ArtistRoman;
            _insertSql.Parameters.Add("@TitleUnicode", DbType.String).Value = beatmap.TitleUnicode;
            _insertSql.Parameters.Add("@ArtistUnicode", DbType.String).Value = beatmap.ArtistUnicode;
            _insertSql.Parameters.Add("@Creator", DbType.String).Value = beatmap.Creator;
            _insertSql.Parameters.Add("@DiffName", DbType.String).Value = beatmap.DiffName;
            _insertSql.Parameters.Add("@Mp3Name", DbType.String).Value = beatmap.Mp3Name;
            _insertSql.Parameters.Add("@Md5", DbType.String).Value = beatmap.Md5;
            _insertSql.Parameters.Add("@OsuFileName", DbType.String).Value = beatmap.OsuFileName;
            _insertSql.Parameters.Add("@MaxBpm", DbType.Double).Value = beatmap.MaxBpm;
            _insertSql.Parameters.Add("@MinBpm", DbType.Double).Value = beatmap.MinBpm;
            _insertSql.Parameters.Add("@Tags", DbType.String).Value = beatmap.Tags;
            _insertSql.Parameters.Add("@State", DbType.Int16).Value = beatmap.State;
            _insertSql.Parameters.Add("@Circles", DbType.Int32).Value = beatmap.Circles;
            _insertSql.Parameters.Add("@Sliders", DbType.Int32).Value = beatmap.Sliders;
            _insertSql.Parameters.Add("@Spinners", DbType.Int32).Value = beatmap.Spinners;
            _insertSql.Parameters.Add("@EditDate", DbType.DateTime).Value = beatmap.EditDate ?? DateTime.MinValue;
            _insertSql.Parameters.Add("@ApproachRate", DbType.Double).Value = beatmap.ApproachRate;
            _insertSql.Parameters.Add("@CircleSize", DbType.Double).Value = beatmap.CircleSize;
            _insertSql.Parameters.Add("@HpDrainRate", DbType.Double).Value = beatmap.HpDrainRate;
            _insertSql.Parameters.Add("@OverallDifficulty", DbType.Double).Value = beatmap.OverallDifficulty;
            _insertSql.Parameters.Add("@SliderVelocity", DbType.Double).Value = beatmap.SliderVelocity;
            _insertSql.Parameters.Add("@DrainingTime", DbType.Int32).Value = beatmap.DrainingTime;
            _insertSql.Parameters.Add("@TotalTime", DbType.Int32).Value = beatmap.TotalTime;
            _insertSql.Parameters.Add("@PreviewTime", DbType.Int32).Value = beatmap.PreviewTime;
            _insertSql.Parameters.Add("@MapId", DbType.Int32).Value = beatmap.MapId;
            _insertSql.Parameters.Add("@MapSetId", DbType.Int32).Value = beatmap.MapSetId;
            _insertSql.Parameters.Add("@ThreadId", DbType.Int32).Value = beatmap.ThreadId;
            _insertSql.Parameters.Add("@MapRating", DbType.Int32).Value = 0; //TODO: store per-gamemode pass ranks
            _insertSql.Parameters.Add("@Offset", DbType.Int32).Value = beatmap.Offset;
            _insertSql.Parameters.Add("@StackLeniency", DbType.Double).Value = beatmap.StackLeniency;
            _insertSql.Parameters.Add("@Mode", DbType.Int16).Value = (byte)beatmap.PlayMode;
            _insertSql.Parameters.Add("@Source", DbType.String).Value = beatmap.Source;
            _insertSql.Parameters.Add("@AudioOffset", DbType.Int32).Value = beatmap.AudioOffset;
            _insertSql.Parameters.Add("@LetterBox", DbType.String).Value = beatmap.LetterBox;
            _insertSql.Parameters.Add("@Played", DbType.Boolean).Value = beatmap.Played;
            _insertSql.Parameters.Add("@LastPlayed", DbType.DateTime).Value = beatmap.LastPlayed;
            _insertSql.Parameters.Add("@IsOsz2", DbType.Boolean).Value = beatmap.IsOsz2;
            _insertSql.Parameters.Add("@Dir", DbType.String).Value = beatmap.Dir;
            _insertSql.Parameters.Add("@LastSync", DbType.DateTime).Value = beatmap.LastSync;
            _insertSql.Parameters.Add("@DisableHitsounds", DbType.Boolean).Value = beatmap.DisableHitsounds;
            _insertSql.Parameters.Add("@DisableSkin", DbType.Boolean).Value = beatmap.DisableSkin;
            _insertSql.Parameters.Add("@DisableSb", DbType.Boolean).Value = beatmap.DisableSb;
            _insertSql.Parameters.Add("@BgDim", DbType.Int16).Value = beatmap.BgDim;
            _insertSql.Parameters.Add("@Somestuff", DbType.Int16).Value = beatmap.Somestuff;
            _insertSql.Parameters.Add("@StarsOsu", DbType.Binary).Value = beatmap.SerializeStars();
            _insertSql.Parameters.Add("@BeatmapChecksum", DbType.String).Value = beatmap.GetChecksum();
            _insertSql.Parameters.Add("@MainBpm", DbType.Double).Value = beatmap.MainBpm;
        }
        public Beatmap GetBeatmap(int mapId)
        {
            string sql = "SELECT * FROM `withID` WHERE MapId = " + mapId;
            var reader = Query(sql);
            Beatmap beatmap = null;

            if (reader.Read())
            {
                beatmap = new Beatmap();
                beatmap.Read(reader);
            }
            else
            {
                reader.Dispose();
                sql = "SELECT * FROM `Temp` WHERE MapId = " + mapId;
                reader = Query(sql);

                if (reader.Read())
                {
                    beatmap = new Beatmap();
                    beatmap.Read(reader);
                }
                reader.Dispose();
            }
            return beatmap;
        }
        public Beatmap GetBeatmap(string mapHash)
        {
            mapHash = mapHash.ToLower();
            List<string> tableNames = new List<string> { "withID", "Temp", "withoutID" };

            Beatmap beatmap = null;
            try
            {
                foreach (var tableName in tableNames)
                {
                    string sql = $"SELECT * FROM `{tableName}` WHERE Md5 = '{mapHash}'";
                    var reader = Query(sql);

                    if (reader.Read())
                    {
                        beatmap = new Beatmap();
                        beatmap.Read(reader);
                        reader.Dispose();
                        break;
                    }

                    reader.Dispose();
                }
            }
            catch (SQLiteException e)
            {
                e.Data.Add("maphash", mapHash);
                _logger?.Log(e, LogLevel.Error);
            }

            return beatmap;
        }

        private Beatmap GetBeatmapUsingReplacements(string table, bool useRaw, Dictionary<string, string> replacements)
        {
            var retBeatmap = new Beatmap();
            bool foundData = false;
            try
            {
                if (useRaw)
                {
                    string sql = string.Format("SELECT * FROM `{0}` WHERE (Raw LIKE @Raw)", table);
                    if (replacements["@diff"] != string.Empty)
                    {
                        sql += " AND DiffName LIKE @diff";
                    }
                    var reader = Query(sql, replacements);
                    if (reader.Read())
                    {
                        retBeatmap.Read(reader);
                        foundData = true;
                    }
                }
                else
                {
                    var sql = string.Format(
                        "SELECT * FROM `{0}` WHERE (TitleRoman LIKE @title OR TitleUnicode LIKE @title) AND (ArtistRoman LIKE @artist OR ArtistUnicode LIKE @artist)",
                        table);
                    if (replacements["@diff"] != string.Empty)
                    {
                        sql += " AND DiffName LIKE @diff";
                    }
                    var reader = Query(sql, replacements);
                    if (reader.Read())
                    {
                        retBeatmap.Read(reader);
                        foundData = true;
                    }
                }
            }
            catch (SQLiteException e)
            {
                var exception = new Exception($"GetBeatmapUsingReplacementsException: ''{lastQuery}'', ''{lastQueryParameters}''", e);
                _logger.Log(exception, LogLevel.Error);
            }
            return foundData ? retBeatmap : null;

        }
        public Beatmap GetBeatmap(string artist, string title, string diff, string raw)
        {
            var pars = new Dictionary<string, string>();
            pars.Add("@artist", artist);
            pars.Add("@title", title);
            pars.Add("@diff", diff);
            pars.Add("@Raw", raw);
            Beatmap beatmap;
            if (!(string.IsNullOrEmpty(artist) || string.IsNullOrEmpty(title)))
            {
                beatmap = GetBeatmapUsingReplacements("withID", false, pars);
                if (beatmap != null) return beatmap;
                beatmap = GetBeatmapUsingReplacements("withoutID", false, pars);
                if (beatmap != null) return beatmap;
                beatmap = GetBeatmapUsingReplacements("Temp", false, pars);
                if (beatmap != null) return beatmap;
            }

            beatmap = GetBeatmapUsingReplacements("withID", true, pars);
            if (beatmap != null) return beatmap;
            beatmap = GetBeatmapUsingReplacements("withoutID", true, pars);
            if (beatmap != null) return beatmap;
            beatmap = GetBeatmapUsingReplacements("Temp", true, pars);

            return beatmap;
        }
    }
}