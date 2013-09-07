#if UNITY
using UnityEngine;
#endif

using bjeb.net;

namespace bjeb.gui
{
	[Serializable("slider")]
	public class Slider: LayoutView
	{
		public float minValue
		{
			get;
			set;
		}

		public float maxValue
		{
			get;
			set;
		}

		public float value
		{
			get;
			set;
		}

		public bool isHorizontal
		{
			get;
			set;
		}

		private bool _isUpdated = false;

		public Slider()
		{
			minValue = 0;
			maxValue = 0;
			value = 0;
			isHorizontal = true;

			onUpdate = null;
			_isUpdated = false;
		}

		public Slider(float minValue, float maxValue, float value)
		{
			this.minValue = minValue;
			this.maxValue = maxValue;
			this.value = value;

			isHorizontal = true;

			onUpdate = null;
			_isUpdated = false;

			thumbStyle = Style.Default;
		}

		public delegate void OnUpdate(Slider slider);

		public OnUpdate onUpdate
		{
			get;
			set;
		}

		override protected Style defaultStyle
		{
			get
			{
				if(isHorizontal)
					return Style.HorizontalSlider;
				else
					return Style.VerticalSlider;
			}
		}

		public Style thumbStyle
		{
			get;
			set;
		}

		override protected void drawLayout()
		{
#if UNITY
			float newValue;

			if(isHorizontal)
				newValue = GUILayout.HorizontalSlider(value, minValue, maxValue, 
													  unityStyle(), 
													  unityStyle(thumbStyle, Style.HorizontalSliderThumb),
													  area.layoutOptions());
			else 
				newValue = GUILayout.VerticalSlider(value, minValue, maxValue, 
													unityStyle(), 
													unityStyle(thumbStyle, Style.VerticalSliderThumb),
													area.layoutOptions());

			if(value != newValue)
			{
				value = newValue;
				_isUpdated = true;
				update();
			}
#endif
		}

		override protected void drawFixed()
		{
#if UNITY
			float newValue;

			if(isHorizontal)
				newValue = GUI.HorizontalSlider(area.rectangle, value, minValue, maxValue, 
												unityStyle(), 
												unityStyle(thumbStyle, Style.HorizontalSliderThumb));
			else
				newValue = GUI.VerticalSlider(area.rectangle, value, minValue, maxValue, 
											  unityStyle(),
											  unityStyle(thumbStyle, Style.VerticalSliderThumb));

			if(value != newValue)
			{
				value = newValue;
				_isUpdated = true;
				update();
			}
#endif
		}

		override protected void doSerialize(Stream stream)
		{
			base.doSerialize(stream);

			stream.write(minValue);
			stream.write(maxValue);
			stream.write(value);
			stream.write(isHorizontal);

			serializeStyle(thumbStyle, stream);
		}

		override protected void doSerializeState(Stream stream)
		{
			if(_isUpdated)
			{
				_isUpdated = false;
				stream.write(value);
			}
			else
			    stream.writeNull();
		}

		override protected void doDeserialize(Stream stream)
		{
			base.doDeserialize(stream);

			minValue = stream.readFloat();
			maxValue = stream.readFloat();
			value = stream.readFloat();
			isHorizontal = stream.readBool();

			thumbStyle = deserializeStyle(stream);
		}

		override protected void doDeserializeState(Stream stream)
		{
		    float? data = stream.tryReadFloat();

		    if(data == null)
			return;

		    value = data.Value;

		    if(onUpdate != null)
			onUpdate(this);
		}
	}
}