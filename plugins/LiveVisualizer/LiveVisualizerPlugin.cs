using CollectionManager.Enums;
using LiveCharts.Helpers;
using Newtonsoft.Json;
using PpCalculator;
using StreamCompanionTypes;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Enums;
using StreamCompanionTypes.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ConfigEntry = StreamCompanionTypes.DataTypes.ConfigEntry;

namespace LiveVisualizer
{
    public class LiveVisualizerPlugin : LiveVisualizerPluginBase
    {
        private MainWindow _visualizerWindow;

        private List<KeyValuePair<string, Token>> _liveTokens;
        private Token _ppToken;
        private Token _hit100Token;
        private Token _hit50Token;
        private Token _hitMissToken;
        private Token _timeToken;
        private List<KeyValuePair<string, Token>> Tokens
        {
            get => _liveTokens;
            set
            {
                _ppToken = value.FirstOrDefault(t => t.Key == "PpIfMapEndsNow").Value;
                _hit100Token = value.FirstOrDefault(t => t.Key == "100").Value;
                _hit50Token = value.FirstOrDefault(t => t.Key == "50").Value;
                _hitMissToken = value.FirstOrDefault(t => t.Key == "miss").Value;
                _timeToken = value.FirstOrDefault(t => t.Key == "time").Value;

                _liveTokens = value;
            }
        }

        private string _lastMapLocation = string.Empty;


        public override void Start(ILogger logger)
        {
            base.Start(logger);
            VisualizerData = new VisualizerDataModel();

            LoadConfiguration();

            EnableVisualizer(VisualizerData.Configuration.Enable);

            VisualizerData.Configuration.PropertyChanged += VisualizerConfigurationPropertyChanged;

            Task.Run(() => { UpdateLiveTokens(); });
        }

