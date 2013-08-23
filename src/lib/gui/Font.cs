using System;
using bjeb.net;

namespace bjeb.gui
{
	public enum Alignment {
		Default,
		Left,
		Center,
		Right,
		UpperLeft,
		Upper,
		UpperRight,
		LowerLeft,
		Lower,
		LowerRight
	}

	public enum FontStyle {
		Default,
		Normal,
		Italic,
		Bold,
		BoldAndItalic
	}

	[XmlSerializable("font")]
	public class Font: Serializable
	{
		public Alignment alignment
		{
			get;
			set;
		}

		public FontStyle style
		{
			get;
			set;
		}

		public Color normal
		{
			get;
			set;
		}

		public Color onNormal
		{
			get;
			set;
		}

		public Color hover
		{
			get;
			set;
		}

		public Color onHover
		{
			get;
			set;
		}

		public Color focused
		{
			get;
			set;
		}

		public Color onFocused
		{
			get;
			set;
		}

		public Color active
		{
			get;
			set;
		}

		public Color onActive
		{
			get;
			set;
		}

		public Font()
		{
			alignment = Alignment.Default;
			style = FontStyle.Default;

			normal = null;
			onNormal = null;
			hover = null;
			onHover = null;
			focused = null;
			onFocused = null;
			active = null;
			onActive = null;
		}

		override protected void doSerialize(XmlNode node)
		{
			if(alignment != Alignment.Default)
				node.attribute("alignment").set(alignment.ToString());

			if(style != FontStyle.Default)
				node.attribute("style").set(style.ToString());

			if(normal != null)
				normal.serialize("normal", node);

			if(onNormal != null)
				onNormal.serialize("onNormal", node);

			if(hover != null)
				hover.serialize("hover", node);

			if(onHover != null)
				onHover.serialize("onHover", node);

			if(focused != null)
				focused.serialize("focused", node);

			if(onFocused != null)
				onFocused.serialize("onFocused", node);

			if(active != null)
				active.serialize("active", node);

			if(onActive != null)
				onActive.serialize("onActive", node);
		}

		override protected void doDeserialize(XmlNode node)
		{
			if(node.attribute("alignment").isSet())
				alignment = (Alignment)Enum.Parse(typeof(Alignment), node.attribute("alignment").getString());
			
			if(node.attribute("style").isSet())
				style = (FontStyle)Enum.Parse(typeof(FontStyle), node.attribute("style").getString());
			
			normal = Color.create("normal", node);
			onNormal = Color.create("onNormal", node);
			hover = Color.create("hover", node);
			onHover = Color.create("onHover", node);
			focused = Color.create("focused", node);
			onFocused = Color.create("onFocused", node);
			active = Color.create("active", node);
			onActive = Color.create("onActive", node);
		}

		public bool isDefault
		{
			get
			{
				return alignment == Alignment.Default &&
					style == FontStyle.Default &&
					normal == null &&
					onNormal == null &&
					hover == null &&
					onHover == null &&
					focused == null &&
					onFocused == null &&
					active == null &&
					onActive == null;
					
			}
		}

#if UNITY
		private UnityEngine.TextAnchor unityAlignment()
		{
			switch(alignment)
			{
			case Alignment.Left:
				return UnityEngine.TextAnchor.MiddleLeft;
			case Alignment.Center:
				return UnityEngine.TextAnchor.MiddleCenter;
			case Alignment.Right:
				return UnityEngine.TextAnchor.MiddleRight;
			case Alignment.UpperLeft:
				return UnityEngine.TextAnchor.UpperLeft;
			case Alignment.Upper:
				return UnityEngine.TextAnchor.UpperCenter;
			case Alignment.UpperRight:
				return UnityEngine.TextAnchor.UpperRight;
			case Alignment.LowerLeft:
				return UnityEngine.TextAnchor.LowerLeft;
			case Alignment.Lower:
				return UnityEngine.TextAnchor.LowerCenter;
			case Alignment.LowerRight:
				return UnityEngine.TextAnchor.LowerRight;
			}

			return UnityEngine.TextAnchor.MiddleCenter;
		}

		private UnityEngine.FontStyle unityStyle()
		{
			switch(style)
			{
			case FontStyle.Italic:
				return UnityEngine.FontStyle.Italic;
			case FontStyle.Bold:
				return UnityEngine.FontStyle.Bold;
			case FontStyle.BoldAndItalic:
				return UnityEngine.FontStyle.BoldAndItalic;
			}

			return UnityEngine.FontStyle.Normal;
		}

		public void apply(UnityEngine.GUIStyle ustyle)
		{
			if(alignment != Alignment.Default)
				ustyle.alignment = unityAlignment();

			if(style != FontStyle.Default)
				ustyle.fontStyle = unityStyle();

			if(normal != null)
				ustyle.normal.textColor = normal.unity;
		}
#endif
	}
}