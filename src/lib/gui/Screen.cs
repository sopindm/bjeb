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

		public Screen(): this(0, 0)
		{
		}

		public Screen(int width, int height)
		{
			this.width = width;
			this.height = height;
		}

		override protected void doSerialize(XmlNode node)
		{
			node.attribute("width").set(width);
			node.attribute("height").set(height);
		}

		override protected void doSerializeState(XmlNode node)
		{
			doSerialize(node);
		}

		override protected void doDeserialize(XmlNode node)
		{
			width = node.attribute("width").getInt();
			height = node.attribute("height").getInt();
		}

		override protected void doDeserializeState(XmlNode node)
		{
			doDeserialize(node);
		}
	}
}
