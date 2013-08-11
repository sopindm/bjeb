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
			private NetworkStream _stream;

			public Connection(string hostname, int port)
            {      
                try
                {
                    _connection = new TcpClient(hostname, port);
					_connection.NoDelay = true;

                    _stream = _connection.GetStream();
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
				_stream = _connection.GetStream();
			}

			public string read()
			{
				try
				{
					byte[] data = new byte[4];
					_stream.Read(data, 0, 4);

					int size = BitConverter.ToInt32(data, 0);

					if(size == 0)
						throw new ConnectionException("failed to read from connection");

					byte[] stringData = new byte[size];
					_stream.Read(stringData, 0, size);

					return Encoding.Default.GetString(stringData);
				}
				catch(System.IO.IOException)
				{
					throw new ConnectionException("failed to read from connection");
				}
			}

			public void write(string str)
			{
				if(_connection == null)
					throw new ConnectionException("failed to write to connection");

				try
				{
					byte[] data = Encoding.Default.GetBytes(str);

					_stream.Write(BitConverter.GetBytes(data.Length), 0, 4);
					_stream.Write(data, 0, data.Length);
				}
				catch(System.IO.IOException)
				{
					throw new ConnectionException("failed to write to connection");
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
	}
}
