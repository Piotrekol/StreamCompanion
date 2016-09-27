using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Text;
using osu_StreamCompanion.Code.Core.DataTypes;
using osu_StreamCompanion.Code.Interfeaces;
using osu_StreamCompanion.Code.Helpers;
namespace osu_StreamCompanion.Code.Core
{
    public class SqliteConnector : IDisposable, IMapDataStorer
    {
        readonly SQLiteConnection _mDbConnection;
        private string DbFilename = "StreamCompanionCache.db";
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
        public SqliteConnector()
        {
            CreateFile(DbFilename);
            _mDbConnection = new SQLiteConnection("Data Source=" + DbFilename + ";Version=3;New=False;Compress=True;");
            OpenConnection();
            CreateTables();
        }

        private void CreateTables()
        {
            _tableStruct.Fieldnames = new List<string>(new[] { "Raw", "TitleRoman", "ArtistRoman", "TitleUnicode", "ArtistUnicode", "Creator", "DiffName", "Mp3Name", "Md5", "OsuFileName", "MaxBpm", "MinBpm", "Tags", "State", "Circles", "Sliders", "Spinners", "EditDate", "ApproachRate", "CircleSize", "HpDrainRate", "OverallDifficulty", "SliderVelocity", "DrainingTime", "TotalTime", "PreviewTime", "MapId", "MapSetId", "ThreadId", "MapRating", "Offset", "StackLeniency", "Mode", "Source", "AudioOffset", "LetterBox", "Played", "LastPlayed", "IsOsz2", "Dir", "LastSync", "DisableHitsounds", "DisableSkin", "DisableSb", "BgDim", "Somestuff", "VideoDir" });
            _tableStruct.Type = new List<string>(new[] { "VARCHAR", "VARCHAR", "VARCHAR", "VARCHAR", "VARCHAR", "VARCHAR", "VARCHAR", "VARCHAR", "VARCHAR", "VARCHAR", "DOUBLE", "DOUBLE", "VARCHAR", "INTEGER", "INTEGER", "INTEGER", "INTEGER", "DATETIME", "DOUBLE", "DOUBLE", "DOUBLE", "DOUBLE", "DOUBLE", "INTEGER", "INTEGER", "INTEGER", "INTEGER", "INTEGER", "INTEGER", "INTEGER", "INTEGER", "DOUBLE", "INTEGER", "VARCHAR", "INTEGER", "VARCHAR", "BOOL", "DATETIME", "BOOL", "VARCHAR", "DATETIME", "BOOL", "BOOL", "BOOL", "INTEGER", "INTEGER", "VARCHAR" });
            _tableStruct.TypeModifiers = new List<string>(new[] { "NOT NULL", "NOT NULL", "NOT NULL ", "NOT NULL ", "NOT NULL ", "NOT NULL ", "NOT NULL ", "NOT NULL ", "NOT NULL UNIQUE", "NOT NULL ", "NOT NULL ", "NOT NULL ", "NOT NULL ", "NOT NULL ", "NOT NULL ", "NOT NULL ", "NOT NULL ", "NOT NULL ", "NOT NULL ", "NOT NULL ", "NOT NULL ", "NOT NULL ", "NOT NULL ", "NOT NULL", "NOT NULL", "NOT NULL", "NOT NULL ", "NOT NULL", "NOT NULL", "NOT NULL", "NOT NULL", "NOT NULL", "NOT NULL", "NOT NULL", "NOT NULL", "NOT NULL", "", "NOT NULL", "", "NOT NULL", "NOT NULL", "", "", "", "NOT NULL", "NOT NULL", "NOT NULL" });
            string sql = "CREATE  TABLE IF NOT EXISTS withID " + _tableStruct.GetTableDef();
            NonQuery(sql);
            sql = "CREATE  TABLE IF NOT EXISTS withoutID " + _tableStruct.GetTableDef();
            NonQuery(sql);

            sql = "DROP TABLE IF EXISTS Temp";
            NonQuery(sql);
            sql = "CREATE TABLE IF NOT EXISTS Temp " + _tableStruct.GetTableDef();
            NonQuery(sql);

            sql = "CREATE TABLE IF NOT EXISTS `cfg` (`LastUpdateDate` TEXT)";
            NonQuery(sql);
        }




