using System;
using System.Collections.Generic;
using bjeb.gui;

namespace bjeb
{
	class StatLabel
	{
		public delegate string UpdateHandler();

		private Layout _layout;
		private Label _valueLabel;
		private UpdateHandler _updateHandler;

		public StatLabel(string name, UpdateHandler onUpdate)
		{
			_layout = Layout.makeHorizontal();

			Label nameLabel = new Label(name);
			nameLabel.area.widthExpandable = true;
			nameLabel.font.alignment = Alignment.UpperLeft;
			nameLabel.font.normal = Color.white;
			nameLabel.font.style = FontStyle.Bold;

			_layout.views.add(nameLabel);

			_valueLabel = new Label();
			_valueLabel.area.width = 150;

			_layout.views.add(_valueLabel);

			_updateHandler = onUpdate;
		}

		public View view
		{
			get
			{
				return _layout;
			}
		}

		public void update()
		{
			_valueLabel.text = _updateHandler();
		}
	}

	class InfoModule: Module
	{
		private Layout _statsLayout;
		private List<StatLabel> _stats;

		public InfoModule(Computer computer): base(computer)
		{
			_stats = new List<StatLabel>();
			_statsLayout = Layout.makeVertical();
		}

		override protected void onSetup(Screen screen)
		{
			window.area.set(1100, 200, 300, 200);
			content.views.clear();

			_stats.Clear();

			_stats.Add(new StatLabel("Mass", (() => vessel.body.mass.ToString("F2") + " t")));
			_stats.Add(new StatLabel("Forward", (() => vessel.rootRotation.forward.ToString())));
			_stats.Add(new StatLabel("Inclination", (() => (vessel.orbit.inclination * 180 / Math.PI) .ToString())));
			_stats.Add(new StatLabel("LAN", (() => (vessel.orbit.LAN * 180 / Math.PI).ToString())));
			_stats.Add(new StatLabel("Parameter", (() => vessel.orbit.parameter.ToString())));
			_stats.Add(new StatLabel("Eccentricity", (() => vessel.orbit.eccentricity.ToString())));
			_stats.Add(new StatLabel("True anomaly", (() => vessel.orbit.trueAnomaly.ToString())));
			_stats.Add(new StatLabel("Argument of periapsis", (() => (vessel.orbit.argumentOfPeriapsis * 180 / Math.PI).ToString())));
			_stats.Add(new StatLabel("Time at periapsis", (() => vessel.orbit.timeAtPeriapsis.ToString())));

			_stats.Add(new StatLabel("Bodies", (() => 
					{
						string names = "";
						
						foreach(var body in computer.universe.celestialBodies)
							names += " " + body.name;

						return names;
					})));

			_statsLayout.views.clear();
			_statsLayout.style = Style.TextArea;

			foreach(var stat in _stats)
				_statsLayout.views.add(stat.view);

			content.views.add(_statsLayout);
		}

		override protected void onUpdate()
		{
			foreach(var stat in _stats)
				stat.update();

			//control for stats
			//need more control on styles (font colors) 
		}

		override public string name
		{
			get
			{
				return "Vessel Info";
			}
		}
	}
}