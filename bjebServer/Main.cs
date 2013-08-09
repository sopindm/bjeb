using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Xml;

namespace bjebServer
{
    class Connection
    {
        private TcpClient _connection;
        private NetworkStream _stream;

        public Connection(TcpClient connection)
        {
            _connection = connection;
            _stream = _connection.GetStream();
        }

        public string readString()
        {
            byte[] data = new byte[4];
            _stream.Read(data, 0, 4);

            int size = BitConverter.ToInt32(data, 0);

            byte[] stringData = new byte[size];
            _stream.Read(stringData, 0, size);

            return Encoding.Default.GetString(stringData);
        }

        public void writeString(string str)
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

    class Server
    {
        private TcpListener _listener;

        public void start()
        {
            _listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 4400);
            _listener.Start();
        }

        public void stop()
        {
            _listener.Stop();
        }

        public Connection accept()
        {
            return new Connection(_listener.AcceptTcpClient());
        }
    }

    class MainClass
    {
        public static void Main(string[] args)
        {
            Server server = new Server();
            server.start();

            Connection connection = server.accept();

            Console.WriteLine("Read: " + connection.readString());

            connection.close();
            server.stop();
        }
    }
}
