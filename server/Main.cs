using System;
using bjeb.net;

namespace bjeb.server
{
    class MainClass
    {
		private static void handleConnection(Connection connection)
		{
			Computer computer = new Computer();

			try
			{
				while(true)
				{
					Xml request = Xml.read(connection);
			
					Console.WriteLine(request.toString());

					computer.screen.deserialize(request.root.node("screen"));

					Xml response = new Xml("msg");

					computer.screen.serialize(new XmlNode(computer.screen.xmlName, response.root));

					response.write(connection);
				}
			}
			catch(ConnectionException)
			{
			}
			finally
			{
				connection.close();
			}
		}

        public static void Main(string[] args)
        {
            bjeb.net.Server server = new bjeb.net.Server("127.0.0.1", 4400);

			while(true)
			{
				server.accept(handleConnection);
			}

            server.stop();
        }
    }
}
