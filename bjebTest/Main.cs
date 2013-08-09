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

            XmlDocument doc = new XmlDocument();

	    XmlDeclaration dec = doc.CreateXmlDeclaration("1.0", null, null);
	    doc.AppendChild(dec);

	    XmlElement root = doc.CreateElement("bjeb");
	    doc.AppendChild(root);

	    connection.writeString(doc.OuterXml);
	    Console.WriteLine("Read: " + connection.readString());

	    connection.close();
        }
    }
}
