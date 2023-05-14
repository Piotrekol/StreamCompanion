using System;
using System.Collections.Concurrent;
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
        private Dictionary<string, ContextTokensState> contextStates = new();
        private readonly object _lockingObject = new object();

        public WebSocketTokenEndpoint(string urlPath, bool enableConnectionWatchdog) : base(urlPath, enableConnectionWatchdog)
        {
            Tokens.AllTokensChanged += Tokens_AllTokensChanged;
        }

        private void Tokens_AllTokensChanged(object sender, string e)
        {
            //New token has been registered
            foreach (var state in contextStates)
            {
                UpdateListenedTokens(state.Value);
            }
        }

        protected override Task OnClientConnectedAsync(IWebSocketContext context)
        {
            var task = base.OnClientConnectedAsync(context);
            lock (_lockingObject)
                contextStates.Add(context.Id, new());
            Task.Run(() => SendLoop(context));
            return task;
        }

        public async Task SendLoop(IWebSocketContext context)
        {
            var keyValuesToSend = new SortedList<string, object>(50);
            Dictionary<string, object> watchedKeyValues = new();
            var state = contextStates[context.Id];
            try
            {
                while (context.WebSocket.State == WebSocketState.Open)
                {
                    state.ManualResetEventSlim.Wait(context.CancellationToken);
                    state.ManualResetEventSlim.Reset();

                    while (state.TokensPendingUpdate.TryDequeue(out var token))
                    {
                        var tokenValue = token.Value;
                        var watchedValue = watchedKeyValues.GetValueOrDefault(token.Name);
                        if (!Equals(tokenValue, watchedValue) || watchedValue == null || (tokenValue is double d && Math.Abs(d - (double)watchedValue) > double.Epsilon))
                            watchedKeyValues[token.Name] = keyValuesToSend[token.Name] = tokenValue;
                    }

                    if (keyValuesToSend.Any())
                    {
                        var payload = Newtonsoft.Json.JsonConvert.SerializeObject(keyValuesToSend);
                        await SendAsync(context, payload);
                        keyValuesToSend.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Log(context.Id);
                try
                {
                    if (context.WebSocket.State != WebSocketState.Closed)
                        await context.WebSocket.CloseAsync();
                }
                catch
                {
                    //ignored
                    //We can't really do anything if connection can't be closed anymore. we somehow have invalid state there.
                }
            }
            finally
            {
                foreach (var token in state.WatchedTokens)
                    token.ValueUpdated -= state.TokenValueUpdated;

                state.Dispose();
                contextStates.Remove(context.Id);
            }
        }

        protected override Task OnMessageReceivedAsync(IWebSocketContext context, byte[] buffer, IWebSocketReceiveResult result)
        {
            var input = Encoding.GetString(buffer);
            List<string> kvNames = null;
            try
            {
                kvNames = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(input);
            }
            catch (Exception ex)
            {
                ex.Log(context.Id);
            }

            if (kvNames == null)
                return Task.CompletedTask;

            var settings = contextStates[context.Id];

            lock (settings)
            {
                settings.RequestedTokenNames.Clear();
                settings.RequestedTokenNames.AddRange(kvNames);
                UpdateListenedTokens(settings);
            }

            return Task.CompletedTask;
        }

        private void UpdateListenedTokens(ContextTokensState settings)
        {
            lock (settings)
            {
                foreach (var token in settings.WatchedTokens)
                {
                    token.ValueUpdated -= settings.TokenValueUpdated;
                }

                settings.WatchedTokens.Clear();
                var validTokens = settings.RequestedTokenNames.Where(tokenName => Tokens.AllTokens.ContainsKey(tokenName)).Select(tokenName => Tokens.AllTokens[tokenName]);
                foreach (var token in validTokens)
                {
                    settings.WatchedTokens.Add(token);

                    token.ValueUpdated += settings.TokenValueUpdated;
                    settings.TokensPendingUpdate.Enqueue(token);
                }

                if (settings.TokensPendingUpdate.TryPeek(out var t))
                    settings.TokenValueUpdated(null, t);
            }
        }

        private class ContextTokensState : IDisposable
        {
            public List<IToken> WatchedTokens { get; set; } = new();
            public List<string> RequestedTokenNames { get; set; } = new();
            public ManualResetEventSlim ManualResetEventSlim { get; set; } = new();
            public LockingQueue<IToken> TokensPendingUpdate { get; private set; } = new();

            public void TokenValueUpdated(object _, IToken token)
            {
                TokensPendingUpdate.Enqueue(token);
                ManualResetEventSlim.Set();
            }

            public void Dispose()
            {
                ManualResetEventSlim.Dispose();
            }
        }
    }
}