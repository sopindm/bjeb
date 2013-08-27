using System;
using System.Collections.Generic;
using System.IO;
using bjeb.net;
//using bjeb.math;

namespace bjeb.server
{
    class MainClass
    {
		private static void handleConnection(Connection connection)
		{
			Computer computer = new Computer();

			DateTime startTime = DateTime.Now;
			int responses = 0;

			double rps = 0;

			try
			{
				while(true)
				{
					ServerProtocol.handle(connection, computer);

					responses++;
					double delta = (DateTime.Now - startTime).TotalMilliseconds;

					if(delta > 2000)
					{
						rps = responses / delta * 1000;
						responses = 0;
						startTime = DateTime.Now;
					}

					Console.WriteLine(rps.ToString("F2") + " responses in seconds.");
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
