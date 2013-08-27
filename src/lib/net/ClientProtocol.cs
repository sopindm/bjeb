using System.Threading;
using System.Collections.Generic;
using bjeb.gui;

namespace bjeb.net
{
	class Task
	{
		private Thread _thread = null;

		public delegate void Action();

		private Action _action;

		public Task(Action action)
		{
			_action = action;
		}

		public void run()
		{
			_thread = new Thread(() => _action());
			_thread.Start();
		}

		public void wait()
		{
			if(_thread == null)
				return;

			_thread.Join();
			_thread = null;

			return;
		}
	}

	public class ClientProtocol
	{
		private Client _client;
		private DebugSettings _settings;

		private Connection _connection
		{
			get
			{
				return _client.connection;
			}
		}

		private UpdateHandler _guiUpdateHandler;

		public ClientProtocol(Client client)
		{
			_client = client;

			_settings = new DebugSettings();
			_guiUpdateHandler = new UpdateHandler(_settings.guiUpdateRate);

			_windows = null;

			_background = new Task(_requestGui);
		}

		public bool connected
		{
			get
			{
				return _client != null && _client.connected;
			}
		}

		private Task _background;

		public void setup(Screen screen)
		{
			_windows = null;

			_settings = Protocol.requestSetup(_connection, screen);			
			_guiUpdateHandler = new UpdateHandler(_settings.guiUpdateRate);

			_background.wait();
			_background = new Task(() => 
					{
						if(_settings.updateGui)
							_client.execute(() => Protocol.requestGuiUpdate(_windows, _connection));

						_requestGui();
					});
		}

		public void setup(int width, int height)
		{
			setup(new Screen(width, height));
		}

		private List<gui.Window> _windows = null;

		public List<gui.Window> windows
		{
			get
			{
				if(!connected)
					_windows = null;

				if(!_settings.showGui)
					return null;

				_background.wait();

				if(_windows != null)
					return _windows;

				if(_windows == null)
				{
					_requestGui();
					return _windows;
				}
				
				return _windows;
			}
		}

		private void _requestGui()
		{
			_client.execute(() => _windows = Protocol.requestGui(_connection, updateWindow));
		}

		public void updateWindow(gui.Window window)
		{
			if(_settings.updateWindows) 
				_client.execute(() => Protocol.requestWindowUpdate(window, _connection));
		}

		public void updateGui()
		{
			if(_guiUpdateHandler.update())
				_background.run();
		}

		public void updateState(game.Vessel vessel)
		{
			if(_settings.updateState)
			{
				_client.execute(() => Protocol.requestUpdate(vessel, _connection));
			}
		}
	}
}