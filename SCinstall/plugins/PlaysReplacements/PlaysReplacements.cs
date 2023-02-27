using System.Threading;
using System.Threading.Tasks;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Enums;
using StreamCompanionTypes.Interfaces;
using StreamCompanionTypes.Interfaces.Sources;

namespace PlaysReplacements
{
    public class PlaysReplacements : IPlugin, ITokensSource
    {
        private int Plays, Retries;
        private Tokens.TokenSetter _tokenSetter;

        public string Description { get; } = "";
        public string Name { get; } = nameof(PlaysReplacements);
        public string Author { get; } = "Piotrekol";
        public string Url { get; } = "";
        public string UpdateUrl { get; } = "";

        public PlaysReplacements()
        {
            _tokenSetter = Tokens.CreateTokenSetter(Name);
            UpdateTokens();
        }

        public Task CreateTokensAsync(IMapSearchResult map, CancellationToken cancellationToken)
        {
            //ignore replays/spect
            if (map.Action != OsuStatus.Playing)
                return Task.CompletedTask;

            switch (map.SearchArgs.EventType)
            {
                case OsuEventType.SceneChange:
                case OsuEventType.MapChange:
                    Plays++;
                    break;
                case OsuEventType.PlayChange:
                    Retries++;
                    break;
            }

            UpdateTokens();
            return Task.CompletedTask;
        }

        private void UpdateTokens()
        {
            _tokenSetter("plays", Plays);
            _tokenSetter("retries", Retries);
        }
    }
}