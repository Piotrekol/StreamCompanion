using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using StreamCompanionTypes.Enums;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Interfaces;
using StreamCompanionTypes.Interfaces.Consumers;
using StreamCompanionTypes.Interfaces.Services;
using StreamCompanionTypes.Interfaces.Sources;

namespace LiveVisualizer
{
    public abstract class LiveVisualizerPluginBase : IPlugin, IMapDataConsumer, IOutputPatternGenerator,
        ISettingsSource, IDisposable
    {
        protected CancellationTokenSource Cts = new CancellationTokenSource();
        private LiveVisualizerSettings _liveVisualizerSettings;
        protected ISettings Settings;
        protected IWpfVisualizerData VisualizerData;
        private Task processNewMapTask;
        private bool disposed = false;

        public string Description { get; } = "";
        public string Name { get; } = "LiveVisualizer";
        public string Author { get; } = "Piotrekol";
        public string Url { get; } = "";
        public string UpdateUrl { get; } = "";
        public string SettingGroup { get; } = "Visualizer";

        public LiveVisualizerPluginBase(ISettings settings)
        {
            Settings = settings;
        }
        public virtual void Dispose()
        {
            disposed = true;
            _liveVisualizerSettings?.Dispose();
        }

        public async void SetNewMap(MapSearchResult mapSearchResult)
        {
            if (disposed)
                return;

            Cts.Cancel();
            if (processNewMapTask != null)
                await processNewMapTask.ConfigureAwait(false);

            if (processNewMapTask == null || processNewMapTask.IsCanceled || processNewMapTask.IsFaulted ||
                processNewMapTask.IsCompleted)
            {
                Cts = new CancellationTokenSource();
                processNewMapTask = Task.Run(() => ProcessNewMap(mapSearchResult), Cts.Token);
            }
        }

        public abstract List<IOutputPattern> GetOutputPatterns(Tokens replacements, OsuStatus status);

        public void Free()
        {
            _liveVisualizerSettings?.Dispose();
        }

        public object GetUiSettings()
        {
            if (_liveVisualizerSettings == null || _liveVisualizerSettings.IsDisposed)
            {
                _liveVisualizerSettings = new LiveVisualizerSettings(Settings, VisualizerData.Configuration);
                _liveVisualizerSettings.ResetSettings += (_, __) => ResetSettings();
            }

            return _liveVisualizerSettings;
        }

        protected abstract void ResetSettings();

        protected abstract void ProcessNewMap(MapSearchResult mapSearchResult);
    }
}