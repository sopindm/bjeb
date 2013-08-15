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

			_window.area.x = _screen.width - 550;
			_window.area.y = 100;
			_window.area.width = 200;
			_window.area.height = 100;
			_window.title = "Annoying title";
			_window.draggable = true;
			_window.skin = gui.AssetBase.Skin.Window6;

			_window.views.clear();

			gui.Button button = new gui.Button();
			button.text = "Click me";
			button.skin = gui.AssetBase.Skin.PlaqueDialog;
			button.onClick = (b => 
					{
						b.text = "Don't click me agan, please";
						b.area.x = 0;
						b.area.y = 50;
						b.area.width = 200;
						b.area.height = 100;

						_window.title = "Fuck off";

						_window.area.width = 400;
						_window.area.height = 300;
					});

			_window.views.add(button);

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

		private Xml handleWindowUpdate(Xml request)
		{
			if(request.root.attribute("id").getInt() != _window.id)
				return null;

			System.Console.WriteLine("Deserializing from: " + request.toString());

			_window.views.deserializeState(request.root);
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
			case "guiWindowUpdate":
				return handleWindowUpdate(request);
			}

			return null;
		}
	}
}

