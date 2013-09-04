using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using bjeb.math;

namespace MuMech
{
    public class MechJebModuleAttitudeController
    {
		private VesselState vesselState;

        public PIDControllerV pid;
        public Vector3d lastAct = Vector3d.zero;


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
			if(vesselState == null)
				vesselState = new VesselState();

			if(pid == null)
				pid = new PIDControllerV(Kp, Ki, Kd, Ki_limit, -Ki_limit);

			vesselState.Update(vessel);
		}

        private double _lastRoll = 0;

        public void Drive(bjeb.game.Vessel vessel, FlightCtrlState s)
        {
            double precision = (Math.Min(vessel.body.torque.x, vessel.body.torque.y) * 20.0 / vessel.body.momentumOfInertia.magnitude).clamp(0.5, 10);

            double rollDelta = Math.Abs(vessel.surfaceRotation.roll - _lastRoll);
            if (rollDelta > Math.PI)
                rollDelta = 2 * Math.PI - rollDelta;
            if (rollDelta > Math.PI / 36)
            {
                pid.Reset();
                _lastRoll = vessel.surfaceRotation.roll;
            }

            // Direction we want to be facing
			bjeb.math.Quaternion target = bjeb.math.Quaternion.look(vessel.north, vessel.up);
			bjeb.math.Quaternion delta = target.inverse * vessel.rotation;

            bjeb.math.Vector3 err = new bjeb.math.Vector3(-((delta.pitch > Math.PI) ? (delta.pitch - 2 * Math.PI) : delta.pitch),
														  -((delta.yaw > Math.PI) ? (delta.yaw - 2 * Math.PI) : delta.yaw),
														  -((delta.roll > Math.PI) ? (delta.roll - 2 * Math.PI) : delta.roll));

            bjeb.math.Vector3 torque = vessel.body.torque;

            bjeb.math.Vector3 inertia = vessel.body.angularMomentum.sign *
				vessel.body.angularMomentum * vessel.body.angularMomentum * 
				(torque * vessel.body.momentumOfInertia).invert;

            err += inertia;
			err *= vessel.body.momentumOfInertia * torque.invert;

            Vector3d act = pid.Compute(err.unity);

            float drive_limit = Mathf.Clamp01((float)(err.magnitude * drive_factor / precision));

            act.x = Mathf.Clamp((float)act.x, drive_limit * -1, drive_limit);
            act.y = Mathf.Clamp((float)act.y, drive_limit * -1, drive_limit);
            act.z = Mathf.Clamp((float)act.z, drive_limit * -1, drive_limit);

            act = lastAct + (act - lastAct) * (TimeWarp.fixedDeltaTime / Tf);

            SetFlightCtrlState(act, s, precision, drive_limit);

            act = new Vector3d(s.pitch, s.yaw, s.roll);
            lastAct = act;
        }

        private void SetFlightCtrlState(Vector3d act, FlightCtrlState s, double precision, float drive_limit)
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
