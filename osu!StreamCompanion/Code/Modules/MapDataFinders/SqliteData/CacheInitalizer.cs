using System;
using System.IO;
using System.Windows.Forms;
using CollectionManager.Modules.FileIO.OsuDb;
using StreamCompanionTypes;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Interfaces;
using StreamCompanionTypes.Enums;

namespace osu_StreamCompanion.Code.Modules.MapDataFinders.SqliteData
{
    public class CacheInitalizer
    {
        private readonly IMainWindowModel _mainWindowHandle;
        private readonly ISqliteControler _sqliteControler;
        private readonly ISettingsHandler _settings;
        private readonly ILogger _logger;
        private readonly SettingNames _names = SettingNames.Instance;
        private OsuDatabaseLoader _osuDatabaseLoader;

        public CacheInitalizer(IMainWindowModel mainWindowHandle, ISqliteControler sqliteControler, ISettingsHandler settings, ILogger logger)
        {
            _mainWindowHandle = mainWindowHandle;
            _sqliteControler = sqliteControler;
            _settings = settings;
            _logger = logger;
        }

        public void Initalize()
        {
            _osuDatabaseLoader = new OsuDatabaseLoader(new BeatmapLoaderLogger(_mainWindowHandle), _sqliteControler, new Beatmap());

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
                        _mainWindowHandle.BeatmapsLoaded = "loading osu!.db FAILED!";
                        var osuDbLoadFailedException = new OsuDbLoadFailedException($"loading osu!.db FAILED\n{e.Message}", e);
                        osuDbLoadFailedException.Data["src"] = osudb;
                        osuDbLoadFailedException.Data["dest"] = newOsuFile;
                        osuDbLoadFailedException.Data["stack"] = e.StackTrace;
                        if (_logger is IContextAwareLogger contextAwareLogger)
                            contextAwareLogger.SetContextData("osu!username", string.IsNullOrEmpty(_osuDatabaseLoader.Username) ? "null" : _osuDatabaseLoader.Username);

                        _logger?.Log(osuDbLoadFailedException, LogLevel.Error);
                        MessageBox.Show("Failed to load osu! beatmap database: " + Environment.NewLine + string.Format("Exception: {0},{1}", e.Message, e.StackTrace), "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        _sqliteControler.EndMassStoring();
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