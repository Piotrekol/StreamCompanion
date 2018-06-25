using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Threading;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Interfaces;


namespace osu_StreamCompanion.Code.Core
{

    public class SqliteControler : ISqliteControler
    {
        private readonly SqliteConnector _sqlConnector;
        private Dictionary<string, int> _md5List;
        public SqliteControler()
        {
            _sqlConnector = new SqliteConnector();
        }
        public SqliteControler(SqliteConnector sqLiteConnector)
        {
            _sqlConnector = sqLiteConnector;
        }

        /// <summary>
        /// Used for querying database with expectation of no result.
        /// </summary>
        /// <param name="query">SQLite query to be executed</param>
        public void NonQuery(string query)
        {
            _sqlConnector.NonQuery(query);
        }
        /// <summary>
        /// Used for querying database.
        /// Remember to dispose returned object after use.
        /// </summary>
        /// <param name="query">SQLite query to be executed</param>
        /// <returns>SQLiteDataReader with query results</returns>
        public SQLiteDataReader Query(string query)
        {
            return _sqlConnector.Query(query);
        }
        /// <summary>
        /// Used for inserting beatmaps/invoking a large number of (no result)queries at once instead of one-by-one with is way slower.
        /// </summary>
        public void StartMassStoring()
        {
            lock (_sqlConnector)
            {
                string sql = "SELECT Md5, MapId FROM (SELECT Md5, MapId FROM `withID` UNION SELECT Md5, MapId FROM `withoutID`)";
                var reader = _sqlConnector.Query(sql);
                _md5List = new Dictionary<string, int>();
                while (reader.Read())
                {
                    var hash = reader.GetString(0);
                    var mapId = reader.GetInt32(1);
                    if (_md5List.ContainsKey(hash))
                    {
                        //On collision delete both entrys.
                        _sqlConnector.RemoveBeatmap(hash);
                    }
                    else
                        _md5List.Add(hash, mapId);
                }
                reader.Dispose();
                _sqlConnector.StartMassStoring();
            }
        }
        /// <summary>
        /// Used for commiting Queued queries in MassStoring
        /// </summary>
        public void EndMassStoring()
        {
            lock (_sqlConnector)
            {
                _sqlConnector.EndMassStoring();
            }
        }
        /// <summary>
        /// Used for inserting beatmap data to SQL table.
        /// When adding large amount of beatmaps it is advised to use MassStoring to speed up adding.
        /// When MassStoring is active, only new maps are queried to Sqlite 
        /// </summary>
        /// <param name="beatmap">Beatmap to insert</param>
        public void StoreBeatmap(Beatmap beatmap)
        {
            lock (_sqlConnector)
            {
                if (_sqlConnector.MassInsertIsActive)
                {
                    var hash = beatmap.Md5;
                    if (_md5List.ContainsKey(hash))
                    {
                        if (_md5List[hash] == beatmap.MapId)
                            return; //no need to save same data.
                        else
                        {//We need to first remove old entry
                            _sqlConnector.RemoveBeatmap(beatmap.Md5);
                        }
                    }
                }
                _sqlConnector.StoreBeatmap(beatmap);
            }

        }
        /// <summary>
        /// Used for inserting temporary beatmap data to sql table.
        /// Data stored this way won't be preserved on the following application runs
        /// </summary>
        /// <param name="beatmap">beatmap to insert</param>
        public void StoreTempBeatmap(Beatmap beatmap)
        {
            lock (_sqlConnector)
            {
                _sqlConnector.StoreTempBeatmap(beatmap);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapId"></param>
        /// <returns>Beatmap object with data, or null on not found</returns>
        public Beatmap GetBeatmap(int mapId)
        {
            lock (_sqlConnector)
            {
                return _sqlConnector.GetBeatmap(mapId);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="artist"></param>
        /// <param name="title"></param>
        /// <param name="diff"></param>
        /// <returns>Beatmap object with filled or not map data</returns>
        public Beatmap GetBeatmap(string artist, string title, string diff, string raw)
        {
            lock (_sqlConnector)
            {
                return _sqlConnector.GetBeatmap(artist, title, diff, raw);
            }
        }
        public void Dispose()
        {
            while (_sqlConnector.MassInsertIsActive)
            {
                Thread.Sleep(1);
            }
            _sqlConnector.Dispose();
        }

        public void StoreBeatmap(CollectionManager.DataTypes.Beatmap beatmap)
        {
            StoreBeatmap((Beatmap)beatmap);
        }

        public CollectionManager.DataTypes.Beatmap GetByHash(string hash)
        {
            throw new NotImplementedException();
        }

        public CollectionManager.DataTypes.Beatmap GetByMapId(int mapId)
        {
            lock (_sqlConnector)
            {
                return _sqlConnector.GetBeatmap(mapId);
            }
        }
    }
}
