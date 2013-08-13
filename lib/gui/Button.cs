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
			_clicked = GUILayout.Button(text, AssetBase.unitySkin(skin).button);
#endif
		}

		override protected void doSerialize(XmlNode node)
		{
			node.attribute("text").set(text);
			node.attribute("skin").set(skin.ToString());
		}

		override protected void doSerializeState(XmlNode node)
		{
			node.attribute("clicked").set(_clicked);
		}

		override protected void doDeserialize(XmlNode node)
        {
			text = node.attribute("text").getString();
			skin = (AssetBase.Skin)System.Enum.Parse(typeof(AssetBase.Skin), node.attribute("skin").getString());
		}

		override protected void doDeserializeState(XmlNode node)
		{
			if(node.attribute("clicked").getBool() && onClick != null)
				onClick(this);
		}
	}
}