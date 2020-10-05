using System.Net;
using System.Net.Sockets;

namespace WebSocketDataSender
{
    public class NetExtensions
    {
        public static string GetLanAddress()
        {
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65030);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                return endPoint?.Address.ToString();
            }
        }
    }
}