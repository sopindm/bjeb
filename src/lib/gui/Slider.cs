#if UNITY
using UnityEngine;
#endif

using bjeb.net;

namespace bjeb.gui
{
	[XmlSerializable("slider")]
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
			}
#endif
		}

		override protected void doSerialize(XmlNode node)
		{
			base.doSerialize(node);

			node.attribute("minValue").set(minValue);
			node.attribute("maxValue").set(maxValue);
			node.attribute("value").set(value);
			node.attribute("isHorizontal").set(isHorizontal);

			serializeStyle(thumbStyle, "thumbStyle", node);
		}

		override protected void doSerializeState(XmlNode node)
		{
			if(_isUpdated)
			{
				_isUpdated = false;
				node.attribute("value").set(value);
			}
		}

		override protected void doDeserialize(XmlNode node)
        {
			base.doDeserialize(node);

			minValue = node.attribute("minValue").getFloat();
			maxValue = node.attribute("maxValue").getFloat();
			value = node.attribute("value").getFloat();
			isHorizontal = node.attribute("isHorizontal").getBool();

			thumbStyle = deserializeStyle("thumbStyle", node);
		}

		override protected void doDeserializeState(XmlNode node)
		{
			if(!node.attribute("value").isSet())
				return;

			value = node.attribute("value").getFloat();

			if(onUpdate != null)
				onUpdate(this);
		}
	}
}