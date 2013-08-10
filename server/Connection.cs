using System;
using System.IO;
using System.Text;
using System.Net.Sockets;

namespace bjeb
{
	namespace net
	{
		public class ConnectionException: System.Exception
		{
		}

		public class Connection
		{
			private TcpClient _connection;
			private NetworkStream _stream;

			public Connection(string hostname, int port)
			{      
				connect(hostname, port);
			}	

			public Connection(TcpClient connection)
			{
				_connection = connection;
				_stream = _connection.GetStream();
			}

			protected void connect(string hostname, int port)
			{
				try
				{
					_connection = new TcpClient(hostname, port);
					_stream = _connection.GetStream();
				}
				catch(SocketException)
				{
					_connection = null;
					_stream = null;
				}
			}

			public string read()
			{
				byte[] data = new byte[4];
				_stream.Read(data, 0, 4);

				int size = BitConverter.ToInt32(data, 0);

				if(size == 0)
					throw new ConnectionException();

				byte[] stringData = new byte[size];
				_stream.Read(stringData, 0, size);

				return Encoding.Default.GetString(stringData);
			}

			public void write(string str)
			{
				if(_connection == null)
					throw new ConnectionException();

				try
				{
					byte[] data = Encoding.Default.GetBytes(str);

					_stream.Write(BitConverter.GetBytes(data.Length), 0, 4);
					_stream.Write(data, 0, data.Length);
				}
				catch(System.IO.IOException)
				{
					throw new ConnectionException();
				}
			}

			public void close()
			{
				if(_connection != null)
				{
					_stream.Close();
					_connection.Close();
				}

				_stream = null;
				_connection = null;
			}
		}

		public class ClientConnection: Connection
		{
			private string _host;
			private int _port;

			public ClientConnection(string host, int port): base(host, port)
			{
				_host = host;
				_port = port;
			}

			public static ClientConnection create(string hostname, int port)
			{
				return new ClientConnection(hostname, port);
			}	

			public void reconnect()
			{
				close();
				connect(_host, _port);
			}
		}
	}
}
