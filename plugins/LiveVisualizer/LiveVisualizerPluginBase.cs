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
using StreamCompanion.Common;

namespace LiveVisualizer
{
    [SCPlugin("Visualizer", "Displays current gameplay info & stats in a separate window", Consts.SCPLUGIN_AUTHOR, Consts.SCPLUGIN_BASEURL)]
    public abstract class LiveVisualizerPluginBase : IPlugin, IMapDataConsumer, IOutputPatternSource,
        ISettingsSource, IDisposable
    {
        private LiveVisualizerSettings _liveVisualizerSettings;
        protected ISettings Settings;
        protected readonly IContextAwareLogger Logger;
        protected IWpfVisualizerData VisualizerData;
        private bool disposed = false;
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

        public Task SetNewMapAsync(IMapSearchResult map, CancellationToken cancellationToken)
        {
            if (disposed)
                return Task.CompletedTask;

            ProcessNewMap(map);
            return Task.CompletedTask;
        }

        public abstract Task<List<IOutputPattern>> GetOutputPatterns(IMapSearchResult map, Tokens tokens, OsuStatus status);

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