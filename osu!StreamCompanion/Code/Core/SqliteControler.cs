using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using System.Threading;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Interfaces;


namespace osu_StreamCompanion.Code.Core
{

    public class SqliteControler : ISqliteControler
    {
        private readonly SqliteConnector _sqlConnector;
        private Dictionary<string, MapIdMd5Pair> _beatmapChecksums;

        private class MapIdMd5Pair
        {
            public int MapId { get; }
            public string Md5 { get; }
            public bool Found { get; set; }
            public MapIdMd5Pair(int mapId, string md5)
            {
                MapId = mapId;
                Md5 = md5;
            }
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
        public DbDataReader Query(string query)
        {
            return _sqlConnector.Query(query);
        }

        /// <summary>
        /// Used for inserting beatmaps/invoking a large number of (no result)queries at once instead of one-by-one with is way slower.
        /// </summary>
        public void StartMassStoring()
        {
            if (_sqlConnector.MassInsertIsActive)
                return;
            lock (_sqlConnector)
            {
                string sql = "SELECT Md5, MapId, BeatmapChecksum FROM (SELECT Md5, MapId, BeatmapChecksum FROM `withID` UNION SELECT Md5, MapId, BeatmapChecksum FROM `withoutID`)";
                SQLiteDataReader reader;
                try
                {
                    reader = _sqlConnector.Query(sql);
                }
                catch (SQLiteException e)
                {
                    if (e.ResultCode == SQLiteErrorCode.Corrupt)
                    {
                        _sqlConnector.ResetDatabase();
                        StartMassStoring();
                        return;
                    }

                    throw;
                }

                _beatmapChecksums = new Dictionary<string, MapIdMd5Pair>();
                while (reader.Read())
                {
                    var hash = reader.GetString(0);
                    var mapId = reader.GetInt32(1);
                    var checksum = reader.GetString(2);
                    if (_beatmapChecksums.ContainsKey(checksum))
                    {
                        //REALLY..? I don't think so.
                        var map1 = _sqlConnector.GetBeatmap(hash);
                        var map2 = _sqlConnector.GetBeatmap(_beatmapChecksums[checksum].Md5);
                        var map1Checksum = map1.GetChecksum();
                        var map2Checksum = map2.GetChecksum();
                        if (map1Checksum == map2Checksum)
                        {
                            //:( fair enough
                            var ex = new SQLiteException(SQLiteErrorCode.Constraint, "uh, oh... beatmap checksum collision");
                            ex.Data.Add("mapId1", map1.MapId);
                            ex.Data.Add("hash1", map1.Md5);
                            ex.Data.Add("mapId2", map2.MapId);
                            ex.Data.Add("hash2", map2.Md5);
                            ex.Data.Add("checksum", map1Checksum);
                            throw ex;
                        }

                        _beatmapChecksums.Remove(map1Checksum);
                        _beatmapChecksums.Remove(map2Checksum);

                        _beatmapChecksums.Add(map1Checksum, new MapIdMd5Pair(map1.MapId, map1.Md5));
                        _beatmapChecksums.Add(map2Checksum, new MapIdMd5Pair(map2.MapId, map2.Md5));

                    }
                    else
                    {
                        _beatmapChecksums.Add(checksum, new MapIdMd5Pair(mapId, hash));
                    }
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
            if (!_sqlConnector.MassInsertIsActive)
                return;

            lock (_sqlConnector)
            {
                var deletedBeatmaps = _beatmapChecksums.Where(x => !x.Value.Found).ToList();
                if (deletedBeatmaps.Any())
                {
                    _sqlConnector.RemoveBeatmaps(deletedBeatmaps.Select(x => x.Key).ToList());
                }

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
                    var checksum = beatmap.GetChecksum();
                    if (_beatmapChecksums.ContainsKey(checksum))
                    {
                        _beatmapChecksums[checksum].Found = true;
                        return;
                    }
                    else
                    {
                        var existingEntry = _beatmapChecksums.FirstOrDefault(x => x.Value.Md5 == beatmap.Md5);
                        if (!existingEntry.Equals(default(KeyValuePair<string, MapIdMd5Pair>)))
                        {
                            _beatmapChecksums.Remove(existingEntry.Key);
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

        public Beatmap GetBeatmap(string mapHash)
        {
            lock (_sqlConnector)
            {
                return _sqlConnector.GetBeatmap(mapHash);
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
