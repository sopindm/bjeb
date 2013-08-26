using System;
using System.Collections.Generic;

namespace bjeb
{
	namespace net
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

			public static Xml createMessage(string type)
 			{
			        Xml ret = new Xml("msg");
				ret.root.attribute("type").set(type);

				return ret;
			}			

			public Xml(string rootName)
			{
			        _xml = new System.Xml.XmlDocument();

				System.Xml.XmlDeclaration dec = _xml.CreateXmlDeclaration("1.0", null, null);
				_xml.AppendChild(dec);

				_root = _xml.CreateElement(rootName);
				_xml.AppendChild(_root);
			}

			public XmlNode root
			{
				get
				{
					return new XmlNode(_root);
				}
			}

			public void write(Connection connection)
			{
				connection.stream.write(toString());
			}

			public bool tryWrite(Connection connection)
			{
				try
				{
					write(connection);
					return true;
				}
				catch(ConnectionException)
				{
					return false;
				}
			}

			public static Xml read(Connection connection)
			{
				System.Xml.XmlDocument document = new System.Xml.XmlDocument();
				document.LoadXml(connection.stream.readString());

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

			public XmlAttribute attribute(string name)
			{
				return new XmlAttribute(_node, name);
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
		} 

		public class XmlAttribute
		{
			System.Xml.XmlNode _node;
			string _name;

			public XmlAttribute(System.Xml.XmlNode node, string name)
			{
				_node = node;
				_name = name;
			}

			public bool isSet()
			{
				return _node.Attributes[_name] != null;
			}

			public void set(string value)
			{
				var attr = _node.Attributes[value];

				if(attr == null)
				{
					attr = _node.OwnerDocument.CreateAttribute(_name);
					_node.Attributes.Append(attr);
				}

				attr.Value = value;
			}

			public void set(int value)
			{
				set(value.ToString());
			}

			public void set(float value)
			{
				set(value.ToString());
			}

			public void set(bool value)
			{
				set(value.ToString());
			}

			public string getString()
			{
				var attr = _node.Attributes[_name];

				if(attr == null)
					return "";

				return attr.Value;
			}

			public int getInt()
			{
				return Int32.Parse(getString());
			}

			public float getFloat()
			{
				return float.Parse(getString());
			}

			public bool getBool()
			{
				return bool.Parse(getString());
			}
		}
	}
}
