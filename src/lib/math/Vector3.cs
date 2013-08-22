using System;
using bjeb.net;

namespace bjeb.math
{
	public static class DoubleExtensions
	{
		public static bool equals(this double d1, double d2)
		{
			return Math.Abs(d1 - d2) < 1e-5;
		}
	}

	public class Vector3
	{
		public Vector3(): this(0, 0, 0)
		{
		}

		public Vector3(double x, double y, double z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public Vector3(Vector3 vector): this(vector.x, vector.y, vector.z)
		{
		}

		#if UNITY
        public Vector3(UnityEngine.Vector3 vector):this(vector.x, vector.y, vector.z)
		{
		}
		#endif

		public static Vector3 operator+(Vector3 v1, Vector3 v2)
		{
			return new Vector3(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z);
		}

		public static Vector3 operator-(Vector3 v)
		{
			return new Vector3(-v.x, -v.y, -v.z);
		}

		public static Vector3 operator-(Vector3 v1, Vector3 v2)
		{
			return new Vector3(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z);
		}

		public static Vector3 operator*(Vector3 v, double x)
		{
			return new Vector3(v.x * x, v.y * x, v.z * x);
		}

		public static Vector3 operator*(double x, Vector3 v)
		{
			return v * x;
		}

		public static Vector3 operator/(Vector3 v, double x)
		{
			return new Vector3(v.x / x, v.y / x, v.z / x);
		}

		public bool equals(Vector3 v)
		{
			return x.equals(v.x) && y.equals(v.y) && z.equals(v.z);
		}

		public double x
		{
			get;
			private set;
		}

		public double y
		{
			get;
			private set;
		}

		public double z
		{
			get;
			private set;
		}

		public double this[int index]
		{
			get
			{
				switch(index)
				{
				case 0:
					return x;
				case 1:
					return y;
				case 2:
					return z;
				default:
					throw new System.ArgumentException(index.ToString() + " is out of range for vector");
				}
			}
			set
			{
				switch(index)
				{
				case 0:
					x = value;
					break;
				case 1:
					y = value;
					break;
				case 2:
					z = value;
					break;
				default:
					throw new System.ArgumentException(index.ToString() + " is out of range for vector");
				}
			}
		}

		public double magnitudeSquare
		{
			get
			{
				return x * x + y * y + z * z;
			}
		}

		public double magnitude
		{
			get
			{
				return Math.Sqrt(magnitudeSquare);
			}
		}

		public Vector3 normalize
		{
			get
			{
				double mag = magnitude;

				if(mag == 0)
					throw new System.DivideByZeroException();

				return this / mag;
			}
		}

		public double dot(Vector3 vector)
		{
			return x * vector.x + y * vector.y + z * vector.z;
		}

		public Vector3 cross(Vector3 vector)
		{
			return new Vector3(y * vector.z - z * vector.y, 
							   z * vector.x - x * vector.z, 
							   x * vector.y - y * vector.x);
		}

		public double angle(Vector3 vector)
		{
			return Math.Acos(dot(vector) / magnitude / vector.magnitude);
		}

		public double angleInPlane(Vector3 vector, Vector3 normal)
		{
			Vector3 v1 = exclude(normal);
			Vector3 v2 = vector.exclude(normal);
			
			if(v1.equals(zero))
				return 0;

			if(v2.equals(zero))
				return 0;

			Vector3 rotationNormal = v1.cross(v2);

			if(rotationNormal.equals(zero))
				return Math.Sign(v1.dot(v2)) > 0 ? 0 : Math.PI;

			return v1.angle(v2) * Math.Sign(v1.cross(v2).dot(normal));
		}

		public Vector3 project(Vector3 vector)
		{
			return vector * dot(vector) / vector.magnitudeSquare;
		}

		public Vector3 exclude(Vector3 vector)
		{
			return this - project(vector);
		}

#if UNITY
		public UnityEngine.Vector3 unity
		{
			get
			{
				return new UnityEngine.Vector3((float)x, (float)y, (float)z);
			}
		}
#endif

		public static Vector3 zero
		{
			get
			{
				return new Vector3(0, 0, 0);
			}
		}

		public static Vector3 forward
		{
			get
			{ 
				return new Vector3(0, 0, 1);
			}
		}

		public static Vector3 backward
		{
			get
			{ 
				return new Vector3(0, 0, 1);
			}
		}

		public static Vector3 up
		{
			get
			{ 
				return new Vector3(0, 1, 0);
			}
		}

		public static Vector3 left
		{
			get
			{ 
				return new Vector3(-1, 0, 0);
			}
		}

		public static Vector3 right
		{
			get
			{ 
				return new Vector3(1, 0, 0);
			}
		}
		
		public static Vector3 one
		{
			get
			{
				return new Vector3(1, 1, 1);
			}
		}

		public override string ToString()
		{
			return "(x: " + x.ToString("F2") + " y: " + y.ToString("F2") + " z: " + z.ToString("F2") + ")";
		}

		public void serialize(string name, XmlNode node)
		{
			XmlNode child = node.node(name);

			if(child == null)
				child = new XmlNode(name, node);

			child.attribute("x").set(x);
			child.attribute("y").set(y);
			child.attribute("z").set(z);
		}

		public void deserialize(string name, XmlNode node)
		{
			XmlNode child = node.node(name);

			x = child.attribute("x").getDouble();
			y = child.attribute("y").getDouble();
			z = child.attribute("z").getDouble();
		}
	}

    class Matrix3x3
    {
        private double[,] _elements = new double[3, 3];

        public double this[int i, int j] 
        {
            get { return _elements[i, j]; }
            set { _elements[i, j] = value; }
        }

        public static Vector3 operator*(Matrix3x3 M, Vector3 v) 
        {
            Vector3 ret = Vector3.zero;

            for(int i = 0; i < 3; i++)
			{
                for(int j = 0; j < 3; j++)
				{
                    ret[i] += M[i, j] * v[j];
                }
            }

            return ret;
        }
    }
}