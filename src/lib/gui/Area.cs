using System.Collections.Generic;
#if UNITY
using UnityEngine;
#endif

namespace bjeb.gui
{
	public class Area
	{
		public float? x
		{
			get;
			set;
		}

		public float? y
		{
			get;
			set;
		}

		public float? width
		{
			get;
			set;
		}

		public float? height
		{
			get;
			set;
		}

		public float? minWidth
		{
			get;
			set;
		}

		public float? minHeight
		{
			get;
			set;
		}

		public float? maxWidth
		{
			get;
			set;
		}

		public float? maxHeight
		{
			get;
			set;
		}

		public bool? widthExpandable
		{
			get;
			set;
		}

		public bool? heightExpandable
		{
			get;
			set;
		}

		public Area()
		{
		}

		public Area(float x, float y, float width, float height)
		{
			set(x, y, width, height);
		}

		public void set(float x, float y, float width, float height)
		{
			this.x = x;
			this.y = y;
			this.width = width;
			this.height = height;
		}

		public bool isSet()
		{
			return x != null && y != null && width != null && height != null;
		}

#if UNITY
		public Rect rectangle
		{
			get
			{
				return new Rect(x.Value, y.Value, width.Value, height.Value);
			}
			set
			{
				x = value.x;
				y = value.y;
				width = value.width;
				height = value.height;
			}
		}

		public GUILayoutOption[] layoutOptions()
        {
            List<GUILayoutOption> options = new List<GUILayoutOption>();

			if(width != null)
				options.Add(GUILayout.Width(width.Value));

			if(height != null)
				options.Add(GUILayout.Height(height.Value));
			
			if(minWidth != null)
				options.Add(GUILayout.MinWidth(minWidth.Value));

			if(minHeight != null)
				options.Add(GUILayout.MinHeight(minHeight.Value));
			
			if(maxWidth != null)
				options.Add(GUILayout.MaxWidth(maxWidth.Value));

			if(maxHeight != null)
				options.Add(GUILayout.MaxHeight(maxHeight.Value));

			if(widthExpandable != null)
				options.Add(GUILayout.ExpandWidth(widthExpandable.Value));
			
			if(heightExpandable != null)
				options.Add(GUILayout.ExpandHeight(heightExpandable.Value));
			
			return options.ToArray();
		}
#endif	

		public void serialize(net.XmlNode node)
		{
			if(x != null)
				node.attribute("x").set(x.Value);

			if(y != null)
				node.attribute("y").set(y.Value);

			if(width != null)
				node.attribute("width").set(width.Value);

			if(height != null)
				node.attribute("height").set(height.Value);

			if(minWidth != null)
				node.attribute("minWidth").set(minWidth.Value);

			if(minHeight != null)
				node.attribute("minHeight").set(minHeight.Value);

			if(maxWidth != null)
				node.attribute("maxWidth").set(maxWidth.Value);

			if(maxHeight != null)
				node.attribute("maxHeight").set(maxHeight.Value);

			if(widthExpandable != null)
				node.attribute("widthExpandable").set(widthExpandable.Value);

			if(heightExpandable != null)
				node.attribute("heightExpandable").set(heightExpandable.Value);
		}

		public void deserialize(net.XmlNode node)
		{
			if(node.attribute("x").isSet())
				x = node.attribute("x").getFloat();
			else
				x = null;

			if(node.attribute("y").isSet())
				y = node.attribute("y").getFloat();
			else
				y = null;

			if(node.attribute("width").isSet())
				width = node.attribute("width").getFloat();
			else
				width = null;

			if(node.attribute("height").isSet())
				height = node.attribute("height").getFloat();
			else height = null;

			if(node.attribute("minWidth").isSet())
				minWidth = node.attribute("minWidth").getFloat();
			else 
				minWidth = null;

			if(node.attribute("minHeight").isSet())
				minHeight = node.attribute("minHeight").getFloat();
			else 
				minHeight = null;

			if(node.attribute("maxWidth").isSet())
				maxWidth = node.attribute("maxWidth").getFloat();
			else 
				maxWidth = null;

			if(node.attribute("maxHeight").isSet())
				maxHeight = node.attribute("maxHeight").getFloat();
			else 
				maxHeight = null;

			if(node.attribute("widthExpandable").isSet())
				widthExpandable = node.attribute("widthExpandable").getBool();
			else
				widthExpandable = null;

			if(node.attribute("heightExpandable").isSet())
				heightExpandable = node.attribute("heightExpandable").getBool();
			else
				heightExpandable = null;
		}
	}
}