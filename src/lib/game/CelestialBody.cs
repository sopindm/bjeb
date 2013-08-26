using bjeb.net;
using bjeb.math;

/*
  Main body - celectial body:
     name

	 g
	 G

	 orbit*/

namespace bjeb.game
{
	[net.Serializable(12)]
	public class CelestialBody: Serializable
	{
		public Vector3 position;
		public Quaternion rotation;

		public double radius;

		public CelestialBody()
		{
			position = new Vector3();
			rotation = new Quaternion();

			radius = 0;
		}

		override protected void doSerialize(Stream stream)
		{
		    position.serialize(stream);
		    rotation.serialize(stream);
		    stream.write(radius);
		}

		override protected void doDeserialize(Stream stream)
		{
		    position.deserialize(stream);
		    rotation.deserialize(stream);
		    radius = stream.readDouble();
		}

#if UNITY
		public void update(global::CelestialBody body)
		{
			position = new Vector3(body.position);
			rotation = new Quaternion(body.GetTransform().rotation);

			radius = body.Radius;
		}
#endif
	}

}