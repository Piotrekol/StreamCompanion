using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Interfaces;
using System.Collections.Generic;

namespace PlaysReplacements
{
    public class PlaysReplacements : IPlugin, IMapDataReplacements
    {
        private int Plays, Retrys;
        public bool Started { get; set; }
        public void Start(ILogger logger) { Started = true; }
        private string lastMapSearchString = "";

        public string Description { get; } = "";
        public string Name { get; } = nameof(PlaysReplacements);
        public string Author { get; } = "Piotrekol";
        public string Url { get; } = "";
        public string UpdateUrl { get; } = "";

        public Tokens GetMapReplacements(MapSearchResult map)
        {
            if (map.Action == OsuStatus.Playing)
            {
                if (lastMapSearchString == map.MapSearchString)
                    Retrys++;
                else
                    Plays++;
                lastMapSearchString = map.MapSearchString;
            }

            return new Tokens
            {
                { "Plays", new Token(Plays)},
                { "Retrys", new Token(Retrys)}
            };
        }

    }
}