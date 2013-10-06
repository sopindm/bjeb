using System;
using System.Collections.Generic;
using UnityEngine;
using bjeb.net;

namespace bjeb
{
    public class BJeb: BasicModule
    {
		private game.Universe _universe;
		private net.ClientProtocol _protocol;

		private void onConnectionSetup()
		{
			_protocol.setup(Screen.width, Screen.height);
			_vessel = null;
		}

		public override void OnStart(StartState state)
        {
            base.OnStart(state);
			
			var client = new Client("127.0.0.1", 4400, onConnectionSetup);
			_protocol = new ClientProtocol(client);
			_universe = new game.Universe();
		}

		public override void OnLoad(ConfigNode sfsNode)
        {
            base.OnLoad(sfsNode);
		}

		private game.Vessel _vessel = null;

		protected override void onUpdate(double delta)
		{
			if(!_protocol.connected)
			{
				statusMessage = "No connection";
				return;
			}

			if(_vessel == null)
				_vessel = new game.Vessel(_universe);

			_universe.update();
			_vessel.update(this.vessel);
			_protocol.updateState(_universe, _vessel);
		}

        protected override void onDraw()
        {
			var windows = _protocol.windows;

			if(windows != null)
			{
				foreach(var window in windows)
					window.draw();

				_protocol.updateGui();
			}
        }

		protected override void onDrive(FlightCtrlState s)
		{
			if(_vessel == null)
				return;

			game.FlightControl control = new game.FlightControl();

			control.update(s);
			_vessel.updateState(this.vessel);

			_protocol.requestControl(_vessel, control);
			
			control.apply(s);
		}
    }
}
