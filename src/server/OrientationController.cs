using System;
using System.Collections.Generic;
using bjeb.gui;
using bjeb.game;
using bjeb.math;

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

		private Selector _x;
		private Selector _y;
		private Selector _z;
		private Selector _roll;

		private Label _actLabel;

		private PIDController _controllerX;
		private PIDController _controllerY;
		private PIDController _controllerZ;

		override protected void onSetup(Screen screen)
		{
			window.area.set(300, 500, 400, 150);
			content.views.clear();

			_sensetivity = new Selector(0.1f, 1, 10, "Sensetivity");

			_x = new Selector(-1, -1, 1, "X");
			_y = new Selector(-1, -1, 1, "Y");
			_z = new Selector(-1, -1, 1, "Z");
			_roll = new Selector(0, 0, (float)(2 * Math.PI), "Roll");

			content.views.add(_sensetivity.view);

			content.views.add(_x.view);
			content.views.add(_y.view);
			content.views.add(_z.view);
			content.views.add(_roll.view);

			_actLabel = new Label("");

			content.views.add(_actLabel);

			_controllerX = new PIDController(10000, 0, 800, -1000000, 1000000);
			_controllerY = new PIDController(10000, 0, 800, -1000000, 1000000);
			_controllerZ = new PIDController(10000, 0, 800, -1000000, 1000000);
		}

		Vector3 _act = null;
		private double _lastRoll = 0;

		override protected void onUpdate()
		{
            double rollDelta = Math.Abs(vessel.rotation.roll - _lastRoll);

            if (rollDelta > Math.PI) 
				rollDelta = 2 * Math.PI - rollDelta;

            if (rollDelta > Math.PI / 36)
            {
				_controllerX.reset();
				_controllerY.reset();

                _lastRoll = vessel.rotation.roll;
            }

            double precision = Math.Max(0.5, Math.Min(10.0, Math.Min(vessel.body.torque.x, vessel.body.torque.y) * 20.0 / vessel.body.momentumOfInertia.magnitude));

			Quaternion target = Quaternion.look(vessel.north, vessel.up);
			target = target * Quaternion.makeRoll(_roll.value);

            Quaternion delta = (vessel.rotation.inverse * target).inverse;

            Vector3 err = new Vector3((delta.pitch > Math.PI ? (delta.pitch - 2 * Math.PI) : delta.pitch),
									  (delta.yaw > Math.PI ? (delta.yaw - 2 * Math.PI) : delta.yaw),
									  (delta.roll > Math.PI ? (delta.roll - 2 * Math.PI) : delta.roll));
				

            Vector3 torque = vessel.body.torque;

            Vector3 inertia = vessel.body.angularMomentum.sign * vessel.body.angularMomentum * vessel.body.angularMomentum * (vessel.body.momentumOfInertia * torque).invert;
			err += inertia;
			err *= vessel.body.momentumOfInertia * torque.invert;

            Vector3 newAct = new Vector3(_controllerX.compute(err.x, vessel.body.timeDelta), 
										 _controllerY.compute(err.y, vessel.body.timeDelta),
										 _controllerZ.compute(err.z, vessel.body.timeDelta));

			double limit = Math.Max(-1, Math.Min(1, (err.magnitude * 100 / precision)));

			newAct = newAct.clamp(-limit, limit);

			//			if(_act != null)
			//_act = _act + (newAct - _act) * (vessel.body.timeDelta / 0.1);
			//else
			_act = newAct;

			_actLabel.text = vessel.rootRotation.ToString();	
		}

		private void controlRotation(FlightControl control)
		{
			if(_act == null)
				return;

			control.yaw = (float)_act.y;
			if(Math.Abs(control.yaw) < 0.05)
				control.yaw = 0;

			control.pitch = (float)_act.x;
			control.roll = (float)_act.z;
		}

		override public void onControl(FlightControl control)
		{
			controlRotation(control);

			control.yaw *= _sensetivity.value;
			control.pitch *= _sensetivity.value;
			control.roll *= _sensetivity.value;
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