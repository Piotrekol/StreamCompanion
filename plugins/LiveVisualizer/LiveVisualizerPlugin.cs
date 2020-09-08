using Newtonsoft.Json;
using System.ComponentModel;
using StreamCompanionTypes.Interfaces.Services;

namespace LiveVisualizer
{
    public class LiveVisualizerPlugin : LiveVisualizerPluginBase
    {
        
        public LiveVisualizerPlugin(IContextAwareLogger logger, ISettings settings, ISaver saver) : base(logger, settings, saver)
        {
            VisualizerConfiguration = new VisualizerConfiguration();
            LoadConfiguration();
            VisualizerConfiguration.PropertyChanged += VisualizerConfigurationPropertyChanged;
        }

        private void VisualizerConfigurationPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            SaveConfiguration();
        }

        private void SaveConfiguration()
        {
            var serializedConfig = JsonConvert.SerializeObject(VisualizerConfiguration);
            Settings.Add(ConfigEntrys.LiveVisualizerConfig.Name, serializedConfig, false);
        }

        private void LoadConfiguration(bool reset = false)
        {
            var rawConfig = Settings.Get<string>(ConfigEntrys.LiveVisualizerConfig);

            if (reset)
            {
                VisualizerConfiguration.PropertyChanged -= VisualizerConfigurationPropertyChanged;

                //WPF window doesn't update its width when replacing configuration object - workaround
                var newConfiguration = new VisualizerConfiguration();
                VisualizerConfiguration = newConfiguration;
                VisualizerConfiguration.PropertyChanged += VisualizerConfigurationPropertyChanged;
                VisualizerConfigurationPropertyChanged(this, new PropertyChangedEventArgs("dummy"));

            }

            if (reset || rawConfig == ConfigEntrys.LiveVisualizerConfig.Default<string>())
                return;

            var config = JsonConvert.DeserializeObject<VisualizerConfiguration>(rawConfig);

            VisualizerConfiguration = config;
        }

        protected override void ResetSettings()
        {
            LoadConfiguration(true);
        }
    }
}
