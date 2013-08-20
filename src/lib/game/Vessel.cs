using System;
using bjeb.net;
using bjeb.math;

namespace bjeb.game
{
	/*
  Vessel:
	 orbit
	 rotatingFrameVelocity

	 altitude
	 altitudeTrue

	 parts

	 mass
	 angularVelocity*/

	[XmlSerializable("vessel")]
	public class Vessel: Serializable
	{
		public Vector3 centerOfMass;

		private Quaternion _rootRotation;
		public Quaternion rotation
		{
			get
			{
				return _rootRotation * Quaternion.makePitch(-Math.PI / 2);
			}
		}

		public math.Vector3 up
		{
			get
			{
				return centerOfMass.normalize;
			}
		}

		public math.Vector3 north
		{
			get
			{
				return (mainBody.rotation.up * mainBody.radius - centerOfMass).exclude(up).normalize;
			}
		}

		public math.Quaternion surfaceRotation
		{
			get
			{
				return (rotation.inverse * Quaternion.look(north, up)).inverse;
			}
		}

		public CelestialBody mainBody;

		public Vessel()
		{
			centerOfMass = new Vector3();
			_rootRotation = new Quaternion();
			mainBody = new CelestialBody();
		}

#if UNITY
		public void update(global::Vessel vessel)
		{
			mainBody.update(vessel.mainBody);

            centerOfMass = new Vector3(vessel.findWorldCenterOfMass()) - mainBody.position;
			_rootRotation = new Quaternion(vessel.GetTransform().rotation);

		}
#endif

		override protected void doSerialize(XmlNode node)
		{
			centerOfMass.serialize("centerOfMass", node);
			_rootRotation.serialize("rootRotation", node);
			mainBody.serialize(node);
		}

		override protected void doDeserialize(XmlNode node)
		{
			centerOfMass.deserialize("centerOfMass", node);
			_rootRotation.deserialize("rootRotation", node);
			mainBody.deserialize(node.node("celestialBody"));
		}
	}
}