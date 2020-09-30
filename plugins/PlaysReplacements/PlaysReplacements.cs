using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Enums;
using StreamCompanionTypes.Interfaces;
using StreamCompanionTypes.Interfaces.Sources;

namespace PlaysReplacements
{
    public class PlaysReplacements : IPlugin, ITokensSource
    {
        private int Plays, Retrys;
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

        public void CreateTokens(MapSearchResult map)
        {
            //ignore replays/spect
            if (map.Action != OsuStatus.Playing)
                return;

            switch (map.SearchArgs.EventType)
            {
                case OsuEventType.SceneChange:
                case OsuEventType.MapChange:
                    Plays++;
                    break;
                case OsuEventType.PlayChange:
                    Retrys++;
                    break;
            }

            UpdateTokens();
        }

        private void UpdateTokens()
        {
            _tokenSetter("plays", Plays);
            _tokenSetter("retrys", Retrys);
        }
    }
}