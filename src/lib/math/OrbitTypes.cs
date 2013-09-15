using System;

namespace bjeb.math
{
	class CircularOrbit
	{
		public static double timeFromPeriapsis(Orbit orbit)
		{
			throw new System.NotImplementedException();
		}

		public static double semimajorAxis(Orbit orbit)
		{
			throw new System.NotImplementedException();
		}
	}

	class EllipticOrbit
	{
		private static double eccentricAnomaly(Orbit orbit)
		{
			return 2 * Math.Atan(((1 - orbit.eccentricity) / (1 + orbit.eccentricity)).sqrt() * Math.Tan(orbit.trueAnomaly / 2));
		}

		public static double timeFromPeriapsis(Orbit orbit)
		{
			double ea = eccentricAnomaly(orbit);

			return Math.Pow(orbit.semimajorAxis, 1.5) / orbit.mainBody.gravParameter.sqrt() * (ea - orbit.eccentricity * Math.Sin(ea));
		}

		public static double semimajorAxis(Orbit orbit)
		{
			return orbit.parameter / (1 - orbit.eccentricity.sqr());
		}
	}

	class ParabolicOrbit
	{
		public static double timeFromPeriapsis(Orbit orbit)
		{
			throw new System.NotImplementedException();
		}

		public static double semimajorAxis(Orbit orbit)
		{
			throw new System.NotImplementedException();
		}
	}

	class HyperbolicOrbit
	{
		public static double timeFromPeriapsis(Orbit orbit)
		{
			throw new System.NotImplementedException();
		}

		public static double semimajorAxis(Orbit orbit)
		{
			throw new System.NotImplementedException();
		}
	}
}