namespace bjeb.net
{
	public class Client
	{
		private string _host;
		private int _port;

		public Connection connection
		{
			get;
			private set;
		}

		public Client(string host, int port)
		{
			_host = host;
			_port = port;

			connection = new Connection(host, port);
		}

		public void reconnect()
		{
			connection.close();
			connection = new Connection(_host, _port);
		}
	}
}
