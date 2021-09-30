using System.Threading;
using System.Threading.Tasks;
using StreamCompanionTypes.Interfaces;
using StreamCompanionTypes.Enums;
using StreamCompanionTypes.Interfaces.Services;
using StreamCompanionTypes.Interfaces.Consumers;
using StreamCompanionTypes.Interfaces.Sources;
using StreamCompanionTypes.DataTypes;

namespace newTestPlugin
{
    public class MyPlugin : IPlugin, ITokensSource, IMapDataConsumer
    {
        public string Description => "my plugin description";
        public string Name => "my plugin name";
        public string Author => "my name";
        public string Url => "Plugin homepage url(github/site)";

        private ISettings Settings;
        private ILogger Logger;
        private Tokens.TokenSetter tokenSetter;
        public static ConfigEntry lastMapConfigEntry = new ConfigEntry("myConfigName", "defaultValue");
        public MyPlugin(ISettings settings, ILogger logger)
        {
            Settings = settings;
            Logger = logger;
            tokenSetter = Tokens.CreateTokenSetter("MyPlugin");
            Logger.Log(settings.Get<string>(lastMapConfigEntry), LogLevel.Trace);
        }

        public Task CreateTokensAsync(IMapSearchResult map, CancellationToken cancellationToken)
        {
            //do: update token values
            //do: execute actions based on map search results
            //don't: execute actions based on token values from other plugins

            tokenSetter("someTokenName", "token value", TokenType.Normal, "{0}", "default value", OsuStatus.Playing | OsuStatus.Watching);
            Settings.Add(lastMapConfigEntry.Name, map.MapSearchString);
            Logger.Log("CreateTokensAsync", LogLevel.Trace);
            return Task.CompletedTask;
        }

        public Task SetNewMapAsync(IMapSearchResult map, CancellationToken cancellationToken)
        {
            //do: execute actions based on token values
            //don't: update token values(unless these are live)

            if (map.PlayMode == CollectionManager.Enums.PlayMode.Osu && map.BeatmapsFound.Count > 0)
            {
                var beatmap = map.BeatmapsFound[0];
                var starRating = (double)Tokens.AllTokens["mStars"].Value;
            }

            Logger.Log("SetNewMapAsync", LogLevel.Trace);
            return Task.CompletedTask;
        }
    }
}
