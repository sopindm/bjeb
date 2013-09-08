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
	[net.Serializable("celestialBody")]
	public class CelestialBody: Serializable
	{
		public Vector3 position
		{
			get;
			private set;
		}

		public Quaternion rotation
		{
			get;
			private set;

		}

		public double radius
		{
			get;
			private set;
		}
		
		public double mass
		{
			get;
			private set;
		}

		public double gravParameter
		{
			get;
			private set;
		}

		public CelestialBody()
		{
			position = new Vector3();
			rotation = new Quaternion();

			radius = 0;

			mass = 0;
			gravParameter = 0;
		}

		override protected void doSerialize(Stream stream)
		{
		    position.serialize(stream);
		    rotation.serialize(stream);

		    stream.write(radius);

			stream.write(mass);
			stream.write(gravParameter);
		}

		override protected void doDeserialize(Stream stream)
		{
		    position.deserialize(stream);
		    rotation.deserialize(stream);

		    radius = stream.readDouble();

			mass = stream.readDouble();
			gravParameter = stream.readDouble();
		}

#if UNITY
		public void update(global::CelestialBody body)
		{
			position = new Vector3(body.position);
			rotation = new Quaternion(body.rotation);

			radius = body.Radius;

			mass = body.Mass;
			gravParameter = body.gravParameter;
		}
#endif
	}

}