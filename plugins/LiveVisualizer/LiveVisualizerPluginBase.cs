using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Interfaces;

namespace LiveVisualizer
{
    public abstract class LiveVisualizerPluginBase : IPlugin, IMapDataGetter, IMapDataParser,
        ISettingsProvider, IDisposable
    {
        private CancellationTokenSource _cts = new CancellationTokenSource();
        private LiveVisualizerSettings _liveVisualizerSettings;
        private CancellationToken _token = CancellationToken.None;
        protected ISettingsHandler Settings;
        protected IWpfVisualizerData VisualizerData;

        public virtual void Dispose()
        {
            _cts?.Dispose();
            _liveVisualizerSettings?.Dispose();
        }

        public async void SetNewMap(MapSearchResult mapSearchResult)
        {
            CancelNewMapProcessing();

            await Task.Run(() => ProcessNewMap(mapSearchResult, _token), _token);
        }

        public abstract List<OutputPattern> GetFormatedPatterns(Tokens replacements, OsuStatus status);

        public bool Started { get; set; }

        public string Description { get; } = "";
        public string Name { get; } = "LiveVisualizer";
        public string Author { get; } = "Piotrekol";
        public string Url { get; } = "";
        public string UpdateUrl { get; } = "";


        public virtual void Start(ILogger logger)
        {
            _token = _cts.Token;
            Started = true;
        }

        public void SetSettingsHandle(ISettingsHandler settings)
        {
            Settings = settings;
        }

        public string SettingGroup { get; } = "Visualizer";

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

        private void CancelNewMapProcessing()
        {
            _cts.Cancel();
            _cts.Dispose();
            _cts = new CancellationTokenSource();
            _token = _cts.Token;
        }

        protected abstract void ProcessNewMap(MapSearchResult mapSearchResult, CancellationToken token);
    }
}