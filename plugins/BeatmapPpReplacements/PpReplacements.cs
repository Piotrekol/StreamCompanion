using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CollectionManager.Enums;
using OppaiSharp;
using StreamCompanionTypes;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Interfaces;
using Beatmap = OppaiSharp.Beatmap;
using Helpers = StreamCompanionTypes.Helpers;

namespace BeatmapPpReplacements
{
    public class PpReplacements : IPlugin, IMapDataReplacements, ISettings
    {
        private readonly SettingNames _names = SettingNames.Instance;

        DiffCalc diffCalculator = new DiffCalc();
        PPv2 _ppCalculator;
        Accuracy _accCalculator = new Accuracy();
        private ISettingsHandler _settings;
        private Mods _lastMods = Mods.NoMod;
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
            if (map.BeatmapsFound[0].PlayMode != PlayMode.Osu) return ret;
            var mapLocation = map.BeatmapsFound[0].FullOsuFileLocation(BeatmapHelpers.GetFullSongsLocation(_settings));

            if (!File.Exists(mapLocation)) return ret;
            FileInfo file = new FileInfo(mapLocation);

            if (file.Length == 0) return ret;
            Beatmap beatmap = null;
            try
            {
                beatmap = BeatmapHelpers.GetOppaiSharpBeatmap(mapLocation);
                if (beatmap == null)
                    return ret;

                ret["MaxCombo"] = new TokenWithFormat(beatmap.GetMaxCombo());

                ret["SSPP"] = new TokenWithFormat(GetPp(beatmap, 100d));
                ret["99.9PP"] = new TokenWithFormat(GetPp(beatmap, 99.9d));
                ret["99PP"] = new TokenWithFormat(GetPp(beatmap, 99d));
                ret["98PP"] = new TokenWithFormat(GetPp(beatmap, 98d));
                ret["95PP"] = new TokenWithFormat(GetPp(beatmap, 95d));
                ret["90PP"] = new TokenWithFormat(GetPp(beatmap, 90d));

                Mods mods = _lastMods;
                string modsStr = _lastModsStr;
                if (map.Action == OsuStatus.Playing || map.Action == OsuStatus.Watching)
                {
                    var tempMods = (map.Mods?.Item1 ?? CollectionManager.DataTypes.Mods.Omod).Convert();
                    if (tempMods != Mods.NoMod)
                    {
                        modsStr = map.Mods?.Item2 ?? "NM";
                        mods = tempMods;

                        _lastMods = tempMods;
                        _lastModsStr = modsStr;
                    }
                }

                ret["mMod"] = new Token(modsStr);
                ret["mSSPP"] = new TokenWithFormat(GetPp(beatmap, 100d, mods));
                ret["m99.9PP"] = new TokenWithFormat(GetPp(beatmap, 99.9d, mods));
                ret["m99PP"] = new TokenWithFormat(GetPp(beatmap, 99d, mods));
                ret["m98PP"] = new TokenWithFormat(GetPp(beatmap, 98d, mods));
                ret["m95PP"] = new TokenWithFormat(GetPp(beatmap, 95d, mods));
                ret["m90PP"] = new TokenWithFormat(GetPp(beatmap, 90d, mods));

                return ret;
            }
            catch
            {
            }

            return ret;
        }
        private double GetPp(Beatmap beatmap, double acc, Mods mods = Mods.NoMod)
        {
            _accCalculator = new Accuracy(acc, beatmap.Objects.Count, 0);
            _ppCalculator = new PPv2(new PPv2Parameters(beatmap, _accCalculator.Count100, _accCalculator.Count50, _accCalculator.CountMiss, -1, _accCalculator.Count300, mods));
            return Math.Round(_ppCalculator.Total, 2);
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