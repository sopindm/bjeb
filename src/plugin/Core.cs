using System;
using System.Collections.Generic;
using UnityEngine;
using bjeb.net;

namespace bjeb
{
    public class BJeb: BasicModule
    {
		private net.ClientProtocol _protocol;

		private void onConnectionSetup()
		{
			_protocol.setup(Screen.width, Screen.height);
		}

		public override void OnStart(StartState state)
        {
            base.OnStart(state);
			
			var client = new Client("127.0.0.1", 4400, onConnectionSetup);
			_protocol = new ClientProtocol(client);
		}

		public override void OnLoad(ConfigNode sfsNode)
        {
            base.OnLoad(sfsNode);
		}

		protected override void onUpdate(double delta)
		{
			game.Vessel vessel = new game.Vessel();
			vessel.update(this.vessel);

			if(!_protocol.connected)
				statusMessage = "No connection";

			_protocol.updateState(vessel);
		}

        protected override void onDraw()
        {
			var windows = _protocol.windows;

			if(windows != null)
				foreach(var window in windows)
					window.draw();

			_protocol.updateGui();
        }
    }
}
