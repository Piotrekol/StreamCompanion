using System.Collections.Generic;
using StreamCompanionTypes.DataTypes;

namespace StreamCompanionTypes.Interfaces
{
    public interface ITokensProvider
    {
        /// <summary>
        /// Tokens should get updated upon calling this method<para/>
        /// Use static <see cref="Tokens.GetToken"/> to generate and update tokens
        /// </summary>
        /// <param name="map"></param>
        void CreateTokens(MapSearchResult map);
    }
}