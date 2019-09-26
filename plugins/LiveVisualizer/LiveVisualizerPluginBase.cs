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
        private bool _disposed = false;

        public string Description { get; } = "";
        public string Name { get; } = "LiveVisualizer";
        public string Author { get; } = "Piotrekol";
        public string Url { get; } = "";
        public string UpdateUrl { get; } = "";
        public string SettingGroup { get; } = "Visualizer";

        public LiveVisualizerPluginBase(ISettingsHandler settings)
        {
            Settings = settings;
            _token = _cts.Token;
        }
        public virtual void Dispose()
        {
            _disposed = true;
            _cts?.Dispose();
            _liveVisualizerSettings?.Dispose();
        }

        public async void SetNewMap(MapSearchResult mapSearchResult)
        {
            CancelNewMapProcessing();

            try
            {
                await Task.Run(() => ProcessNewMap(mapSearchResult, _token), _token);
            }
            catch (TaskCanceledException)
            {
            }
        }

        public abstract List<OutputPattern> GetFormatedPatterns(Tokens replacements, OsuStatus status);

        public void Free()
        {
            _liveVisualizerSettings?.Dispose();
        }

        public UserControl GetUiSettings()
        {
            if (_liveVisualizerSettings == null || _liveVisualizerSettings.IsDisposed)
            {
                _liveVisualizerSettings = new LiveVisualizerSettings(Settings, VisualizerData.Configuration);
                _liveVisualizerSettings.ResetSettings += (_, __) => ResetSettings();
            }

            return _liveVisualizerSettings;
        }

        private void CancelNewMapProcessing()
        {
            if (_disposed)
            {
                return;
            }

            _cts.Cancel();
            _cts.Dispose();
            _cts = new CancellationTokenSource();
            _token = _cts.Token;
        }

        protected abstract void ResetSettings();

        protected abstract void ProcessNewMap(MapSearchResult mapSearchResult, CancellationToken token);
    }
}