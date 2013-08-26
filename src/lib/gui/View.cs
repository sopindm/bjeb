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

		protected static void serializeStyle(Style style, string fieldName, XmlNode node)
		{
			if(style != Style.Default)
				node.attribute(fieldName).set(style.ToString());
		}

		protected static Style deserializeStyle(string fieldName, XmlNode node)
		{
			if(!node.attribute(fieldName).isSet())
				return Style.Default;

			return (Style)System.Enum.Parse(typeof(Style), node.attribute(fieldName).getString());
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

		public View()
		{
			isShowing = true;
			skin = Skin.Default;
			style = Style.Default;
			font = new Font();
			area = new Area();
		}

		override protected void doSerialize(net.XmlNode node)
		{
			if(skin != Skin.Default)
				node.attribute("skin").set(skin.ToString());
			serializeStyle(style, "style", node);

			node.attribute("isShowing").set(isShowing);

			if(!font.isDefault)
				font.serialize(node);

			area.serialize(node);
		}

		override protected void doDeserialize(net.XmlNode node)
		{
			if(node.attribute("skin").isSet())
				skin = (Skin)System.Enum.Parse(typeof(Skin), node.attribute("skin").getString());
			else
				skin = Skin.Default;
			style = deserializeStyle("style", node);

			isShowing = node.attribute("isShowing").getBool();

			if(node.node("font") != null)
				font.deserialize(node.node("font"));

			area.deserialize(node);
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
		}

		public void remove(View view)
		{
			_childs.Remove(view);
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

		public void serialize(XmlNode node)
		{
			XmlNode childs = new XmlNode("views", node);

			foreach(View view in _childs)
				view.serialize(childs);
		}

		public void deserialize(XmlNode node)
		{
			clear();

			net.XmlNode childs = node.node("views");

			foreach(var child in childs.nodes())
				add((View)Serializable.create(child));
		}

		public void serializeState(XmlNode node)
		{
			XmlNode childs = new XmlNode("views", node);

			foreach(View view in _childs)
				view.serializeState(childs);
		}

		public void deserializeState(XmlNode node)
		{
			net.XmlNode childs = node.node("views");

            var nodes = childs.nodes();
            var views = _childs;

			var viewIterator = views.GetEnumerator();
			var childIterator = nodes.GetEnumerator();

			while(childIterator.MoveNext() && viewIterator.MoveNext())
				viewIterator.Current.deserializeState(childIterator.Current);
		}
	}
}