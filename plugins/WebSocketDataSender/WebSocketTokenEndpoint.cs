using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using EmbedIO.WebSockets;
using StreamCompanionTypes.DataTypes;
using Swan;
using Swan.Logging;

namespace WebSocketDataSender
{
    public class WebSocketTokenEndpoint : WebSocketModule
    {
        private readonly IReadOnlyDictionary<string, IToken> _tokens;
        private Dictionary<string, Dictionary<string, object>> watchedTokensPerContext = new Dictionary<string, Dictionary<string, object>>();

        private readonly object _lockingObject = new object();
        public WebSocketTokenEndpoint(string urlPath, bool enableConnectionWatchdog, IReadOnlyDictionary<string, IToken> tokens) : base(urlPath, enableConnectionWatchdog)
        {
            _tokens = tokens;
        }

        protected override Task OnClientDisconnectedAsync(IWebSocketContext context)
        {
            lock (_lockingObject)
            {
                if (watchedTokensPerContext.ContainsKey(context.Id))
                {
                    var watchedTokens = watchedTokensPerContext[context.Id];
                    lock (watchedTokens)
                    {
                        watchedTokensPerContext.Remove(context.Id);
                    }
                }
            }

            return base.OnClientDisconnectedAsync(context);
        }

        protected override Task OnClientConnectedAsync(IWebSocketContext context)
        {
            var task = base.OnClientConnectedAsync(context);
            lock (_lockingObject)
                watchedTokensPerContext.Add(context.Id, new Dictionary<string, object>());
            Task.Run(() => SendLoop(context));
            return task;
        }

        public async Task SendLoop(IWebSocketContext context)
        {
            var tokenKVs = new SortedList<string, object>(50);
            Dictionary<string, object> watchedTokens;
            lock (_lockingObject)
                watchedTokens = watchedTokensPerContext[context.Id];
            try
            {
                while (true)
                {
                    await Task.Delay(33);
                    lock (watchedTokens)
                    {
                        if (context.WebSocket.State != WebSocketState.Open)
                            return;

                        for (var i = 0; i < watchedTokens.Count; i++)
                        {
                            var watchedToken = watchedTokens.ElementAt(i);
                            if (!_tokens.ContainsKey(watchedToken.Key))
                                continue;
                        var tokenValue = _tokens[watchedToken.Key].Value;

                        var valueIsDifferent = (tokenValue is double tv)
                            ? watchedToken.Value == null || Math.Abs(tv - (double)watchedToken.Value) > double.Epsilon
                            : tokenValue != watchedToken.Value;
                        if (valueIsDifferent)
                        {
                                watchedTokens[watchedToken.Key] = tokenValue;
                                tokenKVs[watchedToken.Key] = tokenValue;
                            }
                        }
                    }

                    if (tokenKVs.Any())
                    {
                        var payload = Newtonsoft.Json.JsonConvert.SerializeObject(tokenKVs);
                        await SendAsync(context, payload);
                        tokenKVs.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex, context.Id);
                try
                {
                    await context.WebSocket.CloseAsync();
                }
                catch{}
            }
        }

        protected override Task OnMessageReceivedAsync(IWebSocketContext context, byte[] buffer, IWebSocketReceiveResult result)
        {
            var str = Encoding.GetString(buffer);
            Dictionary<string, object> watchedTokens;

            lock (_lockingObject)
                watchedTokens = watchedTokensPerContext[context.Id];

            lock (watchedTokens)
            {
                watchedTokens.Clear();
                List<string> tokenNames = null;
                try
                {
                    tokenNames = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(str);
                }
                catch (Exception) { }

                if (tokenNames != null && tokenNames.Any())
                {
                    watchedTokens.Clear();
                    foreach (var tokenName in tokenNames)
                    {
                        if (!string.IsNullOrEmpty(tokenName))
                            watchedTokens[tokenName] = null;
                    }
                }

            }
            return Task.CompletedTask;
        }
    }
}