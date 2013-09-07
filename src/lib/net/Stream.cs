using System;
using System.Collections.Generic;

namespace bjeb
{
    namespace net
    {
		public class Stream
		{
			System.IO.BinaryReader _reader;
			System.IO.BinaryWriter _writer;

			public Stream(System.IO.BinaryReader reader, System.IO.BinaryWriter writer)
			{
				_reader = reader;
				_writer = writer;
			}

			private void writeSimpleTag(byte tag)
			{
				_writer.Write(tag);
			}

			public void writeNull()
			{
				writeSimpleTag(0);
			}

			public void write(bool value)
			{
				writeSimpleTag(1);
				_writer.Write(value);
			}

			public void write(bool? value)
			{
				if(value != null)
					write(value.Value);
				else
					writeNull();
			}

			public void write(int value)
			{
				writeSimpleTag(2);
				_writer.Write(value);
			}

			public void write(int? value)
			{
				if(value != null)
					write(value.Value);
				else
					writeNull();
			}

			public void write(float value)
			{
				writeSimpleTag(3);
				_writer.Write(value);
			}

			public void write(float? value)
			{
				if(value != null)
					write(value.Value);
				else
					writeNull();
			}

			public void write(double value)
			{
				writeSimpleTag(4);
				_writer.Write(value);
			}

			public void write(double? value)
			{
				if(value != null)
					write(value.Value);
				else
					writeNull();
			}

			public void write(string value)
			{
				writeSimpleTag(5);
				_writer.Write(value);
			}

			public void writeTag(UInt16 tag, bool open)
			{
				if(tag > 4095)
					throw new System.ArgumentException("Invalid tag value");

				tag = (UInt16)((tag * 2 + (open ? 1 : 0)) * 8 + 7);

				_writer.Write((byte)(tag % 256));
				_writer.Write((byte)(tag / 256));
			}

			private byte readSimpleTag()
			{
				return _reader.ReadByte();
			}

			private bool _readBool(byte tag)
			{
				if(tag != 1)
					throw new System.ArgumentException("Invalid boolean tag " + tag.ToString());

				return _reader.ReadBoolean();
			}

			public bool readBool()
			{
				return _readBool(readSimpleTag());
			}

			public bool? tryReadBool()
			{
				byte tag = readSimpleTag();

				if(tag == 0)
					return null;

				return _readBool(tag);
			}

			private int _readInt(byte tag)
			{
				if(tag != 2)
					throw new System.ArgumentException("Invalid integer tag");

				return _reader.ReadInt32();
			}

			public int readInt()
			{
				return _readInt(readSimpleTag());
			}

			public int? tryReadInt()
			{
				byte tag = readSimpleTag();

				if(tag == 0)
					return null;

				return _readInt(tag);
			}

			private float _readFloat(byte tag)
			{
				if(tag != 3)
					throw new System.ArgumentException("Invalid float tag " + tag.ToString());

				return _reader.ReadSingle();
			}

			public float readFloat()
			{
				return _readFloat(readSimpleTag());
			}

			public float? tryReadFloat()
			{
				byte tag = readSimpleTag();

				if(tag == 0)
					return null;

				return _readFloat(tag);
			}

			private double _readDouble(byte tag)
			{
				if(tag != 4)
					throw new System.ArgumentException("Invalid double tag");

				return _reader.ReadDouble();
			}

			public double readDouble()
			{
				return _readDouble(readSimpleTag());
			}

			public double? tryReadDouble()
			{
				byte tag = readSimpleTag();

				if(tag == 0)
					return null;

				return _readDouble(tag);
			}

			private string _readString(int tag)
			{
				if(tag != 5)
					throw new System.ArgumentException("Invalid string tag " + tag.ToString());

				return _reader.ReadString();
			}

			public string readString()
			{
				return _readString(readSimpleTag());
			}

			public string tryReadString()
			{
				byte tag = readSimpleTag();

				if(tag == 0)
					return null;

				return _readString(tag);
			}

			private UInt16 _readTag(byte tag1, bool open)
			{
				if(tag1 < 7)
					throw new System.ArgumentException("Invalid complex tag");
	
				UInt16 tag = (UInt16)(((UInt16)tag1) + (UInt16)(((UInt16)readSimpleTag()) * 256));
				tag = (UInt16)(tag / 8);

				if(tag % 2 != (open ? 1 : 0))
					throw new System.ArgumentException("Invalid complex tag");

				tag /= 2; 

				return tag;
			}

			public UInt16 readTag(bool open)
			{
				return _readTag(readSimpleTag(), open);
			}

			public UInt16? tryReadTag(bool open)
			{
				byte tag = readSimpleTag();
		
				if(tag == 0)
					return null;

				return _readTag(tag, open);
			}

			public void checkTag(UInt16 tag, bool open)
			{
				if(readTag(open) != tag)
					throw new System.ArgumentException("Wrong complex tag");
			}
		}
    }
}
