using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Enums;
using StreamCompanionTypes.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LiveVisualizer
{
    public abstract class LiveVisualizerPluginBase : IPlugin, IMapDataGetter, ISettings, IMapDataParser
    {
        protected ISettingsHandler Settings;

        private CancellationTokenSource _cts = new CancellationTokenSource();

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
    }
}