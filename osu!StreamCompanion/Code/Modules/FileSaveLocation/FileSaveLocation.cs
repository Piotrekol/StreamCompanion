using System.Windows.Forms;
using osu_StreamCompanion.Code.Misc;
using StreamCompanionTypes.Interfaces.Services;
using StreamCompanionTypes.Interfaces.Sources;

namespace osu_StreamCompanion.Code.Modules.FileSaveLocation
{
    class FileSaveLocation : IModule, ISettingsSource
    {
        private FileSaveLocationSettings _fileSaveLocationSettings;
        private readonly ILogger _logger;
        private ISaver _saver;

        public bool Started { get; set; }

        public FileSaveLocation(ILogger logger, ISaver saver)
        {
            _logger = logger;
            _saver = saver;
        }
        public void Start(ILogger logger)
        {
            Started = true;
        }

        public string SettingGroup { get; } = "General";

        public void Free()
        {
            _fileSaveLocationSettings.Dispose();
        }

        public object GetUiSettings()
        {
            if (_fileSaveLocationSettings == null || _fileSaveLocationSettings.IsDisposed)
            {
                _fileSaveLocationSettings = new FileSaveLocationSettings();
                _fileSaveLocationSettings.SetSaveHandle(_saver);
            }
            return _fileSaveLocationSettings;
        }

    }
}
