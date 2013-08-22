using System;
using bjeb.net;
using bjeb.math;

namespace bjeb.game
{
	[XmlSerializable("rigidBody")]
	public class RigidBody: Serializable
	{
		public Vector3 centerOfMass
		{
			get;
			private set;
		}

		public double mass
		{
			get;
			private set;
		}

		public Vector3 torque
		{
			get;
			private set;
		}

		public Vector3 momentumOfInertia
		{
			get;
			private set;
		}

		public Vector3 angularMomentum
		{
			get;
			private set;
		}
		
		public RigidBody()
		{
			centerOfMass = Vector3.zero;
			mass = 0;
			torque = Vector3.zero;
			momentumOfInertia = Vector3.zero;
			angularMomentum = Vector3.zero;
		}

		override protected void doSerialize(XmlNode node)
		{
			centerOfMass.serialize("centerOfMass", node);
			node.attribute("mass").set(mass);
			torque.serialize("torque", node);
			momentumOfInertia.serialize("momentumOfInertia", node);
			angularMomentum.serialize("angularMomentum", node);
		}

		override protected void doDeserialize(XmlNode node)
		{
			centerOfMass.deserialize("centerOfMass", node);
			mass = node.attribute("mass").getDouble();
			torque.deserialize("torque", node);
			momentumOfInertia.deserialize("momentumOfInertia", node);
			angularMomentum.deserialize("angularMomentum", node);
		}

#if UNITY
		public void update(global::Vessel vessel)
		{
            centerOfMass = new Vector3(vessel.findWorldCenterOfMass());

			mass = 0;
			torque = new Vector3(0, 0, 0);
			momentumOfInertia = new Vector3(0, 0, 0);
			angularMomentum = new Vector3(0, 0, 0);

			foreach(Part p in vessel.parts)
            {
                if (p.physicalSignificance != Part.PhysicalSignificance.NONE)
                {
                    double partMass = p.mass + p.GetResourceMass();
                    mass += partMass;
                }

                if (p is CommandPod)
                {
                    torque = torque + Vector3.one * Math.Abs(((CommandPod)p).rotPower);
                }

                foreach (PartModule pm in p.Modules)
                {
                    if (!pm.isEnabled) continue;

                    if (pm is ModuleReactionWheel)
                    {
                        ModuleReactionWheel rw = (ModuleReactionWheel)pm;
                        if (rw.wheelState == ModuleReactionWheel.WheelState.Active)
                        {
                            torque = torque + new Vector3(rw.PitchTorque, rw.RollTorque, rw.YawTorque);
                        }
                    }
				}
            }

            var inertiaTensor = new Matrix3x3();

            foreach (Part p in vessel.parts)
            {
                if (p.Rigidbody == null) continue;

                Vector3d principalMoments = p.Rigidbody.inertiaTensor;
                UnityEngine.Quaternion princAxesRot = UnityEngine.Quaternion.Inverse(vessel.GetTransform().rotation) * p.transform.rotation * p.Rigidbody.inertiaTensorRotation;
                UnityEngine.Quaternion invPrincAxesRot = UnityEngine.Quaternion.Inverse(princAxesRot);

                for (int i = 0; i < 3; i++)
                {
                    Vector3d iHat = Vector3d.zero;
                    iHat[i] = 1;
                    for (int j = 0; j < 3; j++)
                    {
                        Vector3d jHat = Vector3d.zero;
                        jHat[j] = 1;
                        inertiaTensor[i, j] += Vector3d.Dot(iHat, princAxesRot * Vector3d.Scale(principalMoments, invPrincAxesRot * jHat));
                    }
                }

                double partMass = p.mass + p.GetResourceMass();
                UnityEngine.Vector3 partPosition = vessel.GetTransform().InverseTransformDirection(p.Rigidbody.worldCenterOfMass - centerOfMass.unity);

                for (int i = 0; i < 3; i++)
                {
                    inertiaTensor[i, i] += partMass * partPosition.sqrMagnitude;

                    for (int j = 0; j < 3; j++)
                    {
                        inertiaTensor[i, j] += -partMass * partPosition[i] * partPosition[j];
                    }
                }
            }

            momentumOfInertia = new Vector3(inertiaTensor[0, 0], inertiaTensor[2, 2], inertiaTensor[1, 1]);

			Vector3 angularVelocity = new Quaternion(vessel.GetTransform().rotation).inverse * new Vector3(vessel.rigidbody.angularVelocity);

			angularMomentum = inertiaTensor * angularVelocity;
			angularMomentum = new Vector3(angularMomentum.x, angularMomentum.z, angularMomentum.y);
		}
#endif
	}

