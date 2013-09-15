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

  timeAtPeriapsis *

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

		public Orbit():this(0, 0, 0, 0, 0, 0, 0, new game.CelestialBody())
		{
		}

		public Vector3 position;
		public Vector3 speed;

		public double timeFrom(double tA)
		{
			if(Math.Abs(eccentricity) < 1e-3)
				CircleOrbit.timeFrom(this, tA);
			else if(eccentricity < 1 - 1e-3)
				EllipticOrbit.timeFrom(this, tA);
			else if(eccentricity < 1 + 1-3)
				ParabolicOrbit.timeFrom(this, tA);
			else
				HyperbolicOrbit.timeFrom(this, tA);

			throw new System.ArgumentException();
		}

		private void _update(Vector3 position, Vector3 speed)
		{
			this.position = position;
			this.speed = speed;

			Vector3 c = speed.cross(position);
			inclination = c.angle(mainBody.rotation.up);

			Vector3 an = c.cross(Vector3.up);

			if(an.equals(Vector3.zero))
			{
				LAN = 0;
				an = Vector3.right;
			}
			else
				LAN = an.angleInPlane(Vector3.right, mainBody.rotation.up) + inverseRotAngle / 180 * Math.PI;

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
		}

		private double inverseRotAngle;

#if UNITY
		public void update(global::Orbit orbit, double time)
        {
			mainBody.update(orbit.referenceBody);
			inverseRotAngle = Planetarium.InverseRotAngle;
            _update(new Vector3(orbit.pos.x, orbit.pos.z, orbit.pos.y),
					new Vector3(orbit.vel.x, orbit.vel.z, orbit.vel.y));
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

			position.serialize(stream);
			speed.serialize(stream);

			stream.write(inverseRotAngle);
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

			position = new Vector3();
			speed = new Vector3();

			position.deserialize(stream);
			speed.deserialize(stream);
			
			inverseRotAngle = stream.readDouble();

			_update(position, speed);
		}
	}
}