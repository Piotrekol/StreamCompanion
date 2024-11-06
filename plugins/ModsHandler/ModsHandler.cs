using CollectionManager.DataTypes;
using CollectionManager.Enums;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Interfaces;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using StreamCompanionTypes.Attributes;
using StreamCompanionTypes.Enums;
using StreamCompanionTypes.Interfaces.Services;
using StreamCompanionTypes.Interfaces.Sources;
using Beatmap = StreamCompanionTypes.DataTypes.Beatmap;
using StreamCompanion.Common;
using System.Collections.Generic;

namespace ModsHandler
{
    [SCPlugin(Name, "provides basic modded map stats & mods processing utils for other plugins", Consts.SCPLUGIN_AUTHOR, Consts.SCPLUGIN_BASEURL)]
    public class ModsHandler : IPlugin, IModParser, IDifficultyCalculator, ITokensSource, ISettingsSource
    {
        public const string Name = "Mods handler";
        private readonly ModParser _modParser;
        private readonly DifficultyCalculator _difficultyCalculator = new DifficultyCalculator();
        private Tokens.TokenSetter _tokenSetter;

        public string SettingGroup => _modParser.SettingGroup;

        public ModsHandler(ISettings settings)
        {
            _modParser = new ModParser(settings);
            _tokenSetter = Tokens.CreateTokenSetter(Name);
            var searchResult = new MapSearchResult(new MapSearchArgs("fake", OsuEventType.MapChange) { PlayMode = PlayMode.Osu })
            { 
                BeatmapsFound = { new Beatmap() }
            };

            SetTokenValues(searchResult);
        }

        public IModsEx GetModsFromEnum(int modsEnum)
        {
            return _modParser.GetModsFromEnum(modsEnum);
        }

        public string GetModsFromEnum(int modsEnum, bool shortMod = false)
        {
            return _modParser.GetModsFromEnum(modsEnum, shortMod);
        }

        public IBeatmap ApplyMods(IBeatmap map, Mods mods)
        {
            var c = _difficultyCalculator.ApplyMods(map, mods);
            var retMap = (Beatmap)map.Clone();
            retMap.ApproachRate = c["AR"];
            retMap.CircleSize = c["CS"];
            retMap.OverallDifficulty = c["OD"];
            return retMap;
        }

        public Task CreateTokensAsync(IMapSearchResult map, CancellationToken cancellationToken)
        {
            if (map.BeatmapsFound.Any())
            {
                SetTokenValues(map);
            }
            else
            {
                foreach (var tokenkv in Tokens.AllTokens.Where(t => t.Value.PluginName == Name))
                {
                    tokenkv.Value.Reset();
                }
            }

            return Task.CompletedTask;
        }

        private void SetTokenValues(IMapSearchResult map)
        {
            var foundMap = map.BeatmapsFound[0];
            var mods = map.Mods?.Mods ?? Mods.Omod;
            var moddedStats = _difficultyCalculator.ApplyMods(foundMap, mods);
            var bpm = Math.Abs(moddedStats["MinBpm"] - moddedStats["MaxBpm"]) < float.Epsilon
                ? moddedStats["MinBpm"].ToString("0")
                : $"{moddedStats["MinBpm"]:0}-{moddedStats["MaxBpm"]:0} ({moddedStats["MainBpm"]:0})";

            _tokenSetter("mods", map.Mods?.ShownMods);
            _tokenSetter("modsEnum", map.Mods?.Mods);
            _tokenSetter("mAR", Math.Round(moddedStats["AR"], 2), format: "{0:0.##}");
            _tokenSetter("mCS", Math.Round(moddedStats["CS"], 2), format: "{0:0.##}");
            _tokenSetter("mOD", Math.Round(moddedStats["OD"], 2), format: "{0:0.##}");
            _tokenSetter("mHP", Math.Round(moddedStats["HP"], 2), format: "{0:0.##}");
            _tokenSetter("mStars", Math.Round(foundMap.Stars(map.PlayMode ?? PlayMode.Osu, mods), 3), format: "{0:0.##}");
            _tokenSetter("mBpm", bpm);
            _tokenSetter("mMaxBpm", Math.Round(moddedStats["MaxBpm"], 2), format: "{0:0}");
            _tokenSetter("mMinBpm", Math.Round(moddedStats["MinBpm"], 2), format: "{0:0}");
            _tokenSetter("mMainBpm", Math.Round(moddedStats["MainBpm"], 2), format: "{0:0}");
        }

        public void Free()
        {
            _modParser.Free();
        }

        public object GetUiSettings()
            => _modParser.GetUiSettings();
    }
}