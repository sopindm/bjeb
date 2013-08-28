using System.Collections.Generic;
using bjeb.net;
#if UNITY
using UnityEngine;
#endif

namespace bjeb.gui
{
    public abstract class View: net.Serializable
    {
		public Skin skin
	    {
			get;
			set;
	    }

		public Font font
	    {
			get;
			set;
	    }

		public Style style
	    {
			get;
			set;
	    }

		protected abstract Style defaultStyle
	    {
			get;
	    }

		protected static void serializeStyle(Style style, Stream stream)
	    {
			if(style != Style.Default)
				stream.write(style.ToString());
			else
				stream.writeNull();
	    }

		protected static Style deserializeStyle(Stream stream)
	    {
			string styleString = stream.tryReadString();

			if(styleString == null)
				return Style.Default;

			return (Style)System.Enum.Parse(typeof(Style), styleString);
	    }

#if UNITY
		public GUISkin unitySkin
	    {
			get
			{
				return AssetBase.unitySkin(skin);
			}
	    }

		protected GUIStyle unityStyle()
	    {
			return unityStyle(style, defaultStyle);
	    }

		protected GUIStyle unityStyle(Style style, Style defaultStyle)
	    {
			if(style == Style.Default)
				style = defaultStyle;

			UnityEngine.GUIStyle ustyle = AssetBase.unityStyle(style, skin);

			if(!font.isDefault)
			{
				ustyle = new UnityEngine.GUIStyle(ustyle);
				font.apply(ustyle);
			}

			return ustyle;
	    }
#endif		

		public Area area
	    {
			get;
			set;
	    }

		public bool isShowing
	    {
			get;
			private set;
	    }

		public void show()
	    {
			isShowing = true;
	    }

		public void hide()
	    {
			isShowing = false;
	    }

		public ViewContainer container
		{
			get;
			set;
		}

		public void update()
		{
			doUpdate();

			if(container != null)
				container.update();
		}

		virtual protected void doUpdate()
		{
		}

		public View()
	    {
			isShowing = true;
			skin = Skin.Default;
			style = Style.Default;
			font = new Font();
			area = new Area();

			container = null;
	    }

		override protected void doSerialize(net.Stream stream)
	    {
			if(skin != Skin.Default)
				stream.write(skin.ToString());
			else
				stream.writeNull();

			serializeStyle(style, stream);

			stream.write(isShowing);

			font.serialize(stream);
			area.serialize(stream);
	    }

		override protected void doDeserialize(net.Stream stream)
	    {
			string skinString = stream.tryReadString();

			if(skinString != null)
				skin = (Skin)System.Enum.Parse(typeof(Skin), skinString);
			else
				skin = Skin.Default;

			style = deserializeStyle(stream);

			isShowing = stream.readBool();

			font.deserialize(stream);
			area.deserialize(stream);
	    }

		abstract public void draw();
    }

	public abstract class LayoutView: View
	{
		override public void draw()
		{
			if(!isShowing)
				return;

			if(area.isSet())
				drawFixed();
			else
				drawLayout();
		}

		abstract protected void drawFixed();
		abstract protected void drawLayout();
	}

	public class ViewContainer
	{
		private View _parent;
		private List<View> _childs;

		public View baseView
		{
			get
			{
				return _parent;
			}
		}

		public ViewContainer(View view)
		{
			_parent = view;
			_childs = new List<View>();
		}

		public void add(View view)
		{
			_childs.Add(view);
			view.container = this;
		}

		public void remove(View view)
		{
			_childs.Remove(view);
			view.container = null;
		}

		public void update()
		{
			_parent.update();
		}

		public void clear()
		{
			_childs.Clear();
		}

		public void draw()
		{
#if UNITY
			var currentSkin = GUI.skin;
			GUI.skin = _parent.unitySkin;

			foreach(View view in _childs)
				view.draw();

			GUI.skin = currentSkin;
#endif
		}

	    public void serialize(Stream stream)
	    {
			stream.write(_childs.Count);

			foreach(View view in _childs)
				view.serialize(stream);
	    }

	    public void deserialize(Stream stream)
	    {
			clear();

			int size = stream.readInt();

			for(int i=0;i<size;i++)
				add((View)Serializable.create(stream));
	    }

	    public void serializeState(Stream stream)
	    {
			stream.write(_childs.Count);

			foreach(View view in _childs)
				view.serializeState(stream);
	    }

	    public void deserializeState(Stream stream)
	    {
			var views = _childs;
			var viewIterator = views.GetEnumerator();

			int size = stream.readInt();
			int i=0;

			while(viewIterator.MoveNext() && (i < size))
			{
				viewIterator.Current.deserializeState(stream);		    
				i++;
			}
	    }
	}
}