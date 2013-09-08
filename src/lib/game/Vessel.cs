using System;
using bjeb.net;
using bjeb.math;

namespace bjeb.game
{
	[net.Serializable("rigidBody")]
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

		public double timeDelta
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

		override protected void doSerialize(Stream stream)
		{
		    centerOfMass.serialize(stream);
		    stream.write(mass);
		    torque.serialize(stream);
		    momentumOfInertia.serialize(stream);
		    angularMomentum.serialize(stream);
			stream.write(timeDelta);
		}

		override protected void doDeserialize(Stream stream)
		{
		    centerOfMass.deserialize(stream);
		    mass = stream.readDouble();
		    torque.deserialize(stream);
		    momentumOfInertia.deserialize(stream);
		    angularMomentum.deserialize(stream);
			timeDelta = stream.readDouble();
		}

#if UNITY
		public void update(global::Vessel vessel)
		{
            centerOfMass = new Vector3(vessel.findWorldCenterOfMass());

			mass = 0;
			torque = new Vector3(0, 0, 0);
			momentumOfInertia = new Vector3(0, 0, 0);
			angularMomentum = new Vector3(0, 0, 0);

			timeDelta = TimeWarp.fixedDeltaTime;

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

	[net.Serializable("vessel")]
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

		public Quaternion rootRotation
		{
			get
			{
				return _rootRotation;
			}
		}

		public Quaternion rotation
		{
			get
			{
				return _rootRotation * Quaternion.makePitch(Math.PI / 2);
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

		public math.Orbit orbit
		{
			get;
			private set;
		}

		public CelestialBody mainBody
		{
			get
			{
				return orbit.mainBody;
			}
		}

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

			orbit = new math.Orbit();
			body = new RigidBody();
		}

#if UNITY
		public void update(global::Vessel vessel)
		{
			body.update(vessel);
			orbit.update(vessel.orbit, Planetarium.GetUniversalTime());

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

		public void updateState(global::Vessel vessel)
		{
			_rootRotation = new Quaternion(vessel.GetTransform().rotation);
		}
#endif

		override protected void doSerialize(Stream stream)
		{
			body.serialize(stream);
			orbit.serialize(stream);

			_rootRotation.serialize(stream);
			_rotatingFrameVelocity.serialize(stream);
			gravity.serialize(stream);

			stream.write(altitude);
			stream.write(atmosphericDensity);
		}

		override protected void doDeserialize(Stream stream)
		{
		    body.deserialize(stream);
		    orbit.deserialize(stream);

		    _rootRotation.deserialize(stream);
		    _rotatingFrameVelocity.deserialize(stream);
		    gravity.deserialize(stream);

		    altitude = stream.readDouble();
		    atmosphericDensity = stream.readDouble();
		}

		override protected void doSerializeState(Stream stream)
		{
			_rootRotation.serialize(stream);
		}

		override protected void doDeserializeState(Stream stream)
		{
			_rootRotation.deserialize(stream);
		}
	}
}