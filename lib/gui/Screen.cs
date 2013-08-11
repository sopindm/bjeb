using bjeb.net;

namespace bjeb.gui
{
    [XmlSerializable("screen")]
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

		override protected void doSerialize(XmlNode node)
		{
			node.attribute("width").set(width);
			node.attribute("height").set(height);
		}

		override protected void doDeserialize(XmlNode node)
		{
			width = node.attribute("width").getInt();
			height = node.attribute("height").getInt();
		}
	}
}
