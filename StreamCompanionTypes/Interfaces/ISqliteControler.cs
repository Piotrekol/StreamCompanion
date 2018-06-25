using System;
using System.Data.SQLite;
using StreamCompanionTypes.DataTypes;

namespace StreamCompanionTypes.Interfaces
{
    public interface ISqliteControler : IDisposable, IMapDataStorer, CollectionManager.Interfaces.IMapDataManager
    {
        /// <summary>
        /// Used for querying database with expectation of no result.
        /// </summary>
        /// <param name="query">SQLite query to be executed</param>
        void NonQuery(string query);

        /// <summary>
        /// Used for querying database.
        /// Remember to dispose returned object after use.
        /// </summary>
        /// <param name="query">SQLite query to be executed</param>
        /// <returns>SQLiteDataReader with query results</returns>
        SQLiteDataReader Query(string query);

        /// <summary>
        /// Used for inserting beatmaps/invoking a large number of (no result)queries at once instead of one-by-one with is way slower.
        /// </summary>
        void StartMassStoring();

        /// <summary>
        /// Used for commiting Queued queries in MassStoring
        /// </summary>
        void EndMassStoring();

        /// <summary>
        /// Used for inserting beatmap data to SQL table.
        /// When adding large amount of beatmaps it is advised to use MassStoring to speed up adding.
        /// When MassStoring is active, only new maps are queried to Sqlite 
        /// </summary>
        /// <param name="beatmap">Beatmap to insert</param>
        void StoreBeatmap(Beatmap beatmap);

        /// <summary>
        /// Used for inserting temporary beatmap data to sql table.
        /// Data stored this way won't be preserved on the following application runs
        /// </summary>
        /// <param name="beatmap">beatmap to insert</param>
        void StoreTempBeatmap(Beatmap beatmap);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapId"></param>
        /// <returns>Beatmap object with data, or null on not found</returns>
        Beatmap GetBeatmap(int mapId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="artist"></param>
        /// <param name="title"></param>
        /// <param name="diff"></param>
        /// <returns>Beatmap object with filled or not map data</returns>
        Beatmap GetBeatmap(string artist, string title, string diff, string raw);

        void Dispose();
        void StoreBeatmap(CollectionManager.DataTypes.Beatmap beatmap);
        CollectionManager.DataTypes.Beatmap GetByHash(string hash);
        CollectionManager.DataTypes.Beatmap GetByMapId(int mapId);
    }
}