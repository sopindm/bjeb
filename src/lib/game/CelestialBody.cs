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
	[XmlSerializable("celestialBody")]
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

		override protected void doSerialize(XmlNode node)
		{
			position.serialize("position", node);
			rotation.serialize("rotation", node);
			
			node.attribute("radius").set(radius);
		}

		override protected void doDeserialize(XmlNode node)
		{
			position.deserialize("position", node);
			rotation.deserialize("rotation", node);
			
			radius = node.attribute("radius").getDouble();
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