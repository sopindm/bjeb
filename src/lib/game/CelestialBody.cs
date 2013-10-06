using bjeb.net;
using bjeb.math;

namespace bjeb.game
{
	[net.Serializable("celestialBody")]
	public class CelestialBody: Serializable
	{
		public string name
		{
			get;
			private set;
		}

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

		public Universe universe
		{
			get;
			private set;
		}

		public CelestialBody(Universe universe)
		{
			position = new Vector3();
			rotation = new Quaternion();

			radius = 0;

			mass = 0;
			gravParameter = 0;

			this.universe = universe;
		}

#if UNITY
		public CelestialBody(Universe universe, global::CelestialBody body): this(universe)
		{
			update(body);
		}
#endif

		override protected void doSerialize(Stream stream)
		{
			stream.write(name);

		    position.serialize(stream);
		    rotation.serialize(stream);

		    stream.write(radius);

			stream.write(mass);
			stream.write(gravParameter);
		}

		override protected void doDeserialize(Stream stream)
		{
			name = stream.readString();

		    position.deserialize(stream);
		    rotation.deserialize(stream);

		    radius = stream.readDouble();

			mass = stream.readDouble();
			gravParameter = stream.readDouble();
		}

#if UNITY
		public void update(global::CelestialBody body)
		{
			name = body.GetName();

			position = new Vector3(body.position);
			rotation = new Quaternion(body.rotation);

			radius = body.Radius;

			mass = body.Mass;
			gravParameter = body.gravParameter;
		}
#endif
	}

}