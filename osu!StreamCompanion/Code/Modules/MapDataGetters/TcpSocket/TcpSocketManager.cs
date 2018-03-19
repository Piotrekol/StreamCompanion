using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace osu_StreamCompanion.Code.Modules.MapDataGetters.TcpSocket
{
    public class TcpSocketManager : IDisposable
    {
        private TcpClient _tcpClient;
        BinaryWriter _writer = null;

        public int ServerPort = 7839;
        public string ServerIp = "127.0.0.1";

        public bool Connect()
        {
            if (_writer != null)
                return true;
            _tcpClient = new TcpClient();
            try
            {
                _tcpClient.Connect(IPAddress.Parse("127.0.0.1"), ServerPort);
                _writer = new BinaryWriter(_tcpClient.GetStream());
            }
            catch (SocketException)
            {
                //No server avaliable, or it is busy/full.
                return false;
            }
            return true;
        }

        public void Write(string data)
        {
            try
            {
                if (_tcpClient.Connected)
                    _writer?.Write(data);
            }
            catch (IOException)
            {
                //connection most likely closed
                _writer?.Dispose();
                _writer = null;
                ((IDisposable)_tcpClient)?.Dispose();
            }
        }

        public void Dispose()
        {
            ((IDisposable)_tcpClient)?.Dispose();
            _writer?.Dispose();
        }
    }
}