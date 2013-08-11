using System;
using bjeb.net;
using bjeb.gui;

namespace bjeb.test
{
    class MainClass
    {
		public static void update(Connection connection)
		{
			Screen screen = new Screen();

			screen.width = 100;
			screen.height = 50;

			Xml request = new Xml("msg");

			screen.serialize(new XmlNode(screen.xmlName, request.root));

			request.write(connection);

			Console.WriteLine("Read: " + Xml.read(connection).toString());
		}

        public static void Main(string[] args)
        {
            while (true)
            {
                try
                {
					Client client = new Client("127.0.0.1", 4400);

                    while(true)
					{
						update(client.connection);
						System.Threading.Thread.Sleep(1000);
					}
                } 
				catch (ConnectionException)
                {
					Console.WriteLine("No connection");
					System.Threading.Thread.Sleep(1000);
                } 
			}
		}
    }
}
