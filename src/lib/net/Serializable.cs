using System;
using System.Linq;
using System.Collections.Generic;

namespace bjeb.net
{
    class SerializationException: Exception
    {
        private string _error
	{
	    get;
	    set;
	}

        public SerializationException(string error)
	{
	    _error = error;
	}

        public override string ToString()
	{
	    return "Serialization exception: " + _error;
	}
    }

    [AttributeUsage(AttributeTargets.Class,AllowMultiple=false,Inherited=false)]
    public class SerializableAttribute: Attribute
    {
        public UInt16 tag
	{
	    get;
	    private set;
	}

        public SerializableAttribute(UInt16 tag)
	{
	    this.tag = tag;
	}
    }

    public abstract class Serializable
    {
        private static SerializableAttribute serializableAttribute(Type t)
	{
	    return (SerializableAttribute)Attribute.GetCustomAttribute(t, typeof(SerializableAttribute));
	}

        private static bool isSerializable(Type t)
	{
	    return serializableAttribute(t) != null;
	}

        static SortedDictionary<UInt16, Type> _serializableTypes;

	static Serializable()
	{
	    List<Type> serializableTypes = (from ass in AppDomain.CurrentDomain.GetAssemblies() from t in ass.GetTypes() where isSerializable(t) select t).ToList();

	    _serializableTypes = new SortedDictionary<UInt16, Type>();

	    foreach (Type t in serializableTypes)
	    {
		if(_serializableTypes.ContainsKey(serializableAttribute(t).tag))
		    throw new SerializationException("Cannot define type " + t.ToString() + " with tag " + serializableAttribute(t).tag.ToString() + ". It's already defined.");

		_serializableTypes.Add(serializableAttribute(t).tag, t);
	    }
	}

        protected UInt16 tag()
	{
	    Type type = this.GetType();

	    SerializableAttribute attribute = (SerializableAttribute)Attribute.GetCustomAttribute(GetType(), typeof(SerializableAttribute));

	    if (attribute == null)
		throw new SerializationException(type.Name + " is not serializable");

	    return attribute.tag;
	}

	public void serialize(Stream stream)
	{
	    stream.writeTag(tag(), true);
	    doSerialize(stream);
	    stream.writeTag(tag(), false);
	}

	virtual public void serializeState(Stream stream) {}

        public static Serializable create(Stream stream)
	{
	    var tag = stream.readTag(true);
	    Type t = _serializableTypes[tag];

	    if (t == null)
		throw new SerializationException("No serializable type with tag " + tag.ToString());

	    Serializable ret = (Serializable)Activator.CreateInstance(t);
	    ret.deserialize(stream, tag);

	    return ret;
	}

	public void deserialize(Stream stream)
	{
	    deserialize(stream, stream.readTag(true));
	}

	private void deserialize(Stream stream, UInt16 tag)
	{
	    if (this.tag() != tag)
		throw new SerializationException("Tried to deserialize from node with wrong tag");

	    doDeserialize(stream);

	    if(stream.readTag(false) != tag)
		throw new SerializationException("Wrong closing tag");
	}

	virtual public void deserializeState(Stream stream) {}

        abstract protected void doSerialize(Stream node);
        abstract protected void doDeserialize(Stream node);
    }
}