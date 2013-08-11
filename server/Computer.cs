using bjeb.net;

namespace bjeb
{
	public class Computer
	{
		private gui.Screen _screen;
		private gui.Window _window;

		public Computer()
		{
			_screen = new gui.Screen();
			_window = new gui.Window();
		}

		private void resizeWindow()
		{
			_window.x = _screen.width - 120;
			_window.y = 100;
			_window.width = 100;
			_window.height = _screen.height - 120;
		}

		public Xml handle(Xml request)
		{
			_screen.deserialize(request.root.node("screen"));
			resizeWindow();

			Xml response = new Xml("msg");
			_window.serialize(response.root);

			return response;
		}
	}
}

