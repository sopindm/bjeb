using System;
using System.Collections.Generic;
using UnityEngine;
using bjeb.net;

namespace bjeb
{
    public class BJeb: BasicModule
    {
		private net.Client _client;

        private static GUISkin _skin;
        public static GUISkin Skin
        {
            get
            {
                if (_skin == null)
                {
                    _skin = AssetBase.GetGUISkin("KSP window 2");
                }
                return _skin;
            }
        }

		public override void OnStart(StartState state)
        {
            base.OnStart(state);
			_client = new Client("127.0.0.1", 4400, (c => Protocol.requestSetup(c, Screen.width, Screen.height)));
		}

		public override void OnLoad(ConfigNode sfsNode)
        {
            base.OnLoad(sfsNode);
		}

		protected override void onUpdate(double delta)
		{
			game.Vessel vessel = new game.Vessel();
			vessel.update(this.vessel);

			if(!_client.connected)
				statusMessage = "No connection";

			_client.execute(c => Protocol.requestUpdate(vessel, c));
		}

        protected override void drawGUI()
        {
			if(!isActive)
				return;

            if (vessel == null)
                return;

            if (vessel != FlightGlobals.ActiveVessel)
                return;
			
			List<gui.Window> windows = null;

			if(_client.execute(c => { windows = Protocol.requestGui(c); }))
			{
				foreach(var window in windows)
					window.draw();

				Protocol.requestGuiUpdate(windows, _client.connection);
			}
        }
    }
}
