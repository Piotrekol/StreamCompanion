using System;
using StreamCompanionTypes.DataTypes;

namespace WebSocketDataSender
{
    public static class TokenExtensions
    {
        private static readonly long BaseDateTicks = new DateTime(1970, 1, 1).Ticks;
        public static object WebSocketValue(this IToken token)
        {
            if (token == null)
                return null;
            var rawTokenValue = token.Value;
            
            return (rawTokenValue is TimeSpan tvTimeSpan) ? tvTimeSpan.TotalMilliseconds
                : rawTokenValue;
        }
    }
}