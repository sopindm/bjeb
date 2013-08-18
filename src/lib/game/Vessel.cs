#if UNITY
using UnityEngine;
#endif
using bjeb.net;

namespace bjeb.game
{
	/*
  Vessel:
     position:
	   forward
	   up

       north
	   east

	   orbit
	   rotatingFrameVelocity

	   altitude
	   altitudeTrue
	   altitudeBotton

	 physics:
       centerOfMass
       momentumOfInertia

	   mass

	   angularVelocity
	   angularMomentum

	   torque
	   thrustTorque*/

	[XmlSerializable("vessel")]
	public class Vessel: Serializable
	{
		public math.Vector3 centerOfMass;

		public double yaw
		{
			get;
			private set;
		}

		public double pitch
		{
			get;
			private set;
		}

		public double roll
		{
			get;
			private set;
		}

		public Vessel()
		{
			centerOfMass = new math.Vector3();
		}

#if UNITY
		public void update(global::Vessel vessel)
		{
            centerOfMass = new math.Vector3(vessel.findWorldCenterOfMass());

			var mainBodyPosition = new math.Vector3(vessel.mainBody.position);
            math.Vector3 up = (centerOfMass - mainBodyPosition).normalize;

			math.Vector3 north = (mainBodyPosition + new math.Vector3(vessel.mainBody.transform.up) * vessel.mainBody.Radius - centerOfMass).exclude(up).normalize;

			var surfaceRotation = math.Quaternion.look(north, up);

            Quaternion rotationVesselSurface = Quaternion.Inverse(Quaternion.Euler(90, 0, 0) * Quaternion.Inverse(vessel.GetTransform().rotation) * surfaceRotation.unity);

			yaw = rotationVesselSurface.eulerAngles.y;
			pitch = (rotationVesselSurface.eulerAngles.x > 180) ? (360.0 - rotationVesselSurface.eulerAngles.x) : -rotationVesselSurface.eulerAngles.x;
			roll = (rotationVesselSurface.eulerAngles.z > 180) ? (rotationVesselSurface.eulerAngles.z - 360.0) : rotationVesselSurface.eulerAngles.z;
		}
#endif

		override protected void doSerialize(XmlNode node)
		{
			centerOfMass.serialize("centerOfMass", node);

			node.attribute("yaw").set(yaw);
			node.attribute("pitch").set(pitch);
			node.attribute("roll").set(roll);
		}

		override protected void doDeserialize(XmlNode node)
		{
			centerOfMass.deserialize("centerOfMass", node);

			yaw = node.attribute("yaw").getFloat();
			pitch = node.attribute("pitch").getFloat();
			roll = node.attribute("roll").getFloat();
		}
	}
}