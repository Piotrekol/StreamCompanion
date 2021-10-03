using StreamCompanionTypes.Interfaces;
using StreamCompanionTypes.Enums;
using StreamCompanionTypes.Interfaces.Services;
using StreamCompanionTypes.Interfaces.Sources;

namespace newTestPlugin
{
    public class MyPlugin : IPlugin, ISettingsSource
    {
        public string Description => "my plugin description";
        public string Name => "my plugin name";
        public string Author => "my name";
        public string Url => "Plugin homepage url(github/site)";
        public string SettingGroup => "myGroupName";
        private SettingsUserControl SettingsUserControl;
        public MyPlugin(ILogger logger)
        {
            logger.Log($"Message from {Name}!", LogLevel.Trace);
        }

        public void Free()
        {
            SettingsUserControl?.Dispose();
        }

        public object GetUiSettings()
        {
            if(SettingsUserControl == null || SettingsUserControl.IsDisposed)
                SettingsUserControl = new SettingsUserControl();

            return SettingsUserControl;
        }
    }
}
