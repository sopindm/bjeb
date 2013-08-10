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
        public static void Main(string[] args)
        {
            ClientConnection connection = ClientConnection.create("127.0.0.1", 4400);

			while(!connection.alive())
			{
				System.Threading.Thread.Sleep(100);
				connection.reconnect();
			}

            Xml xml = new Xml("bjeb");

			XmlNode node = new XmlNode("modules", xml.root());

			new XmlNode("module", node);

			XmlNode module2 = new XmlNode("module", node);
			module2.attribute("name").set("Autopilot");
			module2.attribute("value").set(123);

			new XmlNode("module", node);

			xml.write(connection);

			Console.WriteLine("Read: " + Xml.read(connection).toString());

			Console.ReadLine();

			connection.close();
        }
    }
}
