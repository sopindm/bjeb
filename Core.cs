using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace bjeb
{
    public class BJeb: BasicModule
    {
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

		private List<Computer> _computers;

		public override void OnLoad(ConfigNode sfsNode)
		{
			base.OnLoad(sfsNode);
			loadComputers();
		}

		private void loadComputers()
        {
            List<Type> computerTypes = (from ass in AppDomain.CurrentDomain.GetAssemblies() from t in ass.GetTypes() where t.IsSubclassOf(typeof(Computer)) select t).ToList();

            _computers = new List<Computer>();

            foreach(Type t in computerTypes)
            {
				if(t == typeof(Computer))
					continue;

                _computers.Add((Computer)(t.GetConstructor(new Type[]{}).Invoke(new object[]{})));
			}
		}

        protected override void drawGUI()
        {
			if(!isActive)
				return;

            if (vessel == null)
                return;

            if (vessel != FlightGlobals.ActiveVessel)
                return;

			GUI.skin = Skin;

			GUILayout.Window( windowID, new Rect(200, 300, 100, 100), drawWindow, "Burning JEB", GUI.skin.window);
        }

        private void drawWindow(int id)
        {
            GUILayout.Label("Hi, this is Burning JEB.");

			GUILayout.BeginVertical();

			foreach(Computer computer in _computers)
				GUILayout.Label(computer.name);

			GUILayout.EndVertical();
        }
    }
}
