#if UNITY
using UnityEngine;
#endif

using bjeb.net;

namespace bjeb.gui
{
	[XmlSerializable("layout")]
	public class Layout: View
	{
		private ViewContainer _views;

		public ViewContainer views
		{
			get
			{
				return _views;
			}
		}
		
		public bool isVertical
		{
			get;
			set;
		}

		public Layout()
		{
			isVertical = true;
			_views = new ViewContainer(this);
		}

		public static Layout makeVertical()
		{
			return new Layout();
		}

		public static Layout makeHorizontal()
		{
			Layout ret = new Layout();
			ret.isVertical = false;

			return ret;
		}

		public override void draw()
		{
#if UNITY
			if(isVertical)
				GUILayout.BeginVertical(area.layoutOptions());
			else
				GUILayout.BeginHorizontal(area.layoutOptions());

			views.draw();

			if(isVertical)
				GUILayout.EndVertical();
			else
				GUILayout.EndHorizontal();
#endif
		}

		override protected void doSerialize(XmlNode node)
		{
			node.attribute("isVertical").set(isVertical);
			_views.serialize(node);
		}

		override protected void doSerializeState(XmlNode node)
		{
			_views.serializeState(node);
		}

		override protected void doDeserialize(XmlNode node)
        {
			isVertical = node.attribute("isVertical").getBool();
			_views.deserialize(node);
		}

		override protected void doDeserializeState(XmlNode node)
		{
			_views.deserializeState(node);
		}
	}
}