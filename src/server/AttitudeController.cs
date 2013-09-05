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

	class AttitudeController: Module
	{
		public AttitudeController(Computer computer): base(computer)
		{
		}

		private Selector _sensetivity;
		private Selector _inertia;

		private PIDControllerV _controller;

		public Quaternion target
		{
			get;
			set;
		}

		public enum Control
		{
			No,
			KillRotation,
			Full
		}

		public Control controlYaw
		{
			get;
			set;
		}

		public Control controlPitch
		{
			get;
			set;
		}

		public Control controlRoll
		{
			get;
			set;
		}

		override protected void onSetup(Screen screen)
		{
			window.area.set(300, 500, 400, 150);
			content.views.clear();

			_sensetivity = new Selector(0.1f, 1, 10, "Sensetivity");
			_inertia = new Selector(1, 1, 10, "Inertia");

			content.views.add(_sensetivity.view);

			_controller = new PIDControllerV(10000, 0, 800, -1000000, 1000000);
			
			target = Quaternion.identity;

			controlYaw = Control.No;
			controlPitch = Control.No;
			controlRoll = Control.No;
		}

		override protected void onUpdate()
		{
		}

		private Vector3 _act = Vector3.zero;
		private double _lastRoll = 0;

        private const double Tf = 0.1;
        private const double driveFactor = 100000;

        private void drive(FlightControl c)
        {
            double rollDelta = Math.Abs(vessel.rotation.roll - _lastRoll);
            if (rollDelta > Math.PI)
                rollDelta = 2 * Math.PI - rollDelta;
            if (rollDelta > Math.PI / 36)
            {
                _controller.reset();
                _lastRoll = vessel.rotation.roll;
            }

			Quaternion delta = vessel.rotation.inverse * target;

            Vector3 err = new Vector3((delta.pitch > Math.PI) ? (delta.pitch - 2 * Math.PI) : delta.pitch,
									  (delta.yaw > Math.PI) ? (delta.yaw - 2 * Math.PI) : delta.yaw,
									  (delta.roll > Math.PI) ? (delta.roll - 2 * Math.PI) : delta.roll);

			err = new Vector3(controlPitch == Control.KillRotation ? 0 : err.x,
							  controlYaw == Control.KillRotation ? 0 : err.y,
							  controlRoll == Control.KillRotation ? 0 : err.z);

            Vector3 torque = vessel.body.torque;

            Vector3 inertia = vessel.body.angularMomentum.sign *
				vessel.body.angularMomentum * vessel.body.angularMomentum * 
				(torque * vessel.body.momentumOfInertia).invert;

            err += inertia;
			err *= vessel.body.momentumOfInertia * torque.invert;

            Vector3 act = _controller.compute(err, vessel.body.timeDelta);

            double precision = (Math.Min(vessel.body.torque.x, vessel.body.torque.y) * 20.0 / vessel.body.momentumOfInertia.magnitude).clamp(0.5, 10);
            double driveLimit = (err.magnitude * driveFactor / precision).clamp(0, 1);

			act = act.clamp(-driveLimit, driveLimit);

            act = _act + (act - _act) * (vessel.body.timeDelta / Tf);

            setControls(act, c, driveLimit);

            _act = new Vector3(c.pitch, c.yaw, c.roll);
        }

		private void control(double act, double limit, Control control, ref float value)
		{
			if(control == Control.Full || 
			   (control == Control.KillRotation && Math.Abs(value) < 0.1))
			{
				if (!double.IsNaN(act)) 
					value = (float)act.clamp(-limit, limit);

				if (Math.Abs(value) < 0.05)
					value = 0;
			}
		}

        private void setControls(Vector3 act, FlightControl c, double driveLimit)
        {
			control(act.z, driveLimit, controlRoll, ref c.roll);
			control(act.x, driveLimit, controlPitch, ref c.pitch);
			control(act.y, driveLimit, controlYaw, ref c.yaw);
        }

		override public void onControl(FlightControl control)
		{
			drive(control);

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