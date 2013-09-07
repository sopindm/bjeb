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

		private void serializeState(Stream stream, ButtonState state)
		{
			if(state == ButtonState.NONE)
			    stream.writeNull();
			else
			    stream.write(stateString(state));
		}

	    private ButtonState deserializeState(Stream stream)
	    {
		string stateString = stream.tryReadString();

		if(stateString == null)
		    return ButtonState.NONE;

		return fromString(stateString);
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

		public void serialize(Stream stream)
		{
		    serializeState(stream, _leftState);
		    serializeState(stream, _rightState);
		    serializeState(stream, _middleState);
		}

	    public void deserialize(Stream stream)
	    {
		_leftState = deserializeState(stream);
		_rightState = deserializeState(stream);
		_middleState = deserializeState(stream);
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



	[Serializable("button")]
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

		override protected Style defaultStyle
		{
			get
			{
				return Style.Button;
			}
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

		override protected void drawLayout()
		{
#if UNITY
			_clicked = GUILayout.Button(text, unityStyle(), area.layoutOptions());
			_state.update();

			if(_clicked)
				update();
#endif
		}

		override protected void drawFixed()
		{
#if UNITY
			_clicked = GUI.Button(area.rectangle, text, unityStyle());
			_state.update();

			if(_clicked)
				update();
#endif
		}

		override protected void doSerialize(Stream stream)
		{
			base.doSerialize(stream);
			stream.write(text);
		}

		override protected void doSerializeState(Stream stream)
		{
		    _state.serialize(stream);
		    stream.write(_clicked);
		}

		override protected void doDeserialize(Stream stream)
		{
			base.doDeserialize(stream);

			text = stream.readString();
		}

	    override protected void doDeserializeState(Stream stream)
	    {
		_state.deserialize(stream);

		if(stream.readBool() && onClick != null)
		    onClick(this, _state);
	    }
	}
}