	/*
  Vessel:
	 orbit
	 parts*/

	[XmlSerializable("vessel")]
	public class Vessel: Serializable
	{
		public RigidBody body
		{
			get;
			private set;
		}

		public Vector3 position
		{
			get
			{
				return body.centerOfMass;
			}
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
				return (position - mainBody.position).normalize;
			}
		}

		public math.Vector3 north
		{
			get
			{
				return (mainBody.position + mainBody.rotation.up * mainBody.radius - position).exclude(up).normalize;
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

		public double longitude
		{
			get
			{
				return up.angleInPlane(mainBody.rotation.right, mainBody.rotation.up);
			}
		}

		public double latitude
		{
			get
			{
				return Math.PI / 2 - up.angle(mainBody.rotation.up);
			}
		}

		public double atmosphericDensity
		{
			get;
			private set;
		}

		public Vessel()
		{
			_rootRotation = new Quaternion();
			_rotatingFrameVelocity = new Vector3();
			gravity = new Vector3();

			mainBody = new CelestialBody();
			body = new RigidBody();
		}

#if UNITY
		public void update(global::Vessel vessel)
		{
			body.update(vessel);
			mainBody.update(vessel.mainBody);

			_rootRotation = new Quaternion(vessel.GetTransform().rotation);

			_rotatingFrameVelocity = new Vector3(vessel.mainBody.getRFrmVel(position.unity));

			gravity = new Vector3(FlightGlobals.getGeeForceAtPosition(position.unity));

			altitude = vessel.mainBody.GetAltitude(position.unity);

            UnityEngine.RaycastHit sfc;
            if (UnityEngine.Physics.Raycast(position.unity, -up.unity, out sfc, (float)altitude + 10000.0F, 1 << 15))
            {
                altitude = sfc.distance;
            }
            else if (vessel.mainBody.pqsController != null)
            {
                altitude -= (vessel.mainBody.pqsController.GetSurfaceHeight(UnityEngine.QuaternionD.AngleAxis(vessel.mainBody.GetLongitude(position.unity), Vector3d.down) * UnityEngine.QuaternionD.AngleAxis(vessel.mainBody.GetLatitude(position.unity), Vector3d.forward) * Vector3d.right) - vessel.mainBody.pqsController.radius);
            }

			double altitudeASL = vessel.mainBody.GetAltitude(position.unity);

			double atmosphericPressure = FlightGlobals.getStaticPressure(altitudeASL, vessel.mainBody);
            if (atmosphericPressure < vessel.mainBody.atmosphereMultiplier * 1e-6) atmosphericPressure = 0;

            atmosphericDensity = FlightGlobals.getAtmDensity(atmosphericPressure);

			//parts (info for rcs, engines, intakes, tanks)
		}
#endif

		override protected void doSerialize(XmlNode node)
		{
			body.serialize(node);
			mainBody.serialize(node);

			_rootRotation.serialize("rootRotation", node);
			_rotatingFrameVelocity.serialize("rotatingFrameVelocity", node);
			gravity.serialize("gravity", node);

			node.attribute("altitude").set(altitude);
			node.attribute("atmosphericDensity").set(atmosphericDensity);
		}

		override protected void doDeserialize(XmlNode node)
		{
			body.deserialize(node.node("rigidBody"));
			mainBody.deserialize(node.node("celestialBody"));

			_rootRotation.deserialize("rootRotation", node);
			_rotatingFrameVelocity.deserialize("rotatingFrameVelocity", node);
			gravity.deserialize("gravity", node);

			altitude = node.attribute("altitude").getDouble();
			atmosphericDensity = node.attribute("atmosphericDensity").getDouble();
		}
	}
}