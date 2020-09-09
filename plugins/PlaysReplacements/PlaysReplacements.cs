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
        private string lastMapSearchString = "";

        public string Description { get; } = "";
        public string Name { get; } = nameof(PlaysReplacements);
        public string Author { get; } = "Piotrekol";
        public string Url { get; } = "";
        public string UpdateUrl { get; } = "";

        public PlaysReplacements()
        {
            _tokenSetter = Tokens.CreateTokenSetter(Name);
        }

        public void CreateTokens(MapSearchResult map)
        {
            if (map.Action == OsuStatus.Playing)
            {
                if (lastMapSearchString == map.MapSearchString)
                    Retrys++;
                else
                    Plays++;
                lastMapSearchString = map.MapSearchString;
            }

            _tokenSetter("plays", Plays);
            _tokenSetter("retrys", Retrys);
        }

    }
}