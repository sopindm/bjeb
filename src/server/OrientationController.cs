using System.Collections.Generic;
using bjeb.gui;
using bjeb.game;

namespace bjeb
{
	class Selector
	{
		private float _minValue;
		private float _midValue;
		private float _maxValue;

		private string _name;

		public float value
		{
			get;
			private set;
		}

		public Selector(float minValue, float midValue, float maxValue, string name)
		{
			_minValue = minValue;
			_midValue = midValue;
			_maxValue = maxValue;
			_name = name;
		}

		public View view
		{
			get
			{
				Layout main = Layout.makeVertical();

				Layout info = Layout.makeHorizontal();

				Label nameLabel = new Label(_name);
				nameLabel.area.width = 70;

				Label valueLabel = new Label(_midValue.ToString("F2"));

				info.views.add(nameLabel);
				info.views.add(new Space());
				info.views.add(valueLabel);
				info.views.add(new Space(50));
				info.views.add(new Space());

				main.views.add(info);

				Slider valueSlider;

				if(_minValue < _midValue)
					valueSlider = new Slider(0, 20, 10);
				else
					valueSlider = new Slider(10, 20, 10);

				value = _midValue;

				valueSlider.onUpdate = (s => 
						{
							if(s.value <= 10 )
								value = _minValue * (1 - s.value / 10) + _midValue * (s.value / 10);
							else
								value = _midValue * (1 - (s.value - 10) / 10) + _maxValue * (s.value - 10) / 10;

							if(value > 0)
								valueLabel.text = value.ToString("F2");
							else
								valueLabel.text = value.ToString("F3");
						});

				main.views.add(valueSlider);

				return main;
			}
		}
	}

	class OrientationController: Module
	{
		public OrientationController(Computer computer): base(computer)
		{
		}

		private Selector _sensetivity;
		private Selector _inertia;

		override protected void onSetup(Screen screen)
		{
			window.area.set(300, 500, 400, 150);
			content.views.clear();

			_sensetivity = new Selector(0.1f, 1, 10, "Sensetivity");
			_inertia = new Selector(1, 1, 10, "Inertia");

			content.views.add(_sensetivity.view);
			content.views.add(_inertia.view);
			content.views.add(new Selector(-1, -1, 1, "X").view);
			content.views.add(new Selector(-1, -1, 1, "Y").view);
			content.views.add(new Selector(-1, -1, 1, "Z").view);

			_lastControl = null;
		}

		override protected void onUpdate()
		{
		}

		private FlightControl _lastControl = null;

		override public void onControl(FlightControl control)
		{
			control.yaw *= _sensetivity.value;
			control.pitch *= _sensetivity.value;
			control.roll *= _sensetivity.value;

			if(_lastControl != null)
			{
				float a = 1 / (_inertia.value * _inertia.value);
				a = a * a;

				control.yaw = control.yaw * a + _lastControl.yaw * (1 - a);
				control.pitch = control.pitch * a + _lastControl.pitch * (1 - a);
				control.roll = control.roll * a + _lastControl.roll * (1 - a);
			}

			_lastControl = control.copy();
		}

		override public string name
		{
			get
			{
				return "OrientationController";
			}
		}
	}
}