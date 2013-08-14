namespace bjeb.gui
{
	public abstract class View: net.Serializable
	{
		public AssetBase.Skin skin
		{
			get;
			set;
		}

		public Area area
		{
			get;
			set;
		}

		public View()
		{
			skin = AssetBase.Skin.Default;
			area = new Area();
		}

		override protected void doSerialize(net.XmlNode node)
		{
			node.attribute("skin").set(skin.ToString());

			area.serialize(node);
		}

		override protected void doDeserialize(net.XmlNode node)
		{
			skin = (AssetBase.Skin)System.Enum.Parse(typeof(AssetBase.Skin), node.attribute("skin").getString());

			area.deserialize(node);
		}

		abstract public void draw();
	}

	public abstract class LayoutView: View
	{
		override public void draw()
		{
			if(area.isSet())
				drawFixed();
			else
				drawLayout();
		}

		abstract protected void drawFixed();
		abstract protected void drawLayout();
	}
}