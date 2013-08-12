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

		private bool _started = false;

		private void resizeWindow()
		{
			if(_started)
				return;

			_window.x = _screen.width - 550;
			_window.y = 100;
			_window.width = 200;
            _window.height = 100;
			_window.title = "Annoying title";
			_window.draggable = true;
			_window.skin = gui.AssetBase.Skin.PlaqueDialog;

			_started = true;
		}

		public Xml handleGui(Xml request)
		{
			_screen.deserialize(request.root.node("screen"));
			resizeWindow();

			Xml response = new Xml("msg");
			_window.serialize(response.root);

			return response;
		}

		public Xml handleGuiUpdate(Xml request)
		{
			_window.deserializeState(request.root.node("window"));
			return null;
		}

		public Xml handle(Xml request)
		{
			switch(request.root.attribute("type").getString())
			{
			case "gui":
				return handleGui(request);
			case "guiUpdate":
				return handleGuiUpdate(request);
			}

			return null;
		}
	}
}

