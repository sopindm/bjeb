namespace bjeb
{
  class Xml
  {
    private Xml.XmlDocument _xml;

    public Xml(string rootName);

//    public XmlNode root();

    public string write();
    public static Xml read(string data);
  }

/*
  class XmlNode
  {
    Xml document();
    XmlNode parent();

    XmlNode(string name);
    XmlNode(string name, XmlNode parent);

    string name;

    XmlNode[] childs();
    XmlNode[] childs(string name);

    void addChild(XmlNode node);
    void removeChild(XmlNode node);

    XmlAttribute[] attributes();

    template<class T>
    void setAttribute(string name, T value);

    void removeAttribute(string name);

    template<class T>
    T attribute(string name);

    template<class T>
    T attribute(string name);
  } 

  class XmlAttribute
  {
     string name;

     template<class T>
     T value();
  } */
}