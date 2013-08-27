using System;

namespace bjeb.util
{
	public class Timer
	{
		private DateTime _startTime;

		public Timer()
		{
			_startTime = DateTime.Now;
			_frames = 0;
			_rate = 0;

			updateTime = 1;
		}

		public double updateTime
		{
			get;
			set;
		}

		public double elapsed
		{
			get
			{
				return (DateTime.Now - _startTime).TotalMilliseconds / 1000;
			}
		}

		private int _frames;
		private double _rate;

		public double rate
		{
			get
			{
				return _rate;
			}
		}

		private void reset()
		{
			_rate = _frames / elapsed;

			_startTime = DateTime.Now;
			_frames = 0;
		}

		public void update()
		{
			_frames++;

			if(elapsed > updateTime)
				reset();
		}
	}
}
