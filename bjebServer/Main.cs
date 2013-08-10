using System;
using bjeb.net;

namespace bjeb.server
{
    class MainClass
    {
		private static void handleConnection(Connection connection)
		{
			Xml read = Xml.read(connection);
			
            Console.WriteLine("Read: " + read.toString());

			foreach(XmlNode node in read.root().node("modules").nodes("module"))
			{
				var nameAttribute = node.attribute("name");
				var valueAttribute = node.attribute("value");

				if(!nameAttribute.isSet() || !valueAttribute.isSet())
					continue;

				string name = nameAttribute.getString();
				int value = valueAttribute.getInt();

				nameAttribute.set(name + " from server");
				valueAttribute.set(42 + (float)value / 1000);
			}

			read.write(connection);

            connection.close();
		}

        public static void Main(string[] args)
        {
            bjeb.net.Server server = new bjeb.net.Server("127.0.0.1", 4400);

			for(int i=0;i<10;i++)
				server.accept(handleConnection);

            server.stop();
        }
    }
}
