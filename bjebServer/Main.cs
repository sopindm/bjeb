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

            Console.WriteLine("Read: " + connection.readString());
	    connection.writeString("Response from server");

            connection.close();
            server.stop();
        }
    }
}
