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
		public Vector3 centerOfMass
		{
			get;
			private set;
		}

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
				return (centerOfMass - mainBody.position).normalize;
			}
		}

		public math.Vector3 north
		{
			get
			{
				return (mainBody.position + mainBody.rotation.up * mainBody.radius - centerOfMass).exclude(up).normalize;
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

		private Vector3 _rotatingFrameVelocity;

		public Vector3 east
		{
			get
			{
				return north.cross(up);
			}
		}

		public Vector3 gravity
		{
			get;
			private set;
		}

		public double altitude
		{
			get;
			private set;
		}

		public Vessel()
		{
			centerOfMass = new Vector3();
			_rootRotation = new Quaternion();
			_rotatingFrameVelocity = new Vector3();
			gravity = new Vector3();

			mainBody = new CelestialBody();
		}

#if UNITY
		public void update(global::Vessel vessel)
		{
			mainBody.update(vessel.mainBody);

            centerOfMass = new Vector3(vessel.findWorldCenterOfMass());
			_rootRotation = new Quaternion(vessel.GetTransform().rotation);

			_rotatingFrameVelocity = new Vector3(vessel.mainBody.getRFrmVel(centerOfMass.unity));

			gravity = new Vector3(FlightGlobals.getGeeForceAtPosition(centerOfMass.unity));

			altitude = vessel.mainBody.GetAltitude(centerOfMass.unity);

            UnityEngine.RaycastHit sfc;
            if (UnityEngine.Physics.Raycast(centerOfMass.unity, -up.unity, out sfc, (float)altitude + 10000.0F, 1 << 15))
            {
                altitude = sfc.distance;
            }
            else if (vessel.mainBody.pqsController != null)
            {
                altitude -= (vessel.mainBody.pqsController.GetSurfaceHeight(UnityEngine.QuaternionD.AngleAxis(vessel.mainBody.GetLongitude(centerOfMass.unity), Vector3d.down) * UnityEngine.QuaternionD.AngleAxis(vessel.mainBody.GetLatitude(centerOfMass.unity), Vector3d.forward) * Vector3d.right) - vessel.mainBody.pqsController.radius);
            }

			//add longitude and latitude methods
		}
#endif

		override protected void doSerialize(XmlNode node)
		{
			centerOfMass.serialize("centerOfMass", node);
			_rootRotation.serialize("rootRotation", node);
			_rotatingFrameVelocity.serialize("rotatingFrameVelocity", node);
			gravity.serialize("gravity", node);

			node.attribute("altitude").set(altitude);

			mainBody.serialize(node);
		}

		override protected void doDeserialize(XmlNode node)
		{
			centerOfMass.deserialize("centerOfMass", node);
			_rootRotation.deserialize("rootRotation", node);
			_rotatingFrameVelocity.deserialize("rotatingFrameVelocity", node);
			gravity.deserialize("gravity", node);

			altitude = node.attribute("altitude").getDouble();

			mainBody.deserialize(node.node("celestialBody"));
		}
	}
}