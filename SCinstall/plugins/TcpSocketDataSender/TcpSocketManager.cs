using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace TcpSocketDataSender
{
    public class TcpSocketManager : IDisposable
    {
        private TcpClient _tcpClient;
        BinaryWriter _writer = null;

        public int ServerPort = 7839;
        public string ServerIp = "127.0.0.1";
        public bool AutoReconnect = false;

        public bool Connected { get; private set; }

        public virtual bool Connect()
        {
            if (_writer != null)
                return Connected = true;
            _tcpClient = new TcpClient();
            try
            {
                _tcpClient.Connect(IPAddress.Parse(ServerIp), ServerPort);
                _writer = new BinaryWriter(_tcpClient.GetStream());
            }
            catch (Exception e) when (e is SocketException || e is InvalidOperationException)
            {
                //No server avaliable, or it is busy/full.
                return Connected = false;
            }
            return Connected = true;
        }

        public virtual void Write(string data)
        {
            if (!Connected && !Connect())
                return;

            bool written = false;
            try
            {
                if (_tcpClient?.Connected ?? false)
                {
                    _writer?.Write(data);
                    written = true;
                }
            }
            catch (IOException)
            {
                //connection most likely closed
                _writer?.Dispose();
                _writer = null;
                ((IDisposable)_tcpClient)?.Dispose();
                Connected = false;
            }
            if (!written && AutoReconnect)
            {
                if (Connect())
                    Write(data);
            }
        }

        public virtual void Dispose()
        {
            ((IDisposable)_tcpClient)?.Dispose();
            _writer?.Dispose();
        }
    }
}