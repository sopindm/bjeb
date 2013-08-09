using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace bjebClient
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            TcpClient client = new TcpClient("127.0.0.1", 4400);

            NetworkStream stream = client.GetStream();

            byte[] data = Encoding.Default.GetBytes("Hello from client");

            stream.Write(BitConverter.GetBytes(data.Length), 0, 4);
            stream.Write(data, 0, data.Length);

            stream.Close();
            client.Close();
        }
    }
}
