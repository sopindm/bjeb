using System.Collections.Generic;
using bjeb.gui;

namespace bjeb
{
	class AxisController
	{
		private bool _fullCircle = true;

		private Label _info = null;
		private Slider _slider = null;

		private View _view = null;

		private bool _onFrontSide = true;
		private bool onFrontSide
		{
			get
			{
				return _onFrontSide;
			}
			set
			{
				if(_onFrontSide != value)
				{
					_onFrontSide = value;
					switchValue();
				}
			}
		}

		private void switchValue()
		{
			if(onFrontSide)
			{
				if(value >= 180)
					value = 270 + (270 - value);
				else
					value = 90 - (value - 90);
			}
			else
			{
				if(value >= 0)
					value = 90 + (90 - value);
				else
					value = 270 - (value - 270);
			}

			updateValue();
		}

		public int value
		{
			get;
			private set;
		}

		private float sliderValue()
		{
			if(onFrontSide)
				return value > 90 ? value - 360 : value;
			else
				return value - 180;
		}

		private void updateSliderValue(float svalue)
		{
			value = onFrontSide ? (int)svalue : (int)svalue + 180;
			updateValue();
		}

		private void updateValue()
		{
			if(_fullCircle)
			{
				if(value < 0)
					value += 360;
				if(value >= 360)
					value -= 360;
			}
			
			if(!_fullCircle)
			{
				if(value > 90)
					value = 90;
				else if(value < -90)
					value = -90;
			}

			if((value > 90 && value < 270) && onFrontSide)
			{
				_onFrontSide = false;
				_sideSwitch.text = "-";
			}
			else if ((value > 270 || value < 90) && !onFrontSide)
			{
				_onFrontSide = true;
				_sideSwitch.text = "+";
			}

			_slider.value = sliderValue();
			_info.text = value.ToString("F0"); 
		}

		private Button _sideSwitch = null;

		public AxisController(string name, bool fullCircle)
		{
			_fullCircle = fullCircle;

			Layout mainLayout = Layout.makeHorizontal();

			var onToggle = new Toggle(name, false);
			onToggle.area.width = 50;
			onToggle.onSwitch = (t => active = t.toggled);

			mainLayout.views.add(onToggle);

			Layout digitalLayout = Layout.makeHorizontal();

			digitalLayout.views.add(new Space());

			digitalLayout.views.add(_button("<<<", -15));
			digitalLayout.views.add(_button("<<", -5));
			digitalLayout.views.add(_button("<", -1));

			_info = new Label("0");
			_info.area.width = 30;
			_info.font.alignment = Alignment.Center;
			_info.font.style = FontStyle.Bold;

			digitalLayout.views.add(_info);

			digitalLayout.views.add(_button(">", 1));
			digitalLayout.views.add(_button(">>", 5));
			digitalLayout.views.add(_button(">>>", 15));

			digitalLayout.views.add(new Space());

			Layout analogLayout = Layout.makeHorizontal();

			_slider = new Slider(-90, 90, 0);
			_slider.onUpdate = (s => { updateSliderValue(s.value); });

			analogLayout.views.add(_slider);

			Layout controlLayout = Layout.makeVertical();
			controlLayout.views.add(digitalLayout);
			controlLayout.views.add(analogLayout);

			mainLayout.views.add(controlLayout);

			Layout switchLayout = Layout.makeVertical();

			switchLayout.views.add(new Space(10));

			_sideSwitch = new Button("+");
			_sideSwitch.area.width = 30;
			_sideSwitch.area.height = 30;
			_sideSwitch.onClick = ((b, m) => 
					{
						onFrontSide = !onFrontSide;

						if(onFrontSide)
							b.text = "+";
						else
							b.text = "-";
					});

			if(_fullCircle)
				switchLayout.views.add(_sideSwitch);
			else
			{
				Layout placeholder = Layout.makeHorizontal();
				placeholder.views.add(new Space(34));

				switchLayout.views.add(placeholder);
			}

			switchLayout.views.add(new Space());

			mainLayout.views.add(new Space(10));

			mainLayout.views.add(switchLayout);
			_view = mainLayout;
		}

		public bool active
		{
			get;
			private set;
		}

		private Button _button(string text, int delta)
		{
			Button button = new Button(text);
			button.area.widthExpandable = false;
			button.area.heightExpandable = false;
			button.area.width = 25;
			button.area.height = 25;

			button.font.style = FontStyle.Bold;

			button.onClick = ((b, m) => { value = value + delta; updateValue(); });

			return button;
		}

		public View view
		{
			get
			{
				return _view;
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
			content.views.add(new AxisController("YAW", true).view);
			content.views.add(new Space(10));
			content.views.add(new AxisController("PITCH", false).view);
			content.views.add(new Space(10));
			content.views.add(new AxisController("ROLL", true).view);
			content.views.add(new Space(10));

			content.views.add(makeOptionsLayout());
		}

		private View makeOptionsLayout()
		{
			Layout optionsLayout = Layout.makeHorizontal();
			
			optionsLayout.views.add(new Button("OFF"));

			optionsLayout.views.add(new Space());

			optionsLayout.views.add(new Button("SIM"));

			optionsLayout.views.add(new Space());

			var grabInputToggle = new Button("GRAB");
			
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