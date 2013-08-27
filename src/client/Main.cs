using System;
using System.IO;
using bjeb.net;
using bjeb.gui;
using bjeb.game;

namespace bjeb.test
{
    class TestPlugin
    {
		private DebugSettings _settings;
		private UpdateHandler _guiHandler;
		private ClientProtocol _protocol;

		public TestPlugin()
		{
		}

		public void setup(Connection connection)
		{
			_protocol = new ClientProtocol();
			_settings = _protocol.requestSetup(connection, 1000, 500);
			System.Console.WriteLine("Set up");

			_guiHandler = new UpdateHandler(_settings.guiUpdateRate);
		}

		private void updateGui(Connection connection)
		{
			if(!_guiHandler.update())
				return;

			System.Console.WriteLine("Updating gui");
			var windows = _protocol.requestGui(connection, (w => { if(_settings.updateWindows) _protocol.requestWindowUpdate(w, connection); }));

			Window window = windows[0];

			window.area.x--;
			window.area.y++;
			window.area.width++;
			window.area.height--;

			Console.WriteLine("Window ID: " + window.id + " X: " + window.area.x + "Y: " + window.area.y + " Width: " + window.area.width + " Height: " + window.area.height);

			if(_settings.updateWindows)
				foreach(var w in windows)
					_protocol.requestWindowUpdate(w, connection);

			if(_settings.updateGui)
				_protocol.requestGuiUpdate(windows, connection);
		}

		public void update(Connection connection)
		{
			if(_settings.showGui)
				updateGui(connection);

			if(_settings.updateState)
			{
				Vessel vessel = new Vessel();
				_protocol.requestUpdate(vessel, connection);
			}
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
						//System.Threading.Thread.Sleep(1000);
					}))
				{
					Console.WriteLine("No connection");
					System.Threading.Thread.Sleep(1000);
				}
			}
		}
    }
}
