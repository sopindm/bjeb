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
			
					Console.WriteLine("Request: " + request.toString());

					Xml response = computer.handle(request);

					if(response != null)
					{
						Console.WriteLine("Response: " + response.toString());
						Console.WriteLine();
						response.write(connection);
					}
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
        }
    }
}