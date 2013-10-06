using System;
using bjeb.net;
using bjeb.game;

/*
  radius
  speed

  position
  velocity

  apopsisRadius
  periapsisRadius
  apopsisAltitude
  periapsisAltitude

  declension

  timeTo(trueA)
  timeFrom(trueA)

  Constructors:
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

		public game.CelestialBody mainBody
		{
			get;
			private set;
		}

		public game.Universe universe
		{
			get
			{
				return mainBody.universe;
			}
		}

		public Orbit(double trueAnomaly, double eccentricity, double parameter, double inclination, double LAN, double argumentOfPeriapsis, double timeAtPeriapsis, game.CelestialBody mainBody)
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

		public Orbit(Universe universe): this(0, 0, 0, 0, 0, 0, 0, new game.CelestialBody(universe))
		{
		}

		private enum _Type
		{
			Circular,
			Elliptic,
			Parabolic,
			Hyperbolic
		}

		private _Type _type
		{
			get
			{
				if(Math.Abs(eccentricity) < 1e-3)
					return _Type.Circular;
				else if(eccentricity < 1 - 1e-3)
					return _Type.Elliptic;
				else if(eccentricity < 1 + 1e-3)
					return _Type.Parabolic;
				else 
					return _Type.Hyperbolic;
			}
		}

		public double semimajorAxis
		{
			get
			{
				switch(_type)
				{
				case _Type.Circular:
					return CircularOrbit.semimajorAxis(this);
				case _Type.Elliptic:
					return EllipticOrbit.semimajorAxis(this);
				case _Type.Parabolic:
					return ParabolicOrbit.semimajorAxis(this);
				case _Type.Hyperbolic:
					return HyperbolicOrbit.semimajorAxis(this);
				}

				throw new ArgumentException();
			}
		}
			
		public double period
		{
			get
			{
				if(_type != _Type.Circular && _type != _Type.Elliptic)
					throw new ArgumentException();

				return 2 * Math.PI * Math.Pow(semimajorAxis, 1.5) / mainBody.gravParameter.sqrt();
			}
		}

		public double time
		{
			get
			{
				return timeAtPeriapsis + timeFromPeriapsis;
			}
		}

		public double timeFromPeriapsis
		{
			get
			{
				double value = 0;

				switch(_type)
				{
				case _Type.Circular:
					value = CircularOrbit.timeFromPeriapsis(this);
					break;
				case _Type.Elliptic:
					value = EllipticOrbit.timeFromPeriapsis(this);
					break;
				case _Type.Parabolic:
					value = ParabolicOrbit.timeFromPeriapsis(this);
					break;
				case _Type.Hyperbolic:
					value = HyperbolicOrbit.timeFromPeriapsis(this);
					break;
				default:
					throw new ArgumentException();
				}

				if(value < 0)
					return period + value;

				return value;
			}
		}

		public double timeToPeriapsis
		{
			get
			{
				return period - timeFromPeriapsis;
			}
		}

		private void _update(Vector3 position, Vector3 speed, double time)
		{
			Vector3 c = speed.cross(position);
			inclination = c.angle(mainBody.rotation.up);

			Vector3 an = c.cross(Vector3.up);

			if(an.equals(Vector3.zero))
			{
				LAN = 0;
				an = Vector3.right;
			}
			else
				LAN = an.angleInPlane(Vector3.right, mainBody.rotation.up) + universe.inverseRotation / 180 * Math.PI;

			parameter = c.magnitudeSquare / mainBody.gravParameter;

			double h = speed.magnitudeSquare - 2 * mainBody.gravParameter / position.magnitude;

			eccentricity = (1 + h * parameter / mainBody.gravParameter).sqrt();

			double vRadial = speed.dot(position.normalize);
			double vNormal = speed.exclude(position).magnitude;

			if(Math.Abs(eccentricity) > 1e-3)
			{
				double taSin = vRadial / eccentricity * (parameter / mainBody.gravParameter).sqrt();
				double taCos = 1 / eccentricity * (vNormal * (parameter / mainBody.gravParameter).sqrt() - 1);

				trueAnomaly = Math.Atan2(taSin, taCos);
			}
			else 
				trueAnomaly = position.angleInPlane(an, c);

			if(trueAnomaly < 0)
				trueAnomaly += 2 * Math.PI;

			argumentOfPeriapsis = position.angleInPlane(an, c);
			if(argumentOfPeriapsis < 0)
				argumentOfPeriapsis += 2 * Math.PI;

			argumentOfPeriapsis -= trueAnomaly;

			timeAtPeriapsis = time - timeFromPeriapsis;
		}

#if UNITY
		public void update(global::Orbit orbit, double time)
        {
			mainBody.update(orbit.referenceBody);

			inclination = orbit.inclination * Math.PI / 180;
			LAN = orbit.LAN * Math.PI / 180;

			eccentricity = orbit.eccentricity;
			parameter = new Vector3(orbit.h).magnitudeSquare / mainBody.gravParameter;

			argumentOfPeriapsis = orbit.argumentOfPeriapsis * Math.PI / 180;
			trueAnomaly = orbit.trueAnomaly * Math.PI / 180;

			timeAtPeriapsis = time - timeFromPeriapsis;
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