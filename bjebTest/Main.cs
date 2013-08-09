using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Xml;

namespace bjebTest
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            bjeb.Connection connection = bjeb.Connection.create("127.0.0.1", 4400);

            bjeb.Xml xml = new bjeb.Xml("bjeb");
			bjeb.XmlNode node = new bjeb.XmlNode("modules", xml.root());
			bjeb.XmlNode module1 = new bjeb.XmlNode("module", node);

			bjeb.XmlNode module2 = new bjeb.XmlNode("module", node);
			module2.attribute("name").set("Autopilot");
			module2.attribute("value").set(123);

			bjeb.XmlNode module3 = new bjeb.XmlNode("module", node);

			xml.write(connection);

			Console.WriteLine("Read: " + bjeb.Xml.read(connection).toString());

			connection.close();
        }
    }
}
