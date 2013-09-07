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
        public string tag
		{
			get;
			private set;
		}

        public SerializableAttribute(string tag)
		{
			this.tag = tag;
		}
    }

    public abstract class Serializable
    {
		protected Serializable()
		{
			Type type = this.GetType();
			tag = _typeTags[type];
		}

        private static SerializableAttribute serializableAttribute(Type t)
		{
			return (SerializableAttribute)Attribute.GetCustomAttribute(t, typeof(SerializableAttribute));
		}

        private static bool isSerializable(Type t)
		{
			return serializableAttribute(t) != null;
		}

        static SortedDictionary<UInt16, Type> _serializableTypes;
        static Dictionary<Type, UInt16> _typeTags;

		static Serializable()
		{
			List<Type> serializableTypes = (from ass in AppDomain.CurrentDomain.GetAssemblies() from t in ass.GetTypes() where isSerializable(t) select t).ToList();

			var typesDictionary = new SortedDictionary<string, Type>();

			foreach(Type t in serializableTypes)
			{
				if(typesDictionary.ContainsKey(serializableAttribute(t).tag))
					throw new SerializationException("Cannot define type " + t.ToString() + " with tag " + serializableAttribute(t).tag.ToString() + ". It's already defined.");

				typesDictionary.Add(serializableAttribute(t).tag, t);
			}

			_serializableTypes = new SortedDictionary<UInt16, Type>();
			_typeTags = new Dictionary<Type, UInt16>();

			UInt16 id = 0;
			foreach(var keyValue in typesDictionary)
			{
				_serializableTypes.Add(id, keyValue.Value);
				_typeTags.Add(keyValue.Value, id);

				id++;
			}
		}

		protected UInt16 tag
		{
			get;
			private set;
		}

		public void serialize(Stream stream)
		{
			stream.writeTag(tag, true);
			doSerialize(stream);
			stream.writeTag(tag, false);
		}

		virtual public void serializeState(Stream stream)
		{
			stream.writeTag(tag, true);
			doSerializeState(stream);
			stream.writeTag(tag, false);
		}

		virtual protected void doSerializeState(Stream stream) {}

		private static Serializable _create(Stream stream, UInt16 tag)
		{
			Type t = _serializableTypes[tag];

			if (t == null)
				throw new SerializationException("No serializable type with tag " + tag.ToString());

			Serializable ret = (Serializable)Activator.CreateInstance(t);
			ret.deserialize(stream, tag);

			return ret;
		}

        public static Serializable create(Stream stream)
		{
			return _create(stream, stream.readTag(true));
		}

		public static Serializable tryCreate(Stream stream)
		{
			var tag = stream.tryReadTag(true);

			if(tag == null)
				return null;

			return _create(stream, tag.Value);
		}

		public void deserialize(Stream stream)
		{
			deserialize(stream, stream.readTag(true));
		}

		private void deserialize(Stream stream, UInt16 tag)
		{
			if (this.tag != tag)
				throw new SerializationException("Tried to deserialize from node with wrong tag");

			doDeserialize(stream);

			if(stream.readTag(false) != tag)
				throw new SerializationException("Wrong closing tag");
		}

		virtual public void deserializeState(Stream stream)
		{
			if (stream.readTag(true) != this.tag)
				throw new SerializationException("Tried to deserialize state from node with wrong tag");

			doDeserializeState(stream);

			if(stream.readTag(false) != tag)
				throw new SerializationException("Wrong closing tag");
		}

		virtual protected void doDeserializeState(Stream stream)
		{
		}

        abstract protected void doSerialize(Stream node);
        abstract protected void doDeserialize(Stream node);
    }
}