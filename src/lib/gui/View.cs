using System.Collections.Generic;
using bjeb.net;
#if UNITY
using UnityEngine;
#endif

namespace bjeb.gui
{
	public abstract class View: net.Serializable
	{
		public AssetBase.Skin skin
		{
			get;
			set;
		}

#if UNITY
		public GUISkin unitySkin
		{
			get
			{
				return AssetBase.unitySkin(skin);
			}
		}
#endif		

		public Area area
		{
			get;
			set;
		}

		public View()
		{
			skin = AssetBase.Skin.Default;
			area = new Area();
		}

		override protected void doSerialize(net.XmlNode node)
		{
			node.attribute("skin").set(skin.ToString());

			area.serialize(node);
		}

		override protected void doDeserialize(net.XmlNode node)
		{
			skin = (AssetBase.Skin)System.Enum.Parse(typeof(AssetBase.Skin), node.attribute("skin").getString());

			area.deserialize(node);
		}

		abstract public void draw();
	}

	public abstract class LayoutView: View
	{
		override public void draw()
		{
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