        public void NonQuery(string query)
        {
            _insertSql = new SQLiteCommand(query, _mDbConnection);
            _insertSql.ExecuteNonQuery();

        }

        private SQLiteDataReader Query(string query, IDictionary<string, string> replacements)
        {
            SQLiteCommand cmd = new SQLiteCommand(query, _mDbConnection);
            foreach (var replacement in replacements)
            {
                cmd.Parameters.Add(replacement.Key, DbType.String).Value = replacement.Value;
            }
            return cmd.ExecuteReader();
        }
        public SQLiteDataReader Query(string query)
        {
            SQLiteCommand cmd = new SQLiteCommand(query, _mDbConnection);
            return cmd.ExecuteReader();
        }
        private void CreateFile(string filename)
        {
            try
            {
                if (File.Exists("Test.db"))
                {
                    File.Delete("Test.db");
                }
                if (!File.Exists(filename))
                {
                    SQLiteConnection.CreateFile(filename);
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new NonLoggableException(ex,"Could not save beatmap cache file due to insuffisient premissions"+
                    Environment.NewLine+"Please move this exectuable into a non-system folder");
            }
        }
        private void OpenConnection()
        {
            _mDbConnection.Open();
        }
        private void CloseConnection()
        {
            _mDbConnection.Close();
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

        public void StoreBeatmap(Beatmap beatmap)
        {
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

        public void StoreTempBeatmap(Beatmap beatmap)
        {
            string sql =
                    string.Format("INSERT OR REPLACE INTO Temp ({0}) VALUES ({1})", _tableStruct.GetFieldnames(), _tableStruct.GetFieldnames("@"));
            _insertSql = new SQLiteCommand(sql, _mDbConnection);
            FillBeatmapParameters(beatmap);
            _insertSql.ExecuteNonQuery();
        }




        private void FillBeatmapParameters(Beatmap beatmap)
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
            _insertSql.Parameters.Add("@EditDate", DbType.DateTime).Value = beatmap.EditDate;
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
            _insertSql.Parameters.Add("@MapRating", DbType.Int32).Value = beatmap.MapRating;
            _insertSql.Parameters.Add("@Offset", DbType.Int32).Value = beatmap.Offset;
            _insertSql.Parameters.Add("@StackLeniency", DbType.Double).Value = beatmap.StackLeniency;
            _insertSql.Parameters.Add("@Mode", DbType.Int16).Value = beatmap.Mode;
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
            _insertSql.Parameters.Add("@VideoDir", DbType.String).Value = beatmap.VideoDir ?? " ";

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

        private Beatmap GetBeatmapUsingReplacements(string table, bool useRaw, Dictionary<string, string> replacements)
        {
            var retBeatmap = new Beatmap();
            bool foundData = false;
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
                var sql = string.Format("SELECT * FROM `{0}` WHERE (TitleRoman LIKE @title OR TitleUnicode LIKE @title) AND (ArtistRoman LIKE @artist OR ArtistUnicode LIKE @artist)", table);
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

            return foundData ? retBeatmap : null;

        }
        public Beatmap GetBeatmap(string artist, string title, string diff, string raw)
        {
            var pars = new Dictionary<string, string>();
            pars.Add("@artist", artist);
            pars.Add("@title", title);
            pars.Add("@diff", diff);
            pars.Add("@Raw", raw);
            var beatmap = GetBeatmapUsingReplacements("withID", false, pars);
            if (beatmap != null) return beatmap;
            beatmap = GetBeatmapUsingReplacements("withoutID", false, pars);
            if (beatmap != null) return beatmap;
            beatmap = GetBeatmapUsingReplacements("Temp", false, pars);
            if (beatmap != null) return beatmap;

            beatmap = GetBeatmapUsingReplacements("withID", true, pars);
            if (beatmap != null) return beatmap;
            beatmap = GetBeatmapUsingReplacements("withoutID", true, pars);
            if (beatmap != null) return beatmap;
            beatmap = GetBeatmapUsingReplacements("Temp", true, pars);

            return beatmap;
        }
    }
}