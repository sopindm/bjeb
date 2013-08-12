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
			draggable = false;
			skin = AssetBase.Skin.Default;
		}

		public float x
		{
			get;
			set;
		}

		public float y
		{
			get;
			set;
		}

		public float width
		{
			get;
			set;
		}

		public float height
		{
			get;
			set;
		}

		public string title
		{
			get;
			set;
		}

		public AssetBase.Skin skin
		{
			get;
			set;
		}

		public bool draggable
		{
			get;
			set;
		}

		public void draw()
		{
#if UNITY
			Rect area = GUILayout.Window( id, new Rect(x, y, width, height), drawContext, title, AssetBase.unitySkin(skin).window);

			x = area.x;
			y = area.y;
			width = area.width;
			height = area.height;
#endif
		}

		private void drawContext(int id)
		{
#if UNITY
            GUILayout.Label("Hi, this is Burning JEB.");
			
			if(draggable)
				GUI.DragWindow();
#endif
		}

		override protected void doSerialize(XmlNode node)
		{
			node.attribute("id").set(id);

			node.attribute("title").set(title);
			node.attribute("skin").set(skin.ToString());

			doSerializeState(node);

			node.attribute("draggable").set(draggable);
		}

		override protected void doSerializeState(XmlNode node)
		{
			node.attribute("x").set(x);
			node.attribute("y").set(y);
			node.attribute("width").set(width);
			node.attribute("height").set(height);
		}

		override protected void doDeserialize(XmlNode node)
        {
            id = node.attribute("id").getInt();

			title = node.attribute("title").getString();
			skin = (AssetBase.Skin)System.Enum.Parse(typeof(AssetBase.Skin), node.attribute("skin").getString());

			doDeserializeState(node);

			draggable = node.attribute("draggable").getBool();
		}

		override protected void doDeserializeState(XmlNode node)
		{
            x = node.attribute("x").getFloat();
            y = node.attribute("y").getFloat();
            width = node.attribute("width").getFloat();
            height = node.attribute("height").getFloat();
		}
	}
}