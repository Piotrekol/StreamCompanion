using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Interfaces;
using System.Collections.Generic;

namespace PlaysReplacements
{
    public class PlaysReplacements : IPlugin, ITokensProvider
    {
        private int Plays, Retrys;
        public bool Started { get; set; }
        private Tokens.TokenSetter _tokenSetter;

        public void Start(ILogger logger)
        {
            _tokenSetter = Tokens.CreateTokenSetter(Name);

            Started = true;
        }
        private string lastMapSearchString = "";

        public string Description { get; } = "";
        public string Name { get; } = nameof(PlaysReplacements);
        public string Author { get; } = "Piotrekol";
        public string Url { get; } = "";
        public string UpdateUrl { get; } = "";

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

            _tokenSetter("Plays", Plays);
            _tokenSetter("Retrys", Retrys);
        }

    }
}