using CollectionManager.DataTypes;
using CollectionManager.Enums;
using PpCalculator;
using StreamCompanionTypes;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Interfaces;
using System;
using System.IO;

namespace BeatmapPpReplacements
{
    public class PpReplacements : IPlugin, IMapDataReplacements, ISettings
    {
        private readonly SettingNames _names = SettingNames.Instance;

        private const string PpFormat = "{0:0.00}";


        private PpCalculator.PpCalculator _ppCalculator = null;

        private ISettingsHandler _settings;
        private Mods _lastMods;
        private string _lastModsStr = "None";
        public bool Started { get; set; }

        public string Description { get; } = "";
        public string Name { get; } = nameof(PpReplacements);
        public string Author { get; } = "Piotrekol";
        public string Url { get; } = "";
        public string UpdateUrl { get; } = "";

        public void Start(ILogger logger)
        {
            Started = true;
        }

        public Tokens GetMapReplacements(MapSearchResult map)
        {
            var ret = new Tokens
            {
                {"GameMode", new Token(null)},
                {"MaxCombo", new Token(null)},

                {"SSPP", new Token(null)},
                {"99.9PP", new Token(null)},
                {"99PP", new Token(null)},
                {"98PP", new Token(null)},
                {"95PP", new Token(null)},
                {"90PP", new Token(null)},


                {"1 000 000PP", new Token(null)},
                {"990 000PP", new Token(null)},
                {"950 000PP", new Token(null)},
                {"900 000PP", new Token(null)},
                {"800 000PP", new Token(null)},
                {"700 000PP", new Token(null)},
                {"600 000PP", new Token(null)},

                {"mMod", new Token(null)},

                {"mSSPP", new Token(null)},
                {"m99.9PP", new Token(null)},
                {"m99PP", new Token(null)},
                {"m98PP", new Token(null)},
                {"m95PP", new Token(null)},
                {"m90PP", new Token(null)},

                {"m1 000 000PP", new Token(null)},
                {"m990 000PP", new Token(null)},
                {"m950 000PP", new Token(null)},
                {"m900 000PP", new Token(null)},
                {"m800 000PP", new Token(null)},
                {"m700 000PP", new Token(null)},
                {"m600 000PP", new Token(null)},
            };

            if (!map.FoundBeatmaps) return ret;
            var mapLocation = map.BeatmapsFound[0].FullOsuFileLocation(BeatmapHelpers.GetFullSongsLocation(_settings));

            if (!File.Exists(mapLocation)) return ret;
            FileInfo file = new FileInfo(mapLocation);

            if (file.Length == 0) return ret;

            var workingBeatmap = new ProcessorWorkingBeatmap(mapLocation);

            var playMode = (PlayMode)PpCalculatorHelpers.GetRulesetId(workingBeatmap.RulesetID, map.PlayMode.HasValue ? (int?)map.PlayMode : null);

            _ppCalculator = PpCalculatorHelpers.GetPpCalculator((int)playMode, mapLocation, _ppCalculator);

            if (_ppCalculator == null)
                return ret;//Ctb not supported :(

            if (playMode == PlayMode.OsuMania)
                _ppCalculator.Score = 1_000_000;
            else
                _ppCalculator.Score = 0;

            _ppCalculator.Mods = null;

            ret["GameMode"] = new TokenWithFormat(playMode.ToString());

            Mods mods = Mods.Omod;


            if (playMode == PlayMode.OsuMania)
            {
                ret["1 000 000PP"] = new TokenWithFormat(GetPp(_ppCalculator, 0, mods, 1_000_000), format: PpFormat);
                ret["990 000PP"] = new TokenWithFormat(GetPp(_ppCalculator, 0, mods, 990_000), format: PpFormat);
                ret["950 000PP"] = new TokenWithFormat(GetPp(_ppCalculator, 0, mods, 950_000), format: PpFormat);
                ret["900 000PP"] = new TokenWithFormat(GetPp(_ppCalculator, 0, mods, 900_000), format: PpFormat);
                ret["800 000PP"] = new TokenWithFormat(GetPp(_ppCalculator, 0, mods, 800_000), format: PpFormat);
                ret["700 000PP"] = new TokenWithFormat(GetPp(_ppCalculator, 0, mods, 700_000), format: PpFormat);
                ret["600 000PP"] = new TokenWithFormat(GetPp(_ppCalculator, 0, mods, 600_000), format: PpFormat);
            }
            else
            {
                ret["SSPP"] = new TokenWithFormat(GetPp(_ppCalculator, 100d), format: PpFormat);
                ret["99.9PP"] = new TokenWithFormat(GetPp(_ppCalculator, 99.9d), format: PpFormat);
                ret["99PP"] = new TokenWithFormat(GetPp(_ppCalculator, 99d), format: PpFormat);
                ret["98PP"] = new TokenWithFormat(GetPp(_ppCalculator, 98d), format: PpFormat);
                ret["95PP"] = new TokenWithFormat(GetPp(_ppCalculator, 95d), format: PpFormat);
                ret["90PP"] = new TokenWithFormat(GetPp(_ppCalculator, 90d), format: PpFormat);
            }


            ret["MaxCombo"] = new TokenWithFormat(_ppCalculator.GetMaxCombo());


            string modsStr;
            if (map.Action == OsuStatus.Playing || map.Action == OsuStatus.Watching)
            {
                mods = map.Mods?.Item1 ?? Mods.Omod;
                modsStr = map.Mods?.Item2 ?? "NM";
                _lastMods = mods;
                _lastModsStr = modsStr;
            }
            else
            {
                mods = _lastMods;
                modsStr = _lastModsStr;
            }

            ret["mMod"] = new Token(modsStr);

            if (playMode == PlayMode.OsuMania)
            {
                ret["m1 000 000PP"] = new TokenWithFormat(GetPp(_ppCalculator, 0, mods, 1_000_000), format: PpFormat);
                ret["m990 000PP"] = new TokenWithFormat(GetPp(_ppCalculator, 0, mods, 990_000), format: PpFormat);
                ret["m950 000PP"] = new TokenWithFormat(GetPp(_ppCalculator, 0, mods, 950_000), format: PpFormat);
                ret["m900 000PP"] = new TokenWithFormat(GetPp(_ppCalculator, 0, mods, 900_000), format: PpFormat);
                ret["m800 000PP"] = new TokenWithFormat(GetPp(_ppCalculator, 0, mods, 800_000), format: PpFormat);
                ret["m700 000PP"] = new TokenWithFormat(GetPp(_ppCalculator, 0, mods, 700_000), format: PpFormat);
                ret["m600 000PP"] = new TokenWithFormat(GetPp(_ppCalculator, 0, mods, 600_000), format: PpFormat);
            }
            else
            {
                ret["mSSPP"] = new TokenWithFormat(GetPp(_ppCalculator, 100d, mods), format: PpFormat);
                ret["m99.9PP"] = new TokenWithFormat(GetPp(_ppCalculator, 99.9d, mods), format: PpFormat);
                ret["m99PP"] = new TokenWithFormat(GetPp(_ppCalculator, 99d, mods), format: PpFormat);
                ret["m98PP"] = new TokenWithFormat(GetPp(_ppCalculator, 98d, mods), format: PpFormat);
                ret["m95PP"] = new TokenWithFormat(GetPp(_ppCalculator, 95d, mods), format: PpFormat);
                ret["m90PP"] = new TokenWithFormat(GetPp(_ppCalculator, 90d, mods), format: PpFormat);
            }

            return ret;
        }
        private double GetPp(PpCalculator.PpCalculator ppCalculator, double acc, Mods mods = Mods.Omod, int score = 0)
        {
            ppCalculator.Accuracy = acc;
            ppCalculator.Score = score;

            _ppCalculator.Mods = mods.ToString().Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
            try
            {
                return ppCalculator.Calculate();
            }
            catch (ArgumentException e) when (e.Message.Contains("Invalid mod provided"))
            {
            }

            return -1;
        }


        private string GetOsuDir()
        {
            return _settings.Get<string>(_names.MainOsuDirectory);
        }

        public void SetSettingsHandle(ISettingsHandler settings)
        {
            this._settings = settings;
        }

    }
}