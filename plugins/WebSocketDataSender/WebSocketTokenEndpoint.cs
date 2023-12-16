using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using EmbedIO.Utilities;
using EmbedIO.WebSockets;
using StreamCompanion.Common.Helpers.Tokens;
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

        protected override async Task OnClientConnectedAsync(IWebSocketContext context)
        {
            await base.OnClientConnectedAsync(context);
            var contextState = new ContextTokensState();
            var queries = context.RequestUri.Query.TrimStart('?').ToLowerInvariant().Split('&');
            foreach (var query in queries)
            {
                if (query.StartsWith("bulkupdates="))
                {
                    var queryParams = HttpUtility.UrlDecode(query.Split('=')[1]).SplitByComma();
                    foreach (var queryParam in queryParams)
                    {
                        if (!Enum.TryParse(typeof(BulkTokenUpdateType), queryParam, true, out var parsed) || parsed is not BulkTokenUpdateType tokenUpdateType)
                        {
                            await SendAsync(context, $"Invalid bulk update type(s) provided. Valid values: {string.Join(", ", Enum.GetNames(typeof(BulkTokenUpdateType)))}");
                            _ = context.WebSocket.CloseAsync();
                            return;
                        }

                        contextState.MonitoredBulkTokenUpdateStates.Add(TokensBulkUpdate.States[tokenUpdateType]);
                    }
                }
                else if (query.StartsWith("updatespersecond=") || query.StartsWith("ups="))
                {
                    var rawUpdateRate = query.Split('=')[1];
                    if (!int.TryParse(rawUpdateRate, out int updateRate) || updateRate < 1)
                    {
                        await SendAsync(context, $"Invalid UpdatesPerSecond value. Positive number expected.");
                        _ = context.WebSocket.CloseAsync();
                        return;
                    }

                    contextState.UpdateSleepDelayMs = (int)Math.Ceiling(1000f / updateRate);
                }
            }

            lock (_lockingObject)
                contextStates.Add(context.Id, contextState);

            _ = Task.Run(() => SendLoop(context));
            return;
        }

        public async Task SendLoop(IWebSocketContext context)
        {
            var keyValuesToSend = new SortedList<string, object>(50);
            Dictionary<string, object> watchedKeyValues = new();
            var state = contextStates[context.Id];
            Task updateSleepTask = Task.CompletedTask;
            try
            {
                while (context.WebSocket.State == WebSocketState.Open)
                {
                    state.ManualResetEventSlim.Wait(context.CancellationToken);
                    state.ManualResetEventSlim.Reset();

                    foreach (var bulkTokenUpdateState in state.MonitoredBulkTokenUpdateStates)
                        bulkTokenUpdateState.UpdateFinished.Wait(context.CancellationToken);

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
                        await SendAsync(context, payload).ConfigureAwait(false);
                        keyValuesToSend.Clear();

                        if (state.UpdateSleepDelayMs > 0)
                        {
                            await updateSleepTask.ConfigureAwait(false);
                            updateSleepTask = Task.Delay(state.UpdateSleepDelayMs, context.CancellationToken);
                        }
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
                lock (state)
                {
                    foreach (var token in state.WatchedTokens)
                        token.ValueUpdated -= state.TokenValueUpdated;

                    state.Dispose();
                    lock (contextStates)
                    {
                        contextStates.Remove(context.Id);
                    }
                }
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

            ContextTokensState contextTokensState;
            lock (contextStates)
            {
                if (!contextStates.TryGetValue(context.Id, out contextTokensState))
                    return Task.CompletedTask;

                lock (contextTokensState)
                {
                    contextTokensState.RequestedTokenNames.Clear();
                    contextTokensState.RequestedTokenNames.AddRange(kvNames.Where(k => k is not null));
                    UpdateListenedTokens(contextTokensState);
                }
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
            public List<BulkTokenUpdateState> MonitoredBulkTokenUpdateStates = new();
            public List<IToken> WatchedTokens { get; set; } = new();
            public List<string> RequestedTokenNames { get; set; } = new();
            public ManualResetEventSlim ManualResetEventSlim { get; set; } = new();
            public LockingQueue<IToken> TokensPendingUpdate { get; private set; } = new();
            public int UpdateSleepDelayMs { get; set; } = 50; //20 messages/s

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