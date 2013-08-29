using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using bjeb.util;

namespace bjeb
{
	public abstract class BasicModule: PartModule, IComparable<BasicModule>
	{
        [KSPField(isPersistant = false)]
        public float EnergyConsumption;

        [KSPField]
		public new bool active = true;

		[KSPField]
		public bool debug = false;

		[KSPEvent(guiActive = true, guiName = "Toggle debug", active = true)]
		public void toggleDebug()
		{
			debug = !debug;
		}

		[KSPEvent(guiActive = true, guiName = "Activate", active = true)]
        public void activate()
        {
			active = true;
        }

		[KSPEvent(guiActive = true, guiName = "Deactivate", active = false)]
        public void deactivate()
        {
			active = false;
        }

        [KSPAction("Activate")]
		public void activateAction(KSPActionParam param)
        {
			activate();
        }

        [KSPAction("Deactivate")]
		public void deactivateAction(KSPActionParam param)
        {
			deactivate();
        }

		[KSPAction("Toggle")]
		public void toggleAction(KSPActionParam param)
		{
			if(isActive)
				deactivate();
			else
				activate();
		}

		private bool _haveResources = true;

		protected bool isActive
		{
			get
			{
				return active && _haveResources;
			}
		}

        public override void OnUpdate()
        {
            Actions["activateAction"].active = !active && _haveResources;
            Actions["deactivateAction"].active = active && _haveResources;

			Events["activate"].active = !active && _haveResources;
			Events["deactivate"].active = active && _haveResources;
        } 

		[KSPField(isPersistant=false, guiActive=true, guiName="State")]
		public string statusMessage = "";

		[KSPField(isPersistant=false, guiActive=true, guiName="gps")]
		public string _guiFramesPerSecond = "";

		private Vessel _controlled = null;

        bool checkControlledVessel()
        {
            if (_controlled == vessel) 
				return true;

            if (_controlled != null) 
				_controlled.OnFlyByWire -= onFlyByWire;

            if (vessel != null)
            {
                vessel.OnFlyByWire -= onFlyByWire;
                vessel.OnFlyByWire += onFlyByWire;
            }

            _controlled = vessel;

            return false;
        }

        public int importance
        {
			get
			{
				if (part.State == PartStates.DEAD)
				{
					return 0;
				}
				else
				{
					return GetInstanceID();
				}
			}
        }

        public int CompareTo(BasicModule other)
        {
            if (other == null) 
				return 1;

            return importance.CompareTo(other.importance);
        }


		public override void OnFixedUpdate()
		{
			if(!active)
				statusMessage = "Disabled";
			else if(!_haveResources)
				statusMessage = "Not enough energy";
			else 
				statusMessage = "Active";

			if(!active)
				return;

			double requiredEnergy = EnergyConsumption * TimeWarp.fixedDeltaTime;

			if(debug)
				requiredEnergy = 0;

			double got = part.RequestResource("ElectricCharge", requiredEnergy);

			if(got < requiredEnergy - 1e-3)
			{
				_haveResources = false;
				Debug.Log("Need: " + requiredEnergy.ToString() + " Got: " + got.ToString());
			}
			else 
				_haveResources = true;

			if(!_haveResources)
				return;

            if (vessel == null) 
				return;

            checkControlledVessel();

            if (this != masterModule())
                return;

			onUpdate(TimeWarp.fixedDeltaTime);
		}

		private BasicModule masterModule()
		{
            List<Part> parts;
            if (HighLogic.LoadedSceneIsEditor) 
				parts = EditorLogic.SortedShipList;
            else if (vessel == null) 
				return null;
            else 
				parts = vessel.Parts;

            return (from part in parts from module in part.Modules.OfType<BasicModule>() select module).Max();
        }

        public override void OnStart(StartState state)
        {
            if (state == PartModule.StartState.None) 
				return;

            if (state != StartState.Editor)
                RenderingManager.AddToPostDrawQueue(0, drawGUI);

			part.force_activate();

            if (vessel != null)
            {
                vessel.OnFlyByWire -= onFlyByWire;
                vessel.OnFlyByWire += onFlyByWire;

                _controlled = vessel;
            }
        }

		abstract protected void onUpdate(double delta);
		abstract protected void onDraw();

		private Timer _guiTimer;

		private void drawGUI()
		{
			if(!isActive)
				return;

            if(vessel == null)
                return;

            if(vessel != FlightGlobals.ActiveVessel)
                return;

			if(this != masterModule())
				return;

			if(HighLogic.LoadedSceneIsEditor || (FlightGlobals.ready && (vessel == FlightGlobals.ActiveVessel) && (part.State != PartStates.DEAD)))
				onDraw();

			_guiTimer.update();
			_guiFramesPerSecond = _guiTimer.rate.ToString("F1");
		}

		public BasicModule()
		{
			_guiTimer = new Timer();
		}

        public void OnDestroy()
		{
            if (vessel != null)
                vessel.OnFlyByWire -= onFlyByWire;

            _controlled = null;
        }

        public void checkFlightCtrlState(FlightCtrlState s)
        {
            if (float.IsNaN(s.mainThrottle)) 
				s.mainThrottle = 0;
            if (float.IsNaN(s.yaw)) 
				s.yaw = 0;
            if (float.IsNaN(s.pitch)) 
				s.pitch = 0;
            if (float.IsNaN(s.roll)) 
				s.roll = 0;
            if (float.IsNaN(s.X)) 
				s.X = 0;
            if (float.IsNaN(s.Y)) 
				s.Y = 0;
            if (float.IsNaN(s.Z)) 
				s.Z = 0;

            s.mainThrottle = Mathf.Clamp01(s.mainThrottle);
            s.yaw = Mathf.Clamp(s.yaw, -1, 1);
            s.pitch = Mathf.Clamp(s.pitch, -1, 1);
            s.roll = Mathf.Clamp(s.roll, -1, 1);
            s.X = Mathf.Clamp(s.X, -1, 1);
            s.Y = Mathf.Clamp(s.Y, -1, 1);
            s.Z = Mathf.Clamp(s.Z, -1, 1);
        }

		abstract protected void onDrive(FlightCtrlState s);

        private void onFlyByWire(FlightCtrlState s)
        {
            if (!checkControlledVessel() || this != masterModule())
            {
                return;
            }

			onDrive(s);
            checkFlightCtrlState(s);

            if (vessel == FlightGlobals.ActiveVessel)
            {
                FlightInputHandler.state.mainThrottle = s.mainThrottle;
            }
        }
	}
}