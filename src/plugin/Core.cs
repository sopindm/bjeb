using System;
using System.Collections.Generic;
using UnityEngine;
using bjeb.net;

namespace bjeb
{
    public class BJeb: BasicModule
    {
		private net.ClientProtocol _protocol;
		private MuMech.MechJebModuleAttitudeController _controller;

		private void onConnectionSetup()
		{
			_protocol.setup(Screen.width, Screen.height);

			_controller = new MuMech.MechJebModuleAttitudeController();
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

		private game.Vessel _vessel = null;

		protected override void onUpdate(double delta)
		{
			if(_vessel == null)
				_vessel = new game.Vessel();

			_vessel.update(this.vessel);

			if(!_protocol.connected)
				statusMessage = "No connection";

			_protocol.updateState(_vessel);
			_controller.Update(vessel);
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
			game.FlightControl control = new game.FlightControl();
			control.update(s);

			_protocol.requestControl(control);

			control.apply(s);

			_controller.Drive(_vessel, s);
		}
    }
}
