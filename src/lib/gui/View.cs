namespace bjeb.gui
{
	public abstract class View: net.Serializable
	{
		public AssetBase.Skin skin
		{
			get;
			set;
		}

		public int? x
		{
			get;
			set;
		}

		public int? y
		{
			get;
			set;
		}

		public int? width
		{
			get;
			set;
		}

		public int? height
		{
			get;
			set;
		}

		private bool isFixed()
		{
			return x != null && y != null && width != null && height != null;
		}

#if UNITY
		protected UnityEngine.Rect area
		{
			get
			{
				return new UnityEngine.Rect(x.Value, y.Value, width.Value, height.Value);
			}
		}
#endif		

		public View()
		{
			skin = AssetBase.Skin.Default;

			x = null;
			y = null;
			width = null;
			height = null;
		}

		override protected void doSerialize(net.XmlNode node)
		{
			node.attribute("skin").set(skin.ToString());

			if(x != null)
				node.attribute("x").set(x.Value);

			if(y != null)
				node.attribute("y").set(y.Value);

			if(width != null)
				node.attribute("width").set(width.Value);

			if(height != null)
				node.attribute("height").set(height.Value);

		}

		override protected void doDeserialize(net.XmlNode node)
		{
			skin = (AssetBase.Skin)System.Enum.Parse(typeof(AssetBase.Skin), node.attribute("skin").getString());

			if(node.attribute("x").isSet())
				x = node.attribute("x").getInt();

			if(node.attribute("y").isSet())
				y = node.attribute("y").getInt();

			if(node.attribute("width").isSet())
				width = node.attribute("width").getInt();

			if(node.attribute("height").isSet())
				height = node.attribute("height").getInt();
		}

		public void draw()
		{
			if(isFixed())
				drawFixed();
			else
				drawLayout();
		}

		abstract protected void drawLayout();
		abstract protected void drawFixed();
	}
}