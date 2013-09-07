using bjeb.net;
using bjeb.game;

/*
  trueAnomaly *
  
  eccentricity *
  parameter *

  radius
  speed

  position
  velocity

  apopsisRadius
  periapsisRadius
  apopsisAltitude
  periapsisAltitude

  timeAtPeriapsis *

  inclination *
  declension

  LAN *

  timeTo(trueA)
  timeFrom(trueA)

  Constructors:
    main parameters
	position and velocity
 */


namespace bjeb.math
{
	[net.Serializable("orbit")]
	public class Orbit: net.Serializable
	{
		public double trueAnomaly
		{
			get;
			private set;
		}

		public double eccentricity
		{
			get;
			private set;
		}

		public double parameter
		{
			get;
			private set;
		}

		public double inclination
		{
			get;
			private set;
		}

		public double LAN
		{
			get;
			private set;
		}

		public double argumentOfPeriapsis
		{
			get;
			private set;
		}

		public double timeAtPeriapsis
		{
			get;
			private set;
		}

		public CelestialBody mainBody
		{
			get;
			private set;
		}

		public Orbit(double trueAnomaly, double eccentricity, double parameter, double inclination, double LAN, double argumentOfPeriapsis, double timeAtPeriapsis, CelestialBody mainBody)
		{
			this.trueAnomaly = trueAnomaly;
			this.eccentricity = eccentricity;
			this.parameter = parameter;
			this.inclination = inclination;
			this.LAN = LAN;
			this.argumentOfPeriapsis = argumentOfPeriapsis;
			this.timeAtPeriapsis = timeAtPeriapsis;
			this.mainBody = mainBody;
		}

		public Orbit(CelestialBody mainBody):this(0, 0, 0, 0, 0, 0, 0, mainBody)
		{
		}

#if UNITY
		public void update(global::Orbit orbit, double time)
		{
			
		}
#endif

		override protected void doSerialize(Stream stream)
		{
			stream.write(trueAnomaly);
			stream.write(eccentricity);
			stream.write(parameter);
			stream.write(inclination);
			stream.write(LAN);
			stream.write(argumentOfPeriapsis);
			stream.write(timeAtPeriapsis);
			mainBody.serialize(stream);
		}

		override protected void doDeserialize(Stream stream)
		{
			trueAnomaly = stream.readDouble();
			eccentricity = stream.readDouble();
			parameter = stream.readDouble();
			inclination = stream.readDouble();
			LAN = stream.readDouble();
			argumentOfPeriapsis = stream.readDouble();
			timeAtPeriapsis = stream.readDouble();
			mainBody.deserialize(stream);
		}
	}
}