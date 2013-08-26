using System;
using System.IO;
using bjeb.net;
using bjeb.gui;
//using bjeb.game;

namespace bjeb.test
{
    class TestPlugin
    {
	public void setup(Connection connection)
	{
	    Protocol.requestSetup(connection, 1000, 500);
	    System.Console.WriteLine("Set up");
	}

	public void update(Connection connection)
	{
	    System.Console.WriteLine("Updating");
	    var windows = Protocol.requestGui(connection);
	    System.Console.WriteLine("Updated");

            Window window = windows[0];

	    window.area.x--;
	    window.area.y++;
	    window.area.width++;
	    window.area.height--;

            Console.WriteLine("Window ID: " + window.id + " X: " + window.area.x + "Y: " + window.area.y + " Width: " + window.area.width + " Height: " + window.area.height);

/*
	    Protocol.requestGuiUpdate(windows, connection);
	    Protocol.requestWindowUpdate(window, connection);

	    Xml tmp = new net.Xml("msg");
	    window.views.serialize(tmp.root);

	    Console.WriteLine("Views: " + tmp.toString());

	    Vessel vessel = new Vessel();

	    Protocol.requestUpdate(vessel, connection);*/
	}
    }
	
    class MainClass
    {
        public static void Main(string[] args)
        {
	    TestPlugin plugin = new TestPlugin();
	    Client client = new Client("127.0.0.1", 4400, plugin.setup);

            while (true)
            {
                if(!client.execute((c) => 
		    {
		        plugin.update(c);
			System.Threading.Thread.Sleep(1000);
		    }))
		{
		    Console.WriteLine("No connection");
		    System.Threading.Thread.Sleep(1000);
		}
	    }
	}
    }
}
