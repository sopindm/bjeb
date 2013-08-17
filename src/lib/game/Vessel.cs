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
		}

#if UNITY
		public void update(global::Vessel vessel)
		{
            Vector3d CoM = vessel.findWorldCenterOfMass();
            Vector3d up = (CoM - vessel.mainBody.position).normalized;

            Vector3d north = Vector3d.Exclude(up, (vessel.mainBody.position + vessel.mainBody.transform.up * (float)vessel.mainBody.Radius) - CoM).normalized;

            Quaternion rotationSurface = Quaternion.LookRotation(north, up);
            Quaternion rotationVesselSurface = Quaternion.Inverse(Quaternion.Euler(90, 0, 0) * Quaternion.Inverse(vessel.GetTransform().rotation) * rotationSurface);

			yaw = rotationVesselSurface.eulerAngles.y;
			pitch = (rotationVesselSurface.eulerAngles.x > 180) ? (360.0 - rotationVesselSurface.eulerAngles.x) : -rotationVesselSurface.eulerAngles.x;
			roll = (rotationVesselSurface.eulerAngles.z > 180) ? (rotationVesselSurface.eulerAngles.z - 360.0) : rotationVesselSurface.eulerAngles.z;
		}
#endif

		override protected void doSerialize(XmlNode node)
		{
			node.attribute("yaw").set(yaw);
			node.attribute("pitch").set(pitch);
			node.attribute("roll").set(roll);
		}

		override protected void doDeserialize(XmlNode node)
		{
			yaw = node.attribute("yaw").getFloat();
			pitch = node.attribute("pitch").getFloat();
			roll = node.attribute("roll").getFloat();
		}
	}
}