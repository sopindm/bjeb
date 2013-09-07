#if UNITY
using UnityEngine;
#endif

using bjeb.net;

namespace bjeb.gui
{
	[Serializable("toggle")]
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
				update();
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
				update();
			}
#endif
		}

		override protected void doSerialize(Stream stream)
		{
			base.doSerialize(stream);

			stream.write(text);
			stream.write(toggled);
		}

		override protected void doSerializeState(Stream stream)
		{
			if(_updated)
			{
				_updated = false;
				stream.write(toggled);
			}
			else
			    stream.writeNull();
		}

		override protected void doDeserialize(Stream stream)
		{
			base.doDeserialize(stream);

			text = stream.readString();
			toggled = stream.readBool();
		}

		override protected void doDeserializeState(Stream stream)
		{
		    bool? state = stream.tryReadBool();

		    if(state != null)
		    {
			toggled = state.Value;

			if(onSwitch != null)
			    onSwitch(this);
		    }
		}
	}
}