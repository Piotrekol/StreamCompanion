using StreamCompanion.Common;
using StreamCompanionTypes.Interfaces;
using StreamCompanionTypes.Interfaces.Services;
using StreamCompanionTypes.Interfaces.Sources;

namespace osu_StreamCompanion.Code.Core.Plugins
{
    internal class PluginManagerSettings : IPlugin, ISettingsSource
    {
        public string SettingGroup => "General__Plugins";
        public string Description => "";
        public string Name => "";
        public string Author => "";
        public string Url => "";

        private readonly ISettings _settings;
        private PluginManagerSettingsUserControl _pluginManagerSettingsUserControl;

        public PluginManagerSettings(ISettings settings)
        {
            _settings = settings;
        }

        public void Free()
        {
            _pluginManagerSettingsUserControl?.Dispose();
        }

        public object GetUiSettings()
        {
            if (_pluginManagerSettingsUserControl == null || _pluginManagerSettingsUserControl.IsDisposed)
            {
                return _pluginManagerSettingsUserControl = new PluginManagerSettingsUserControl(_settings.GetConfiguration<PluginManagerConfiguration>(LocalPluginManager.PluginManagerConfigEntry));
            }

            return _pluginManagerSettingsUserControl;
        }
    }
}
