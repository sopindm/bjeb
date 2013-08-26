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

	    try
	    {
		while(true)
		{
		    Protocol.handle(connection, computer);
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
