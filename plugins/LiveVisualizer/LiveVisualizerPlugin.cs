using CollectionManager.Enums;
using Newtonsoft.Json;
using PpCalculator;
using StreamCompanionTypes;
using StreamCompanionTypes.DataTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LiveCharts;
using StreamCompanionTypes.Enums;
using StreamCompanionTypes.Interfaces.Services;

namespace LiveVisualizer
{
    public class LiveVisualizerPlugin : LiveVisualizerPluginBase
    {
        private MainWindow _visualizerWindow;
        private PpCalculator.PpCalculator _ppCalculator;
        private readonly object _ppCalculatorLock = new object();
        private CancellationTokenSource cts = new CancellationTokenSource();

        private List<KeyValuePair<string, IToken>> _liveTokens;
        private IToken _ppToken;
        private IToken _hit100Token;
        private IToken _hit50Token;
        private IToken _hitMissToken;
        private IToken _timeToken;
        private IToken _statusToken;
        private List<KeyValuePair<string, IToken>> Tokens
        {
            get => _liveTokens;
            set
            {
                _ppToken = value.FirstOrDefault(t => t.Key == "ppIfMapEndsNow").Value;
                _hit100Token = value.FirstOrDefault(t => t.Key == "c100").Value;
                _hit50Token = value.FirstOrDefault(t => t.Key == "c50").Value;
                _hitMissToken = value.FirstOrDefault(t => t.Key == "miss").Value;
                _timeToken = value.FirstOrDefault(t => t.Key == "time").Value;
                _statusToken = value.FirstOrDefault(t => t.Key == "status").Value;

                _liveTokens = value;
            }
        }

        private string _lastMapLocation = string.Empty;
        private string _lastMapHash = string.Empty;
        private IModsEx _lastMods = null;


        public LiveVisualizerPlugin(IContextAwareLogger logger, ISettings settings) : base(logger, settings)
        {
            VisualizerData = new VisualizerDataModel();

            LoadConfiguration();

            EnableVisualizer(VisualizerData.Configuration.Enable);

            VisualizerData.Configuration.PropertyChanged += VisualizerConfigurationPropertyChanged;

            Task.Run(async () => { await UpdateLiveTokens(); });
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
                    SetAxisValues(VisualizerData.Display.Strains?.ToList());
                    break;

            }
            SaveConfiguration();
        }

        private void SaveConfiguration()
        {
            var serializedConfig = JsonConvert.SerializeObject(VisualizerData.Configuration);
            Settings.Add(ConfigEntrys.LiveVisualizerConfig.Name, serializedConfig, false);
        }

        private void LoadConfiguration(bool reset = false)
        {
            var rawConfig = Settings.Get<string>(ConfigEntrys.LiveVisualizerConfig);

            if (reset)
            {
                VisualizerData.Configuration.PropertyChanged -= VisualizerConfigurationPropertyChanged;

                //WPF window doesn't update its width when replacing configuration object - workaround
                var newConfiguration = new VisualizerConfiguration();
                VisualizerData.Configuration.WindowWidth = newConfiguration.WindowWidth;
                VisualizerData.Configuration = newConfiguration;
                VisualizerData.Configuration.PropertyChanged += VisualizerConfigurationPropertyChanged;
                VisualizerConfigurationPropertyChanged(this, new PropertyChangedEventArgs("dummy"));

            }

            if (reset || rawConfig == ConfigEntrys.LiveVisualizerConfig.Default<string>())
            {
                VisualizerData.Configuration.ChartCutoffsSet = new SortedSet<int>(new[] { 30, 60, 100, 200, 350 });
                return;
            }

            var config = JsonConvert.DeserializeObject<VisualizerConfiguration>(rawConfig);
            config.AxisYStep = 100;
            config.MaxYValue = 200;

            VisualizerData.Configuration = config;
        }

