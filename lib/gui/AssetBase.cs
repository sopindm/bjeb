using System.Collections.Generic;

namespace bjeb.gui
{
	public class AssetBase
	{
		public enum Skin {
			FlagBrowser,
			Window1,
			Window2,
			Window3,
			Window4,
			Window5,
			Window6,
			Window7,
			OrbitMap,
			PlaqueDialog,
			Default = Window2
		}

		private static string skinName(Skin skin)
		{
			switch(skin)
			{
			case Skin.FlagBrowser:
				return "FlagBrowserSkin";
			case Skin.Window1:
				return "KSP window 1";
			case Skin.Window2:
				return "KSP window 2";
			case Skin.Window3:
				return "KSP window 3";
			case Skin.Window4:
				return "KSP window 4";
			case Skin.Window5:
				return "KSP window 5";
			case Skin.Window6:
				return "KSP window 6";
			case Skin.Window7:
				return "KSP window 7";
			case Skin.OrbitMap:
				return "OrbitMapSkin";
			case Skin.PlaqueDialog:
				return "PlaqueDialogSkin";
			}

			return "";
		}

#if UNITY
		private static SortedDictionary<Skin, UnityEngine.GUISkin> _skins = null;

		public static UnityEngine.GUISkin unitySkin(Skin skin)
		{
			if(_skins == null)
				_skins = new SortedDictionary<Skin, UnityEngine.GUISkin>();

			if(!_skins.ContainsKey(skin))
			{
				_skins[skin] = global::AssetBase.GetGUISkin(skinName(skin));
			}

			return _skins[skin];
		}
#endif
	}
}