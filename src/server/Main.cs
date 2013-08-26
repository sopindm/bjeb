using System;
using System.IO;
using bjeb.net;
//using bjeb.math;

namespace bjeb.server
{
    class TestServer: IServer
    {
	public void onSetup(gui.Screen screen)
	{
	    Console.WriteLine("Set up with screen width: " + screen.width + " height: " + screen.height);
	}
    }

    class MainClass
    {
	private static void handleConnection(Connection connection)
	{
	    TestServer server = new TestServer();

//			Computer computer = new Computer();

	    try
	    {
		while(true)
		{
		    Protocol.handle(connection, server);
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
