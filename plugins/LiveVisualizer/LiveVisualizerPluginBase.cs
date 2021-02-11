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
        private LiveVisualizerSettings _liveVisualizerSettings;
        protected ISettings Settings;
        protected readonly IContextAwareLogger Logger;
        protected IWpfVisualizerData VisualizerData;
        private bool disposed = false;

        public string Description { get; } = "";
        public string Name { get; } = "LiveVisualizer";
        public string Author { get; } = "Piotrekol";
        public string Url { get; } = "";
        public string UpdateUrl { get; } = "";
        public string SettingGroup { get; } = "Visualizer";

        public LiveVisualizerPluginBase(IContextAwareLogger logger, ISettings settings)
        {
            Logger = logger;
            Settings = settings;
        }
        public virtual void Dispose()
        {
            disposed = true;
            _liveVisualizerSettings?.Dispose();
        }

        public void SetNewMap(IMapSearchResult mapSearchResult, CancellationToken cancellationToken)
        {
            if (disposed)
                return;

            ProcessNewMap(mapSearchResult);
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

        protected abstract void ProcessNewMap(IMapSearchResult mapSearchResult);
    }
}