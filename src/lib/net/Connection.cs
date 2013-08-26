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
			private string _error
			{
				get;
				set;
			}

			public ConnectionException(string error)
			{
				_error = error;
			}

			public override string ToString()
			{
				return "Serialization exception: " + _error;
			}
        }

		public class Connection
		{
			private TcpClient _connection;
			private System.IO.BufferedStream _stream;
			private BinaryReader _reader;
			private BinaryWriter _writer;

			public Connection(string hostname, int port)
			{      
				try
				{
					_connection = new TcpClient(hostname, port);
					_connection.NoDelay = true;

					_stream = new BufferedStream(_connection.GetStream());
					_reader = new BinaryReader(_stream);
					_writer = new BinaryWriter(_stream);
				} 
				catch (SocketException)
				{
					_connection = null;
					_stream = null;

					throw new ConnectionException("Failed to connect to server");
				}
			}	

			public Connection(TcpClient connection)
			{
				_connection = connection;
				_connection.NoDelay = true;

				_stream = new BufferedStream(_connection.GetStream());
				_reader = new BinaryReader(_stream);
				_writer = new BinaryWriter(_stream);
			}

			public void flush()
			{
				_stream.Flush();
			}

			public bjeb.net.Stream stream
			{
				get
				{
					return new bjeb.net.Stream(_reader, _writer);
				}
			}

			public void close()
			{
			}
		}
    }
}
