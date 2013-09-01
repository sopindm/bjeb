using System;

namespace bjeb
{
    class PIDController
    {
		public double Kp
		{
			get;
			private set;
		}

		public double Ki
		{
			get;
			private set;
		}

		public double Kd
		{
			get;
			private set;
		}

		private double _lastError;
		private double _intAccum;

		private double _min;
		private double _max;

        public PIDController(double Kp, double Ki, double Kd, double max = double.MaxValue, double min = double.MinValue)
        {
            this.Kp = Kp;
            this.Ki = Ki;
            this.Kd = Kd;
            this._max = max;
            this._min = min;

            reset();
        }

        public double compute(double error, double dT)
        {
            _intAccum += error * dT;

            double action = (Kp * error) + (Ki * _intAccum) + (Kd * (error - _lastError) * dT);
            double clamped = Math.Max(_min, Math.Min(_max, action));
            if (clamped != action)
            {
                _intAccum -= error * dT;
            }

            _lastError = error;

            return action;
        }

        public void reset()
        {
			_lastError = 0;
			_intAccum = 0;
        }
    }
}