namespace bjeb.net
{
	public class UpdateHandler
	{
		private float? _updateRate;

		public float updateRate
		{
			get
			{
				return _updateRate.Value;
			}
			set
			{
				_updateRate = value;
			}
		}

		System.DateTime? _start;
		bool _updated = false;

		public UpdateHandler()
		{
			_updateRate = null;
			_start = null;
		}

		public UpdateHandler(float rate)
		{
			updateRate = rate;
			_start = null;
		}

		public bool update()
		{
			if(_updateRate == null)
			{
				if(_updated)
				{
					_updated = false;
					return true;
				}

				return false;
			}

			System.DateTime newTime = System.DateTime.Now;

			if(_start == null)
			{
				_start = newTime;
				return true;
			}

			if((newTime - _start.Value).TotalMilliseconds > (1000 / updateRate))
			{
				_start = newTime;
				return true;
			}

			return false;
		}

		public void trigger()
		{
			_updated = true;
		}
	}
}