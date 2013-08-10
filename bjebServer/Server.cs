using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace bjeb.net
{
	public class Server
	{
		private TcpListener _listener;

		public Server(string host, int port)
		{
			_listener = new TcpListener(IPAddress.Parse(host), port);
			_listener.Start();
		} 

		public void stop()
		{
			_listener.Stop();
		}

		public delegate void ConnectionDelegate(Connection connection);

		public void accept(ConnectionDelegate handler)
		{
			Connection connection = new Connection(_listener.AcceptTcpClient());

			new Thread( () => handler(connection) ).Start();
		}
	}
}