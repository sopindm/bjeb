using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using bjeb.math;

namespace bjeb;
{
    public class AttitudeController
    {
        private PIDControllerV _pid;

        private const double Tf = 0.1;
        private const double driveFactor = 100000;

        public AttitudeController()
        {
			_pid = new PIDControllerV(10000, 0, 800, 1000000, -1000000);
        }

        private double _lastRoll = 0;
        private Vector3 _act = Vector3.zero;

        public void drive(bjeb.game.Vessel vessel, bjeb.game.FlightControl c)
        {
            double rollDelta = Math.Abs(vessel.surfaceRotation.roll - _lastRoll);
            if (rollDelta > Math.PI)
                rollDelta = 2 * Math.PI - rollDelta;
            if (rollDelta > Math.PI / 36)
            {
                _pid.Reset();
                _lastRoll = vessel.surfaceRotation.roll;
            }

            // Direction we want to be facing
			Quaternion target = Quaternion.look(vessel.north, vessel.up);
			Quaternion delta = target.inverse * vessel.rotation;

            Vector3 err = new Vector3(-((delta.pitch > Math.PI) ? (delta.pitch - 2 * Math.PI) : delta.pitch),
									  -((delta.yaw > Math.PI) ? (delta.yaw - 2 * Math.PI) : delta.yaw),
									  -((delta.roll > Math.PI) ? (delta.roll - 2 * Math.PI) : delta.roll));

            Vector3 torque = vessel.body.torque;

            Vector3 inertia = vessel.body.angularMomentum.sign *
				vessel.body.angularMomentum * vessel.body.angularMomentum * 
				(torque * vessel.body.momentumOfInertia).invert;

            err += inertia;
			err *= vessel.body.momentumOfInertia * torque.invert;

            Vector3 act = new Vector3(_pid.Compute(err.unity));

            double precision = (Math.Min(vessel.body.torque.x, vessel.body.torque.y) * 20.0 / vessel.body.momentumOfInertia.magnitude).clamp(0.5, 10);
            double driveLimit = (err.magnitude * driveFactor / precision).clamp(0, 1);

			act = act.clamp(-driveLimit, driveLimit);

            act = _act + (act - _act) * (vessel.body.timeDelta / Tf);

            setControls(act, c, driveLimit);

            _act = new Vector3(c.pitch, c.yaw, c.roll);
        }

        private void setControls(Vector3 act, bjeb.game.FlightControl c, double driveLimit)
        {
			if (!double.IsNaN(act.z)) 
				c.roll = (float)act.z.clamp(-driveLimit, driveLimit);

			if (Math.Abs(c.roll) < 0.05)
				c.roll = 0;

			if (!double.IsNaN(act.x))
				c.pitch = (float)act.x.clamp(-driveLimit, driveLimit);

			if (Math.Abs(c.pitch) < 0.05)
				c.pitch = 0;

			if (!double.IsNaN(act.y)) 
				c.yaw = (float)act.y.clamp(-driveLimit, driveLimit);

			if (Math.Abs(c.yaw) < 0.05)
			{
				c.yaw = 0;
			}
        }
    }
}
