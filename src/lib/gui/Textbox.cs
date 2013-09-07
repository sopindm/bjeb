#if UNITY
using UnityEngine;
#endif

using bjeb.net;

namespace bjeb.gui
{
	[Serializable("textbox")]
	public class Textbox: LayoutView
	{
		public string text
		{
			get;
			set;
		}

		public int maxLength
		{
			get;
			set; 
		}

		private bool _isUpdated = false;

		public Textbox()
		{
			this.text = "";
			maxLength = 32;
			onUpdate = null;

			_isUpdated = false;
		}

		public Textbox(string defaultText = "", int maxLength = 32)
		{
			this.text = defaultText;
			this.maxLength = maxLength;
			onUpdate = null;

			_isUpdated = false;
		}

		override protected Style defaultStyle
		{
			get
			{
				return Style.Textbox;
			}
		}

		public delegate void OnUpdate(Textbox textbox);

		public OnUpdate onUpdate
		{
			get;
			set;
		}

		override protected void drawLayout()
		{
#if UNITY
			string newText = GUILayout.TextField(text, maxLength, unityStyle(), area.layoutOptions());

			if(newText != text)
			{
				_isUpdated = true;
				text = newText;
				update();
			}
#endif
		}

		override protected void drawFixed()
		{
#if UNITY
			string newText = GUI.TextField(area.rectangle, text, maxLength, unityStyle());

			if(newText != text)
			{
				_isUpdated = true;
				text = newText;
				update();
			}
#endif
		}

		override protected void doSerialize(Stream stream)
		{
			base.doSerialize(stream);

			stream.write(text);
			stream.write(maxLength);
		}

		override protected void doSerializeState(Stream stream)
		{
			if(_isUpdated)
			{
				_isUpdated = false;
				stream.write(text);
			}
			else
			    stream.writeNull();
		}

		override protected void doDeserialize(Stream stream)
		{
			base.doDeserialize(stream);

			text = stream.readString();
			maxLength = stream.readInt();
		}

		override protected void doDeserializeState(Stream stream)
		{
		    string textString = stream.tryReadString();

		    if(textString == null)
			return;

		    text = stream.readString();

		    if(onUpdate != null)
			onUpdate(this);
		}
	}
}