        private void VisualizerConfigurationPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(IVisualizerConfiguration.Enable):
                    EnableVisualizer(VisualizerData.Configuration.Enable);
                    break;
                case nameof(IVisualizerConfiguration.AutoSizeAxisY):
                case nameof(IVisualizerConfiguration.ChartCutoffsSet):
                    SetAxisValues();
                    break;

            }
            SaveConfiguration();
        }

        private void SaveConfiguration()
        {
            var serializedConfig = JsonConvert.SerializeObject(VisualizerData.Configuration);
            Settings.Add(ConfigEntrys.LiveVisualizerConfig.Name, serializedConfig, false);
        }

        private void LoadConfiguration()
        {
            var config = Settings.Get<string>(ConfigEntrys.LiveVisualizerConfig);

            if (config == ConfigEntrys.LiveVisualizerConfig.Default<string>())
            {
                VisualizerData.Configuration.ChartCutoffsSet = new SortedSet<int>(new[] { 30, 60, 100, 200, 350 });
                return;
            }

            VisualizerData.Configuration = JsonConvert.DeserializeObject<VisualizerConfiguration>(config);
        }

        private void EnableVisualizer(bool enable)
        {
            if (enable)
            {
                if (_visualizerWindow == null || !_visualizerWindow.IsLoaded)
                    _visualizerWindow = new MainWindow(VisualizerData);

                _visualizerWindow.Width = VisualizerData.Configuration.WindowWidth;
                _visualizerWindow.Height = VisualizerData.Configuration.WindowHeight;
                _visualizerWindow.Show();

            }
            else
            {
                _visualizerWindow?.Hide();
            }
        }


        protected override void ProcessNewMap(MapSearchResult mapSearchResult, CancellationToken token)
        {
            if (VisualizerData == null ||
                !mapSearchResult.FoundBeatmaps ||
                !mapSearchResult.BeatmapsFound[0].IsValidBeatmap(Settings, out var mapLocation) ||
                mapLocation == _lastMapLocation
            )
                return;

            var workingBeatmap = new ProcessorWorkingBeatmap(mapLocation);

            var playMode = (PlayMode)PpCalculatorHelpers.GetRulesetId(workingBeatmap.RulesetID,
                mapSearchResult.PlayMode.HasValue ? (int?)mapSearchResult.PlayMode : null);

            var ppCalculator = PpCalculatorHelpers.GetPpCalculator((int)playMode, mapLocation, null);

            var strains = new Dictionary<int, double>(300);

            //Length refers to beatmap time, not song total time
            var mapLength = workingBeatmap.Length;

            if (ppCalculator != null && (playMode == PlayMode.Osu || playMode == PlayMode.Taiko))
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
                    else if (strain > 2000)
                        strain = 2000;//lets not freeze everything with aspire/fancy 100* maps

                    strains.Add(time, strain);
                    time += interval;
                }
            }

            VisualizerData.Display.DisableChartAnimations = strains.Count >= 400; //10min+ maps

            _lastMapLocation = mapLocation;

            VisualizerData.Display.TotalTime = mapLength;

            VisualizerData.Display.Title = Tokens.First(r => r.Key == "TitleRoman").Value.Value?.ToString();
            VisualizerData.Display.Artist = Tokens.First(r => r.Key == "ArtistRoman").Value.Value?.ToString();

            VisualizerData.Display.Strains = strains.Select(s => s.Value).AsChartValues();

            SetAxisValues();

            var imageLocation = Path.Combine(mapSearchResult.BeatmapsFound[0]
                .BeatmapDirectory(BeatmapHelpers.GetFullSongsLocation(Settings)), workingBeatmap.BackgroundFile ?? "");

            VisualizerData.Display.ImageLocation = File.Exists(imageLocation) ? imageLocation : null;

        }

        private void SetAxisValues()
        {
            if (VisualizerData.Display.Strains != null && VisualizerData.Display.Strains.Any())
            {
                var strainsMax = VisualizerData.Display.Strains.Max();
                VisualizerData.Configuration.MaxYValue = getMaxY(strainsMax);
                VisualizerData.Configuration.AxisYStep = GetAxisYStep(double.IsNaN(VisualizerData.Configuration.MaxYValue)
                    ? strainsMax
                    : VisualizerData.Configuration.MaxYValue);
            }
        }

        public override List<OutputPattern> GetFormatedPatterns(Tokens replacements, OsuStatus status)
        {
            //TODO: UHH... would be nice to have better way of sharing this data (instead of relying on Tokens with magic strings and blind value casts)
            Tokens = replacements.ToList();

            return null;
        }

        private void UpdateLiveTokens()
        {
            while (true)
            {
                try
                {
                    if (Tokens != null)
                    {
                        //Blind casts :/
                    VisualizerData.Display.Pp = Math.Round((double)_ppToken.Value);
                    VisualizerData.Display.Hit100 = (ushort)_hit100Token.Value;
                    VisualizerData.Display.Hit50 = (ushort)_hit50Token.Value;
                    VisualizerData.Display.HitMiss = (ushort)_hitMissToken.Value;
                    VisualizerData.Display.CurrentTime = (double)_timeToken.Value * 1000;

                    var normalizedCurrentTime = VisualizerData.Display.CurrentTime < 0 ? 0 : VisualizerData.Display.CurrentTime;
                    var progress = VisualizerData.Configuration.WindowWidth * (normalizedCurrentTime / VisualizerData.Display.TotalTime);
                    VisualizerData.Display.PixelMapProgress = progress < VisualizerData.Configuration.WindowWidth
                            ? progress
                            : VisualizerData.Configuration.WindowWidth;
                    }
                }
                catch
                {
                    return;
                }

                Thread.Sleep(22);
            }
        }

        private bool AutomaticAxisControlIsEnabled => VisualizerData.Configuration.AutoSizeAxisY;
        private double getMaxY(double maxValue)
        {
            if (!AutomaticAxisControlIsEnabled)
                foreach (var cutoff in VisualizerData.Configuration.ChartCutoffsSet)
                {
                    if (maxValue < cutoff)
                        return cutoff;
                }

            return Math.Ceiling(maxValue);
        }

        private double GetAxisYStep(double maxYValue)
        {
            if (double.IsNaN(maxYValue) || maxYValue <= 0)
                return 100;

            if (maxYValue < 10)
                maxYValue += 10;
            return RoundOff((int)(maxYValue / 3.1));

            int RoundOff(int num)
            {
                return ((int)Math.Ceiling(num / 10.0)) * 10;
            }
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
