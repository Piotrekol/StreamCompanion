﻿using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using StreamCompanionTypes;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Interfaces;
using StreamCompanionTypes.Enums;
using StreamCompanionTypes.Interfaces.Services;
using StreamCompanionTypes.Interfaces.Sources;

namespace OsuSongsFolderWatcher
{
    class OsuSongsFolderWatcher : IPlugin, IDisposable, IOsuEventSource, IMapDataFinder
    {
        private readonly SettingNames _names = SettingNames.Instance;

        private FileSystemWatcher _watcher;
        private ISettings _settings;
        private ILogger _logger;
        private IDatabaseController _databaseController;
        private int _numberOfBeatmapsCurrentlyBeingLoaded = 0;
        private Thread _consumerThread;

        public string Description { get; } = "";
        public string Name { get; } = nameof(OsuSongsFolderWatcher);
        public string Author { get; } = "Piotrekol";
        public string Url { get; } = "";
        public string UpdateUrl { get; } = "";

        public OsuSongsFolderWatcher(ILogger logger, ISettings settings, IDatabaseController databaseController)
        {
            _settings = settings;
            _databaseController = databaseController;
            _logger = logger;

            if (_settings.Get<bool>(_names.LoadingRawBeatmaps))
                _settings.Add(_names.LoadingRawBeatmaps.Name, false);

            var dir = _settings.Get<string>(_names.SongsFolderLocation);
            if (dir == _names.SongsFolderLocation.Default<string>())
            {
                dir = _settings.Get<string>(_names.MainOsuDirectory);
                dir = Path.Combine(dir, "Songs\\");
            }

            if (Directory.Exists(dir))
            {
                _watcher = new FileSystemWatcher(dir, "*.osu");
                _watcher.Created += Watcher_FileCreated;
                _watcher.Changed += Watcher_FileChanged;
                _watcher.IncludeSubdirectories = true;
                _watcher.EnableRaisingEvents = true;
                _consumerThread = new Thread(ConsumerTask);
                _consumerThread.Start();
            }
            else
            {
                MessageBox.Show($"Could not find osu! songs directory at \"{dir}\"" + Environment.NewLine +
                                "This is most likely caused by moved or incorrectly detected osu! songs directory" + Environment.NewLine +
                                "Set osu! path manually in settings for StreamCompanion to be able to provide data for newly loaded songs"
                    , "StreamCompanion - New songs watcher error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Watcher_FileChanged(object sender, FileSystemEventArgs e)
        {
            Watcher_FileCreated(sender, e);
        }

        private void ConsumerTask()
        {
            try
            {
                while (true)
                {
                    if (_filesChanged.TryDequeue(out var fsArgs))
                    {
                        Beatmap beatmap = null;
                        _settings.Add(_names.LoadingRawBeatmaps.Name, true);
                        Interlocked.Increment(ref _numberOfBeatmapsCurrentlyBeingLoaded);
                        _logger.Log("Processing new beatmap", LogLevel.Debug);
                        beatmap = BeatmapHelpers.ReadBeatmap(fsArgs.FullPath);

                        _databaseController.StoreTempBeatmap(beatmap);
                        
                        _logger.Log("Added new Temporary beatmap {0} - {1} [{2}]", LogLevel.Debug, beatmap.ArtistRoman,
                            beatmap.TitleRoman, beatmap.DiffName);
                        if (Interlocked.Decrement(ref _numberOfBeatmapsCurrentlyBeingLoaded) == 0)
                        {
                            _settings.Add(_names.LoadingRawBeatmaps.Name, false);
                        }

                        if (fsArgs.ChangeType == WatcherChangeTypes.Changed && lastMapSearchArgs != null)
                        {
                            var l = lastMapSearchArgs;
                            NewOsuEvent?.Invoke(this, new MapSearchArgs($"OsuMemory-FolderWatcherReplay")
                            {
                                Artist = beatmap.Artist,
                                MapHash = beatmap.Md5,
                                Title = beatmap.Title,
                                Diff = beatmap.DiffName,
                                EventType = OsuEventType.MapChange,
                                PlayMode = beatmap.PlayMode,
                                Status = l.Status,
                                MapId = -123//bogus id to force string search
                            });
                        }
                    }
                    Thread.Sleep(5);
                }
            }
            catch (ThreadAbortException)
            {
                return;
            }
        }
        private readonly ConcurrentQueue<FileSystemEventArgs> _filesChanged = new ConcurrentQueue<FileSystemEventArgs>();
        private void Watcher_FileCreated(object sender, FileSystemEventArgs e)
        {
            _filesChanged.Enqueue(e);
            _logger.Log("New osu file: " + e.FullPath, LogLevel.Debug);
        }

        public void Dispose()
        {
            _watcher?.Dispose();
            _consumerThread?.Abort();
        }
        private MapSearchArgs lastMapSearchArgs;

        public EventHandler<MapSearchArgs> NewOsuEvent { get; set; }
        public MapSearchResult FindBeatmap(MapSearchArgs searchArgs)
        {
            lastMapSearchArgs = searchArgs;
            return null;
        }

        public OsuStatus SearchModes { get; } = OsuStatus.All;
        public string SearcherName { get; } = nameof(OsuSongsFolderWatcher);
        public int Priority { get; set; } = 1000;
    }
}
