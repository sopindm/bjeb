using bjeb.net;

namespace bjeb.game
{
	[Serializable("flightControl")]
	public class FlightControl: Serializable
	{
		public FlightControl()
		{
		}

		public float throttle
		{
			get;
			set;
		}

		public float yaw;
		public float pitch;
		public float roll;

		public float dx
		{
			get;
			set;
		}

		public float dy
		{
			get;
			set;
		}

		public float dz
		{
			get;
			set;
		}

		public FlightControl copy()
		{
			FlightControl ret = new FlightControl();

			ret.throttle = throttle;

			ret.yaw = yaw;
			ret.pitch = pitch;
			ret.roll = roll;

			ret.dx = dx;
			ret.dy = dy;
			ret.dz = dz;

			return ret;
		} 

#if UNITY
		public void update(global::FlightCtrlState s)
		{
			throttle = s.mainThrottle;

			yaw = s.yaw;
			pitch = s.pitch;
			roll = s.roll;

			dx = s.X;
			dy = s.Y;
			dz = s.Z;
		}

		public void apply(global::FlightCtrlState s)
		{
			s.mainThrottle = throttle;

			s.yaw = yaw;
			s.pitch = pitch;
			s.roll = roll;

			s.X = dx;
			s.Y = dy;
			s.Z = dz;
		}
#endif

		override protected void doSerialize(Stream stream)
		{
			stream.write(throttle);

			stream.write(yaw);
			stream.write(pitch);
			stream.write(roll);

			stream.write(dx);
			stream.write(dy);
			stream.write(dz);
		}

		override protected void doDeserialize(Stream stream)
		{
			throttle = stream.readFloat();

			yaw = stream.readFloat();
			pitch = stream.readFloat();
			roll = stream.readFloat();

			dx = stream.readFloat();
			dy = stream.readFloat();
			dz = stream.readFloat();
		}
	}
}