using System.Net.WebSockets;
using System.Threading.Tasks;
using EmbedIO.WebSockets;

namespace WebSocketDataSender
{
    public class WebSocketDataEndpoint : WebSocketModule
    {
        private readonly DataContainer _dataContainer;

        public WebSocketDataEndpoint(string urlPath, bool enableConnectionWatchdog, DataContainer dataContainer) : base(urlPath, enableConnectionWatchdog)
        {
            _dataContainer = dataContainer;
        }

        protected override Task OnClientConnectedAsync(IWebSocketContext context)
        {
            var task = base.OnClientConnectedAsync(context);
            Task.Run(() => SendLoop(context));
            return task;
        }

        public async Task SendLoop(IWebSocketContext context)
        {
            string lastSentData = string.Empty;
            while (true)
            {
                if(context.WebSocket.State != WebSocketState.Open)
                    return;
                await Task.Delay(33);
                if (lastSentData != _dataContainer.Data)
                {
                    lastSentData = _dataContainer.Data;
                    await SendAsync(context, lastSentData);
                }
            }
        }

        protected override Task OnMessageReceivedAsync(IWebSocketContext context, byte[] buffer, IWebSocketReceiveResult result)
        {
            return Task.CompletedTask;
        }
    }
}