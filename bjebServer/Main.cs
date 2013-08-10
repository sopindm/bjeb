using System;

namespace bjeb.server
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            Server server = new Server();
            server.start();

			Connection connection = server.accept();

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
            server.stop();
        }
    }
}