        protected override void ResetSettings()
        {
            LoadConfiguration(true);
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

        protected override void ProcessNewMap(MapSearchResult mapSearchResult)
        {
            if (VisualizerData == null ||
                !mapSearchResult.FoundBeatmaps ||
                !mapSearchResult.BeatmapsFound[0].IsValidBeatmap(Settings, out var mapLocation) ||
                (mapLocation == _lastMapLocation && mapSearchResult.Mods == _lastMods && _lastMapHash == mapSearchResult.BeatmapsFound[0].Md5)
            )
            {
                if (!mapSearchResult.FoundBeatmaps && VisualizerData != null)
                {
                    VisualizerData.Display.ImageLocation = null;
                    VisualizerData.Display.Artist = null;
                    VisualizerData.Display.Title = null;
                    VisualizerData.Display.Strains?.Clear();
                    _lastMapLocation = null;
                    _lastMapHash = null;
                    lock (_ppCalculatorLock)
                    {
                        _ppCalculator = null;
                    }
                }
                return;
            }

            var workingBeatmap = new ProcessorWorkingBeatmap(mapLocation);

            var playMode = (PlayMode)PpCalculatorHelpers.GetRulesetId(workingBeatmap.RulesetID,
                mapSearchResult.PlayMode.HasValue ? (int?)mapSearchResult.PlayMode : null);

            var ppCalculator = PpCalculatorHelpers.GetPpCalculator((int)playMode, mapLocation, null);

            var strains = new Dictionary<int, double>(300);

            //Length refers to beatmap time, not song total time
            var mapLength = workingBeatmap.Length;
            var strainLength = 5000;
            var interval = 1500;
            int time = 0;

            if (ppCalculator != null && (playMode == PlayMode.Osu || playMode == PlayMode.Taiko || playMode == PlayMode.OsuMania))
            {
                ppCalculator.Mods = mapSearchResult.Mods?.WorkingMods.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                ppCalculator.Score = 1_000_000;

                while (time + strainLength / 2 < mapLength)
                {
                    if (Cts.Token.IsCancellationRequested)
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
            else
            {
                while (time + strainLength / 2 < mapLength)
                {
                    strains.Add(time, 50);
                    time += interval;
                }
            }

            VisualizerData.Display.DisableChartAnimations = strains.Count >= 400; //10min+ maps

            _lastMapLocation = mapLocation;
            _lastMods = mapSearchResult.Mods;
            _lastMapHash = mapSearchResult.BeatmapsFound[0].Md5;

            VisualizerData.Display.TotalTime = mapLength;

            VisualizerData.Display.Title = Tokens.First(r => r.Key == "titleRoman").Value.Value?.ToString();
            VisualizerData.Display.Artist = Tokens.First(r => r.Key == "artistRoman").Value.Value?.ToString();

            if (VisualizerData.Display.Strains == null)
                VisualizerData.Display.Strains = new ChartValues<double>();

            VisualizerData.Display.Strains.Clear();
            var strainValues = strains.Select(kv => kv.Value).ToList();
            SetAxisValues(strainValues);
            VisualizerData.Display.Strains.AddRange(strainValues);

            var imageLocation = Path.Combine(mapSearchResult.BeatmapsFound[0]
                .BeatmapDirectory(BeatmapHelpers.GetFullSongsLocation(Settings)), workingBeatmap.BackgroundFile ?? "");

            VisualizerData.Display.ImageLocation = File.Exists(imageLocation) ? imageLocation : null;

            lock (_ppCalculatorLock)
            {
                _ppCalculator = ppCalculator;
            }

            _visualizerWindow?.ForceChartUpdate();
        }
        private void SetAxisValues(IReadOnlyList<double> strainValues)
        {
            if (strainValues != null && strainValues.Any())
            {
                var strainsMax = strainValues.Max();
                VisualizerData.Configuration.MaxYValue = getMaxY(strainsMax);
                VisualizerData.Configuration.AxisYStep = GetAxisYStep(double.IsNaN(VisualizerData.Configuration.MaxYValue)
                    ? strainsMax
                    : VisualizerData.Configuration.MaxYValue);
            }
            else
            {
                VisualizerData.Configuration.MaxYValue = 200;
                VisualizerData.Configuration.AxisYStep = 100;
            }
        }

        public override List<IOutputPattern> GetOutputPatterns(Tokens replacements, OsuStatus status)
        {
            //TODO: UHH... would be nice to have better way of sharing this data (instead of relying on Tokens with magic strings and blind value casts)
            Tokens = replacements.ToList();

            return null;
        }

        private async Task UpdateLiveTokens()
        {
            while (true)
            {
                while (!VisualizerData.Configuration.Enable)
                {
                    await Task.Delay(500);
                }

                try
                {
                    if (Tokens != null)
                    {
                        //Blind casts :/
                        VisualizerData.Display.CurrentTime = (double)_timeToken.Value * 1000;

                        var normalizedCurrentTime = VisualizerData.Display.CurrentTime < 0 ? 0 : VisualizerData.Display.CurrentTime;
                        var progress = VisualizerData.Configuration.WindowWidth * (normalizedCurrentTime / VisualizerData.Display.TotalTime);
                        VisualizerData.Display.PixelMapProgress = progress < VisualizerData.Configuration.WindowWidth
                            ? progress
                            : VisualizerData.Configuration.WindowWidth;

                        var status = (OsuStatus)_statusToken.Value;
                        if (VisualizerData.Configuration.SimulatePPWhenListening &&
                            (status == OsuStatus.Editing || status == OsuStatus.Listening))
                        {
                            lock (_ppCalculatorLock)
                            {
                                if (_ppCalculator != null)
                                {
                                    _ppCalculator.Goods = null;
                                    _ppCalculator.Mehs = null;
                                    _ppCalculator.Misses = 0;
                                    _ppCalculator.Accuracy = 100;
                                    var pp = Math.Round(_ppCalculator.Calculate(VisualizerData.Display.CurrentTime, null));
                                    VisualizerData.Display.Pp = pp < 0 ? 0 : pp;
                                }
                            }
                        }
                        else
                        {
                            VisualizerData.Display.Pp = Math.Round((double)_ppToken.Value);
                            VisualizerData.Display.Hit100 = (ushort)_hit100Token.Value;
                            VisualizerData.Display.Hit50 = (ushort)_hit50Token.Value;
                            VisualizerData.Display.HitMiss = (ushort)_hitMissToken.Value;

                        }
                    }
                }
                catch
                {
                    return;
                }

                Thread.Sleep(22);

                if (cts.Token.IsCancellationRequested)
                    return;
            }
        }

        private bool AutomaticAxisControlIsEnabled => VisualizerData.Configuration.AutoSizeAxisY;
        private double getMaxY(double maxValue)
        {
            if (!AutomaticAxisControlIsEnabled)
            {
                foreach (var cutoff in VisualizerData.Configuration.ChartCutoffsSet)
                {
                    if (maxValue < cutoff && cutoff > 0)
                        return cutoff;
                }
            }

            var maxY = Math.Ceiling(maxValue);
            return maxY > 0
                ? maxY
                : 200;
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
            cts.Cancel();
            _visualizerWindow.Close();
            base.Dispose();
        }

    }
}
