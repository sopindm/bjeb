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
		}

		private void loadComputers()
        {
            KSP.IO.FileInfo lib = KSP.IO.FileInfo.CreateForType<BJeb>("bjebModules.dll");
            string assemblyPath = lib.DirectoryName + "/" + lib.ToString();

            Debug.Log("Assembly info: " + assemblyPath);

            _computers = new List<Computer>();

            AppDomain domain = AppDomain.CreateDomain("bjebModulesDomain");
            AppDomain.Unload(domain);
            //Assembly modules = domain.Load(assemblyPath);

            /*
            List<Type> computerTypes = (from ass in domain.GetAssemblies() from t in ass.GetTypes() where t.IsSubclassOf(typeof(Computer)) select t).ToList();

            foreach (Type t in computerTypes)
                Debug.Log("Assembly info: " + t.FullName);

            _computers = new List<Computer>();

            foreach (Type t in computerTypes)
            {
                if (t == typeof(Computer))
                    continue;

                _computers.Add((Computer)(domain.CreateInstanceAndUnwrap("bjebModules", t.FullName)));
			}*/
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

        private Boolean showed = false;

        private void drawWindow(int id)
        {
            if (showed)
                return;

            showed = true;

            loadComputers();

            GUILayout.Label("Hi, this is Burning JEB.");

			GUILayout.BeginVertical();

			foreach(Computer computer in _computers)
				GUILayout.Label(computer.name);

			GUILayout.EndVertical();
        }
    }
}
