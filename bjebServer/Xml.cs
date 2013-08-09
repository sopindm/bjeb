using System;
using System.Collections.Generic;

namespace bjeb
{
	public class Xml
	{
		private System.Xml.XmlDocument _xml;
		private System.Xml.XmlNode _root;

		private Xml(System.Xml.XmlDocument xml)
        {
            _xml = xml;
			_root = _xml.LastChild;
		}

		public Xml(string rootName) {
			_xml = new System.Xml.XmlDocument();

			System.Xml.XmlDeclaration dec = _xml.CreateXmlDeclaration("1.0", null, null);
			_xml.AppendChild(dec);

			_root = _xml.CreateElement(rootName);
			_xml.AppendChild(_root);
		}

		public XmlNode root() {
			return new XmlNode(_root);
		}

		public void write(Connection connection)
		{
			connection.write(toString());
		}

		public static Xml read(Connection connection)
        {
            System.Xml.XmlDocument document = new System.Xml.XmlDocument();
            document.LoadXml(connection.read());

            return new Xml(document);
		}

		public string toString()
		{
			return _xml.OuterXml;
		}
	}

	public class XmlNode
	{
		System.Xml.XmlNode _node;

		public string name
		{
			get
			{
				return _node.LocalName;
			}
		}

		public XmlNode(System.Xml.XmlNode node) {
			_node = node;
		}

		public XmlNode(string name, XmlNode parent)
		{
			_node = parent._node.OwnerDocument.CreateElement(name);
			parent._node.AppendChild(_node);
		}

		public bool haveAttribute(string name)
		{
			return _node.Attributes[name] != null;
		}

		public XmlAttribute attribute(string name)
		{
			System.Xml.XmlAttribute ret = _node.Attributes[name];

			if(ret != null)
				return new XmlAttribute(ret);

			System.Xml.XmlAttribute attr = _node.OwnerDocument.CreateAttribute(name);

			_node.Attributes.Append(attr);

            return new XmlAttribute(attr);
		}

		public XmlNode node(string name)
		{										
			List<XmlNode> all = nodes(name);

			if(all.Count == 0)
				return null;

			return all[0];
		}

		public List<XmlNode> nodes(string name)
		{
			System.Xml.XmlNodeList nodes = _node.ChildNodes;

			List<XmlNode> ret = new List<XmlNode>();

			foreach(System.Xml.XmlNode node in nodes)
			{
				if(node.Name == name)
					ret.Add(new XmlNode(node));
			}

			return ret;
		}

		//XmlNode[] childs();
		//XmlNode[] childs(string name);

		//void addChild(XmlNode node);
		//void removeChild(XmlNode node);

		/*
		  XmlAttribute[] attributes();
		  
		  template<class T>
		  void setAttribute(string name, T value);

		  void removeAttribute(string name);

		  template<class T>
		  T attribute(string name);

		  template<class T>
		  T attribute(string name);*/
	} 

	public class XmlAttribute
	{
		System.Xml.XmlAttribute _attribute;

		public XmlAttribute(System.Xml.XmlAttribute attribute)
		{
			_attribute = attribute;
		}

		public void set(string value)
		{
			_attribute.Value = value;
		}

		public void set(int value)
		{
			set(value.ToString());
		}

		public void set(float value)
		{
			set(value.ToString());
		}

		public string getString()
		{
			return _attribute.Value;
		}

		public int getInt()
		{
			return Int32.Parse(getString());
		}

		public float getFloat()
		{
			return float.Parse(getString());
		}
	}

	/*
	  class XmlAttribute
	  {
	  string name;

	  template<class T>
	  T value();
	  } */
}