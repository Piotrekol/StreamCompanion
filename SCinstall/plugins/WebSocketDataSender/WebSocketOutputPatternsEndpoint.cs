using System.Collections.Generic;

namespace WebSocketDataSender
{
    public class WebSocketOutputPatternsEndpoint : WebSocketKeyValueEndpoint<string>
    {
        public WebSocketOutputPatternsEndpoint(string urlPath, bool enableConnectionWatchdog, IDictionary<string, string> keyValues) : base(urlPath, enableConnectionWatchdog, keyValues)
        {
        }
    }
}