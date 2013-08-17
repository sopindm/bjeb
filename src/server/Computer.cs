using bjeb.net;
using bjeb.gui;

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

		private void setupWindow()
		{
			_window.area.set(_screen.width - 550, 50, 200, 30);

			_window.title = "BJEB";
			_window.draggable = true;
			_window.skin = gui.Skin.Window2;

			_window.views.clear();

			_window.views.add(new Button("D") { area = new Area(_window.area.width.Value - 60, 5, 20, 20),
						onClick = ((b, m) => {
								if(_window.draggable)
								{
									b.text = "F";
									_window.draggable = false;
								}
								else
								{
									b.text = "D";
									_window.draggable = true;
								}}) });

			Layout content = Layout.makeVertical();

			_window.views.add(new Button("_")
				{ area = new Area(_window.area.width.Value - 40, 5, 20, 20),
						onClick = ((b, m) => { 
								if(content.isShowing) 
								{
									content.hide(); 
									_window.area.height = 50;
								}
								else content.show(); })});
						
			_window.views.add(new Button("X") { area = new Area(_window.area.width.Value - 20, 5, 20, 20) });

			content.views.add(new Toggle("Sample module", false));

			_window.views.add(content);
			_window.views.add(new Layout());
		}

		public Xml handleSetup(Xml request)
		{
			_screen.deserialize(request.root.node("screen"));
			setupWindow();

			return null;
		}

		public Xml handleGui(Xml request)
		{
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
			case "setup":
				return handleSetup(request);
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

