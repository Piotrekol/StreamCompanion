using CollectionManager.Enums;
using LiveCharts.Helpers;
using PpCalculator;
using StreamCompanionTypes;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Enums;
using StreamCompanionTypes.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LiveVisualizer
{
    public class LiveVisualizerPlugin : LiveVisualizerPluginBase
    {
        private IWpfVisualizerData _visualizerData;
        private MainWindow _visualizerWindow;

        private List<KeyValuePair<string, Token>> _liveTokens;
        private TokenWithFormat _ppToken;
        private TokenWithFormat _hit100Token;
        private TokenWithFormat _hit50Token;
        private TokenWithFormat _hitMissToken;
        private TokenWithFormat _timeToken;
        private List<KeyValuePair<string, Token>> LiveTokens
        {
            get => _liveTokens;
            set
            {
                _ppToken = (TokenWithFormat)value.FirstOrDefault(t => t.Key == "PpIfMapEndsNow").Value;
                _hit100Token = (TokenWithFormat)value.FirstOrDefault(t => t.Key == "100").Value;
                _hit50Token = (TokenWithFormat)value.FirstOrDefault(t => t.Key == "50").Value;
                _hitMissToken = (TokenWithFormat)value.FirstOrDefault(t => t.Key == "miss").Value;
                _timeToken = (TokenWithFormat)value.FirstOrDefault(t => t.Key == "time").Value;

                _liveTokens = value;
            }
        }

        private string _lastMapLocation = string.Empty;


        public override void Start(ILogger logger)
        {
            base.Start(logger);
            _visualizerData = new VisualizerDataModel();

            _visualizerWindow = new MainWindow(_visualizerData);
            _visualizerWindow.Show();

            Task.Run(() => { UpdateLiveTokens(); });
        }

        protected override void ProcessNewMap(MapSearchResult mapSearchResult, CancellationToken token)
        {
            if (_visualizerData == null ||
                !mapSearchResult.FoundBeatmaps ||
                !mapSearchResult.BeatmapsFound[0].IsValidBeatmap(Settings, out var mapLocation) ||
                mapLocation == _lastMapLocation
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
                    var strain = ppCalculator.Calculate(time, time + strainLength, a);

                    if (double.IsNaN(strain) || strain < 0)
                        strain = 0;
                    strains.Add(time, strain);
                    time += interval;
                }
            }

            _lastMapLocation = mapLocation;

            _visualizerData.TotalTime = mapLength;

            _visualizerData.Strains = strains.Select(s => s.Value).AsChartValues();

            if (strains.Any())
                _visualizerData.MaxYValue = getMaxY(_visualizerData.Strains.Max());

            var imageLocation = Path.Combine(mapSearchResult.BeatmapsFound[0]
                .BeatmapDirectory(BeatmapHelpers.GetFullSongsLocation(Settings)), workingBeatmap.BackgroundFile);

            if (File.Exists(imageLocation))
                _visualizerData.ImageLocation = imageLocation;



        }

        public override List<OutputPattern> GetFormatedPatterns(Tokens replacements, OsuStatus status)
        {
            //TODO: UHH... would be nice to have better way of sharing this data (instead of relying on Tokens with magic strings and blind value casts)
            LiveTokens = replacements.Where(r => r.Value.Type == TokenType.Live).ToList();

            if (_visualizerData == null)
                return null;

            _visualizerData.Title = replacements.First(r => r.Key == "TitleRoman").Value.Value?.ToString();
            _visualizerData.Artist = replacements.First(r => r.Key == "ArtistRoman").Value.Value?.ToString();

            return null;
        }

        private void UpdateLiveTokens()
        {
            while (true)
            {
                try
                {
                    if (LiveTokens != null)
                    {
                        //Blind casts :/
                        _visualizerData.Pp = Math.Round((double)_ppToken.Value);
                        _visualizerData.Hit100 = (ushort)_hit100Token.Value;
                        _visualizerData.Hit50 = (ushort)_hit50Token.Value;
                        _visualizerData.HitMiss = (ushort)_hitMissToken.Value;
                        _visualizerData.CurrentTime = (double)_timeToken.Value * 1000;
                        _visualizerData.PixelMapProgress = 700 * (_visualizerData.CurrentTime / _visualizerData.TotalTime);
                    }
                }
                catch
                {
                    return;
                }

                Thread.Sleep(22);
            }
        }


        private double getMaxY(double maxValue)
        {
            if (maxValue < 50)
                return 50;
            if (maxValue < 100)
                return 100;
            if (maxValue < 200)
                return 200;
            if (maxValue < 350)
                return 350;

            return double.NaN;//auto size
        }

    }
}
