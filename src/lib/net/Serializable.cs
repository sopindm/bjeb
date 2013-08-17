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
    public class XmlSerializableAttribute: Attribute
    {
        public string nodeName
        {
            get;
            private set;
        }

        public XmlSerializableAttribute(string nodeName)
        {
            this.nodeName = nodeName;
        }
    }

	public abstract class Serializable
	{
        private static XmlSerializableAttribute serializableAttribute(Type t)
        {
            return (XmlSerializableAttribute)Attribute.GetCustomAttribute(t, typeof(XmlSerializableAttribute));
        }

        private static bool isSerializable(Type t)
        {
            return serializableAttribute(t) != null;
        }

        static SortedDictionary<string, Type> _serializableTypes;

		static Serializable()
		{
            List<Type> serializableTypes = (from ass in AppDomain.CurrentDomain.GetAssemblies() from t in ass.GetTypes() where isSerializable(t) select t).ToList();

            _serializableTypes = new SortedDictionary<string, Type>();

            foreach (Type t in serializableTypes)
				_serializableTypes.Add(serializableAttribute(t).nodeName, t);
        }

		virtual public Serializable state() { return null; }

        private string nodeName()
        {
            Type type = this.GetType();

            XmlSerializableAttribute attribute = (XmlSerializableAttribute)Attribute.GetCustomAttribute(
                type,
                typeof(XmlSerializableAttribute)
            );

            if (attribute == null)
                throw new SerializationException(type.Name + " is not serializable to xml");

            return attribute.nodeName;
        }

		public XmlNode serialize(XmlNode parent)
        {
            XmlNode node = new XmlNode(nodeName(), parent);
            doSerialize(node);

            return node;
        }

		public XmlNode serializeState(XmlNode parent)
		{
            XmlNode node = new XmlNode(nodeName(), parent);
            doSerializeState(node);

            return node;
		}

        public static Serializable create(XmlNode node)
        {
            Type t = _serializableTypes[node.name];

            if (t == null)
                throw new SerializationException("No serializable type " + node.name);

            Serializable ret = (Serializable)Activator.CreateInstance(t);
            ret.deserialize(node);

            return ret;
        }

		public void deserialize(XmlNode node)
        {
            if (nodeName() != node.name)
                throw new SerializationException("Tried to deserialize from node with wrong name");

            doDeserialize(node);
        }

		public void deserializeState(XmlNode node)
        {
            if (nodeName() != node.name)
                throw new SerializationException("Tried to deserialize from node with wrong name");

            doDeserializeState(node);
        }

        abstract protected void doSerialize(XmlNode node);
        abstract protected void doDeserialize(XmlNode node);

        virtual protected void doSerializeState(XmlNode node) {}
        virtual protected void doDeserializeState(XmlNode node) {}
	}
}