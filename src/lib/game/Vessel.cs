using System;
using bjeb.net;
using bjeb.math;

namespace bjeb.game
{
#if UNITY
    class Matrix3x3
    {
        //row index, then column index
        private double[,] e = new double[3, 3];

        public double this[int i, int j] 
        {
            get { return e[i, j]; }
            set { e[i, j] = value; }
        }

        public static Vector3d operator *(Matrix3x3 M, Vector3d v) 
        {
            Vector3d ret = Vector3d.zero;
            for(int i = 0; i < 3; i++) {
                for(int j = 0; j < 3; j++) {
                    ret[i] += M.e[i, j] * v[j];
                }
            }
            return ret;
        }
    }
#endif

	/*
  Vessel:
	 orbit
	 parts*/


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
		
		public Vessel()
		{
			centerOfMass = new Vector3();
			_rootRotation = new Quaternion();
			_rotatingFrameVelocity = new Vector3();
			gravity = new Vector3();

			mainBody = new CelestialBody();

			torque = new Vector3();
			momentumOfInertia = new Vector3();
			angularMomentum = new Vector3();
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

			double altitudeASL = vessel.mainBody.GetAltitude(centerOfMass.unity);

			double atmosphericPressure = FlightGlobals.getStaticPressure(altitudeASL, vessel.mainBody);
            if (atmosphericPressure < vessel.mainBody.atmosphereMultiplier * 1e-6) atmosphericPressure = 0;

            atmosphericDensity = FlightGlobals.getAtmDensity(atmosphericPressure);

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

			torque = new Vector3(torque.x, torque.z, torque.y);

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

			Vector3d angularVelocity = UnityEngine.Quaternion.Inverse(vessel.GetTransform().rotation) * vessel.rigidbody.angularVelocity;
			Vector3d angularMomentumUnity = inertiaTensor * angularVelocity;

            angularMomentum = new Vector3(angularMomentumUnity.x, angularMomentumUnity.z, angularMomentumUnity.y);

			//parts (info for rcs, engines, intakes, tanks)
		}
#endif

		override protected void doSerialize(XmlNode node)
		{
			centerOfMass.serialize("centerOfMass", node);
			_rootRotation.serialize("rootRotation", node);
			_rotatingFrameVelocity.serialize("rotatingFrameVelocity", node);
			gravity.serialize("gravity", node);

			node.attribute("altitude").set(altitude);
			node.attribute("atmosphericDensity").set(atmosphericDensity);
			node.attribute("mass").set(mass);

			momentumOfInertia.serialize("momentumOfInertia", node);
			angularMomentum.serialize("angularMomentum", node);
			torque.serialize("torque", node);

			mainBody.serialize(node);
		}

		override protected void doDeserialize(XmlNode node)
		{
			centerOfMass.deserialize("centerOfMass", node);
			_rootRotation.deserialize("rootRotation", node);
			_rotatingFrameVelocity.deserialize("rotatingFrameVelocity", node);
			gravity.deserialize("gravity", node);

			altitude = node.attribute("altitude").getDouble();
			atmosphericDensity = node.attribute("atmosphericDensity").getDouble();
			torque.deserialize("torque", node);
			mass = node.attribute("mass").getDouble();

			momentumOfInertia.deserialize("momentumOfInertia", node);
			angularMomentum.deserialize("angularMomentum", node);

			mainBody.deserialize(node.node("celestialBody"));
		}
	}
}