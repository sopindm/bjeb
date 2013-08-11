#if UNITY
using UnityEngine;
#endif

using bjeb.net;

namespace bjeb.gui
{
	[XmlSerializable("window")]
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

		public string title
		{
			get;
			set;
		}

		public void draw()
		{
#if UNITY
			GUILayout.Window( id, new Rect(x, y, width, height), drawContext, title, GUI.skin.window);
#endif
		}

		private void drawContext(int id)
		{
#if UNITY
            GUILayout.Label("Hi, this is Burning JEB.");
#endif
		}

		override protected void doSerialize(XmlNode node)
		{
			node.attribute("id").set(id);

			node.attribute("x").set(x);
			node.attribute("y").set(y);
			node.attribute("width").set(width);
			node.attribute("height").set(height);

			node.attribute("title").set(title);
		}

		override protected void doDeserialize(XmlNode node)
        {
            id = node.attribute("id").getInt();

            x = node.attribute("x").getInt();
            y = node.attribute("y").getInt();
            width = node.attribute("width").getInt();
            height = node.attribute("height").getInt();

			title = node.attribute("title").getString();
		}
	}
}