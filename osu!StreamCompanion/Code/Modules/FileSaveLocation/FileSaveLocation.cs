using System.Windows.Forms;
using StreamCompanionTypes.Interfaces;

namespace osu_StreamCompanion.Code.Modules.FileSaveLocation
{
    class FileSaveLocation : IModule, ISettingsProvider
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
        public void SetSettingsHandle(ISettingsHandler settings)
        {
        }

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
