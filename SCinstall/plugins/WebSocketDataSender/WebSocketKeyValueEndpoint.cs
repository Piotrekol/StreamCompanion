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
    public abstract class WebSocketKeyValueEndpoint<V> : WebSocketModule where V : class
    {
        protected readonly IDictionary<string, V> keyValueDictionary;
        private Dictionary<string, Dictionary<string, object>> watchedKeyValuesPerContext = new Dictionary<string, Dictionary<string, object>>();

        private readonly object _lockingObject = new object();
        public WebSocketKeyValueEndpoint(string urlPath, bool enableConnectionWatchdog, IDictionary<string, V> keyValues) : base(urlPath, enableConnectionWatchdog)
        {
            keyValueDictionary = keyValues;
        }

        protected override Task OnClientDisconnectedAsync(IWebSocketContext context)
        {
            lock (_lockingObject)
            {
                if (watchedKeyValuesPerContext.ContainsKey(context.Id))
                {
                    var watchedKeyValues = watchedKeyValuesPerContext[context.Id];
                    lock (watchedKeyValues)
                    {
                        watchedKeyValuesPerContext.Remove(context.Id);
                    }
                }
            }

            return base.OnClientDisconnectedAsync(context);
        }

        protected override Task OnClientConnectedAsync(IWebSocketContext context)
        {
            var task = base.OnClientConnectedAsync(context);
            lock (_lockingObject)
                watchedKeyValuesPerContext.Add(context.Id, new Dictionary<string, object>());
            Task.Run(() => SendLoop(context));
            return task;
        }

        protected virtual object GetValue(string key)
        {
            return keyValueDictionary[key];
        }

        public async Task SendLoop(IWebSocketContext context)
        {
            var keyValuesToSend = new SortedList<string, object>(50);
            Dictionary<string, object> watchedKeyValues;
            lock (_lockingObject)
                watchedKeyValues = watchedKeyValuesPerContext[context.Id];
            try
            {
                while (true)
                {
                    await Task.Delay(33);
                    lock (watchedKeyValues)
                    {
                        if (context.WebSocket.State != WebSocketState.Open)
                            return;

                        for (var i = 0; i < watchedKeyValues.Count; i++)
                        {
                            var watchedKv = watchedKeyValues.ElementAt(i);
                            if (!keyValueDictionary.ContainsKey(watchedKv.Key))
                                continue;
                            var kvValue = GetValue(watchedKv.Key);

                            var valueIsDifferent = (kvValue == null || watchedKv.Value == null)
                                ? kvValue != watchedKv.Value
                                    : (kvValue is double kvv)
                                        ? watchedKv.Value == null || Math.Abs(kvv - (double)watchedKv.Value) > double.Epsilon
                                        : !kvValue.Equals(watchedKv.Value);
                            if (valueIsDifferent)
                            {
                                watchedKeyValues[watchedKv.Key] = kvValue;
                                keyValuesToSend[watchedKv.Key] = kvValue;
                            }
                        }
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
        }

        protected override Task OnMessageReceivedAsync(IWebSocketContext context, byte[] buffer, IWebSocketReceiveResult result)
        {
            var str = Encoding.GetString(buffer);
            Dictionary<string, object> watchedKeyValues;

            lock (_lockingObject)
                watchedKeyValues = watchedKeyValuesPerContext[context.Id];

            lock (watchedKeyValues)
            {
                watchedKeyValues.Clear();
                List<string> kvNames = null;
                try
                {
                    kvNames = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(str);
                }
                catch (Exception ex)
                {
                    ex.Log(context.Id);
                }

                if (kvNames != null && kvNames.Any())
                {
                    watchedKeyValues.Clear();
                    foreach (var key in kvNames)
                    {
                        if (!string.IsNullOrEmpty(key))
                            watchedKeyValues[key] = null;
                    }
                }

            }
            return Task.CompletedTask;
        }
    }
}