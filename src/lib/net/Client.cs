namespace bjeb.net
{
	public class Client
	{
		private string _host;
		private int _port;

		private bool _connected;

		public Connection connection
		{
			get;
			private set;
		}

		public Client(string host, int port)
		{
			_host = host;
			_port = port;

			try
			{
				connection = new Connection(host, port);
				_connected = true;
			}
			catch(ConnectionException)
			{
				_connected = false;
			}
		}

		public bool isConnected()
		{
			return _connected;
		}

		public void disconnect()
		{
			if(_connected)
			{
				_connected = false;
				connection.close();
			}
		}

		public void connect()
		{
			if(_connected)
				disconnect();

			connection = new Connection(_host, _port);
			_connected = true;
		}

		public delegate void ClientRequest();

		public bool execute(ClientRequest request)
		{
			try
			{
				if(!isConnected())
					connect();

				request();
				return true;
			}
			catch(net.ConnectionException)
			{
				disconnect();
				return false;
			}
			catch(System.IO.IOException)
			{
				disconnect();
				return false;
			}
		}
	}
}
