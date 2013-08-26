using System;
using System.IO;
using bjeb.net;
using bjeb.math;

namespace bjeb.server
{
    class MainClass
    {
		private static void handleConnection(Connection connection)
		{
//			Computer computer = new Computer();

			try
			{
				while(true)
				{
				    Console.WriteLine("Read: " + connection.stream.readTag(true).ToString());
				    connection.stream.write(1.23F);

					/*
					Xml request = Xml.read(connection);
			
					Console.WriteLine("Request: " + request.toString());

					Xml response = Protocol.handle(request, computer);

					if(response != null)
					{
						Console.WriteLine("Response: " + response.toString());
						Console.WriteLine();
						response.write(connection);
					}*/
				}
			}
			catch(ConnectionException)
			{
			}
			catch(System.IO.IOException)
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
        }
    }
}
