using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using osu_StreamCompanion.Code.Helpers;
using osu_StreamCompanion.Code.Misc;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Interfaces;

namespace osu_StreamCompanion.Code.Modules.osuSongsFolderWatcher
{
    class OsuSongsFolderWatcher : IModule, ISettings, ISqliteUser, IDisposable
    {
        private readonly SettingNames _names = SettingNames.Instance;

        private FileSystemWatcher _watcher;
        private ISettingsHandler _settings;
        private ILogger _logger;
        private ISqliteControler _sqlite;
        private int _numberOfBeatmapsCurrentlyBeingLoaded = 0;
        public bool Started { get; set; }
        private Thread _consumerThread;

        public void Start(ILogger logger)
        {
            Started = true;
            _logger = logger;

            if (_settings.Get<bool>(_names.LoadingRawBeatmaps))
                _settings.Add(_names.LoadingRawBeatmaps.Name, false);

            var dir = _settings.Get<string>(_names.SongsFolderLocation);
            if (dir == _names.SongsFolderLocation.Default<string>())
            {
                dir = _settings.Get<string>(_names.MainOsuDirectory);
                dir = Path.Combine(dir, "Songs\\");
            }

            if (dir != "")
            {
                if (Directory.Exists(dir))
                {
                    _watcher = new FileSystemWatcher(dir, "*.osu");
                    _watcher.Created += Watcher_FileCreated;
                    _watcher.IncludeSubdirectories = true;
                    _watcher.EnableRaisingEvents = true;
                    _consumerThread = new Thread(ConsumerTask);
                    _consumerThread.Start();
                }
                else
                {
                    _logger.Log("Could not find osu! songs directory", LogLevel.Error);
                }
            }
        }

        private void ConsumerTask()
        {
            try
            {
                while (true)
                {
                    string fileFullPath;
                    if (_filesChanged.TryDequeue(out fileFullPath))
                    {
                        Beatmap beatmap = null;
                        _settings.Add(_names.LoadingRawBeatmaps.Name, true);
                        Interlocked.Increment(ref _numberOfBeatmapsCurrentlyBeingLoaded);
                        _logger.Log("Processing new beatmap", LogLevel.Debug);
                        beatmap = BeatmapHelpers.ReadBeatmap(fileFullPath);

                        _sqlite.StoreTempBeatmap(beatmap);
                        _logger.Log("Added new Temporary beatmap {0} - {1} [{2}]", LogLevel.Debug, beatmap.ArtistRoman,
                            beatmap.TitleRoman, beatmap.DiffName);
                        if (Interlocked.Decrement(ref _numberOfBeatmapsCurrentlyBeingLoaded) == 0)
                        {
                            _settings.Add(_names.LoadingRawBeatmaps.Name, false);
                        }
                    }
                    Thread.Sleep(5);
                }
            }
            catch (ThreadAbortException ex)
            {
                return;
            }
        }
        private readonly ConcurrentQueue<string> _filesChanged = new ConcurrentQueue<string>();
        private void Watcher_FileCreated(object sender, FileSystemEventArgs e)
        {
            _filesChanged.Enqueue(e.FullPath);
            _logger.Log("New osu file: "+e.FullPath, LogLevel.Debug);
        }


        public string SettingGroup { get; } = "Passwords";

        public void SetSettingsHandle(ISettingsHandler settings)
        {
            _settings = settings;
        }

        public void Free()
        {
        }

        public UserControl GetUiSettings()
        {
            return new UserControl();
        }

        public void SetSqliteControlerHandle(ISqliteControler sqLiteControler)
        {
            _sqlite = sqLiteControler;
        }

        public void Dispose()
        {
            _watcher?.Dispose();
            _consumerThread?.Abort();
        }
    }
}
