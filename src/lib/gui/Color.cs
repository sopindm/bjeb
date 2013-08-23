using bjeb.net;

namespace bjeb.gui
{
	public class Color
	{
		public float a
		{
			get;
			private set;
		}

		public float r
		{
			get;
			private set;
		}

		public float g
		{
			get;
			private set;
		}

		public float b
		{
			get;
			private set;
		}

		public Color(): this(0, 0, 0, 0)
		{
		}

		public Color(float r, float g, float b): this(1, r, g, b)
		{
		}

		public Color(float a, float r, float g, float b)
		{
			this.a = a;
			this.r = r;
			this.g = g;
			this.b = g;
		}

		public Color(double r, double g, double b): this(1, r, g, b)
		{
		}

		public Color(double a, double r, double g, double b): this((float)a, (float)r, (float)g, (float)b)
		{
		}

		public void serialize(string name, XmlNode node)
		{
			XmlNode myNode = node.node(name);

			if(myNode == null)
				myNode = new XmlNode(name, node);

			myNode.attribute("a").set(a);
			myNode.attribute("r").set(r);
			myNode.attribute("g").set(g);
			myNode.attribute("b").set(b);
		}

		public void deserialize(string name, XmlNode node)
		{
			XmlNode myNode = node.node(name);

			a = myNode.attribute("a").getFloat();
			r = myNode.attribute("r").getFloat();
			g = myNode.attribute("g").getFloat();
			b = myNode.attribute("b").getFloat();
		}

		public static Color create(string name, XmlNode node)
		{
			XmlNode myNode = node.node(name);

			if(myNode == null)
				return null;

			Color ret = new Color();
			ret.deserialize(name, node);

			return ret;
		}

		public static Color black
		{
			get
			{
				return new Color(1, 0, 0, 0);
			}
		}

		public static Color blue
		{
			get
			{
				return new Color(1, 0, 0, 1);
			}
		}

		public static Color clear
		{
			get
			{
				return new Color(0, 0, 0, 0);
			}
		}

		public static Color cyan
		{
			get
			{
				return new Color(1, 0, 1, 1);
			}
		}

		public static Color gray
		{
			get
			{
				return new Color(1, 0.5, 0.5, 0.5);
			}
		}

		public static Color green
		{
			get
			{
				return new Color(1, 0, 1, 0);
			}
		}

		public static Color magenta
		{
			get
			{
				return new Color(1, 1, 0, 1);
			}
		}

		public static Color red
		{
			get
			{
				return new Color(1, 1, 0, 0);
			}
		}

		public static Color white
		{
			get
			{
				return new Color(1, 1, 1, 1);
			}
		}

		public static Color yellow
		{
			get
			{
				return new Color(1, 1, 0.92, 0.016);
			}
		}

#if UNITY
		public UnityEngine.Color unity
		{
			get
			{
				return new UnityEngine.Color(r, g, b, a);
			}
		}
#endif
	}
}