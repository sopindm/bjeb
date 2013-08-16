#if UNITY
using UnityEngine;
#endif

using bjeb.net;

namespace bjeb.gui
{
	[XmlSerializable("textbox")]
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

		public delegate void OnUpdate(Textbox textbox);

		public OnUpdate onUpdate
		{
			get;
			set;
		}

#if UNITY
		private UnityEngine.GUIStyle style
		{
			get
			{
				return AssetBase.unitySkin(skin).textField;
			}
		}
#endif

		override protected void drawLayout()
		{
#if UNITY
			string newText = GUILayout.TextField(text, maxLength, style, area.layoutOptions());

			if(newText != text)
			{
				_isUpdated = true;
				text = newText;
			}
#endif
		}

		override protected void drawFixed()
		{
#if UNITY
			string newText = GUI.TextField(area.rectangle, text, maxLength, style);

			if(newText != text)
			{
				_isUpdated = true;
				text = newText;
			}
#endif
		}

		override protected void doSerialize(XmlNode node)
		{
			base.doSerialize(node);

			node.attribute("text").set(text);
			node.attribute("maxLength").set(maxLength);
		}

		override protected void doSerializeState(XmlNode node)
		{
			if(_isUpdated)
			{
				_isUpdated = false;
				node.attribute("text").set(text);
			}
		}

		override protected void doDeserialize(XmlNode node)
        {
			base.doDeserialize(node);

			text = node.attribute("text").getString();
			maxLength = node.attribute("maxLength").getInt();
		}

		override protected void doDeserializeState(XmlNode node)
		{
			if(!node.attribute("text").isSet())
				return;

			text = node.attribute("text").getString();

			if(onUpdate != null)
				onUpdate(this);
		}
	}
}