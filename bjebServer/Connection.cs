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

			public bool alive()
			{
				if(_connection == null)
					return false;

				Socket client = _connection.Client;
				bool blockingState = client.Blocking;

				try
				{
					byte [] tmp = new byte[1];

					client.Blocking = false;
					client.Send(tmp, 0, 0);
					return true;
				}
				catch (SocketException e) 
				{
					if (e.NativeErrorCode.Equals(10035))
						return true;
					else
						return false;
				}
				finally
				{
					client.Blocking = blockingState;
				}
			}

			public string read()
			{
				if(!alive())
					throw new ConnectionException();

				byte[] data = new byte[4];
				_stream.Read(data, 0, 4);

				int size = BitConverter.ToInt32(data, 0);

				byte[] stringData = new byte[size];
				_stream.Read(stringData, 0, size);

				return Encoding.Default.GetString(stringData);
			}

			public void write(string str)
			{
				if(!alive())
					throw new ConnectionException();

				byte[] data = Encoding.Default.GetBytes(str);

				_stream.Write(BitConverter.GetBytes(data.Length), 0, 4);
				_stream.Write(data, 0, data.Length);
			}

			public void close()
			{
				if(_connection != null)
				{
					_stream.Close();
					_connection.Close();
				}
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
