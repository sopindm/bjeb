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

		public delegate void OnSetup();

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
			onSetup = setup;

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
			_setup = false;
		}

		public delegate void ClientRequest();

		public bool execute(ClientRequest request)
		{
			try
			{
				if(!connected)
					connect();

				if(!_setup && onSetup != null)
				{
					onSetup();
					_setup = true;
				}

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
