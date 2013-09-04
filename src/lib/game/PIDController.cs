using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using bjeb.math;

namespace bjeb
{
    public class PIDController
    {
		private double _lastError;
		private double _intAccum;

		private double Kp, Ki, Kd;
		private double _min, _max;

        public PIDController(double Kp, double Ki, double Kd, double max = double.MaxValue, double min = double.MinValue)
        {
            this.Kp = Kp;
            this.Ki = Ki;
            this.Kd = Kd;
            this._max = max;
            this._min = min;

            reset();
        }

        public double compute(double error, double delta)
        {
            _intAccum += error * delta;

            double action = (Kp * error) + (Ki * _intAccum) + (Kd * (error - _lastError) / delta);

            double clamped = action.clamp(_min, _max);
            if (clamped != action)
                _intAccum -= error *delta;

            _lastError = error;

            return action;
        }

        public void reset()
        {
			_lastError = 0;
			_intAccum = 0;
        }
    }

    public class PIDControllerV
    {
		private Vector3 _lastError;
		private Vector3 _intAccum;

		private double Kp, Ki, Kd;
		private double _min, _max;

        public PIDControllerV(double Kp, double Ki, double Kd, double max = double.MaxValue, double min = double.MinValue)
        {
            this.Kp = Kp;
            this.Ki = Ki;
            this.Kd = Kd;

            this._min = min;
            this._max = max;


            reset();
        }

        public Vector3 compute(Vector3 error, double delta)
        {
            _intAccum += error * delta;

            Vector3 action = (Kp * error) + (Ki * _intAccum) + (Kd * (error - _lastError) / delta);

            Vector3 clamped = action.clamp(_min, _max);
            if(!clamped.equals(action))
            {
                _intAccum -= error * delta;
            }

            _lastError = error;

            return action;
        }

        public void reset()
        {
			_lastError = Vector3.zero;
			_intAccum = Vector3.zero;
        }
    }
}
