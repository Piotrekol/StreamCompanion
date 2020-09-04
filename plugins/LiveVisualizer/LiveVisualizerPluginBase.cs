using System;
using System.Diagnostics;
using System.IO;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Interfaces;
using StreamCompanionTypes.Interfaces.Services;
using StreamCompanionTypes.Interfaces.Sources;
using WebSocketDataSender;

namespace LiveVisualizer
{
    public abstract class LiveVisualizerPluginBase : IPlugin, ISettingsSource, IDisposable
    {
        private LiveVisualizerSettings _liveVisualizerSettings;
        protected ISettings Settings;
        private readonly ISaver _saver;
        protected readonly IContextAwareLogger Logger;
        protected IVisualizerConfiguration VisualizerConfiguration;
        protected Tokens.TokenSetter TokenSetter;

        public string Description { get; } = "";
        public string Name { get; } = "LiveVisualizer";
        public string Author { get; } = "Piotrekol";
        public string Url { get; } = "";
        public string UpdateUrl { get; } = "";
        public string SettingGroup { get; } = "Visualizer";

        public LiveVisualizerPluginBase(IContextAwareLogger logger, ISettings settings, ISaver saver)
        {
            Logger = logger;
            Settings = settings;
            _saver = saver;
            TokenSetter = Tokens.CreateTokenSetter(nameof(LiveVisualizerPlugin));
        }

        public virtual void Dispose()
        {
            _liveVisualizerSettings?.Dispose();
        }

        public void Free()
        {
            _liveVisualizerSettings?.Dispose();
        }

        public object GetUiSettings()
        {
            if (_liveVisualizerSettings == null || _liveVisualizerSettings.IsDisposed)
            {
                var filesLocation = WebSocketDataGetter.HttpContentRoot(_saver);
                var webUrl = WebSocketDataGetter.BaseAddress(Settings);
                _liveVisualizerSettings = new LiveVisualizerSettings(Settings, VisualizerConfiguration);
                _liveVisualizerSettings.ResetSettings += (_, __) => ResetSettings();
                _liveVisualizerSettings.OpenWebUrl += (_, __) => Process.Start(webUrl);
                _liveVisualizerSettings.OpenFilesFolder += (_, __) => Process.Start("explorer.exe", filesLocation);
                _liveVisualizerSettings.FilesLocation = filesLocation;
                _liveVisualizerSettings.WebUrl = webUrl;

            }

            return _liveVisualizerSettings;
        }

        protected abstract void ResetSettings();
    }
}