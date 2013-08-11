namespace bjeb.net
{
	public abstract class Serializable
	{
		virtual public Serializable state() { return null; }

		abstract public string xmlName
		{
			get;
		}

		abstract public void serialize(XmlNode node);
		abstract public void deserialize(XmlNode node);
	}
}