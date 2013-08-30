using bjeb.game;
using bjeb.gui;

namespace bjeb
{
	public abstract class Module
	{
		private Computer _computer;

		protected Vessel vessel
		{
			get;
			private set;
		}

		public Window window
		{
			get;
			private set;
		}

		protected Layout content
		{
			get;
			private set;
		}

		public bool isShowing
		{
			get;
			private set;
		}

		public void hide()
		{
			isShowing = false;

			_computer.hide(this);
		}

		public void show()
		{
			isShowing = true;

			_computer.show(this);
		}

		public Module(Computer computer)
		{
			_computer = computer;
			//this.vessel = vessel;

			window = new Window();
			content = Layout.makeVertical();

			isShowing = false;
		}

		public void setup(Screen screen)
		{
			window.draggable = true;
			window.title = name;
			window.skin = gui.Skin.Window7;

			window.views.clear();

			window.views.add(content);
			window.views.add(new Layout());

			content.views.clear();
			onSetup(screen);

			window.views.add(new Button("D") { area = new Area(window.area.width.Value - 60, 5, 20, 20),
						onClick = ((b, m) => {
								if(window.draggable)
								{
									b.text = "F";
									window.draggable = false;
								}
								else
								{
									b.text = "D";
									window.draggable = true;
								}}) });


			window.views.add(new Button("_")
				{ area = new Area(window.area.width.Value - 40, 5, 20, 20),
						onClick = ((b, m) => { 
								if(content.isShowing) 
								{
									content.hide(); 
									window.area.height = 50;
								}
								else content.show(); })});
						
			window.views.add(new Button("X") { area = new Area(window.area.width.Value - 20, 5, 20, 20),
						onClick= ((b, m) => hide())});
		}

		abstract protected void onSetup(Screen screen);

		public void update(Vessel vessel)
		{
			this.vessel = vessel;
			onUpdate();
		}

		abstract protected void onUpdate();

		public abstract string name
		{
			get;
		}

		virtual public void onControl(FlightControl c)
		{
		}
	}
}