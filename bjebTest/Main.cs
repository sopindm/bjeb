using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace bjebTest
{
    class MainClass
    {
        public static void Main(string[] args)
        {
	    bjeb.Connection connection = bjeb.Connection.create("127.0.0.1", 4400);

	    connection.writeString("Hello from client");
	    Console.WriteLine("Read: " + connection.readString());

	    connection.close();
        }
    }
}
