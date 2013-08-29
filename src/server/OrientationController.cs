using System.Collections.Generic;
using bjeb.gui;

namespace bjeb
{
	class Selector
	{
		private float _minValue;
		private float _midValue;
		private float _maxValue;

		private string _name;

		public Selector(float minValue, float midValue, float maxValue, string name)
		{
			_minValue = minValue;
			_midValue = midValue;
			_maxValue = maxValue;
			_name = name;
		}

		public View view
		{
			get
			{
				Layout main = Layout.makeVertical();

				Layout info = Layout.makeHorizontal();

				Label nameLabel = new Label(_name);
				nameLabel.area.width = 70;

				Label valueLabel = new Label(_midValue.ToString("F2"));

				info.views.add(nameLabel);
				info.views.add(new Space());
				info.views.add(valueLabel);
				info.views.add(new Space(50));
				info.views.add(new Space());

				main.views.add(info);

				Slider valueSlider;

				if(_minValue < _midValue)
					valueSlider = new Slider(0, 20, 10);
				else
					valueSlider = new Slider(10, 20, 10);

				valueSlider.onUpdate = (s => 
						{
							if(s.value <= 10 )
							{
								valueLabel.text = (_minValue * (1 - s.value / 10) + _midValue * (s.value / 10)).ToString("F2");
							}
							else
							{
								valueLabel.text = (_midValue * (1 - (s.value - 10) / 10) + _maxValue * (s.value - 10) / 10).ToString("F2");
							}
						});

				main.views.add(valueSlider);

				return main;
			}
		}
	}

	class OrientationController: Module
	{
		public OrientationController(Computer computer): base(computer)
		{
		}

		override protected void onSetup(Screen screen)
		{
			window.area.set(300, 500, 400, 150);
			content.views.clear();

			content.views.add(new Selector(0.1f, 1, 10, "Sensetivity").view);
			content.views.add(new Selector(1, 1, 10, "Inertia").view);
		}

		override protected void onUpdate()
		{
		}

		override public string name
		{
			get
			{
				return "OrientationController";
			}
		}
	}
}