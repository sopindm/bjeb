namespace bjeb.net
{
	public class Client
	{
		private string _host;
		private int _port;

		private bool _connected;
		private bool _setup;

		public Connection connection
		{
			get;
			private set;
		}

		public delegate void OnSetup(Connection connection);

		public OnSetup onSetup
		{
			get;
			set;
		}

		public Client(string host, int port, OnSetup setup = null)
		{
			_host = host;
			_port = port;
			_setup = false;

			try
			{
				connection = new Connection(host, port);
				onSetup = setup;
				_connected = true;
			}
			catch(ConnectionException)
			{
				_connected = false;
			}
		}

		public bool connected
		{
			get
			{
				return _connected;
			}
		}

		public void disconnect()
		{
			if(_connected)
			{
				_connected = false;
				_setup = false;
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

		public delegate void ClientRequest(Connection connection);

		public bool execute(ClientRequest request)
		{
			try
			{
				if(!connected)
					connect();

				if(!_setup && onSetup != null)
				{
					onSetup(connection);
					_setup = true;
				}

				request(connection);
				return true;
			}
			catch(net.ConnectionException)
			{
				disconnect();
				return false;
			}
		}
	}
}
