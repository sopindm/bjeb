using System;
using System.Collections.Generic;
using bjeb.gui;
using bjeb.math;

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
				if(_value >= 180)
					_value = 270 + (270 - _value);
				else
					_value = 90 - (_value - 90);
			}
			else
			{
				if(_value >= 0)
					_value = 90 + (90 - _value);
				else
					_value = 270 - (_value - 270);
			}

			updateValue();
		}

		private int _value;

		public double value
		{
			get
			{
				return _value * Math.PI / 180;
			}
		}

		private float sliderValue()
		{
			if(onFrontSide)
				return _value > 90 ? _value - 360 : _value;
			else
				return _value - 180;
		}

		private void updateSliderValue(float svalue)
		{
			_value = onFrontSide ? (int)svalue : (int)svalue + 180;
			updateValue();
		}

		private void updateValue()
		{
			if(_fullCircle)
			{
				if(_value < 0)
					_value += 360;
				if(_value >= 360)
					_value -= 360;
			}
			
			if(!_fullCircle)
			{
				if(_value > 90)
					_value = 90;
				else if(_value < -90)
					_value = -90;
			}

			if((_value > 90 && _value < 270) && onFrontSide)
			{
				_onFrontSide = false;
				_sideSwitch.text = "-";
			}
			else if ((_value > 270 || _value < 90) && !onFrontSide)
			{
				_onFrontSide = true;
				_sideSwitch.text = "+";
			}

			_slider.value = sliderValue();
			_info.text = _value.ToString("F0"); 
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

			button.onClick = ((b, m) => { _value = _value + delta; updateValue(); });

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

		private AxisController _yaw, _pitch, _roll;

		override protected void onSetup(Screen screen)
		{
			window.area.set(0, 200, 400, 200);
			content.views.clear();

			Layout referenceLayout = Layout.makeHorizontal();

			referenceLayout.views.add(new Button("ORB"));
			referenceLayout.views.add(new Button("SUR"));
			referenceLayout.views.add(new Button("TRG"));

			active = false;

			_yaw = new AxisController("YAW", true);
			_pitch = new AxisController("PITCH", false);
			_roll = new AxisController("ROLL", true);

			content.views.add(referenceLayout);
			content.views.add(new Space(10));
			content.views.add(_yaw.view);
			content.views.add(new Space(10));
			content.views.add(_pitch.view);
			content.views.add(new Space(10));
			content.views.add(_roll.view);
			content.views.add(new Space(10));

			content.views.add(makeOptionsLayout());
		}

		private bool active = false;

		private Toggle switchToggle = null;

		private void deactivate()
		{
			active = false;
			switchToggle.text = "OFF";
		}

		private void activate()
		{
			active = true;
			switchToggle.text = "ON";
		}

		private View makeOptionsLayout()
		{
			Layout optionsLayout = Layout.makeHorizontal();
			
			switchToggle = new Toggle("OFF", false);
			switchToggle.onSwitch = (t =>
					{
						if(active)
							deactivate();
						else
							activate();
					});

			optionsLayout.views.add(switchToggle);

			optionsLayout.views.add(new Space());

			optionsLayout.views.add(new Button("SIM"));

			optionsLayout.views.add(new Space());

			var grabInputToggle = new Button("GRAB");
			
			optionsLayout.views.add(grabInputToggle);

			return optionsLayout;
		}

		override protected void onUpdate()
		{
			if(!active)
			{
				computer.attitude.controlYaw = AttitudeController.Control.No;
				computer.attitude.controlPitch = AttitudeController.Control.No;
				computer.attitude.controlRoll = AttitudeController.Control.No;

				return;
			}

			computer.attitude.target = Quaternion.look(vessel.north, vessel.up);

			if(_yaw.active)
			{
				computer.attitude.target *= Quaternion.makeYaw(_yaw.value);
				computer.attitude.controlYaw = AttitudeController.Control.Full;
			}

			if(_pitch.active)
			{
				computer.attitude.target *= Quaternion.makePitch(_pitch.value);
				computer.attitude.controlPitch = AttitudeController.Control.Full;
			}

			if(_roll.active)
			{
				computer.attitude.target *= Quaternion.makeRoll(_roll.value);
				computer.attitude.controlRoll = AttitudeController.Control.Full;
			}
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