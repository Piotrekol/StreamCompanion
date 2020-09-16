using System;
using System.Data.SQLite;
using System.IO;
using System.Windows.Forms;
using CollectionManager.Modules.FileIO.OsuDb;
using StreamCompanionTypes;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Interfaces;
using StreamCompanionTypes.Enums;
using StreamCompanionTypes.Interfaces.Services;

namespace osu_StreamCompanion.Code.Modules.MapDataFinders.SqliteData
{
    public class CacheInitalizer
    {
        private readonly IMainWindowModel _mainWindowHandle;
        private readonly IDatabaseController _databaseControler;
        private readonly ISettings _settings;
        private readonly ILogger _logger;
        private readonly SettingNames _names = SettingNames.Instance;
        private OsuDatabaseLoader _osuDatabaseLoader;

        public CacheInitalizer(IMainWindowModel mainWindowHandle, IDatabaseController databaseControler, ISettings settings, ILogger logger)
        {
            _mainWindowHandle = mainWindowHandle;
            _databaseControler = databaseControler;
            _settings = settings;
            _logger = logger;
        }

        public void Initalize()
        {
            _osuDatabaseLoader = new OsuDatabaseLoader(new BeatmapLoaderLogger(_mainWindowHandle), _databaseControler, new Beatmap());

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

                        if (_logger is IContextAwareLogger contextAwareLogger)
                            contextAwareLogger.SetContextData("osu!username", string.IsNullOrEmpty(_osuDatabaseLoader.Username) ? "null" : _osuDatabaseLoader.Username);

                        File.Delete(newOsuFile);
                    }
                    catch (Exception e)
                    {
                        if (e.Message == "Connection was closed, statement was terminated")
                        {
                            _mainWindowHandle.BeatmapsLoaded = "loading of osu!.db aborted";
                            return;
                        }

                        _mainWindowHandle.BeatmapsLoaded = "loading of osu!.db failed";
                        var osuDbLoadFailedException = new OsuDbLoadFailedException($"loading of osu!.db failed\n{e.Message}", e);
                        osuDbLoadFailedException.Data["src"] = osudb;
                        osuDbLoadFailedException.Data["dest"] = newOsuFile;
                        osuDbLoadFailedException.Data["osuDbVersion"] = _osuDatabaseLoader.FileDate;
                        osuDbLoadFailedException.Data["counts"] = $"{_osuDatabaseLoader.ExpectedNumberOfMapSets} - {_osuDatabaseLoader.ExpectedNumOfBeatmaps}";
                        osuDbLoadFailedException.Data["stack"] = e.StackTrace;
                        if (_logger is IContextAwareLogger contextAwareLogger)
                            contextAwareLogger.SetContextData("osu!username", string.IsNullOrEmpty(_osuDatabaseLoader.Username) ? "null" : _osuDatabaseLoader.Username);

                        _logger?.Log(osuDbLoadFailedException, LogLevel.Error);
                        MessageBox.Show("Failed to load osu! beatmap database: " + Environment.NewLine + string.Format("Exception: {0},{1}", e.Message, e.StackTrace), "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        _databaseControler.EndMassStoring();
                    }
                }
                else
                {
                    _mainWindowHandle.BeatmapsLoaded = "Could not locate osu!.db";
                }
            }).Start();
        }

        class OsuDbLoadFailedException : Exception
        {
            public OsuDbLoadFailedException(string message, Exception exception) : base(message, exception)
            {
            }
        }
        class BeatmapLoaderLogger : CollectionManager.Interfaces.ILogger
        {
            private readonly IMainWindowModel _handle;
            private object _mainWindowHandle;

            public BeatmapLoaderLogger(IMainWindowModel handle)
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