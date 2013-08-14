using UnityEngine;

namespace bjeb
{
	public abstract class BasicModule: PartModule
	{
        [KSPField(isPersistant = false)]
        public float EnergyConsumption;

        [KSPField]
		public bool active = true;

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

			if(part.RequestResource("ElectricCharge", requiredEnergy) < requiredEnergy)
				_haveResources = false;
			else 
				_haveResources = true;

			if(!_haveResources)
				return;

		}

        public override void OnStart(StartState state)
        {
            if (state != StartState.Editor)
                RenderingManager.AddToPostDrawQueue(0, drawGUI);

			part.force_activate();
        }

		protected int windowID
		{
			get;
			private set;
		}

		abstract protected void drawGUI();

		public BasicModule()
		{
			windowID = new System.Random().Next();
		}
	}
}