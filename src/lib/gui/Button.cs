#if UNITY
using UnityEngine;
#endif

using bjeb.net;

namespace bjeb.gui
{
	[XmlSerializable("button")]
	public class Button: LayoutView
	{
		public Button()
		{
			text = "";

		}

		public string text
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

#if UNITY
		private UnityEngine.GUIStyle style
		{
			get
			{
				return AssetBase.unitySkin(skin).button;
			}
		}
#endif

		override protected void drawLayout()
		{
#if UNITY
			_clicked = GUILayout.Button(text, style);
#endif
		}

		override protected void drawFixed()
		{
#if UNITY
			_clicked = GUI.Button(area.rectangle, text, style);
#endif
		}

		override protected void doSerialize(XmlNode node)
		{
			base.doSerialize(node);

			node.attribute("text").set(text);
		}

		override protected void doSerializeState(XmlNode node)
		{
			node.attribute("clicked").set(_clicked);
		}

		override protected void doDeserialize(XmlNode node)
        {
			base.doDeserialize(node);

			text = node.attribute("text").getString();
		}

		override protected void doDeserializeState(XmlNode node)
		{
			if(node.attribute("clicked").getBool() && onClick != null)
				onClick(this);
		}
	}
}