using bjeb.net;

namespace bjeb.gui
{
	public class Screen: Serializable
	{
		public int width
		{
			get;
			set;
		}

		public int height
		{
			get;
			set;
		}

		override public string xmlName
		{
			get
			{
				return "screen";
			}
		}

		override public void serialize(XmlNode node)
		{
			node.attribute("width").set(width);
			node.attribute("height").set(height);
		}

		override public void deserialize(XmlNode node)
		{
			width = node.attribute("width").getInt();
			height = node.attribute("height").getInt();
		}
	}
}
