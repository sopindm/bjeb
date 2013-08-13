#if UNITY
using UnityEngine;
#endif

using bjeb.net;

namespace bjeb.gui
{
	[XmlSerializable("button")]
	public class Button: Serializable
	{
		public Button()
		{
			text = "";
			skin = AssetBase.Skin.Default;
			x = null;
			y = null;
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

		public string text
		{
			get;
			set;
		}

		public AssetBase.Skin skin
		{
			get;
			set;
		}

		public delegate void OnClick(Button button);

		public OnClick onClick
		{
			get;
			set;
		}

		private bool _clicked = false;

		public void draw()
		{
#if UNITY
			if(x != null && y != null && width != null && height != null)
				_clicked = GUI.Button(new UnityEngine.Rect(x.Value, y.Value, width.Value, height.Value), text, AssetBase.unitySkin(skin).button);
			else
				_clicked = GUILayout.Button(text, AssetBase.unitySkin(skin).button);
#endif
		}

		override protected void doSerialize(XmlNode node)
		{
			node.attribute("text").set(text);
			node.attribute("skin").set(skin.ToString());

			if(x != null && y != null && width != null && height != null)
			{
				node.attribute("x").set(x.Value);
				node.attribute("y").set(y.Value);
				node.attribute("width").set(width.Value);
				node.attribute("height").set(height.Value);
			}
		}

		override protected void doSerializeState(XmlNode node)
		{
			node.attribute("clicked").set(_clicked);
		}

		override protected void doDeserialize(XmlNode node)
        {
			text = node.attribute("text").getString();
			skin = (AssetBase.Skin)System.Enum.Parse(typeof(AssetBase.Skin), node.attribute("skin").getString());
			
			if(node.attribute("x").isSet() && 
			   node.attribute("y").isSet() &&
			   node.attribute("width").isSet() &&
			   node.attribute("height").isSet())
			{
				x = node.attribute("x").getInt();
				y = node.attribute("y").getInt();
				width = node.attribute("width").getInt();
				height = node.attribute("height").getInt();
			}
		}

		override protected void doDeserializeState(XmlNode node)
		{
			if(node.attribute("clicked").getBool() && onClick != null)
				onClick(this);
		}
	}
}