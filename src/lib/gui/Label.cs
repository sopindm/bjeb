#if UNITY
using UnityEngine;
#endif

using bjeb.net;

namespace bjeb.gui
{
	[XmlSerializable("label")]
	public class Label: LayoutView
	{
		public string text
		{
			get;
			set;
		}

		public Label()
		{
			this.text = "";
		}

		public Label(string text = "")
		{
			this.text = text;
		}

#if UNITY
		private UnityEngine.GUIStyle style
		{
			get
			{
				return AssetBase.unitySkin(skin).label;
			}
		}
#endif

		override protected void drawLayout()
		{
#if UNITY
			GUILayout.Label(text, style, area.layoutOptions());
#endif
		}

		override protected void drawFixed()
		{
#if UNITY
			GUI.Label(area.rectangle, text, style);
#endif
		}

		override protected void doSerialize(XmlNode node)
		{
			base.doSerialize(node);
			node.attribute("text").set(text);
		}

		override protected void doSerializeState(XmlNode node)
		{
		}

		override protected void doDeserialize(XmlNode node)
        {
			base.doDeserialize(node);
			text = node.attribute("text").getString();
		}

		override protected void doDeserializeState(XmlNode node)
		{
		}
	}
}