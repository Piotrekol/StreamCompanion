using System.IO;
using System.Threading;
using System.Windows.Forms;
using osu_StreamCompanion.Code.Core;
using osu_StreamCompanion.Code.Core.DataTypes;
using osu_StreamCompanion.Code.Helpers;
using osu_StreamCompanion.Code.Interfeaces;
using osu_StreamCompanion.Code.Misc;

namespace osu_StreamCompanion.Code.Modules.osuSongsFolderWatcher
{
    class OsuSongsFolderWatcher : IModule, ISettings, ISqliteUser
    {
        private readonly SettingNames _names = SettingNames.Instance;

        private FileSystemWatcher watcher;
        private Settings _settings;
        private ILogger _logger;
        private SqliteControler _sqlite;
        private int _numberOfBeatmapsCurrentlyBeingLoaded = 0;
        public bool Started { get; set; }
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
                    watcher = new FileSystemWatcher(dir, "*.osu");
                    watcher.Created += Watcher_FileCreated;
                    watcher.IncludeSubdirectories = true;
                    watcher.EnableRaisingEvents = true;
                }
                else
                {
                    _logger.Log("Could not find osu! songs directory", LogLevel.Error);
                }
            }

        }

        private void Watcher_FileCreated(object sender, FileSystemEventArgs e)
        {
            _settings.Add(_names.LoadingRawBeatmaps.Name, true);
            Interlocked.Increment(ref _numberOfBeatmapsCurrentlyBeingLoaded);
            _logger.Log("Detected new beatmap in songs folder", LogLevel.Debug);
            var beatmap = BeatmapHelpers.ReadBeatmap(e.FullPath);

            _sqlite.StoreTempBeatmap(beatmap);
            _logger.Log("Added new Temporary beatmap {0} - {1}", LogLevel.Debug, beatmap.ArtistRoman, beatmap.TitleRoman);
            if (Interlocked.Decrement(ref _numberOfBeatmapsCurrentlyBeingLoaded) == 0)
            {
                _settings.Add(_names.LoadingRawBeatmaps.Name,false);
            }
        }


        public string SettingGroup { get; } = "Passwords";

        public void SetSettingsHandle(Settings settings)
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

        public void SetSqliteControlerHandle(SqliteControler sqLiteControler)
        {
            _sqlite = sqLiteControler;
        }

    }
}
