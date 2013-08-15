namespace bjeb.gui
{
	public class Area
	{
		public float? x
		{
			get;
			set;
		}

		public float? y
		{
			get;
			set;
		}

		public float? width
		{
			get;
			set;
		}

		public float? height
		{
			get;
			set;
		}

		public void set(float x, float y, float width, float height)
		{
			this.x = x;
			this.y = y;
			this.width = width;
			this.height = height;
		}

		public bool isSet()
		{
			return x != null && y != null && width != null && height != null;
		}

#if UNITY
		public UnityEngine.Rect rectangle
		{
			get
			{
				return new UnityEngine.Rect(x.Value, y.Value, width.Value, height.Value);
			}
			set
			{
				x = value.x;
				y = value.y;
				width = value.width;
				height = value.height;
			}
		}
#endif	

		public void serialize(net.XmlNode node)
		{
			if(x != null)
				node.attribute("x").set(x.Value);

			if(y != null)
				node.attribute("y").set(y.Value);

			if(width != null)
				node.attribute("width").set(width.Value);

			if(height != null)
				node.attribute("height").set(height.Value);
		}

		public void deserialize(net.XmlNode node)
		{
			if(node.attribute("x").isSet())
				x = node.attribute("x").getFloat();

			if(node.attribute("y").isSet())
				y = node.attribute("y").getFloat();

			if(node.attribute("width").isSet())
				width = node.attribute("width").getFloat();

			if(node.attribute("height").isSet())
				height = node.attribute("height").getFloat();
		}
	}
}