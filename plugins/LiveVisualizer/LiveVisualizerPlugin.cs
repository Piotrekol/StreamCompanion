using CollectionManager.Enums;
using PpCalculator;
using StreamCompanionTypes;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LiveVisualizer
{
    public class LiveVisualizerPlugin : IPlugin, IMapDataGetter, ISettings
    {
        private ISettingsHandler _settings;
        private IVisualizerForm _visualizer;
        public bool Started { get; set; }
        public void Start(ILogger logger)
        {
            Started = true;

            _visualizer?.Show();
        }

        public string Description { get; } = "";
        public string Name { get; } = "LiveVisualizer";
        public string Author { get; } = "Piotrekol";
        public string Url { get; } = "";
        public string UpdateUrl { get; } = "";
        CancellationTokenSource _cts = new CancellationTokenSource();
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

        private void ProcessNewMap(MapSearchResult mapSearchResult, CancellationToken token)
        {
            if (_visualizer == null ||
                !mapSearchResult.FoundBeatmaps ||
                !mapSearchResult.BeatmapsFound[0].IsValidBeatmap(_settings, out var mapLocation)
            )
                return;

            var workingBeatmap = new ProcessorWorkingBeatmap(mapLocation);

            var playMode = (PlayMode)PpCalculatorHelpers.GetRulesetId(workingBeatmap.RulesetID,
                mapSearchResult.PlayMode.HasValue ? (int?)mapSearchResult.PlayMode : null);

            var ppCalculator = PpCalculatorHelpers.GetPpCalculator((int)playMode, mapLocation, null);

            if (ppCalculator == null)
                return;


            var strains = new Dictionary<int, double>(300);
            var mapLength = workingBeatmap.Length;

            if (playMode != PlayMode.OsuMania)
            {
                var strainLength = 5000;
                var interval = 1500;
                int time = 0;
                while (time + strainLength / 2 < mapLength)
                {
                    if (token.IsCancellationRequested)
                        return;
                    var a = new Dictionary<string, double>();
                    var s = ppCalculator.Calculate(time, time + strainLength, a);

                    if (double.IsNaN(s) || s < 0)
                        s = 0;
                    strains.Add(time, s);
                    time += interval;
                }

            }

            _visualizer.SetStrains(strains.Select(s => s.Value).ToList());

            var imageLocation = Path.Combine(mapSearchResult.BeatmapsFound[0]
                .BeatmapDirectory(BeatmapHelpers.GetFullSongsLocation(_settings)), workingBeatmap.BackgroundFile);

            if (File.Exists(imageLocation))
                _visualizer.SetBackgroundImage(imageLocation);

            _visualizer.SetHits(0, 0, 0);

            _visualizer.SetPp(727d);
        }




        public void SetSettingsHandle(ISettingsHandler settings)
        {
            _settings = settings;
        }
    }
}
