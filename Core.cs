using UnityEngine;

namespace bjeb
{
    public class BJeb: PartModule
    {
        private bool _isActive = false;

        public BJeb()
        {
        }

		[KSPEvent(guiActive = true, guiName = "Activate", active = true)]
        public void activate()
        {
			_isActive = true;
        }

		[KSPEvent(guiActive = true, guiName = "Deactivate", active = false)]
        public void deactivate()
        {
			_isActive = false;
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
			if(_isActive)
				deactivate();
			else
				activate();
		}

        public override void OnUpdate()
        {
            Actions["activateAction"].active = !_isActive;
            Actions["deactivateAction"].active = _isActive;

			Events["activate"].active = !_isActive;
			Events["deactivate"].active = _isActive;
        } 

        public override void OnStart(StartState state)
        {
            if (state != StartState.Editor)
                RenderingManager.AddToPostDrawQueue(0, drawGUI);
        }

        int windowID = new System.Random().Next();

        private void drawGUI()
        {
            if (vessel == null)
                return;

            if (vessel != FlightGlobals.ActiveVessel)
                return;

			GUILayout.Window( windowID, new Rect(200, 300, 100, 100), drawWindow, "Burning JEB", GUI.skin.window);
        }

        private void drawWindow(int id)
        {
            GUILayout.Label("Hi, this is Burning JEB.");
        }
    }
}
