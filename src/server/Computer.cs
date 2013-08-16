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

			_window.area.set(_screen.width - 550, 100, 300, 100);
			_window.title = "Annoying title";
			_window.draggable = true;
			_window.skin = gui.AssetBase.Skin.Window2;

			_window.views.clear();

			gui.Layout layout = gui.Layout.makeHorizontal();

			gui.Button button = new gui.Button("X");
			button.onClick = ((b, m) =>
					{ 
						if(m.leftButtonUp)
						{
							if(m.rightButtonPressed)
								b.text = "F";
							else
								b.text = "L"; 
						}
						else if(m.rightButtonUp)
							b.text = "R";
						else if(m.middleButtonUp)
							b.text = "M";
					});

			button.area.x = _window.area.width - 22;
			button.area.y = 2;
			button.area.width = 20;
			button.area.height = 20;

			_window.views.add(button);

			layout.views.add(new gui.Textbox());
			layout.views.add(new gui.Label("TEXT"));
			
			gui.Button button2 = new gui.Button("Click me");
			button2.onClick = ((b, m) => { b.text = "Click me again"; });

			layout.views.add(new gui.Label("Other text..."));

			gui.Layout layout2  = gui.Layout.makeVertical();

			layout2.views.add(button2);
			layout2.views.add(layout);
 
			gui.Textbox box = new gui.Textbox("");
			box.onUpdate = (b  => { button2.text = b.text; });

			layout2.views.add(box);

			gui.Slider slider = new gui.Slider(0, 1000, 500);
			slider.isHorizontal = false;

			layout2.views.add(slider);

			_window.views.add(layout2);

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

