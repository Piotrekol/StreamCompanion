using StreamCompanion.Common;
using StreamCompanionTypes.Interfaces;
using StreamCompanionTypes.Interfaces.Services;
using StreamCompanionTypes.Interfaces.Sources;

namespace osu_StreamCompanion.Code.Core.Plugins
{
    [SCPlugin("Plugin manager", "Manage & install plugins", "", "")]
    internal class PluginManagerSettings : IPlugin, ISettingsSource
    {
        public string SettingGroup => "General__Plugins";

        private readonly ISettings _settings;
        private readonly Delegates.Restart _restart;
        private PluginManagerSettingsUserControl _pluginManagerSettingsUserControl;

        public PluginManagerSettings(ISettings settings, Delegates.Restart restart)
        {
            _settings = settings;
            _restart = restart;
        }

        public void Free()
        {
            _pluginManagerSettingsUserControl?.Dispose();
        }

        public object GetUiSettings()
        {
            if (_pluginManagerSettingsUserControl == null || _pluginManagerSettingsUserControl.IsDisposed)
            {
                return _pluginManagerSettingsUserControl = new PluginManagerSettingsUserControl(_settings.GetConfiguration<PluginManagerConfiguration>(LocalPluginManager.PluginManagerConfigEntry), _restart);
            }

            return _pluginManagerSettingsUserControl;
        }
    }
}
