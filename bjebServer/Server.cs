using System;
using System.Net;
using System.Net.Sockets;

namespace bjeb
{
    public class Server
    {
        private TcpListener _listener;

        public void start()
        {
            _listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 4400);
            _listener.Start();
        }

        public void stop()
        {
            _listener.Stop();
        }

        public Connection accept()
        {
            return new Connection(_listener.AcceptTcpClient());
        }
   }
}