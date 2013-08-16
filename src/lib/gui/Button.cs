#if UNITY
using UnityEngine;
#endif

using bjeb.net;

namespace bjeb.gui
{
	public class MouseState
	{
		private enum ButtonState
		{
			DOWN,
			PRESSED,
			UP,
			NONE
		}

		private string stateString(ButtonState state)
		{
			return state.ToString();
		}

		private ButtonState fromString(string str)
		{
			return (ButtonState)System.Enum.Parse(typeof(ButtonState),str);
		}

		private void serializeState(XmlNode node, string button, ButtonState state)
		{
			if(state == ButtonState.NONE)
				return;

			node.attribute(button).set(stateString(state));
		}

		private ButtonState deserializeState(XmlNode node, string button)
		{
			if(!node.attribute(button).isSet())
				return ButtonState.NONE;

			return fromString(node.attribute(button).getString());
		}

		private ButtonState _leftState = ButtonState.NONE;
		private ButtonState _middleState = ButtonState.NONE;
		private ButtonState _rightState = ButtonState.NONE;

#if UNITY
		private ButtonState updateButton(int id)
		{
			if(Input.GetMouseButtonDown(id))
				return ButtonState.DOWN;

			if(Input.GetMouseButtonUp(id))
				return ButtonState.UP;

			if(Input.GetMouseButton(id))
				return ButtonState.PRESSED;

			return ButtonState.NONE;
		}

		public void update()
		{
			_leftState = updateButton(0);
			_rightState = updateButton(1);
			_middleState = updateButton(2);
		}
#endif

		public void serialize(XmlNode node)
		{
			serializeState(node, "left", _leftState);
			serializeState(node, "right", _rightState);
			serializeState(node, "middle", _middleState);
		}

		public void deserialize(XmlNode node)
		{
			_leftState = deserializeState(node, "left");
			_rightState = deserializeState(node, "right");
			_middleState = deserializeState(node, "middle");
		}

		public bool leftButtonDown
		{
			get
			{
				return _leftState == ButtonState.DOWN;
			}
		}

		public bool leftButtonPressed
		{
			get
			{
				return _leftState == ButtonState.PRESSED;
			}
		}

		public bool leftButtonUp
		{
			get
			{
				return _leftState == ButtonState.UP;
			}
		}

		public bool middleButtonDown
		{
			get
			{
				return _middleState == ButtonState.DOWN;
			}
		}

		public bool middleButtonPressed
		{
			get
			{
				return _middleState == ButtonState.PRESSED;
			}
		}

		public bool middleButtonUp
		{
			get
			{
				return _middleState == ButtonState.UP;
			}
		}

		public bool rightButtonDown
		{
			get
			{
				return _rightState == ButtonState.DOWN;
			}
		}

		public bool rightButtonPressed
		{
			get
			{
				return _rightState == ButtonState.PRESSED;
			}
		}

		public bool rightButtonUp
		{
			get
			{
				return _rightState == ButtonState.UP;
			}
		}
	}              



	[XmlSerializable("button")]
	public class Button: LayoutView
	{
		public Button()
		{
			text = "";
		}

		public Button(string text)
		{
			this.text = text;
		}

		public string text
		{
			get;
			set;
		}

		public delegate void OnClick(Button button, MouseState mouse);

		public OnClick onClick
		{
			get;
			set;
		}

		private bool _clicked = false;
		private MouseState _state = new MouseState();

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
			_clicked = GUILayout.Button(text, style, area.layoutOptions());
			_state.update();
#endif
		}

		override protected void drawFixed()
		{
#if UNITY
			_clicked = GUI.Button(area.rectangle, text, style);
			_state.update();
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
			_state.serialize(node);
		}

		override protected void doDeserialize(XmlNode node)
        {
			base.doDeserialize(node);

			text = node.attribute("text").getString();
		}

		override protected void doDeserializeState(XmlNode node)
		{
			_state.deserialize(node);
			if(node.attribute("clicked").getBool() && onClick != null)
				onClick(this, _state);
		}
	}
}