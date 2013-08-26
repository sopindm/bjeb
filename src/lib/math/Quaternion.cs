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

		public Quaternion()
		{
			w = 0;
			v = new Vector3();
		}

		public Quaternion(double w, Vector3 v)
		{
			this.w = w;
			this.v = v;
		}

		#if UNITY
		public Quaternion(UnityEngine.Quaternion q)
		{
			w = q.w;

			v = new Vector3(q.x, q.y, q.z);
		}
		#endif

		public static Quaternion identity
		{
			get
			{
				return new Quaternion(1, new Vector3(0, 0, 0));
			}
		}

		public Vector3 forward
		{
			get
			{
				return this * Vector3.forward;
			}
		}

		public Vector3 up
		{
			get
			{
				return this * Vector3.up;
			}
		}

		public Vector3 right
		{
			get
			{
				return this * Vector3.right;
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

		public void serialize(string name, net.XmlNode node)
		{
			net.XmlNode child = node.node(name);

			if(child == null)
				child = new net.XmlNode(name, node);

			child.attribute("w").set(w);
			v.serialize(name, node);
		}

		public void deserialize(string name, net.XmlNode node)
		{
			w = node.node(name).attribute("w").getDouble();
			v.deserialize(name, node);
		}

		public static Quaternion makePitch(double angle)
		{
			return angleAxis(angle, Vector3.right);
		}

		public double yaw
		{
			get
			{
				double ret = Vector3.right.angleInPlane(Vector3.up.cross(this * Vector3.forward), Vector3.up);

				if(ret < 0)
					ret = 2 * Math.PI + ret;

				return ret;
			}
		}

		public double pitch
		{
			get
			{
				Quaternion yawTransform = Quaternion.angleAxis(yaw, Vector3.up);

				Vector3 forward1 = yawTransform * Vector3.forward;
				Vector3 right1 = yawTransform * Vector3.right;

				double ret = forward1.angleInPlane(this * Vector3.forward, right1);
				return -ret;
			}
		}

		public double roll
		{
			get
			{
				Vector3 up2 = Quaternion.angleAxis(yaw, Vector3.up) * Quaternion.angleAxis(-pitch, Vector3.right) * Vector3.up;

				return -up2.angleInPlane(this * Vector3.up, this * Vector3.forward);
			}
		}
	}
}