using System;
using System.Collections.Generic;
using UnityEngine;
using bjeb;

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

		private gui.Screen _screen;

		public override void OnStart(StartState state)
        {
            base.OnStart(state);

            object[] objects = AssetBase.FindObjectsOfTypeIncludingAssets(typeof(GUISkin));
            foreach (object obj in objects)
                Debug.Log("GUI Skin: " + ((GUISkin)obj).name);

            Debug.Log("High logic skin: " + HighLogic.Skin.name);

			_client = new net.Client("127.0.0.1", 4400);

			_screen = new gui.Screen();
			_screen.width = Screen.width;
			_screen.height = Screen.height;
		}

		public override void OnLoad(ConfigNode sfsNode)
        {
            base.OnLoad(sfsNode);
		}

		private List<gui.Window> requestGUI()
		{
			net.Xml request = new net.Xml("msg");
			request.root.attribute("type").set("gui");

			_screen.serialize(request.root);
			request.write(_client.connection);

			net.Xml response = net.Xml.read(_client.connection);
				
			List<gui.Window> windows = new List<gui.Window>();

			foreach(var node in response.root.nodes("window"))
			{
				gui.Window newWindow = new gui.Window();
				newWindow.deserialize(node);

				newWindow.onDrawFinished = updateWindow;

				windows.Add(newWindow);
			}

			return windows;
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

			if(_client.execute(() => { windows = requestGUI(); }))
			{
				GUI.skin = Skin;

				net.Xml request = new net.Xml("msg");
				request.root.attribute("type").set("guiUpdate");

				foreach(var window in windows)
				{
					window.draw();
					window.serializeState(request.root);
				}

				request.tryWrite(_client.connection);
			}
        }

		private void updateWindow(gui.Window window)
		{
			net.Xml request = new net.Xml("msg");
			request.root.attribute("type").set("guiWindowUpdate");
			request.root.attribute("id").set(window.id);
			window.button.serializeState(request.root);

			request.tryWrite(_client.connection);
		}

        private void drawWindow(int id)
        {
            GUILayout.Label("Hi, this is Burning JEB.");
        }
    }
}
