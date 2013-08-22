using bjeb.gui;

namespace bjeb
{
	class InfoModule: Module
	{
		public InfoModule(Computer computer): base(computer)
		{
		}

		private Label _massLabel;
		private Label _angularMomentumLabel;
		private Label _momentumOfInertiaLabel;

		override protected void onSetup(Screen screen)
		{
			window.area.set(0, 0, 200, 200);

			_massLabel = new Label();
			_angularMomentumLabel = new Label();
			_momentumOfInertiaLabel = new Label();

			Layout massLayout = Layout.makeHorizontal();
			massLayout.style = Style.TextArea;

			Label massText = new Label("Mass: ");
			massText.area.widthExpandable = true;

			massLayout.views.add(massText);
			massLayout.views.add(_massLabel);
			massLayout.views.add(new Button("x") { style = Style.Label });

			content.views.add(massLayout);
		}

		override protected void onUpdate()
		{
			_massLabel.text = vessel.body.mass.ToString("F2") + " t  ";
			/*
				"Angular momentum: " + vessel.body.angularMomentum.ToString() + " " +
				"Momentum of inertia: " + vessel.body.momentumOfInertia.ToString();*/

			//control for stats
			//need more control on styles (font colors) 
		}

		override public string name
		{
			get
			{
				return "Vessel Info";
			}
		}
	}
}