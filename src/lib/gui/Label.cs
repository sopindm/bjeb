#if UNITY
using UnityEngine;
#endif

using bjeb.net;

namespace bjeb.gui
{
	[Serializable("label")]
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

		override protected void doSerialize(Stream stream)
		{
			base.doSerialize(stream);
			stream.write(text);
		}

		override protected void doDeserialize(Stream stream)
		{
			base.doDeserialize(stream);
			text = stream.readString();
		}
	}
}