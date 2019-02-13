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

        //TODO: SC can now support all gamemodes
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
                {"mMod", new Token(null)},
                {"mSSPP", new Token(null)},
                {"m99.9PP", new Token(null)},
                {"m99PP", new Token(null)},
                {"m98PP", new Token(null)},
                {"m95PP", new Token(null)},
                {"m90PP", new Token(null)},
            };

            if (!map.FoundBeatmaps) return ret;
            var mapLocation = map.BeatmapsFound[0].FullOsuFileLocation(BeatmapHelpers.GetFullSongsLocation(_settings));

            if (!File.Exists(mapLocation)) return ret;
            FileInfo file = new FileInfo(mapLocation);

            if (file.Length == 0) return ret;

            _ppCalculator = PpCalculatorHelpers.GetPpCalculator(mapLocation, _ppCalculator);

            if (_ppCalculator == null)
                return ret;

            //TODO: mania needs separate tokens
            if (_ppCalculator.RulesetId.Value == (int) PlayMode.OsuMania)
                _ppCalculator.Score = 1_000_000;
            else
                _ppCalculator.Score = 0;



            ret["GameMode"] = new TokenWithFormat(map.BeatmapsFound[0].PlayMode.ToString());
            ret["MaxCombo"] = new TokenWithFormat(_ppCalculator.GetMaxCombo());
            ret["SSPP"] = new TokenWithFormat(GetPp(_ppCalculator, 100d), format: "{0:0.00}");
            ret["99.9PP"] = new TokenWithFormat(GetPp(_ppCalculator, 99.9d), format: "{0:0.00}");
            ret["99PP"] = new TokenWithFormat(GetPp(_ppCalculator, 99d), format: "{0:0.00}");
            ret["98PP"] = new TokenWithFormat(GetPp(_ppCalculator, 98d), format: "{0:0.00}");
            ret["95PP"] = new TokenWithFormat(GetPp(_ppCalculator, 95d), format: "{0:0.00}");
            ret["90PP"] = new TokenWithFormat(GetPp(_ppCalculator, 90d), format: "{0:0.00}");

            Mods mods;
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
            ret["mSSPP"] = new TokenWithFormat(GetPp(_ppCalculator, 100d, mods), format: "{0:0.00}");
            ret["m99.9PP"] = new TokenWithFormat(GetPp(_ppCalculator, 99.9d, mods), format: "{0:0.00}");
            ret["m99PP"] = new TokenWithFormat(GetPp(_ppCalculator, 99d, mods), format: "{0:0.00}");
            ret["m98PP"] = new TokenWithFormat(GetPp(_ppCalculator, 98d, mods), format: "{0:0.00}");
            ret["m95PP"] = new TokenWithFormat(GetPp(_ppCalculator, 95d, mods), format: "{0:0.00}");
            ret["m90PP"] = new TokenWithFormat(GetPp(_ppCalculator, 90d, mods), format: "{0:0.00}");

            return ret;
        }
        private double GetPp(PpCalculator.PpCalculator ppCalculator, double acc, Mods mods = Mods.Omod)
        {
            ppCalculator.Accuracy = acc;

            _ppCalculator.Mods = mods == Mods.Omod ? null : mods.ToString().Replace("Au", "").Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
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