using System.Collections.Generic;
using bjeb.net;

namespace bjeb.game
{
	[Serializable("universe")]
	public class Universe: Serializable
	{
		public double inverseRotation
		{
			get;
			private set;
		}

		public double time
		{
			get;
			private set;
		}

		public double fixedDeltaTime
		{
			get;
			private set;
		}

		public List<CelestialBody> celestialBodies
		{
			get;
			private set;
		}

		override protected void doSerialize(Stream stream)
		{
			stream.write(inverseRotation);
			stream.write(fixedDeltaTime);
			stream.write(time);

			stream.write(celestialBodies.Count);

			foreach(var body in celestialBodies)
				body.serialize(stream);
		}

		override protected void doDeserialize(Stream stream)
		{
			inverseRotation = stream.readDouble();
			fixedDeltaTime = stream.readDouble();
			time = stream.readDouble();

			celestialBodies = new List<CelestialBody>();
			
			int size = stream.readInt();

			for(int i=0;i<size;i++)
			{
				CelestialBody body = new CelestialBody(this);
				body.deserialize(stream);

				celestialBodies.Add(body);
			}
		}

		#if UNITY
		private void _pushBody(global::CelestialBody body)
		{
			celestialBodies.Add(new game.CelestialBody(this, body));

			foreach(var subbody in body.orbitingBodies)
				_pushBody(subbody);
		}

		public void update()
		{
			inverseRotation = Planetarium.InverseRotAngle;
			fixedDeltaTime = TimeWarp.fixedDeltaTime;
			time = Planetarium.GetUniversalTime();

			celestialBodies = new List<CelestialBody>();
			_pushBody(Planetarium.fetch.Sun);
		}
		#endif
	}
}