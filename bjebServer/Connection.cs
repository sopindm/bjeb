using System;
using System.IO;
using System.Text;
using System.Net.Sockets;

namespace bjeb
{
    public class Connection
    {
        private TcpClient _connection;
        private NetworkStream _stream;

	public static Connection create(string hostname, int port)
	{
	    return new Connection(hostname, port);
	}	

   	private Connection(string hostname, int port)
	{      
	    _connection = new TcpClient(hostname, port);
	    _stream = _connection.GetStream();
        }	

	public Connection(TcpClient connection)
        {
            _connection = connection;
            _stream = _connection.GetStream();
        }

        public string read()
        {
            byte[] data = new byte[4];
            _stream.Read(data, 0, 4);

            int size = BitConverter.ToInt32(data, 0);

            byte[] stringData = new byte[size];
            _stream.Read(stringData, 0, size);

            return Encoding.Default.GetString(stringData);
        }

        public void write(string str)
        {
            byte[] data = Encoding.Default.GetBytes(str);

            _stream.Write(BitConverter.GetBytes(data.Length), 0, 4);
            _stream.Write(data, 0, data.Length);
        }

        public void close()
        {
            _stream.Close();
            _connection.Close();
        }
    }
}
