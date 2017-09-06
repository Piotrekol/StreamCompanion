using System.Collections.Generic;
using System.Windows.Forms;
using osu_StreamCompanion.Code.Core;
using osu_StreamCompanion.Code.Core.DataTypes;
using osu_StreamCompanion.Code.Interfaces;

namespace osu_StreamCompanion.Code.Modules.CommandsPreview
{
    public class CommandsPreview : IModule, IMapDataParser, ISettingsProvider
    {
        public bool Started { get; set; }
        public void Start(ILogger logger)
        { Started = true; }

        public Dictionary<string, string> GetFormatedMapStrings(Dictionary<string, string> replacements, OsuStatus status)
        {
            if (_commandsPreviewSettings != null && !_commandsPreviewSettings.IsDisposed)
                _commandsPreviewSettings.Add(replacements);

            return new Dictionary<string, string>();
        }

        public string SettingGroup { get; } = "Commands Preview";
        public void SetSettingsHandle(Settings settings)
        {
        }

        public void Free()
        {
            _commandsPreviewSettings.Dispose();
        }

        private CommandsPreviewSettings _commandsPreviewSettings;
        public UserControl GetUiSettings()
        {
            if (_commandsPreviewSettings == null || _commandsPreviewSettings.IsDisposed)
            {
                _commandsPreviewSettings = new CommandsPreviewSettings();
            }
            return _commandsPreviewSettings;
        }
    }
}