using System;
using System.IO;
using bjeb.net;
using bjeb.gui;
using bjeb.game;

namespace bjeb.test
{
    class TestPlugin
    {
		private ClientProtocol _protocol = null;
		private Universe universe;

		public TestPlugin(Client client)
		{
			_protocol = new ClientProtocol(client);
			universe = new Universe();
		}

		public void setup()
		{
			_protocol.setup(1000, 500);
			System.Console.WriteLine("Set up");
		}

		private void updateGui()
		{
			var windows = _protocol.windows;

			if(windows == null)
				return;

			System.Console.WriteLine("Updating gui");

			Window window = windows[0];

			window.area.x--;
			window.area.y++;
			window.area.width++;
			window.area.height--;

			Console.WriteLine("Window ID: " + window.id + " X: " + window.area.x + "Y: " + window.area.y + " Width: " + window.area.width + " Height: " + window.area.height);

			_protocol.updateWindow(window);

			_protocol.updateGui();
		}

		public void update()
		{
			updateGui();
			
			_protocol.updateState(universe, new Vessel(universe));
			_protocol.requestControl(new Vessel(universe), new FlightControl());

		}
    }
	
    class MainClass
    {
        public static void Main(string[] args)
		{
			Client client = new Client("127.0.0.1", 4400);
			TestPlugin plugin = new TestPlugin(client);

			client.onSetup = plugin.setup;

            while (true)
            {
				plugin.update();

				if(!client.connected)
				{
					Console.WriteLine("No connection");
					System.Threading.Thread.Sleep(1000);
				}
			}
		}
    }
}
