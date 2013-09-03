using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using bjeb.math;

namespace MuMech
{
    public class MechJebModuleAttitudeController
    {
		private Vessel vessel;
		private VesselState vesselState;

        public PIDControllerV pid;
        public Vector3d lastAct = Vector3d.zero;
        private double lastResetRoll = 0;

        public double Kp = 10000;
        public double Ki = 0;
        public double Kd = 800;
        public double Ki_limit = 1000000;
        public double Tf = 0.1;
        public double drive_factor = 100000;

        public bool attitudeKILLROT = false;

        public MechJebModuleAttitudeController()
        {
        }

        public void OnStart()
        {
        }

		public void Update(Vessel vessel)
		{
			this.vessel = vessel;

			if(vesselState == null)
				vesselState = new VesselState();

			if(pid == null)
				pid = new PIDControllerV(Kp, Ki, Kd, Ki_limit, -Ki_limit);

			vesselState.Update(vessel);
		}

        public void Drive(FlightCtrlState s)
        {
            // Used in the killRot activation calculation and drive_limit calculation
            double precision = Math.Max(
                0.5,
                Math.Min(
                10.0,
                (Math.Min(
                vesselState.torqueAvailable.x,
                vesselState.torqueAvailable.z
            ) + vesselState.torqueThrustPYAvailable * s.mainThrottle) * 20.0 / vesselState.MoI.magnitude
            )
            );

            // Reset the PID controller during roll to keep pitch and yaw errors
            // from accumulating on the wrong axis.
            double rollDelta = Mathf.Abs((float)(vesselState.vesselRoll - lastResetRoll));
            if (rollDelta > 180)
                rollDelta = 360 - rollDelta;
            if (rollDelta > 5)
            {
                pid.Reset();
                lastResetRoll = vesselState.vesselRoll;
            }

            // Direction we want to be facing
            Quaternion target = Quaternion.LookRotation(vesselState.north, vesselState.up);
            Quaternion delta = Quaternion.Inverse(Quaternion.Euler(90, 0, 0) * Quaternion.Inverse(vessel.GetTransform().rotation) * target);

            Vector3d deltaEuler = new Vector3d(
                                                    (delta.eulerAngles.x > 180) ? (delta.eulerAngles.x - 360.0F) : delta.eulerAngles.x,
                                                    -((delta.eulerAngles.y > 180) ? (delta.eulerAngles.y - 360.0F) : delta.eulerAngles.y),
                                                    (delta.eulerAngles.z > 180) ? (delta.eulerAngles.z - 360.0F) : delta.eulerAngles.z
            );

            Vector3d torque = new Vector3d(
                                                    vesselState.torqueAvailable.x + vesselState.torqueThrustPYAvailable * s.mainThrottle,
                                                    vesselState.torqueAvailable.y,
                                                    vesselState.torqueAvailable.z + vesselState.torqueThrustPYAvailable * s.mainThrottle
            );

			

            Vector3d inertia = Vector3d.Scale(
                                                    vesselState.angularMomentum.Sign(),
                                                    Vector3d.Scale(
                                                        Vector3d.Scale(
                vesselState.angularMomentum,
                vesselState.angularMomentum
            ),
                                                        Vector3d.Scale(
                torque,
                vesselState.MoI
            )
                .Invert()
            )
            );

            Vector3d err = deltaEuler * Math.PI / 180.0F;
            err += inertia.Reorder(132);
            err.Scale(Vector3d.Scale(vesselState.MoI, torque.Invert()).Reorder(132));

            Vector3d act = pid.Compute(err);

            float drive_limit = Mathf.Clamp01((float)(err.magnitude * drive_factor / precision));

            act.x = Mathf.Clamp((float)act.x, drive_limit * -1, drive_limit);
            act.y = Mathf.Clamp((float)act.y, drive_limit * -1, drive_limit);
            act.z = Mathf.Clamp((float)act.z, drive_limit * -1, drive_limit);

            act = lastAct + (act - lastAct) * (TimeWarp.fixedDeltaTime / Tf);

            SetFlightCtrlState(act, deltaEuler, s, precision, drive_limit);

            act = new Vector3d(s.pitch, s.yaw, s.roll);
            lastAct = act;
        }

        private void SetFlightCtrlState(Vector3d act, Vector3d deltaEuler, FlightCtrlState s, double precision, float drive_limit)
        {
			if (!double.IsNaN(act.z)) s.roll = Mathf.Clamp((float)(act.z), -drive_limit, drive_limit);
			if (Math.Abs(s.roll) < 0.05)
			{
				s.roll = 0;
			}

			if (!double.IsNaN(act.x)) s.pitch = Mathf.Clamp((float)(act.x), -drive_limit, drive_limit);
			if (!double.IsNaN(act.y)) s.yaw = Mathf.Clamp((float)(act.y), -drive_limit, drive_limit);

			if (Math.Abs(s.pitch) < 0.05)
			{
				s.pitch = 0;
			}

			if (Math.Abs(s.yaw) < 0.05)
			{
				s.yaw = 0;
			}
        }
    }
}
