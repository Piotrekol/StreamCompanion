using System.Collections.Generic;
using StreamCompanionTypes.DataTypes;

namespace WebSocketDataSender
{
    public class WebSocketTokenEndpoint : WebSocketKeyValueEndpoint<IToken>
    {
        public WebSocketTokenEndpoint(string urlPath, bool enableConnectionWatchdog, IDictionary<string, IToken> keyValues) : base(urlPath, enableConnectionWatchdog, keyValues)
        {
        }

        protected override object GetValue(string key) => keyValueDictionary[key].Value;
    }
}