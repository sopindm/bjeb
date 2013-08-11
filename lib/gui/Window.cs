using bjeb.net;

namespace bjeb.gui
{
	[XmlSerializable("Window")]
	public class Window: Serializable
	{
		private static int _nextId = 92142814;
		private static int nextId()
		{
			_nextId++;
			return _nextId - 1;
		}

		public int id
		{
			get;
			private set;
		}

		public Window()
		{
			id = nextId();
		}

		public int x
		{
			get;
			set;
		}

		public int y
		{
			get;
			set;
		}

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
			node.attribute("id").set(id);

			node.attribute("x").set(x);
			node.attribute("y").set(y);
			node.attribute("width").set(width);
			node.attribute("height").set(height);
		}

		override protected void doDeserialize(XmlNode node)
		{
		}
	}
}