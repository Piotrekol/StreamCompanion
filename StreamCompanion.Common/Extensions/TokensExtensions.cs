using StreamCompanionTypes.DataTypes;
using System;
using System.Threading;
using System.Threading.Tasks;
using StreamCompanionTypes.Enums;

namespace StreamCompanion.Common
{
    public static class TokensExtensions
    {
        public static Action<IToken, Func<object>> LiveTokenSetter { private get; set; }

        /// <summary>
        /// Adds token to existing live token update loop, which is synced with internal osu! memory data updates.
        /// </summary>
        /// <param name="token">Configured live token</param>
        /// <param name="valueUpdaterFunc">Value generator for the token</param>
        public static async Task ConvertToLiveToken(this IToken token, Func<object> valueUpdaterFunc, CancellationToken cancellationToken = default)
        {
            if (token == null)
                throw new ArgumentNullException("token");

            if (valueUpdaterFunc == null)
                throw new ArgumentNullException("value");

            if ((token.Type & TokenType.Live) == 0)
                throw new ArgumentException("Token has to be created with TokenType.Live");

            while (LiveTokenSetter == null)
            {
                await Task.Delay(50, cancellationToken);
            }

            LiveTokenSetter(token, valueUpdaterFunc);
        }
    }
}