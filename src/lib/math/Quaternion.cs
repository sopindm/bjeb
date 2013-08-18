using System;

namespace bjeb.math
{
	public class Quaternion
	{
		public double w
		{
			get;
			private set;
		}

		public Vector3 v
		{
			get;
			private set;
		}

		public Quaternion(double w, Vector3 v)
		{
			this.w = w;
			this.v = v;
		}

		public static Quaternion identity
		{
			get
			{
				return new Quaternion(1, new Vector3(0, 0, 0));
			}
		}

		public double magnitudeSquare
		{
			get
			{
				return w * w + v.magnitudeSquare;
			}
		}

		public double magnitude
		{
			get
			{
				return Math.Sqrt(magnitudeSquare);
			}
		}

		public override string ToString()
		{
			return "(" + w.ToString("F2") + " " + v.ToString() + ")";
		}

		#if UNITY
		public UnityEngine.Quaternion unity
		{
			get
			{
				return new UnityEngine.Quaternion((float)v.x, (float)v.y, (float)v.z, (float)w);
			}
		}
		#endif

		public static Quaternion operator*(Quaternion q, double s)
		{
			return new Quaternion(q.w * s, q.v * s);
		}

		public static Quaternion operator/(Quaternion q, double s)
		{
			return new Quaternion(q.w / s, q.v / s);
		}

		public Quaternion inverse
		{
			get
			{
				return new Quaternion(w, -v) / magnitudeSquare;
			}
		}

		public static Quaternion operator*(Quaternion q1, Quaternion q2)
		{
			return new Quaternion(q1.w * q2.w - q1.v.dot(q2.v),
								  q1.w * q2.v + q2.w * q1.v + q1.v.cross(q2.v));
		}

		public static Vector3 operator*(Quaternion q, Vector3 v)
		{
			return (q * new Quaternion(0, v) * q.inverse).v;
		}

		public static Quaternion angleAxis(double angle, Vector3 axis)
		{
			double s = Math.Sin(angle / 2);

			return new Quaternion(Math.Cos(angle / 2), axis.normalize * s);
		}

		private static Quaternion _look(Vector3 from, Vector3 to)
		{
			if(from.equals(to))
				return identity;

			return angleAxis(from.angle(to), from.cross(to));
		}

		public static Quaternion look(Vector3 forward)
		{
			return _look(Vector3.forward, forward);
		}

		public static Quaternion look(Vector3 forward, Vector3 upward)
		{
			Quaternion look1 = look(forward);

			return _look(look1 * Vector3.up, upward) * look1;
		}
	}
}