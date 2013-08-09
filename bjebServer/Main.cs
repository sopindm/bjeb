using System;

namespace bjebServer
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            bjeb.Server server = new bjeb.Server();
            server.start();

			bjeb.Connection connection = server.accept();

			bjeb.Xml read = bjeb.Xml.read(connection);
			
			foreach(bjeb.XmlNode node in read.root().node("modules").nodes("module"))
			{
				if(!node.haveAttribute("name") || !node.haveAttribute("value"))
					continue;

				string name = node.attribute("name").getString();
				int value = node.attribute("value").getInt();

				node.attribute("name").set(name + " from server");
				node.attribute("value").set(42 + (float)value / 1000);
			}

            Console.WriteLine("Read: " + read.toString());

			read.write(connection);

            connection.close();
            server.stop();
        }
    }
}
