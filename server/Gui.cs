using bjeb.net;

namespace bjeb
{
	public class Gui
	{
		public int width
		{
			get;
			set;
		}

		public int height
		{
			get;
			set;
		}

		public Xml request()
		{
			Xml ret = new Xml("msg");

			ret.root.attribute("type").set("gui");
			ret.root.attribute("width").set(width);
			ret.root.attribute("height").set(height);
			
			return ret;
		}

		public Xml response(Xml request)
		{
			width += 1;//request.root.attribute("width").getInt();
			height += 1;//request.root.attribute("height").getInt();

			Xml ret = new Xml("msg");

			ret.root.attribute("type").set("gui");
			ret.root.attribute("width").set(width);
			ret.root.attribute("height").set(height);

			return ret;
		}
	}
}
