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

		override protected Style defaultStyle
		{
			get 
			{
				return Style.Label;
			}
		}

		override protected void drawLayout()
		{
#if UNITY
			GUILayout.Label(text, unityStyle(), area.layoutOptions());
#endif
		}

		override protected void drawFixed()
		{
#if UNITY
			GUI.Label(area.rectangle, text, unityStyle());
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