#if UNITY
using UnityEngine;
#endif

using bjeb.net;

namespace bjeb.gui
{
	[XmlSerializable("window")]
	public class Window: View
	{
		private ViewContainer _views;

		public ViewContainer views
		{
			get
			{
				return _views;
			}
		}

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

			_views = new ViewContainer(this);
		}

		public string title
		{
			get;
			set;
		}

		public bool draggable
		{
			get;
			set;
		}

		public delegate void OnDrawFinished(Window window);

		public OnDrawFinished onDrawFinished
		{
			get;
			set;
		}

#if UNITY
		private GUIStyle style
		{
			get
			{
				return AssetBase.unitySkin(skin).window;
			}
		}
#endif

		public override void draw()
		{
#if UNITY
			area.rectangle = GUILayout.Window( id, area.rectangle, drawContext, title, style, area.layoutOptions());
#endif
		}

		private void drawContext(int id)
		{
#if UNITY
			_views.draw();

			if(draggable)
				GUI.DragWindow();

			if(onDrawFinished != null)
				onDrawFinished(this);
#endif
		}

		override protected void doSerialize(XmlNode node)
		{
			node.attribute("id").set(id);

			node.attribute("title").set(title);
			node.attribute("skin").set(skin.ToString());

			doSerializeState(node);

			node.attribute("draggable").set(draggable);

			_views.serialize(node);
		}

		override protected void doSerializeState(XmlNode node)
		{
			area.serialize(node);
		}

		override protected void doDeserialize(XmlNode node)
        {
            id = node.attribute("id").getInt();

			title = node.attribute("title").getString();
			skin = (AssetBase.Skin)System.Enum.Parse(typeof(AssetBase.Skin), node.attribute("skin").getString());

			doDeserializeState(node);

			draggable = node.attribute("draggable").getBool();

			_views.deserialize(node);
		}

		override protected void doDeserializeState(XmlNode node)
		{
			area.deserialize(node);
		}
	}
}