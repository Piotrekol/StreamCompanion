using System;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Enums;
using StreamCompanionTypes.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LiveVisualizer
{
    public abstract class LiveVisualizerPluginBase : IPlugin, IMapDataGetter, ISettings, IMapDataParser, ISettingsProvider, IDisposable
    {
        protected ISettingsHandler Settings;
        protected IWpfVisualizerData VisualizerData;

        private CancellationTokenSource _cts = new CancellationTokenSource();
        public string SettingGroup { get; } = "Visualizer";
        private LiveVisualizerSettings _liveVisualizerSettings = null;

        public bool Started { get; set; }

        public string Description { get; } = "";
        public string Name { get; } = "LiveVisualizer";
        public string Author { get; } = "Piotrekol";
        public string Url { get; } = "";
        public string UpdateUrl { get; } = "";


        public virtual void Start(ILogger logger)
        {
            Started = true;
        }

        public async void SetNewMap(MapSearchResult mapSearchResult)
        {
            _cts.Cancel();

            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            await Task.Run(() =>
            {
                ProcessNewMap(mapSearchResult, token);
            });
        }

        public void SetSettingsHandle(ISettingsHandler settings)
        {
            Settings = settings;
        }

        public abstract List<OutputPattern> GetFormatedPatterns(Tokens replacements, OsuStatus status);

        protected abstract void ProcessNewMap(MapSearchResult mapSearchResult, CancellationToken token);
        
        public void Free()
        {
            _liveVisualizerSettings?.Dispose();
        }

        public UserControl GetUiSettings()
        {
            if (_liveVisualizerSettings == null || _liveVisualizerSettings.IsDisposed)
            {
                _liveVisualizerSettings = new LiveVisualizerSettings(Settings, VisualizerData.Configuration);
            }

            return _liveVisualizerSettings;
        }

        public virtual void Dispose()
        {
            _cts?.Dispose();
            _liveVisualizerSettings?.Dispose();
        }
    }
}