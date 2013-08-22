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
		private Label vesselInfo = null;

		public Computer()
		{
		}

		public void onSetup(Screen screen)
		{
			_window = new gui.Window();
			_window.area.set(screen.width - 550, 50, 200, 30);

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

			vesselInfo = new Label();

			content.views.add(new Toggle("Sample module", false));
			content.views.add(vesselInfo);

			_window.views.add(content);
			_window.views.add(new Layout());
		}

		public IEnumerable<Window> windows
		{
			get
			{
				List<Window> ret = new List<Window>();
				ret.Add(_window);

				return ret;
			}
		}

		public void onUpdate(Vessel vessel)
		{
			vesselInfo.text = 
				"Mass: " + vessel.body.mass.ToString("F2") + " " +
				"Angular momentum: " + vessel.body.angularMomentum.ToString() + " " +
				"Momentum of inertia: " + vessel.body.momentumOfInertia.ToString();
		}
	}
}

