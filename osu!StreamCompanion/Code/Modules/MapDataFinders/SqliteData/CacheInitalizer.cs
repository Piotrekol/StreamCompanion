using System;
using System.IO;
using CollectionManager.Modules.FileIO.OsuDb;
using osu_StreamCompanion.Code.Core;
using osu_StreamCompanion.Code.Core.DataTypes;
using osu_StreamCompanion.Code.Interfaces;
using osu_StreamCompanion.Code.Misc;
using osu_StreamCompanion.Code.Windows;

namespace osu_StreamCompanion.Code.Modules.MapDataFinders.SqliteData
{
    public class CacheInitalizer
    {
        private readonly MainWindowUpdater _mainWindowHandle;
        private readonly SqliteControler _sqliteControler;
        private readonly Settings _settings;
        private readonly ILogger _logger;
        private readonly SettingNames _names = SettingNames.Instance;
        private OsuDatabaseLoader _osuDatabaseLoader;

        public CacheInitalizer(MainWindowUpdater mainWindowHandle, SqliteControler sqliteControler, Settings settings, ILogger logger)
        {
            _mainWindowHandle = mainWindowHandle;
            _sqliteControler = sqliteControler;
            _settings = settings;
            _logger = logger;
        }
        public void Initalize()
        {
            _osuDatabaseLoader = new LOsuDatabaseLoader(new BeatmapLoaderLogger(_mainWindowHandle), _sqliteControler, new Beatmap());

            new System.Threading.Thread(() =>
            {
                string osudb = Path.Combine(_settings.Get<string>(_names.MainOsuDirectory), "osu!.db");
                if (File.Exists(osudb))
                {
                    string newOsuFile = "";
                    try
                    {
                        newOsuFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                            @"Files", "osu!.db");

                        if (File.Exists(newOsuFile))
                            File.Delete(newOsuFile);

                        File.Copy(osudb, newOsuFile);

                        File.SetAttributes(newOsuFile, FileAttributes.Normal);

                        _osuDatabaseLoader.LoadDatabase(newOsuFile);

                        File.Delete(newOsuFile);
                    }
                    catch (Exception e)
                    {
                        _mainWindowHandle.BeatmapsLoaded = "loading osu!.db FAILED: "+ e.Message;
                        _logger?.Log("loading osu!.db FAILED\nsrc:{0}\ndest:{1}\n{2}\n{3} ", LogLevel.Error, osudb, newOsuFile, e.Message, e.StackTrace);
                    }
                }
                else
                {
                    _mainWindowHandle.BeatmapsLoaded = "Could not locate osu!.db";
                }
            }).Start();
        }
        
        class BeatmapLoaderLogger : CollectionManager.Interfaces.ILogger
        {
            private readonly MainWindowUpdater _handle;
            private object _mainWindowHandle;

            public BeatmapLoaderLogger(MainWindowUpdater handle)
            {
                _handle = handle;
            }

            public BeatmapLoaderLogger(object mainWindowHandle)
            {
                _mainWindowHandle = mainWindowHandle;
            }

            public void Log(string logMessage, params string[] vals)
            {
                _handle.BeatmapsLoaded = string.Format(logMessage, vals);
            }
        }
    }
}