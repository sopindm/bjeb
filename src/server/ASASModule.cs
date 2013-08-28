using System.Collections.Generic;
using bjeb.gui;

namespace bjeb
{
	class AxisController
	{
		private string _name;

		public AxisController(string name)
		{
			_name = name;
		}

		static private Button _button(string text)
		{
			Button button = new Button(text);
			button.area.widthExpandable = false;
			button.area.heightExpandable = false;
			button.area.width = 25;
			button.area.height = 25;

			button.font.style = FontStyle.Bold;

			return button;
		}

		public View view
		{
			get
			{
				Layout mainLayout = Layout.makeHorizontal();

				var onToggle = new Toggle(_name, false);
				onToggle.area.width = 50;

				mainLayout.views.add(onToggle);

				Layout digitalLayout = Layout.makeHorizontal();

				digitalLayout.views.add(new Space());

				digitalLayout.views.add(_button("<<<"));
				digitalLayout.views.add(_button("<<"));
				digitalLayout.views.add(_button("<"));

				Label infoLabel = new Label("180");
				infoLabel.area.width = 30;
				infoLabel.font.alignment = Alignment.Center;
				infoLabel.font.style = FontStyle.Bold;

				digitalLayout.views.add(infoLabel);

				digitalLayout.views.add(_button(">"));
				digitalLayout.views.add(_button(">>"));
				digitalLayout.views.add(_button(">>>"));

				digitalLayout.views.add(new Space());

				Layout analogLayout = Layout.makeHorizontal();

				analogLayout.views.add(new Slider(-180, 180, 0));

				Layout controlLayout = Layout.makeVertical();
				controlLayout.views.add(digitalLayout);
				controlLayout.views.add(analogLayout);

				mainLayout.views.add(controlLayout);

				Layout switchLayout = Layout.makeVertical();

				switchLayout.views.add(new Space(10));

				Button switchButton = new Button("+");
				switchButton.area.width = 30;
				switchButton.area.height = 30;

				switchLayout.views.add(switchButton);

				switchLayout.views.add(new Space());

				mainLayout.views.add(new Space(10));

				mainLayout.views.add(switchLayout);

				return mainLayout;
			}
		}
	}

	class ASASModule: Module
	{
		public ASASModule(Computer computer): base(computer)
		{
		}

		override protected void onSetup(Screen screen)
		{
			window.area.set(0, 200, 400, 200);
			content.views.clear();

			Layout referenceLayout = Layout.makeHorizontal();

			referenceLayout.views.add(new Button("ORB"));
			referenceLayout.views.add(new Button("SUR"));
			referenceLayout.views.add(new Button("TRG"));

			content.views.add(referenceLayout);
			content.views.add(new Space(10));
			content.views.add(new AxisController("YAW").view);
			content.views.add(new Space(10));
			content.views.add(new AxisController("PITCH").view);
			content.views.add(new Space(10));
			content.views.add(new AxisController("ROLL").view);
			content.views.add(new Space(10));

			content.views.add(makeOptionsLayout());
		}

		private View makeOptionsLayout()
		{
			Layout optionsLayout = Layout.makeHorizontal();
			
			optionsLayout.views.add(new Toggle("OFF", false));

			optionsLayout.views.add(new Space());

			Toggle grabInputToggle = new Toggle("GRAB", false);
			grabInputToggle.area.width = 80;
			
			optionsLayout.views.add(grabInputToggle);

			return optionsLayout;
		}

		override protected void onUpdate()
		{
		}

		override public string name
		{
			get
			{
				return "ASAS";
			}
		}
	}
}