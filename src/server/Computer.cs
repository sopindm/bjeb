using System.Collections;
using System.Collections.Generic;
using bjeb.net;
using bjeb.gui;
using bjeb.game;

namespace bjeb
{
	public class Angle
	{
		public static double fromRadians(double radians)
		{
			return radians / System.Math.PI * 180;
		}
	}

	public class Computer: IServer
	{
		private gui.Window _window = null;

		private List<Module> modules;

		public DebugSettings settings
		{
			get;
			private set;
		}

		public Computer()
		{			
			modules = new List<Module>();

			modules.Add(new ASASModule(this));
			modules.Add(new InfoModule(this));

			_switches = new Dictionary<Module, ModuleSwitch>();

			settings = new DebugSettings() { guiUpdateRate = 30f, stateUpdateRate = 30f };
		}

		public void onSetup(Screen screen)
		{
			_window = new gui.Window();
			_window.area.set(screen.width - 550, 50, 200, 50);

			_window.title = "BJEB";
			_window.draggable = true;
			_window.skin = gui.Skin.Window7;

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

			_switches.Clear();

			foreach(var module in modules)
			{
				_switches.Add(module, new ModuleSwitch(content, module));
				module.show();
			}

			_window.views.add(content);
			_window.views.add(new Layout());

			foreach(var module in modules)
			{
				module.setup(screen);
			}
		}

		private class ModuleSwitch
		{
			private Toggle _toggle;

			public ModuleSwitch(Layout layout, Module module)
			{
				_toggle = new Toggle(module.name, false);

				_toggle.onSwitch = (m => 
						{
							if(m.toggled)
								module.show();
							else
								module.hide();
						});

				layout.views.add(_toggle);
			}

			public void show()
			{
				_toggle.toggled = true;
			}

			public void hide()
			{
				_toggle.toggled = false;
			}
		}

	    private Dictionary<Module, ModuleSwitch> _switches;

		public void show(Module module)
		{
		    _switches[module].show();
		}

		public void hide(Module module)
		{
		    _switches[module].hide();
		}

		public List<Window> windows
		{
			get
			{
				List<Window> ret = new List<Window>();
				ret.Add(_window);

				foreach(var module in modules)
				{
					if(module.isShowing)
						ret.Add(module.window);
				}

				return ret;
			}
		}

		public void onUpdate(Vessel vessel)
		{
		    foreach(var module in modules)
				module.update(vessel);
		}
	}
}

