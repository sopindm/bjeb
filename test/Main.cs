using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using bjeb.net;

namespace bjeb.test
{
    class MainClass
    {
		public static void update(ClientConnection connection)
		{
			Gui gui = new Gui();

			gui.width = 100;
			gui.height = 50;

			gui.request().write(connection);

			Console.WriteLine("Read: " + Xml.read(connection).toString());
		}

        public static void Main(string[] args)
        {
            ClientConnection connection = ClientConnection.create("127.0.0.1", 4400);

			while(true)
			{
				try
				{
					while(true)
					{
						update(connection);
						System.Threading.Thread.Sleep(1000);
					}
				}
				catch(ConnectionException)
				{
				}
				finally
				{
					connection.reconnect();
				}
			}
		}
    }
}
