using System;
using System.Collections.Generic;
using System.Data.SQLite;
using osu_StreamCompanion.Code.Core.DataTypes;
using osu_StreamCompanion.Code.Interfaces;


namespace osu_StreamCompanion.Code.Core
{
    public class SqliteControler : IDisposable, IMapDataStorer, CollectionManager.Interfaces.IMapDataManager
    {
        private readonly SqliteConnector _sqlConnector;
        private HashSet<string> _md5List;
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
                string sql = "SELECT Md5 FROM (SELECT Md5 FROM `withID` UNION SELECT Md5 FROM `withoutID`)";
                var reader = _sqlConnector.Query(sql);
                _md5List = new HashSet<string>();
                while (reader.Read())
                {
                    _md5List.Add(reader.GetString(0));
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
                    if (_md5List.Contains(beatmap.Md5))
                        return;//no need to save same data.
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
