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

	[bjeb.net.Serializable("font")]
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

		override protected void doSerialize(Stream stream)
		{
			if(alignment != Alignment.Default)
			    stream.write(alignment.ToString());
			else
			    stream.writeNull();

			if(style != FontStyle.Default)
			    stream.write(style.ToString());
			else
			    stream.writeNull();

			if(normal != null)
			    normal.serialize(stream);
			else
			    stream.writeNull();

			if(onNormal != null)
			    onNormal.serialize(stream);
			else
			    stream.writeNull();
			
			if(hover != null)
			    hover.serialize(stream);
			else
			    stream.writeNull();

			if(onHover != null)
			    onHover.serialize(stream);
			else
			    stream.writeNull();
			
			if(focused != null)
			    focused.serialize(stream);
			else
			    stream.writeNull();

			if(onFocused != null)
			    onFocused.serialize(stream);
			else
			    stream.writeNull();

			if(active != null)
			    active.serialize(stream);
			else
			    stream.writeNull();

			if(onActive != null)
			    onActive.serialize(stream);
			else
			    stream.writeNull();
		}

		override protected void doDeserialize(Stream stream)
		{
		    string alignmentString = stream.tryReadString();
		    if(alignmentString != null)
			alignment = (Alignment)Enum.Parse(typeof(Alignment), alignmentString);
		    else
			alignment = Alignment.Default;
			
		    string styleString = stream.tryReadString();

		    if(styleString != null)
			style = (FontStyle)Enum.Parse(typeof(FontStyle), styleString);
		    else style = FontStyle.Default;
		    
		    normal = Color.tryCreate(stream);
		    onNormal = Color.tryCreate(stream);
		    hover = Color.tryCreate(stream);
		    onHover = Color.tryCreate(stream);
		    focused = Color.tryCreate(stream);
		    onFocused = Color.tryCreate(stream);
		    active = Color.tryCreate(stream);
		    onActive = Color.tryCreate(stream);
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