#if UNITY
using UnityEngine;
#endif

using bjeb.net;

namespace bjeb.gui
{
	[XmlSerializable("toggle")]
	public class Toggle: LayoutView
	{
		public bool toggled
		{
			get;
			set;
		}

		public string text
		{
			get;
			set;
		}

		public Toggle()
		{
			toggled = false;
			text = "";
		}

		public Toggle(string text, bool on)
		{
			this.text = text;
			toggled = on;
		}

		override protected Style defaultStyle
		{
			get
			{
				return Style.Toggle;
			}
		}

		public delegate void OnSwitch(Toggle toggle);

		public OnSwitch onSwitch
		{
			get;
			set;
		}

		private bool _updated = false;


		override protected void drawLayout()
		{
#if UNITY
			bool newState = GUILayout.Toggle(toggled, text, unityStyle(), area.layoutOptions());
			
			if(newState != toggled)
			{
				_updated = true;
				toggled = newState;
			}
#endif
		}

		override protected void drawFixed()
		{
#if UNITY
			bool newState = GUI.Toggle(area.rectangle, toggled, text, unityStyle());

			if(newState != toggled)
			{
				_updated = true;
				toggled = newState;
			}
#endif
		}

		override protected void doSerialize(XmlNode node)
		{
			base.doSerialize(node);

			node.attribute("text").set(text);
			node.attribute("toggled").set(toggled);
		}

		override protected void doSerializeState(XmlNode node)
		{
			if(_updated)
			{
				_updated = false;
				node.attribute("toggled").set(toggled);
			}
		}

		override protected void doDeserialize(XmlNode node)
        {
			base.doDeserialize(node);

			text = node.attribute("text").getString();
			toggled = node.attribute("toggled").getBool();
		}

		override protected void doDeserializeState(XmlNode node)
		{
			if(node.attribute("toggled").isSet() && onSwitch != null)
			{
				toggled = node.attribute("toggled").getBool();
				onSwitch(this);
			}
		}
	}
}